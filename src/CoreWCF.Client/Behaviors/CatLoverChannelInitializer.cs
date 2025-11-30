using System.ServiceModel;
using System.ServiceModel.Dispatcher;

namespace CoreWCF.Client.Behaviors;

public class CatLoverChannelInitializer(ILogger<ContractBehavior> logger) : IChannelInitializer
{
    public void Initialize(IClientChannel channel)
    {
        logger.LogInformation("The channel has been initialized. This runs once unless an unhandled exception occurs.");
    }
}