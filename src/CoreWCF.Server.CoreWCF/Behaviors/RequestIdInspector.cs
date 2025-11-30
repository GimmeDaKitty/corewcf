using CoreWCF.Contracts;
using CoreWCF.Dispatcher;

namespace CoreWCF.Server.CoreWCF.Behaviors;

public class RequestIdInspector(ILogger<RequestIdInspector> logger) : IParameterInspector
{
    public object BeforeCall(string operationName, object[] inputs)
    {
        var request = (GetCatTypesRequest)inputs[0];
        
        logger.LogInformation("Received request with Id: {RequestId} for operation {Operation}", request.RequestId, operationName);
        
        return null;
    }

    public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
    {
        // do nothing
    }
}