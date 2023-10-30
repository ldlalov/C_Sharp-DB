using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType ("Supplier")]
    public class SupplierDTO
    {
        [XmlElement ("name")]
        public string name { get; set; }

        [XmlElement("isImporter")]
        public bool isImporter { get; set; }
    }
}
