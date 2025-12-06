using CoreWCF.Server.REST.RestWrappers;
using CoreWCF.Server.REST.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWCF.Server.REST.Controllers;

[AllowAnonymous]
[ApiController]
[Route("CatInformationService")]
public class CatInformationController(
    ICatInformationService catInformationService,
    ILogger<CatInformationController> logger)
    : ControllerBase
{
    /// <summary>
    /// GetCatTypes SOAP endpoint - equivalent to the minimal API in Program.cs line 60
    /// </summary>
    [AllowAnonymous]
    [HttpPost("GetCatTypes")]
    [Consumes("text/xml", "application/xml")]
    [Produces("text/xml")]
    public async Task<IActionResult> GetCatTypes(
        [FromBody] GetCatTypesRequestEnvelope envelope)
    {
        // Needs the SWAPPING since the serializer does not understand MessageContracts
        envelope.Body.Request.CatLoverHeader = envelope.Header.CatLoverHeader;
        
        var response = new GetCatTypesResponseEnvelope
        {
            Body = new GetCatTypesResponseBody
            {
                Response = await catInformationService.GetCatTypesAsync(envelope.Body.Request)
            }
        };
        
        // Generic serializer can be used here because the response wrapper contains namespace info
        var xmlString = await GenericSerializer.Serialize(response);
        return Content(xmlString, "text/xml; charset=utf-8");
    }
}
