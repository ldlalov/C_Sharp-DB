using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Import
{
    [XmlRoot("User")]
    [XmlType("User")]

    public class ImportedUsers
    {
        [XmlElement("firstName")]
        public string firstName { get; set; }

        [XmlElement("lastName")]
        public string lastName { get; set; }

        [XmlElement("age")]
        public int age { get; set; }
    }
}
