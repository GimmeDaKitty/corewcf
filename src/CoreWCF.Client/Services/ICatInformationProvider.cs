using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

internal interface ICatInformationProvider
{
    Task<Result<string>> GetCatFactAsync();
    Task<Result<byte[]>> GetCatPictureAsync();
    Task<Result<CatType[]>> GetCatTypes(bool containsHeader, bool onlyCatsThatLikeChildren);
    Task<Result> CanPetTheCatAsync(HumanType humanType);
}