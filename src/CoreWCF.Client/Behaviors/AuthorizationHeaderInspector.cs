using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using CoreWCF.Client.Services;

namespace CoreWCF.Client.Behaviors;

public class AuthorizationHeaderInspector(FakeJwtTokenProvider tokenProvider) : IClientMessageInspector
{
    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        var httpRequestProperty = new HttpRequestMessageProperty();
        
        if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out var property))
        {
            httpRequestProperty = (HttpRequestMessageProperty)property;
        }
        else
        {
            request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestProperty);
        }

        var token = tokenProvider.GenerateToken();
        httpRequestProperty.Headers["Authorization"] = $"Bearer {token}";

        return null;
    }

    public void AfterReceiveReply(ref Message reply, object correlationState) { }
}