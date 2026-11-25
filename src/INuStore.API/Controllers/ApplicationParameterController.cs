using INuStore.API.Filters;
using INuStore.Contract.Service.V1.ApplicationParameter;
using INuStore.Domain.Enums;
using INuStore.UseCase.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INuStore.API.Controllers;

[Authorize(Roles = nameof(RoleType.Admin))]
[ApiVersion("1")]
public sealed class ApplicationParameterController(IApplicationParameterService service) : ApiController
{
    [HttpGet("GetByKey")]
    [Validate<Query.GetByKey>]
    public async Task<IActionResult> GetByKey([FromQuery] Query.GetByKey request, CancellationToken cancellationToken = default)
        => Ok(await service.GetByKeyAsync(request, cancellationToken));

    [HttpPost("Create")]
    [Validate<Command.Create>]
    public async Task<IActionResult> Create([FromBody] Command.Create request, CancellationToken cancellationToken = default)
        => Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("Update")]
    [Validate<Command.Update>]
    public async Task<IActionResult> Update([FromBody] Command.Update request, CancellationToken cancellationToken = default)
        => Ok(await service.UpdateAsync(request, cancellationToken));
}
