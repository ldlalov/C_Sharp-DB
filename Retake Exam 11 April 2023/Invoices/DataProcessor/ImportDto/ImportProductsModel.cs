using Invoices.Data.Models;
using Invoices.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.DataProcessor.ImportDto
{
    internal class ImportProductsModel
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        [Range (0,4)]
        public CategoryType CategoryType { get; set; }
        public ICollection<int> clients { get; set; } = new HashSet<int>();
    }
}
