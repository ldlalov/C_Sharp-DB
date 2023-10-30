using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Trucks.Data.Models;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType("Despatcher")]
    public class ExportrDespatchersModel
    {
        [XmlAttribute("TrucksCount")]
        public int TrucksCount { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [XmlElement("DespatcherName")]
        public string Name { get; set; }

        [XmlArray("Trucks")]
        public List<ExportTrucksModel> Trucks { get; set; }

    }
}
