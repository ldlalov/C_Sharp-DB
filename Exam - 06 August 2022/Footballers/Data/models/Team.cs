using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Footballers.Data.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression(@"[A-Za-z0-9\s\.\-]*")]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Nationality { get; set; }

        [Required]
        public int Trophies { get; set; }

        public ICollection<TeamFootballer> TeamsFootballers { get; set; } = new HashSet<TeamFootballer>();



    }
}
