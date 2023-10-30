using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType("Truck")]
    public class ExportTrucksModel
    {
        [MaxLength(8)]
        [RegularExpression(@"[A-Z]{2}[0-9]{4}[A-Z]{2}")]
        [XmlElement("RegistrationNumber")]
        public string RegistrationNumber { get; set; }

        [Required]
        [XmlElement("Make")]
        public MakeType MakeType { get; set; }


    }
}
