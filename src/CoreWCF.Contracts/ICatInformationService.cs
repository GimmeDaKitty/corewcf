
namespace CoreWCF.Contracts
{
    [ServiceContract]
    public interface ICatInformationService
    {
        [OperationContract]
        byte[] GetPhoto();

        [OperationContract]
        [CoreWCF.FaultContract(typeof(CatLoverFault))]
        GetCatTypesResponse GetCatTypes(GetCatTypesRequest request);
    }
}
