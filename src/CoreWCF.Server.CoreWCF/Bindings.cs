namespace CoreWCF.Server.CoreWCF;

public static class Bindings
{
    public static BasicHttpBinding BasicHttpBindingWithEncoding => new(BasicHttpSecurityMode.Transport) 
    {
        // Required to ensure the generated client always include Content-Type: text/xml; charset=utf-8 header
        MessageEncoding = WSMessageEncoding.Text,
        TextEncoding = System.Text.Encoding.UTF8
    };

    public static BasicHttpBinding AuthorizationHttpBinding => new()
    {
        MessageEncoding = WSMessageEncoding.Text,
        TextEncoding = System.Text.Encoding.UTF8,
        Security = new BasicHttpSecurity
        {
            Mode = BasicHttpSecurityMode.Transport,
            Transport = new HttpTransportSecurity
            {
                ClientCredentialType = HttpClientCredentialType.InheritedFromHost
            }
        }
    };
}