namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var xmlData = new XmlSerializer(typeof(ImportClientsModel[]), new XmlRootAttribute("Clients"));
            using StringReader clientsData = new StringReader(xmlString);
            ImportClientsModel[] importClients = (ImportClientsModel[])xmlData.Deserialize(clientsData);
            var clients = new List<Client>();
            var addresses = new List<Address>();
            foreach (var imp in importClients)
            {
                Client client = new Client { Name = imp.Name, NumberVat = imp.NumberVat };
                if (!IsValid(client))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                foreach (var add in imp.Addresses.AddressList)
                {
                    Address address = new Address { StreetName = add.StreetName, StreetNumber = add.StreetNumber, PostCode = add.PostCode, City = add.City, Country = add.Country };
                    if (!IsValid(address))
                    {
                        sb.AppendLine(string.Format(ErrorMessage));
                       continue;
                    }
                    client.Addresses.Add(address);
                }
                clients.Add(client);
                sb.AppendLine(string.Format(SuccessfullyImportedClients, client.Name));
            }
            context.AddRange(clients);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var invoicesData = JsonConvert.DeserializeObject<Invoice[]>(jsonString);
            List<Invoice> invoices = new List<Invoice>();
            foreach (var inv in invoicesData)
            {
                if (!IsValid(inv) || inv.IssueDate > inv.DueDate)
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                if (inv.DueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture) || inv.IssueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                invoices.Add(inv);
                sb.AppendLine(string.Format(SuccessfullyImportedInvoices, inv.Number));
            }
            context.AddRange(invoices);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }
    
            public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var productsData = JsonConvert.DeserializeObject<ImportProductsModel[]>(jsonString);
            List<Product> products = new List<Product>();
            List<ProductClient> productsClients = new List<ProductClient>();
            foreach (var p in productsData)
            {
                int clientsCount = 0;
                Product product = new Product { Name = p.Name, Price = p.Price, CategoryType = p.CategoryType };
                if (!IsValid(product))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                foreach (var client in p.clients)
                {
                    if (!context.Clients.Any(x => x.Id == client))
                    {
                        sb.AppendLine(string.Format(ErrorMessage));
                        continue;
                    }
                    product.ProductsClients.Add(new ProductClient { ClientId = client });
                    clientsCount++;
                }
                products.Add(product);
                sb.AppendLine(string.Format(SuccessfullyImportedProducts, p.Name, clientsCount));
            }
            context.AddRange(products);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    } 
}
