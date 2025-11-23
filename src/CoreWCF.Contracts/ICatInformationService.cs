using Microsoft.AspNetCore.Authorization;

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
        
        [OperationContract]
        [Authorize(Policy = "IsCoolHuman")]
        BellyRubResponse AttemptBellyRub();
    }
}
