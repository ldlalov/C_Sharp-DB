using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Car")]
    public class CarDTO
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("traveledDistance")]
        public int TraveledDistance { get; set; }

        [XmlArray("parts")]
        [XmlArrayItem("partId")]
        public List<PartId> Parts { get; set; } = new List<PartId>();
    }
}
