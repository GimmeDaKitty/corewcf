using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

public sealed class RestCatInformationProviderEasy(CatInformationServiceClient client) : ICatInformationProvider
{
    public async Task<Result<byte[]>> GetCatPictureAsync()
    {
        try
        {
            var response = await client.GetPhotoAsync(new GetPhotoRequest());
            return Result<byte[]>.OkResult(response.GetPhotoResult);
        }
        catch (Exception ex)
        {
            return Result<byte[]>.NOkResult($"Error while processing request: {ex.Message}");
        }
    }

    public async Task<Result<CatType[]>> GetCatTypes(bool containsHeader, bool onlyCatsThatLikeChildren)
    {
        try
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

            return Result<CatType[]>.OkResult(response.CatTypes);
        }
        catch (Exception ex)
        {
            return Result<CatType[]>.NOkResult($"Error while processing request: {ex.Message}");
        }
    }
}