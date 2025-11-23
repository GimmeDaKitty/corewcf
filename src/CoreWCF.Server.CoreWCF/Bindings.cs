namespace CoreWCF.Server.CoreWCF;

public static class Bindings
{
    public static BasicHttpBinding RegularHttpBinding => new(BasicHttpSecurityMode.Transport);

    public static BasicHttpBinding AuthorizationHttpBinding => new()
    {
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