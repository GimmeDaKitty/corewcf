using CoreWCF.Contracts;
using CoreWCF.Server.REST.Filters;
using CoreWCF.Server.REST.RestWrappers;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;
using CoreWCF.Server.REST.Services;

namespace CoreWCF.Server.REST.Controllers;

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
    [HttpPost("GetCatTypes")]
    [Consumes("text/xml", "application/xml")]
    [Produces("text/xml")]
    [ServiceFilter(typeof(SoapAuthorizationFilter))]
    public async Task<IActionResult> GetCatTypes(
        [FromBody] GetCatTypesRequestEnvelope envelope)
    {
        // Needs the SWAPPING since the serializer does not understand MessageContracts
        envelope.Body.Request.CatLoverHeader = envelope.Header.CatLoverHeader;
        var serviceResponse = catInformationService.GetCatTypes(envelope.Body.Request);

        // Server serialization flavor 2: use envelopes, which effectively means
        // you have to reconstruct 100% your data contract to indicate XML namespaces
        var response = new GetCatTypesResponseEnvelope
        {
            Body = new GetCatTypesResponseBody
            {
                Response = serviceResponse
            }
        };
        
        // Using the generic serializer, since the response already contains the SOAP envelope.
        var xmlString = await GenericSerializer.Serialize(response);
        return Content(xmlString, "text/xml; charset=utf-8");
    }
}

