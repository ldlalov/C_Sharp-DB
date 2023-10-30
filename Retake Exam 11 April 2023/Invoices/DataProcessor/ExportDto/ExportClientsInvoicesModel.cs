using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.DataProcessor.ExportDto
{
    public class ExportClientsInvoicesModel
    {
        public string ClientName { get; set; }
        public string VatNumber { get; set; }
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();

    }
}
