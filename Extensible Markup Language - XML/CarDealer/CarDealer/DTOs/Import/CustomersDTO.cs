using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Customer")]
    public class CustomersDTO
    {
        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("birthDate")]
        public DateTime birthDate { get; set; }

        [XmlElement("isYoungDriver")]
        public bool isYoungDriver { get; set; }
    }
}
