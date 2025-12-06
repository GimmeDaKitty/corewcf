using CoreWCF.Dispatcher;

namespace CoreWCF.Server.CoreWCF.Behaviors;

public class CatLoverHeaderInspector(ILogger<CatLoverHeaderInspector> logger) : IDispatchMessageInspector
{
    public object? AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
    {
        // Applies only for 1 specific operation
        // MesageInspectors are bound to DispatchRuntime, which is per endpoint,
        // it is not possible to bind per operation
        var action = request.Headers.Action;
        if (action != null && !action.Contains("GetCatTypes"))
        {
            return null;
        }
        
        CheckForCatLoverHeader(request);

        return null;
    }

    private void CheckForCatLoverHeader(Message request)
    {
        var header = request.Headers
            .FirstOrDefault(h => h.Name == "CatLoverHeader");
        
        var headerValue = request.Headers.GetHeader<string>(header?.Name, header?.Namespace);
        
        if (headerValue != null)
        {
            logger.LogInformation("CatLoverHeader found in GetCatTypes request: {HeaderValue}", headerValue);
        }
        else
        {
            logger.LogError("Authorization inspector: I'm sorry this API is only for true cat lovers");
            throw new CommunicationException("401 - I'm sorry this API is only for true cat lovers");
        }
    }

    public void BeforeSendReply(ref Message reply, object correlationState)
    {
        reply.Headers.Add(MessageHeader.CreateHeader("CatLoverHeaderResponse", 
            string.Empty, 
            "Thank you for being a cat lover!"));
    }
}