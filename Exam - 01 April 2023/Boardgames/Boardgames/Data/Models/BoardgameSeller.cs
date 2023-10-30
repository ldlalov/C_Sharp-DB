using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Data.Models
{
    public class BoardgameSeller
    {
        [Required]
        [Key]
        [Column(Order = 1)]
        [ForeignKey(nameof(BoardgameId))]
        public int BoardgameId { get; set; }
        public Boardgame Boardgame { get; set; }

        [Required]
        [Key]
        [Column(Order = 2)]
        [ForeignKey(nameof(SellerId))]
        public int SellerId { get; set; }
        public Seller Seller { get; set; }
    }
}
