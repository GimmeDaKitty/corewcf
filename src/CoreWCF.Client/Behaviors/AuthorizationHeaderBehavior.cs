using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using CoreWCF.Client.Services;

namespace CoreWCF.Client.Behaviors;

public class AuthorizationHeaderBehavior(FakeJwtTokenProvider tokenProvider) : IEndpointBehavior
{
    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(new AuthorizationHeaderInspector(tokenProvider));
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }
    public void Validate(ServiceEndpoint endpoint) { }
}