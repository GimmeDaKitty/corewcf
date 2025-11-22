using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

public sealed class CoreWcfCatInformationProvider(CatInformationServiceClient client) : ICatInformationProvider
{
    public async Task<byte[]?> GetCatPictureAsync()
    {
        return await client.GetPhotoAsync();
    }

    public async Task<CatType[]> GetCatTypes(bool containsHeader, bool onlyCatsThatLikeChildren)
    {
        var catLoverHeader = containsHeader ? "I love cats!" : null;
        var requestId = Guid.NewGuid().ToString();
        var requestDateTime = DateTime.UtcNow;
        var response = await client.GetCatTypesAsync(catLoverHeader, onlyCatsThatLikeChildren, requestId, requestDateTime);
        
        return response.CatTypes;
    }
}