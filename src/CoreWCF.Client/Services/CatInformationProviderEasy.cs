using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

public sealed class CatInformationProviderEasy(
    CatFactsServiceClient catFactsServiceClient, 
    CatInformationServiceClient catInformationServiceClient,
    FakeJwtTokenProvider tokenProvider,
    BellyRubServiceClient bellyRubServiceClient) : ICatInformationProvider
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

            // TODO - FIX - when using REST with client type Easy, ot does not work
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

    public async Task<Result> CanPetTheCatAsync(HumanType humanType)
    {
        try
        {
            tokenProvider.SetScope(humanType);

            using (new OperationContextScope(bellyRubServiceClient.InnerChannel))
            {
                var httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers["Authorization"] = $"Bearer {tokenProvider.GenerateToken()}";
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
            
                var attemptBellyRub = await bellyRubServiceClient.AllowBellyRubAsync(new AllowBellyRubRequest());
            
                return attemptBellyRub.AllowBellyRubResult
                    ? Result.Ok
                    : Result.NOk("You dont have the rights to pet the cat");
            }
        }
        catch (MessageSecurityException)
        {
            return Result.NOk("Authentication failed (401)");
        }
        catch (CommunicationException ex) when (ex.Message.Contains("Access is denied") || 
                                                ex.Message.Contains("401") ||
                                                ex.InnerException?.Message.Contains("401") == true)
        {
            return Result.NOk("Authentication failed (401)");
        }
        catch (FaultException ex)
        {
            return Result.NOk($"Service error: {ex.Message}");
        }
        catch (CommunicationException ex)
        {
            return Result.NOk($"Communication error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.NOk($"Error while processing request: {ex.Message}");
        }
    }
}