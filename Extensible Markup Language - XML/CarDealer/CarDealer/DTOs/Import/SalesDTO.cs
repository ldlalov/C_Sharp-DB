using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("Sale")]
    public class SalesDTO
    {
        [XmlElement("carId")]
        public int carId { get; set; }

        [XmlElement("customerId")]
        public int customerId { get; set; }

        [XmlElement("discount")]
        public decimal discount { get; set; }
    }
}
