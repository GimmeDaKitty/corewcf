
using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

internal interface ICatInformationProvider
{
    Task<byte[]?> GetCatPictureAsync();
    Task<CatType[]> GetCatTypes(bool containsHeader, bool onlyCatsThatLikeChildren);
}