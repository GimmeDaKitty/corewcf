using CoreWCF.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace CoreWCF.Server.Common.Services;

public sealed class CatInformationService(IHttpClientFactory httpClientFactory) : ICatInformationService
{
    private readonly List<CatType> _catRaces = new()
    {
        new CatType { RaceName = "Siamese", LikesChildren = true },
        new CatType { RaceName = "Persian", LikesChildren = true },
        new CatType { RaceName = "Maine Coon", LikesChildren = true },
        new CatType { RaceName = "Bengal", LikesChildren = false },
        new CatType { RaceName = "Sphynx", LikesChildren = true },
        new CatType { RaceName = "British Shorthair", LikesChildren = true },
        new CatType { RaceName = "Abyssinian", LikesChildren = false },
        new CatType { RaceName = "Ragdoll", LikesChildren = true },
        new CatType { RaceName = "Scottish Fold", LikesChildren = true },
        new CatType { RaceName = "Russian Blue", LikesChildren = true }
    };

    public byte[] GetPhoto()
    {
        var httpClient = httpClientFactory.CreateClient();
        var response = httpClient.GetAsync("https://cataas.com/cat").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsByteArrayAsync().Result;
    }
    
    public GetCatTypesResponse GetCatTypes(GetCatTypesRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CatLoverHeader))
        {
            var fault = new CatLoverFault
            {
                ErrorMessage = "I'm sorry this API is only for true cat lovers",
                ErrorCode = "MISSING_CAT_LOVER_HEADER",
                ErrorTimestamp = DateTime.UtcNow
            };
            throw new FaultException<CatLoverFault>(fault, new FaultReason(fault.ErrorMessage));
        }
        
        var result = request.LikesChildren
            ? _catRaces
                .Where(c=> c.LikesChildren == request.LikesChildren)
                .ToArray()
            : _catRaces.ToArray();
        
        return new GetCatTypesResponse
        {
            ResponseId = Guid.NewGuid().ToString(),
            TotalCount = result.Length,
            CatTypes = result
        };
    }
}