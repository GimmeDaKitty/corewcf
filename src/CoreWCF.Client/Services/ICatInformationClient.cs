namespace CoreWCF.Client.Services;

internal interface ICatInformationClient
{
    Task<byte[]?> GetCatPictureAsync();
}