using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    //[XmlType("Address")]
    public class ImportAddressesModel
    {
        [XmlElement("StreetName")]
        public string StreetName { get; set; }

        [XmlElement("StreetNumber")]
        public int StreetNumber { get; set; }

        [XmlElement("PostCode")]
        public string PostCode { get; set; }

        [XmlElement("City")]
        public string City { get; set; }

        [XmlElement("Country")]
        public string Country { get; set; }
    }
}
