using CoreWCF.Contracts;
using CoreWCF.Server.REST.Filters;
using CoreWCF.Server.REST.RestWrappers;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;

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
        //envelope.Request!.CatLoverHeader = envelope.Header?.CatLoverHeader;
        envelope.Body.Request.CatLoverHeader = envelope.Header.CatLoverHeader;
        var serviceResponse = catInformationService.GetCatTypes(envelope.Body.Request);

        var response = new GetCatTypesResponseEnvelope
        {
            Body = new GetCatTypesResponseBody
            {
                Response = serviceResponse
            }
        };
        
        // TODO - BEA - REVISIT. I have generated the contract from ?wsdl.
        // WITH THE BRP WE HAD XML, AND WE GENERATED USING XMLSERIALIZER. IS THERE A DIFFERENCE IN THE WAY OF OBTAINING THE SOURCES? 
        // Serialize to XML
        var xmlSerializer = new XmlSerializer(typeof(GetCatTypesResponseEnvelope));
        await using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, response);
        
        return Content(stringWriter.ToString(), "text/xml; charset=utf-8");
    }
}

