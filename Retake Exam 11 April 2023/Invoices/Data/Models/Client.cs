using System.ComponentModel.DataAnnotations;

namespace Invoices.Data.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(25)]
        public string Name { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(15)]
        public string NumberVat { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        public ICollection<ProductClient> ProductsClients { get; set; } = new List<ProductClient>();
    }
}