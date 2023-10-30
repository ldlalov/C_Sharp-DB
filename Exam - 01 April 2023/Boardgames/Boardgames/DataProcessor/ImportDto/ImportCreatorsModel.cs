using Boardgames.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Creator")]
    public class ImportCreatorsModel
    {
        [XmlElement("FirstName")]
        [MinLength(2)]
        [MaxLength(7)]
        public string FirstName { get; set; }

        [XmlElement("LastName")]
        [MinLength(2)]
        [MaxLength(7)]
        public string LastName { get; set; }

        [XmlArray("Boardgames")]
        [XmlArrayItem("Boardgame")]
        public List<ImportBoardgamesModel> Boardgames { get; set; }
    }

}
