using CoreWCF.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace CoreWCF.Server.Common.Services;

public sealed class BellyRubService : IBellyRubService
{
    [Authorize(Policy = "IsCoolHuman")]
    public bool AllowBellyRub() => true;
}