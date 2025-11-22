using CoreWCF.Client.Services;
using CoreWCF.Contracts;

namespace CoreWCF.Client.REST;

// TODO - BEA - WHAT I HAVE LEARNED - ACTUALLY THIS IS UNRELATED TO COREWCF. THIS IS JUST ABOUT CODE GENERATION
// I HAVE SEEN SOCRATES USE THIS.
public sealed class RestCatInformationProviderEasyWay(CatInformationServiceClient client) : ICatInformationProvider
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