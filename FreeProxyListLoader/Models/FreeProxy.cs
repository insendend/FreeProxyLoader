using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using FreeProxyListLoader.Models.Enums;

namespace FreeProxyListLoader.Models
{
    [XmlRoot("FreeProxy")]
    [DataContract]
    public class FreeProxy
    {
        [XmlAttribute]
        [DataMember(Name = "IP")]
        public string Ip { get; set; }

        [XmlAttribute]
        [DataMember(Name = "Port")]
        public int Port { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string Code { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string Country { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public Anonymity Anonymity { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public bool IsHttps { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string LastChecked { get; set; }

        public bool Contains(string value)
        {
            var sbConcated = new StringBuilder();
            sbConcated
                .Append(Ip).Append(Environment.NewLine)
                .Append(Port).Append(Environment.NewLine)
                .Append(Code).Append(Environment.NewLine)
                .Append(Country).Append(Environment.NewLine)
                .Append(Anonymity).Append(Environment.NewLine)
                .Append(IsHttps).Append(Environment.NewLine)
                .Append(LastChecked).Append(Environment.NewLine);

            return sbConcated.ToString().ToLower().Contains(value.ToLower());
        }

        public override string ToString() => $"{Ip}:{Port}";
    }
}
