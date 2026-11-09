using AutoMapper;
using INuStore.Contract.Service.V1.Role;
using INuStore.Domain.Entities.Identity;

namespace INustore.UseCase.Mappings;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, Response.RoleResponse>()
            .ForCtorParam(nameof(Response.RoleResponse.RoleId), options => options.MapFrom(role => role.Id));
        CreateMap<Command.CreateRole, Role>()
            .ForMember(role => role.Id, options => options.Ignore())
            .ForMember(role => role.Name, options => options.MapFrom(request => request.Name.Trim()))
            .ForMember(role => role.Description, options => options.MapFrom(request => request.Description ?? string.Empty))
            .ForMember(role => role.UserRoles, options => options.Ignore());
    }
}
