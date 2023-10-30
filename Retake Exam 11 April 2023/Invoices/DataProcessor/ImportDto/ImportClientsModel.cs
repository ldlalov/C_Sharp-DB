using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Client")]
    public class ImportClientsModel
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("NumberVat")]
        public string NumberVat { get; set; }

        [XmlElement("Addresses")]
        public Addresses Addresses { get; set; }
    }
    public class Addresses
    {
        [XmlElement("Address")]
        public List<ImportAddressesModel> AddressList { get; set; }
    }
}
