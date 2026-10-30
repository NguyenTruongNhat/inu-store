using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]

    public abstract class ApiController : ControllerBase
    {
    }
}
