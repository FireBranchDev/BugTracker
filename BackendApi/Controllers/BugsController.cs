using BackendApi.DTOs;
using BackendClassLib;
using BackendClassLib.Database.Repository;
using ClassLib;
using ClassLib.Exceptions;
using JsonApiSerializer;
using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using JsonApiBugDto = BackendApi.DTOs.JsonApi.BugDto;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BugsController(IAuthRepository authRepository, IUserRepository userRepository, IProjectRepository projectRepository,
    IBugRepository bugRepository, IBugPermissionRepository bugPermissionRepository,
    IBugBugPermissionUserRepository bugPermissionUserRepository,
    IRepository repository) : ControllerBase
{
    readonly IAuthRepository _authRepository = authRepository;
    readonly IUserRepository _userRepository = userRepository;
    readonly IProjectRepository _projectRepository = projectRepository;
    readonly IBugRepository _bugRepository = bugRepository;
    readonly IBugPermissionRepository _bugPermissionRepository = bugPermissionRepository;
    readonly IBugBugPermissionUserRepository _bugPermissionUserRepository = bugPermissionUserRepository;
    readonly IRepository _repository = repository;

    [HttpPost("{projectId}")]
    [Consumes("application/vnd.api+json")]
    [Produces("application/vnd.api+json")]
    public async Task<IActionResult> CreateBug(int projectId)
    {
        var jsonApiSerializerSettings = new JsonApiSerializerSettings();

        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null)
        {
            var content = Content(
                JsonConvert.SerializeObject(new Error()
                {
                    Title = "Missing Sub Claim",
                    Detail = ApiErrorMessages.MissingSubClaim
                }, jsonApiSerializerSettings)
            );
            content.StatusCode = StatusCodes.Status401Unauthorized;
            return content;
        }

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            var content = Content(
                JsonConvert.SerializeObject(new Error()
                {
                    Title = "User Not Found",
                    Detail = ApiErrorMessages.NoRecordOfUserAccount
                }, jsonApiSerializerSettings)
            );
            content.StatusCode = StatusCodes.Status404NotFound;
            return content;
        }

        BackendClassLib.Database.Models.Project foundProject;
        try
        {
            foundProject = await _projectRepository.FindAsync(projectId, user.Id);
        }
        catch (ProjectNotFoundException)
        {
            var content = Content(
                JsonConvert.SerializeObject(
                    new Error()
                    {
                        Title = "Project Not Found",
                        Detail = ApiErrorMessages.ProjectNotFound
                    }, jsonApiSerializerSettings
                )
            );
            content.StatusCode = StatusCodes.Status404NotFound;
            return content;
        }
        catch (UserNotProjectCollaboratorException)
        {
            var content = Content(
                JsonConvert.SerializeObject(
                    new Error()
                    {
                        Title = "User Not Project Collaborator",
                        Detail = ApiErrorMessages.UserNotProjectCollaborator
                    }, jsonApiSerializerSettings
                )
            );
            content.StatusCode = StatusCodes.Status403Forbidden;
            return content;
        }

        var requestBody = await Request.Body.ReadAsStringAsync();

        DocumentRoot<BugDto>? bug;
        try
        {
            bug = JsonConvert.DeserializeObject<DocumentRoot<BugDto>>(requestBody, jsonApiSerializerSettings);
        }
        catch (JsonApiFormatException ex)
        {
            var content = Content(
                JsonConvert.SerializeObject(
                    new Error()
                    {
                        Title = ex.Message
                    },
                    jsonApiSerializerSettings
                )
            );
            content.StatusCode = StatusCodes.Status403Forbidden;
            return content;
        }

        if (bug == null)
        {
            var content = Content(
                JsonConvert.SerializeObject(
                    new Error()
                    {
                        Title = "Invalid Document Structure"
                    },
                    jsonApiSerializerSettings
                )
            );
            content.StatusCode = StatusCodes.Status400BadRequest;
            return content;
        }

        if (bug.Data == null)
        {
            var content = Content(
                JsonConvert.SerializeObject(
                    new Error()
                    {
                        Title = "Missing Document's Primary Data"
                    },
                    jsonApiSerializerSettings
                )
            );
            content.StatusCode = StatusCodes.Status400BadRequest;
            return content;
        }

        if (bug.Data.Title == null)
        {
            var content = Content(
               JsonConvert.SerializeObject(
                   new Error()
                   {
                       Title = "A Title Attribute is Required"
                   },
                   jsonApiSerializerSettings
               )
            );
            content.StatusCode = StatusCodes.Status400BadRequest;
            return content;
        }

        try
        {
            var createdBug = _bugRepository.Add(new BackendClassLib.Database.Models.Bug()
            {
                Title = bug.Data.Title,
                Description = bug.Data.Description,
                Project = foundProject,
            });

            createdBug = _bugRepository.AddUser(createdBug, user);

            var bugPermissions = await _bugPermissionRepository.GetAllAsync();
            foreach (var permission in bugPermissions)
            {
                _bugPermissionUserRepository.Add(new()
                {
                    Bug = createdBug,
                    BugPermission = permission,
                    User = user
                });
            }

            await _repository.UnitOfWork.SaveChangesAsync();

            HttpContext.Response.Headers.Location = $"{Request.Scheme}://{Request.Host}/api/bugs/{createdBug.Id}/find";

            var content = base.Content(
                JsonConvert.SerializeObject(
                    new JsonApiBugDto()
                    {
                        Id = createdBug.Id.ToString(),
                        Title = createdBug.Title,
                        Description = createdBug.Description,
                        Created = createdBug.Created,
                        Updated = createdBug.Updated,
                    }, jsonApiSerializerSettings
                )
            );
            content.StatusCode = StatusCodes.Status201Created;
            return content;
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden,
                JsonConvert.SerializeObject(
                    new Error()
                    {
                        Title = "User Not Project Collaborator",
                        Detail = ApiErrorMessages.UserNotProjectCollaborator
                    }, jsonApiSerializerSettings
                )
            );
        }
        catch (InsufficientPermissionToCreateBugException)
        {
            var content = Content(
                JsonConvert.SerializeObject(
                    new Error()
                    {
                        Title = "Insufficient Permission To Create Bug",
                        Detail = ApiErrorMessages.InsufficientPermissionToCreateBug
                    },
                    jsonApiSerializerSettings
                )
            );
            content.StatusCode = StatusCodes.Status403Forbidden;
            return content;
        }
        catch (UserNotFoundException)
        {
            var content = Content(
                JsonConvert.SerializeObject(
                    new Error()
                    {
                        Title = "User Not Found",
                        Detail = ApiErrorMessages.NoRecordOfUserAccount
                    }, jsonApiSerializerSettings
                )
            );
            content.StatusCode = StatusCodes.Status404NotFound;
            return content;
        }
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetBugs(int projectId)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Problem(
            title: "Missing Sub Claim",
            detail: ApiErrorMessages.MissingSubClaim,
            statusCode: StatusCodes.Status403Forbidden);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return Problem(
                title: "User Not Found",
                detail: ApiErrorMessages.NoRecordOfUserAccount,
                statusCode: StatusCodes.Status403Forbidden);
        }

        BackendClassLib.Database.Models.Project project;
        try
        {
            project = await _projectRepository.FindAsync(projectId, user.Id);
        }
        catch (ProjectNotFoundException)
        {
            return Problem(
                title: "Project Not Found",
                detail: ApiErrorMessages.ProjectNotFound,
                statusCode: StatusCodes.Status404NotFound);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return Problem(
                title: "User Not Project Collaborator",
                detail: ApiErrorMessages.UserNotProjectCollaborator,
                statusCode: StatusCodes.Status403Forbidden);
        }

        try
        {
            List<BackendClassLib.Database.Models.Bug> bugs = await _bugRepository.GetBugsAsync(projectId, user.Id);
            return Ok(new
            {
                Data = new
                {
                    Bugs = bugs.Select(ConvertToDto).ToList()
                }
            });
        }
        catch (UserNotProjectCollaboratorException)
        {
            return Problem(
                title: "User Not Project Collaborator",
                detail: ApiErrorMessages.UserNotProjectCollaborator,
                statusCode: StatusCodes.Status403Forbidden);
        }
    }

    [Route("/api/bugs/{bugId}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteBug(int bugId)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(ApiErrorMessages.NoRecordOfUserAccount);
        }

        int projectId;
        try
        {
            projectId = await _bugRepository.GetBugProjectIdAsync(bugId);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }

        try
        {
            await _bugRepository.DeleteBugAsync(projectId, user.Id, bugId);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (InsufficientPermissionToDeleteBugException)
        {
            return BadRequest(ApiErrorMessages.InsufficientPermissionToDeleteBug);
        }

        return NoContent();
    }

    [Route("/api/bugs/{bugId}/assign/{assigneeUserId}")]
    [HttpPost]
    public async Task<IActionResult> AssignCollaboratorToBugAsync(int bugId, int assigneeUserId)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return Problem(
               type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
               title: "No Record of User Account",
               detail: ApiErrorMessages.NoRecordOfUserAccount,
               statusCode: StatusCodes.Status404NotFound);
        }

        try
        {
            await _bugRepository.AssignCollaboratorToBugAsync(bugId, user.Id, assigneeUserId);
        }
        catch (BugNotFoundException)
        {
            return Problem(
               type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
               title: "No Record of Bug",
               detail: ApiErrorMessages.NoRecordOfBug,
               statusCode: StatusCodes.Status404NotFound);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return Problem(
               type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4",
               title: "User Not Project Collaborator",
               detail: ApiErrorMessages.UserNotProjectCollaborator,
               statusCode: StatusCodes.Status403Forbidden);
        }
        catch (InsufficientPermissionToAssignCollaboratorToBug)
        {
            return Problem(
              type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4",
              title: "Insufficient Permission to Assign Collaborator to Bug",
              detail: ApiErrorMessages.InsufficientPermissionAssignCollaboratorToBug,
              statusCode: StatusCodes.Status403Forbidden);
        }
        catch (UserNotFoundException)
        {
            return Problem(
             type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
             title: "Assign Collaborator to Bug Assignee Not Found",
             detail: ApiErrorMessages.AssignCollaboratorToBugAssigneeNotFound,
             statusCode: StatusCodes.Status404NotFound);
        }
        catch (CollaboratorAlreadyAssignedBug)
        {
            return Problem(
                type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10",
                title: "Already Assigned Bug Collaborator",
                detail: "The collaborator is already assigned to the bug.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return Ok(new {
          title = "Successfully assigned collaborator"
        });
    }

    [Route("/api/bugs/{bugId}/unassign/{collaboratorId}")]
    [HttpPost]
    public async Task<IActionResult> UnassignCollaboratorFromBugAsync(int bugId, int collaboratorId)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return StatusCode(403, ApiErrorMessages.NoRecordOfUserAccount);
        }

        try
        {
            await _bugRepository.UnassignCollaboratorAsync(bugId, user.Id, collaboratorId);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode(403, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (AssignedCollaboratorUserIdNotFoundException)
        {
            return NotFound(ApiErrorMessages.AssignedCollaboratorUserIdNotFound);
        }
        catch (InsufficientPermissionToUnassignCollaboratorFromBugException)
        {
            return StatusCode(403, ApiErrorMessages.InsufficientPermissionUnassignCollaboratorFromBug);
        }
        catch (CollaboratorNotAssignedToBugException)
        {
            return Ok(ApiErrorMessages.CollaboratorNotAssignedToBug);
        }

        return Ok(ApiSuccessMessages.CollaboratorUnassignedFromBug);
    }

    [Route("/api/bugs/{bugId:int}/status/{bugStatus:int}")]
    [HttpPost]
    public async Task<IActionResult> UpdateBugStatus(int bugId, int bugStatus)
    {
        Claim? subClaim = HttpContext.User.Claims.SingleOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.NoRecordOfUserAccount);
        }

        if (!Enum.IsDefined(typeof(BugStatusType), bugStatus))
        {
            return BadRequest(ApiErrorMessages.NotDefinedInBugStatusType);
        }

        try
        {
            await _bugRepository.UpdateStatusAsync(bugId, user.Id, (BugStatusType)bugStatus);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (UserNotAssignedToBugException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.UserNotAssignedToBug);
        }
        catch (BugPermissionNotFoundException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApiErrorMessages.BugPermissionNotFound);
        }
        catch (InsufficientPermissionToUpdateBugStatusException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.InsufficientPermissionToUpdateBugStatus);
        }

        return NoContent();
    }

    [Route("{bugId}/find")]
    [HttpGet]
    public async Task<IActionResult> FindAsync(int bugId)
    {
        Claim? subClaim = HttpContext.User.Claims.SingleOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.NoRecordOfUserAccount);
        }

        try
        {
            BackendClassLib.Database.Models.Bug bug = await _bugRepository.FindBugAsync(bugId, user.Id);
            return Ok(ConvertToDto(bug));
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
    }

    static DTOs.BugDto ConvertToDto(BackendClassLib.Database.Models.Bug bug)
    {
        return new()
        {
            Id = bug.Id,
            Title = bug.Title,
            Description = bug.Description,
            Status = bug.Status,
            CreatedAt = bug.Created,
            UpdatedAt = bug.Updated,
        };
    }
}
