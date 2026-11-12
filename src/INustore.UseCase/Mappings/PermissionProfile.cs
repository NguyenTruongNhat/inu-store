using AutoMapper;
using INuStore.Contract.Service.V1.Permission;
using INuStore.Domain.Entities.Identity;

namespace INuStore.UseCase.Mappings;

public sealed class PermissionProfile : Profile
{
    public PermissionProfile()
    {
        CreateMap<Permission, Response.PermissionResponse>();
    }
}
