using ClassLib.Exceptions;

namespace ClassLib;

public static class ApiErrorMessages
{
    public const string MissingSubClaim = "The request is missing a sub claim.";
    public const string UserAccountAlreadyCreated = "A user account was already created.";
    public const string NoRecordOfAuth0UserId = "No record of the Auth0 user id being associated to a user account.";
    public const string NoRecordOfUserAccount = "There is no record of a user account being linked to this auth0 user id.";
    public const string ProjectNotFound = "The project was not found.";
    public const string UserNotProjectCollaborator = "The user is not a project collaborator.";
    public const string NoRecordOfBug = "There is no record of a bug existing with this id.";
    public const string InsufficientPermissionToDeleteBug = "Insufficient permission to delete bug.";
    public const string BugNotAssociatedWithProject = "The bug is not associated with this project.";
    public const string InsufficientPermissionAssignCollaboratorToBug = "Insufficient permission to assign a collaborator to bug.";
    public const string AssignCollaboratorToBugAssigneeNotFound = "The assignee user id doesn't exist.";
    public const string InsufficientPermissionUnassignCollaboratorFromBug = "Insufficient permission to unassign a collaborator from a bug.";
    public const string AssignedCollaboratorUserIdNotFound = "The assigned collaborator user id could not be found.";
    public const string CollaboratorNotAssignedToBug = "The collaborator is not assigned to the bug.";
    public const string InsufficientPermissionToDeleteProject = "You have insufficient permission to delete the project.";
    public const string NotDefinedInBugStatusType = "The number is not defined in the bug status type enum.";
    public const string UserNotAssignedToBug = "The user is not assigned to the bug.";
    public const string BugPermissionNotFound = "The bug permission could not be found.";
    public const string InsufficientPermissionToUpdateBugStatus = "The insufficient permission to update bug status.";
}
