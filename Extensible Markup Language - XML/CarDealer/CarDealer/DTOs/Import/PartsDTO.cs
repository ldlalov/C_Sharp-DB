using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Part")]
    public class PartsDTO
    {
        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("price")]
        public decimal price { get; set; }

        [XmlElement("quantity")]
        public int quantity { get; set; }

        [XmlElement("supplierId")]
        public int supplierId { get; set; }
    }
}
