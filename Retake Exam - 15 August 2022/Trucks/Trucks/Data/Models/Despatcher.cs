using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Trucks.DataProcessor.ExportDto;

namespace Trucks.Data.Models
{
    public class Despatcher
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; }

        public string Position  { get; set; }
        public ICollection<Truck> Trucks { get; set; } = new List<Truck>();

    }
}
