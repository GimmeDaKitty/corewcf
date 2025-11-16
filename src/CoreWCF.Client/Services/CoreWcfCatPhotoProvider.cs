namespace CoreWCF.Client.Services;

public sealed class CoreWcfCatInformationService(CatInformationServiceClient client) : ICatInformationService
{
    public async Task<byte[]?> GetCatPictureAsync()
    {
        return await client.GetPhotoAsync();
    }
}