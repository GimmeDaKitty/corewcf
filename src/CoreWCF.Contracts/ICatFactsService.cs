namespace CoreWCF.Contracts;

[ServiceContract]
public interface ICatFactsService
{
    [OperationContract]
    public CatFactResponse GetCatFact();
}