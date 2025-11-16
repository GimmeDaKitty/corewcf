
using CoreWCF.Contracts;

namespace CoreWCF.Client.Services;

internal interface ICatPhotoProvider
{
    Task<byte[]?> GetCatPictureAsync();
    Task<CatType[]> GetCatTypes(bool containsHeader, bool onlyCatsThatLikeChildren);
}