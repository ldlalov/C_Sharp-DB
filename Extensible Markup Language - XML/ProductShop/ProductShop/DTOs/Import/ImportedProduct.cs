using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Import
{
    [XmlRoot("Product")]
    [XmlType("Product")]

    public class ImportedProduct
    {
        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("price")]
        public decimal price { get; set; }

        [XmlElement("sellerId")]
        public int sellerId { get; set; }

        [XmlElement("buyerId")]
        [Ignore]
        public int? buyerId { get; set; }
    }
}
