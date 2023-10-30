using Boardgames.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Boardgame")]
    public class ImportBoardgamesModel
    {
        [XmlElement("Name")]
        [MinLength(10)]
        [MaxLength(20)]
        public string Name { get; set; }

        [XmlElement("Rating")]
        [Range(1, 10.00)]
        public double Rating { get; set; }

        [XmlElement("YearPublished")]
        [Range(2018,2023)]
        public int YearPublished { get; set; }

        [XmlElement("CategoryType")]
        public int CategoryType { get; set; }

        [XmlElement("Mechanics")]
        public string Mechanics { get; set; }

    }
}
