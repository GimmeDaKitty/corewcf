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
        
        LogHeader(request);

        return null;
    }

    private void LogHeader(Message request)
    {
        var header = request.Headers
            .FirstOrDefault(h => h.Name == "CatLoverHeader");
        
        if (header != null)
        {
            var headerValue = request.Headers.GetHeader<string>(header.Name, header.Namespace);
            logger.LogInformation("CatLoverHeader found in GetCatTypes request: {HeaderValue}", headerValue);
        }
        else
        {
            logger.LogWarning("No CatLoverHeader found in GetCatTypes request");
        }
    }

    public void BeforeSendReply(ref Message reply, object correlationState)
    {
        reply.Headers.Add(MessageHeader.CreateHeader("CatLoverHeaderResponse", 
            string.Empty, 
            "Thank you for being a cat lover!"));
    }
}