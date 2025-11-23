
using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

internal interface ICatInformationProvider
{
    Task<Result<byte[]>> GetCatPictureAsync();
    Task<Result<CatType[]>> GetCatTypes(bool containsHeader, bool onlyCatsThatLikeChildren);
}