namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ExportDto;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualBasic;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {

            var clients = context.Clients
                .ToArray()
                .Where(c => c.Invoices.Any(i => i.IssueDate >= date))
                .Select(c => new { InvoicesCount = c.Invoices.Count(), ClientName = c.Name, VatNumber = c.NumberVat, Invoices = c.Invoices
                                                           .Select(i => new { InvoiceNumber = i.Number, InvoiceAmount = i.Amount, i.DueDate, Currency = i.CurrencyType, i.IssueDate })
                                                           .OrderBy(i => i.IssueDate)
                                                           .ThenByDescending(i => i.DueDate)})
                .OrderByDescending(c => c.Invoices.Count())
                .ThenBy(c => c.ClientName)
                .ToArray();

            List<ExportClientsInvoicesModel> exportClients = new List<ExportClientsInvoicesModel>();
            foreach (var item in clients)
            {
                var exportClient = new ExportClientsInvoicesModel { ClientName = item.ClientName, VatNumber = item.VatNumber };
                foreach (var inv in item.Invoices)
                {
                    exportClient.Invoices.Add(new Invoice { Number = inv.InvoiceNumber, Amount = inv.InvoiceAmount,CurrencyType = inv.Currency, DueDate = inv.DueDate});

                }            
                exportClients.Add(exportClient);

            }
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
            XElement element = new XElement("Clients");
            foreach (var client in exportClients)
            {
                XElement clientElement = new XElement("Client",
                    new XAttribute("InvoicesCount", client.Invoices.Count()),
                    new XElement("ClientName", client.ClientName),
                    new XElement("VatNumber", client.VatNumber));
                XElement invoicesElement = new XElement("Invoices");
                foreach (var invoice in client.Invoices)
                {
                    XElement invoiceElement = new XElement("Invoice",
                        new XElement("InvoiceNumber", invoice.Number),
                        new XElement("InvoiceAmount", invoice.Amount),
                        new XElement("DueDate", invoice.DueDate.ToString("MM/dd/yyyy",CultureInfo.InvariantCulture)),
                        new XElement("Currency", invoice.CurrencyType.ToString()));

                    invoicesElement.Add(invoiceElement);
                }
                clientElement.Add(invoicesElement);
                element.Add(clientElement);
            }
            XDocument xDoc = new XDocument(declaration);
            xDoc.Add(element);
            var sw = new StringWriter();
            xDoc.Save(sw);
            return sw.ToString().Trim();

            //string xmlData;
            //XmlSerializer serializer = new XmlSerializer(typeof(ExportClientsInvoicesModel[]), new XmlRootAttribute("Clients"));

            //// Create StringWriter to hold the XML data
            //using (StringWriter stringWriter = new StringWriter())
            //{
            //    // Serialize the data to XML
            //    serializer.Serialize(stringWriter, clients);

            //    // Get the XML string
            //    xmlData = stringWriter.ToString();

            //    // Now you can use the xmlData string as needed
            //    Console.WriteLine(xmlData);
            //}
            //return xmlData;
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {

            var products = context.Products
                .ToArray()
                .Where(p => p.ProductsClients.Count > 0)
                .Select(p =>  new {p.Name, p.Price, Category = p.CategoryType.ToString(), Clients =  p.ProductsClients
                                                                                .Select(c => new { c.Client.Name, c.Client.NumberVat})
                                                                                .Where(c => c.Name.Length >= nameLength)
                                                                                .OrderBy(c => c.Name)})
                .OrderByDescending(p => p.Clients.Count())
                .ThenBy(p => p.Name)
                .Take(5)
                .ToArray();
                string result = JsonConvert.SerializeObject(products, Formatting.Indented);
            return result;
        }
    }
}