namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportDespatchersModel[]),new XmlRootAttribute("Despatchers"));
            using StringReader despatchersData = new StringReader(xmlString);
            ImportDespatchersModel[] importedDispatchers = (ImportDespatchersModel[])serializer.Deserialize(despatchersData);
            List<Despatcher> despatchers = new List<Despatcher>();
            foreach (var desp in importedDispatchers)
            {
                if (!IsValid(desp))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                Despatcher despatcher = new Despatcher
                {
                    Name = desp.Name,
                    Position = desp.Position
                };
                foreach (var tr in desp.Trucks)
                {
                    if (!IsValid(tr))
                    {
                        sb.AppendLine(string.Format(ErrorMessage));
                        continue;
                    }
                    despatcher.Trucks.Add(new Truck
                    {
                        RegistrationNumber = tr.RegistrationNumber,
                        VinNumber = tr.VinNumber,
                        TankCapacity = tr.TankCapacity,
                        CargoCapacity = tr.CargoCapacity,
                        CategoryType = (CategoryType)tr.CategoryType,
                        MakeType = (MakeType)tr.MakeType
                    });
                }
                despatchers.Add(despatcher);
                sb.AppendLine(String.Format(SuccessfullyImportedDespatcher, despatcher.Name, despatcher.Trucks.Count));
            }
            context.Despatchers.AddRange(despatchers);
            context.SaveChanges();
            return sb.ToString();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var clientsData = JsonConvert.DeserializeObject<ImportClientsModel[]>(jsonString);
            List<Client> clients = new List<Client>();
            foreach (var cd in clientsData)
            {
                if (!IsValid(cd) || cd.Type == "usual")
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                Client client = new Client
                {
                    Name = cd.Name,
                    Nationality = cd.Nationality,
                    Type = cd.Type
                };
                foreach (var tr in cd.Trucks)
                {
                    if (tr> context.Trucks.Count())
                    {
                        sb.AppendLine(string.Format(ErrorMessage));
                        continue;
                    }
                    client.ClientsTrucks.Add(new ClientTruck { TruckId = tr });
                }
                clients.Add(client);
                sb.AppendLine(String.Format(SuccessfullyImportedClient, client.Name, client.ClientsTrucks.Count));
            }
            context.Clients.AddRange(clients);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}