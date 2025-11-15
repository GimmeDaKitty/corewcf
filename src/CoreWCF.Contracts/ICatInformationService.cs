
namespace CoreWCF.Contracts
{
    [ServiceContract]
    public interface ICatInformationService
    {
        [OperationContract]
        byte[] GetPhoto();

        [OperationContract]
        CatType[] GetCatTypes(CatType cat);
    }
}
