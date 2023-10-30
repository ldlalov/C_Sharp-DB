using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Import
{
    [XmlRoot("Category")]
    [XmlType("Category")]

    public class ImportedCategory
    {
        [XmlElement("name")]
        public string name { get; set; }
    }
}
