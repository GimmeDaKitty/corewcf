using CoreWCF.Contracts;

namespace CoreWCF.Server.REST.Services;

public sealed class GetPhotoHandler(ICatInformationService catInformationService)
{
    public GetPhotoResponse GetPhoto()
    {
        var photo = catInformationService.GetPhoto();
        return new GetPhotoResponse
        {
            GetPhotoResult = photo
        };
    }
}