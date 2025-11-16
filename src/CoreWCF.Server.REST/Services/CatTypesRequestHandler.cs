using CoreWCF.Contracts;

namespace CoreWCF.Server.REST.Services;

public sealed class CatTypesRequestHandler(ICatInformationService catInformationService)
{
    public GetCatTypesResponse GetCatTypes(GetCatTypesRequest request)
    {
        return catInformationService.GetCatTypes(request);
    }
}