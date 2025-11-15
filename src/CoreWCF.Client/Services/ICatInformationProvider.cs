namespace CoreWCF.Client.Services;

internal interface ICatInformationProvider
{
    Task<Result<string>> GetCatFactAsync();
}