using System.Text;
using System.Xml;

namespace CoreWCF.Client.Services;

public static class SoapRequestBuilder
{
    private const string SOAPNS = @"http://schemas.xmlsoap.org/soap/envelope/";
    private const string TEMPURINGNSPREFIX = @"tem";
    private const string CORENSPREFIX = @"cor";
    private const string TEMPURINS = @"http://tempuri.org/";
    private const string CORENS = @"http://schemas.datacontract.org/2004/07/CoreWCF.Contracts";

    /// <summary>
    /// Build GetPhoto request
    /// </summary>
    /*
     <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
           <soapenv:Header/>
           <soapenv:Body>
              <tem:GetPhoto/>
           </soapenv:Body>
     </soapenv:Envelope>
     */
    public static string BuildGetCatPhotoRequest()
    {
        using var stream = new MemoryStream();

        Encoding utf8 = new UTF8Encoding(false); // omit BOM
        using (var writer = new XmlTextWriter(stream, utf8))
        {
            // <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
            writer.WriteStartDocument();
            writer.WriteStartElement("soapenv", "Envelope", SOAPNS);

            writer.WriteStartElement("soapenv", "Header", SOAPNS); // SOAP Header
            writer.WriteEndElement(); // Header

            //<s:Body xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:tem="http://tempuri.org/" xmlns:cor="http://schemas.datacontract.org/2004/07/CoreWCF.Contracts">
            writer.WriteStartElement("soapenv", "Body", SOAPNS);
            writer.WriteAttributeString("xmlns", TEMPURINGNSPREFIX, null, TEMPURINS);
            writer.WriteAttributeString("xmlns", CORENSPREFIX, null, CORENS);

            // <tem:GetPhoto/>
            writer.WriteStartElement(TEMPURINGNSPREFIX, "GetPhoto", TEMPURINS);
            writer.WriteEndElement();

            writer.WriteEndElement(); // Body
            writer.WriteEndElement(); // Envelope
        }
        
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Build CatTypes request
    /// </summary>
    /* Example request
        <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/" xmlns:cor="http://schemas.datacontract.org/2004/07/CoreWCF.Contracts">
           <soapenv:Header>
   	        <tem:CatLoverHeader/>
           </soapenv:Header>
           <soapenv:Body>
              <tem:GetCatTypesRequest><!--Optional:-->
                 <tem:LikesChildren>?</tem:LikesChildren>
                 <!--Optional:-->
                 <tem:RequestId>?</tem:RequestId>
                 <!--Optional:-->
                 <tem:RequestTimestamp>?</tem:RequestTimestamp>
              </tem:GetCatTypesRequest>
           </soapenv:Body>
        </soapenv:Envelope>
    */
    public static string BuildGetCatTypesRequest(string? headerContent, bool likesChildren)
    {
        using var stream = new MemoryStream();
        
        Encoding utf8 = new UTF8Encoding(false); // omit BOM
        using (var writer = new XmlTextWriter(stream, utf8))
        {
            // <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
            writer.WriteStartDocument();
            writer.WriteStartElement("soapenv", "Envelope", SOAPNS);
            writer.WriteStartElement("soapenv", "Header", SOAPNS); // SOAP Header
            
            if (headerContent != null)
            {
                // <soapenv:Header>
                //    <tem:CatLoverHeader>HeaderValue</tem:CatLoverHeader>
                // </soapenv:Header>
                writer.WriteStartElement(TEMPURINGNSPREFIX, "CatLoverHeader", TEMPURINS); // CatLoverHeader Header
                writer.WriteString(headerContent); // Write the header value here
                writer.WriteEndElement(); // CatLoverHeader 
            }
            
            writer.WriteEndElement(); // Header
        
            //<s:Body xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:tem="http://tempuri.org/" xmlns:cor="http://schemas.datacontract.org/2004/07/CoreWCF.Contracts">
            writer.WriteStartElement("soapenv", "Body", SOAPNS);
            writer.WriteAttributeString("xmlns", TEMPURINGNSPREFIX, null, TEMPURINS);
            writer.WriteAttributeString("xmlns", CORENSPREFIX, null, CORENS);
                
            // Create GetCatTypesRequest nodes
            // <tem:GetCatTypesRequest><!--Optional:-->
            //     <tem:LikesChildren>?</tem:LikesChildren>
            //     <tem:RequestId>?</tem:RequestId>
            //     <tem:RequestTimestamp>?</tem:RequestTimestamp>
            // </tem:GetCatTypesRequest>
            writer.WriteStartElement(TEMPURINGNSPREFIX, "GetCatTypesRequest", TEMPURINS);
            writer.WriteStartElement(TEMPURINGNSPREFIX, "LikesChildren", TEMPURINS);
            writer.WriteString(likesChildren.ToString().ToLower());
            writer.WriteEndElement();
    
            writer.WriteStartElement(TEMPURINGNSPREFIX, "RequestId", TEMPURINS);
            writer.WriteString(Guid.NewGuid().ToString());
            writer.WriteEndElement();
    
            writer.WriteStartElement(TEMPURINGNSPREFIX, "RequestTimestamp", TEMPURINS);
            writer.WriteString(DateTime.UtcNow.ToString("o"));
            writer.WriteEndElement();
            
            writer.WriteEndElement(); // GetCatTypesRequest
            writer.WriteEndElement(); // Body
            writer.WriteEndElement(); // Envelope
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}