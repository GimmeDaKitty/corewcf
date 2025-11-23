namespace CoreWCF.Contracts;

[ServiceContract]
public interface IBellyRubService
{
    [OperationContract]
    bool AllowBellyRub();
}