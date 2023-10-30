using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
    internal class ImportClientsModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [XmlElement("Name")]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("Nationality")]
        public string Nationality { get; set; }

        [Required]
        public string Type { get; set; }

        [XmlArrayAttribute("Trucks")]
        public HashSet<int> Trucks { get; set; }

    }
}
