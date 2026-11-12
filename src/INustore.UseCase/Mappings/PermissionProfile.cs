using AutoMapper;
using INuStore.Contract.Service.V1.Permission;
using INuStore.Domain.Entities.Identity;

namespace INuStore.UseCase.Mappings;

/// <summary>Maps permission entities to read responses.</summary>
public sealed class PermissionProfile : Profile
{
    /// <summary>Registers the permission response mapping.</summary>
    public PermissionProfile()
    {
        CreateMap<Permission, Response.PermissionResponse>();
    }
}
