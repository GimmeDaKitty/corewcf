namespace CoreWCF.Contracts;

[MessageContract]
public sealed class BellyRubResponse
{
    [MessageBodyMember]
    public bool Allowed { get; set; }
}