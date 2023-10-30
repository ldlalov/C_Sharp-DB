using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trucks.Data.Models
{
    public class ClientTruck
    {
        [Key]
        [Required]
        //[Column(Order = 1)]
        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }

        public Client Client { get; set; }

        [Key]
        [Required]
        //[Column(Order = 2)]
        [ForeignKey(nameof(Truck))]
        public int TruckId { get; set; }

        public Truck Truck { get; set; }

    }
}