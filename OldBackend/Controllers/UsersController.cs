using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiController]
public class UsersController : ControllerBase
{
}
