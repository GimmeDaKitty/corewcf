using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

public sealed class CatInformationProviderEasy(
    CatFactsServiceClient catFactsServiceClient, 
    CatInformationServiceClient catInformationServiceClient) : ICatInformationProvider
{
    public async Task<Result<string>> GetCatFactAsync()
    {
        try
        {
            var response = await catFactsServiceClient.GetCatFactAsync(new GetCatFactRequest());
            return Result<string>.OkResult(response.Fact);
        }
        catch (Exception ex)
        {
            return Result<string>.NOkResult($"Error while processing request: {ex.Message}");
        }    }

    public async Task<Result<byte[]>> GetCatPictureAsync()
    {
        try
        {
            var response = await catInformationServiceClient.GetPhotoAsync(new GetPhotoRequest());
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

            // TODO - BEA - FIX? - when using REST with client type Easy, ot does not work
            // the namespaces of the generated client/requests/responses include DataContractAttribute
            // for responses. So CatType is not seen here when the server.REST because Server.REST
            // ignores the DataContractAttribute namespaces.
            var response = await catInformationServiceClient.GetCatTypesAsync(request);

            return Result<CatType[]>.OkResult(response.CatTypes);
        }
        catch (Exception ex)
        {
            return Result<CatType[]>.NOkResult($"Error while processing request: {ex.Message}");
        }
    }
}