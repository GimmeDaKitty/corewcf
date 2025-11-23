using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
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

            // TODO - BEA - FIX? - when using REST with client type Easy, ot does not work
            // the namespaces of the generated client/requests/responses include DataContractAttribute
            // for responses. So CatType is not seen here when the server.REST because Server.REST
            // ignores the DataContractAttribute namespaces.
            var response = await client.GetCatTypesAsync(request);

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
            var token = FakeJwtTokenGenerator.GenerateToken(humanType);

            // TODO - BEA - CAN I DO THIS IN A MORE ELEGANT WAY THROUGH DI?
            using (new OperationContextScope(client.InnerChannel))
            {
                var httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers["Authorization"] = $"Bearer {token}";
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                var attemptBellyRub = await client.AllowBellyRubAsync(new AllowBellyRubRequest());

                return attemptBellyRub.AllowBellyRubResult
                    ? Result.Ok
                    : Result.NOk("You dont have the rights to pet the cat");
            }  
        }
        catch (MessageSecurityException)
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