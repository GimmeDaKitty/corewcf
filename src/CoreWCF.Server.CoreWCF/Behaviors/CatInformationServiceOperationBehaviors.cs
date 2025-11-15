using CoreWCF.Dispatcher;

namespace CoreWCF.Server.CoreWCF.Behaviors;

public class CatInformationServiceOperationBehaviors(ILogger<RequestIdInspector> logger) : IOperationBehavior
{
    public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    {
        // do nothing
    }

    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
        // do nothing
    }

    public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
        dispatchOperation.ParameterInspectors.Add(new RequestIdInspector(logger));

    }

    public void Validate(OperationDescription operationDescription)
    {
        // do nothing
    }
}