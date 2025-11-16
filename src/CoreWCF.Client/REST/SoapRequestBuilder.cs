using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CoreWCF.Client.REST;

public static class BaseEnvelopeBuilder
{
    private const string SOAPNS = @"http://schemas.xmlsoap.org/soap/envelope/";
    private const string TEMPURINGNSPREFIX = @"tem";
    private const string CORENSPREFIX = @"cor";
    private const string TEMPURINS = @"http://tempuri.org/";
    private const string CORENS = @"http://schemas.datacontract.org/2004/07/CoreWCF.Contracts";
    
    /* Example request
        <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/" xmlns:cor="http://schemas.datacontract.org/2004/07/CoreWCF.Contracts">
            <soapenv:Header>
   	            <tem:CatLoverHeader>HeaderValue</tem:CatLoverHeader>
   	        </soapenv:Header>
            <soapenv:Body>
                <tem:GetCatTypesRequest/>
            </soapenv:Body>
        </soapenv:Envelope>
     */
    
    /// <summary>
    /// bouw de basis soap envelop op
    /// </summary>
    public static XmlDocument BuildRequest(object bodyContent)
    {
        using var stream = new MemoryStream();
        
        Encoding utf8 = new UTF8Encoding(false); // omit BOM
        using (var writer = new XmlTextWriter(stream, utf8))
        {
            WriteSoapEnvelope(writer, bodyContent);
        }

        // signing pass
        var signable = Encoding.UTF8.GetString(stream.ToArray());
        var doc = new XmlDocument();
        doc.LoadXml(signable);

        return doc;
    }

    private static void WriteSoapEnvelope(XmlTextWriter writer, object bodyContent, string headerContent)
    {
        // timestamp
        var dt = DateTime.UtcNow;
        var now = dt.ToString("o").Substring(0, 23) + "Z";

        // <s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
        writer.WriteStartDocument();
        writer.WriteStartElement("soapenv", "Envelope", SOAPNS);
        
        // <soapenv:Header>
        //    <tem:CatLoverHeader>HeaderValue</tem:CatLoverHeader>
        // </soapenv:Header>
        writer.WriteStartElement("soapenv", "Header", SOAPNS); // SOAP Header
        writer.WriteStartElement(TEMPURINGNSPREFIX, "CatLoverHeader", TEMPURINS); // CatLoverHeader Header
        writer.WriteString(headerContent); // Write the header value here
        writer.WriteEndElement(); // CatLoverHeader
        writer.WriteEndElement(); // Header
        
        //<s:Body xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:tem="http://tempuri.org/" xmlns:cor="http://schemas.datacontract.org/2004/07/CoreWCF.Contracts">
        writer.WriteStartElement("soapenv", "Body", SOAPNS);
        writer.WriteAttributeString("xmlns", TEMPURINGNSPREFIX, null, TEMPURINS);
        writer.WriteAttributeString("xmlns", CORENSPREFIX, null, CORENS);
                
        new XmlSerializer(bodyContent.GetType()).Serialize(writer, bodyContent);

        writer.WriteEndElement(); // Body
        writer.WriteEndElement(); // Envelope
    }
}