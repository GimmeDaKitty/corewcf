using System.ServiceModel;

namespace CoreWCF.Server.REST.Services;

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

    public async Task<GetPhotoResponse> GetPhotoAsync(GetPhotoRequest request)
    {
        var httpClient = httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync("https://cataas.com/cat");
        response.EnsureSuccessStatusCode();

        return new GetPhotoResponse
        {
            GetPhotoResult = await response.Content.ReadAsByteArrayAsync()
        };
    }

    public Task<GetCatTypesResponse> GetCatTypesAsync(GetCatTypesRequest request)
    {
        var result = request.LikesChildren
            ? _catRaces
                .Where(c=> c.LikesChildren == request.LikesChildren)
                .ToArray()
            : _catRaces.ToArray();
        
        return Task.FromResult(new GetCatTypesResponse
        {
            ResponseId = Guid.NewGuid().ToString(),
            TotalCount = result.Length,
            CatTypes = result
        });
    }
}