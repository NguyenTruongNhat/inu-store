using AutoMapper;
using INuStore.Contract.Service.V1.User;
using INuStore.Domain.Entities.Identity;

namespace INustore.UseCase.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, Response.UserResponse>()
                .ForCtorParam(nameof(Response.UserResponse.UserId), options => options.MapFrom(user => user.Id));

            CreateMap<Command.CreateUser, User>()
                .ForMember(user => user.Id, options => options.Ignore())
                .ForMember(user => user.PasswordHash, options => options.MapFrom(request => request.Password));
        }
    }
}
