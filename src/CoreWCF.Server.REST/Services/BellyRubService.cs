namespace CoreWCF.Server.REST.Services;

public sealed class BellyRubService : IBellyRubService
{
    public Task<AllowBellyRubResponse> AllowBellyRubAsync(AllowBellyRubRequest request)
    {
        return Task.FromResult(new AllowBellyRubResponse
        {
            AllowBellyRubResult = true
        });
    }
}