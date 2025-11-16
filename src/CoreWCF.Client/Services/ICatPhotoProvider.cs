namespace CoreWCF.Client.Services;

internal interface ICatInformationService
{
    Task<byte[]?> GetCatPictureAsync();
}