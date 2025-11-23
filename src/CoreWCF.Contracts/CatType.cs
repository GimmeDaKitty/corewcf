using System.Runtime.Serialization;

namespace CoreWCF.Contracts;

[DataContract]
public class CatType
{
    [DataMember]
    public bool LikesChildren { get; set; }

    [DataMember]
    public string? RaceName { get; set; }
}