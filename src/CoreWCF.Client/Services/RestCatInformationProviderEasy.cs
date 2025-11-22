using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

public sealed class RestCatInformationProviderEasy(CatInformationServiceClient client) : ICatInformationProvider
{
    public async Task<byte[]?> GetCatPictureAsync()
    {
        var response = await client.GetPhotoAsync(new GetPhotoRequest());
        return response.GetPhotoResult;
    }

    public async Task<CatType[]> GetCatTypes(bool containsHeader, bool onlyCatsThatLikeChildren)
    {
        var catLoverHeader = containsHeader ? "I love cats!" : null;
        var requestId = Guid.NewGuid().ToString();
        var requestDateTime = DateTime.UtcNow;
        
        var request = new GetCatTypesRequest
        {
            CatLoverHeader = catLoverHeader,
            LikesChildren = onlyCatsThatLikeChildren,
            RequestId = requestId,
            RequestTimestamp = requestDateTime
        };
        
        var response = await client.GetCatTypesAsync(request);
        
        return response.CatTypes;
    }
}