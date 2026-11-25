using AutoMapper;
using INuStore.Contract.Service.V1.ApplicationParameter;
using INuStore.Domain.Entities;

namespace INuStore.UseCase.Mappings;

public sealed class ApplicationParameterProfile : Profile
{
    public ApplicationParameterProfile()
    {
        CreateMap<ApplicationParameter, Response.ApplicationParameterResponse>();
    }
}
