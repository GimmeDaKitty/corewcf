using CoreWCF.Dispatcher;

namespace CoreWCF.Server.CoreWCF.Behaviors;

public class CatInformationServiceEndpointBehaviors(ILogger<CatLoverHeaderInspector> logger) : IEndpointBehavior
{
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
        // do nothing
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        // do nothing
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
        endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CatLoverHeaderInspector(logger));
    }

    public void Validate(ServiceEndpoint endpoint)
    {
        // do nothing
    }
}