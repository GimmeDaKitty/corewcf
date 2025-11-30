using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace CoreWCF.Client.Behaviors;

public class ContractBehavior(ILogger<ContractBehavior> logger) : IContractBehavior
{
    public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
        BindingParameterCollection bindingParameters)
    {
        endpoint.Address = new EndpointAddress("https://localhost:10002/DoesNotExist");
    }

    public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
        ClientRuntime clientRuntime)
    {
        clientRuntime.ChannelInitializers.Add(new CatLoverChannelInitializer(logger));
    }

    public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
        DispatchRuntime dispatchRuntime)
    {
        // for server side, not needed here
    }

    public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
    {
        // do nothing
    }
}