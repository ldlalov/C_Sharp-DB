namespace Trucks.DataProcessor
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Data;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;
    using static System.Net.Mime.MediaTypeNames;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            StringBuilder sb = new StringBuilder();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TrucksProfile>();
            });
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            var despatchers = context.Despatchers
                .Where(d => d.Trucks.Count() > 0)
                .ProjectTo<ExportrDespatchersModel>(config)
                .Select(d => new ExportrDespatchersModel
                {
                    Name = d.Name,
                    TrucksCount = d.Trucks.Count(),
                    Trucks = d.Trucks
                })
                .OrderByDescending(d => d.Trucks.Count)
                .ThenBy(d => d.Name)
                .ToArray();
            foreach (var despatcher in despatchers)
            {
                despatcher.Trucks = despatcher.Trucks.OrderBy(x => x.RegistrationNumber).ToList();
            }
            XmlSerializer serializer = new XmlSerializer(typeof(ExportrDespatchersModel[]), new XmlRootAttribute("Despatchers"));
            using StringWriter stringWriter = new StringWriter(sb);
            serializer.Serialize(stringWriter, despatchers, namespaces);
            return sb.ToString().Trim();
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            StringBuilder sb = new StringBuilder();
            var clients = context.Clients
                .ToArray()
                .Where(c => c.ClientsTrucks.Any(x => x.Truck.TankCapacity >= capacity))
                .Select(c => new
                {
                    Name = c.Name,
                    Trucks = c.ClientsTrucks.Where(x => x.Truck.TankCapacity >= capacity)
                                                        .Select(t => new
                                                        {
                                                            TruckRegistrationNumber = t.Truck.RegistrationNumber,
                                                            VinNumber = t.Truck.VinNumber,
                                                            TankCapacity = t.Truck.TankCapacity,
                                                            CargoCapacity = t.Truck.CargoCapacity,
                                                            CategoryType = t.Truck.CategoryType.ToString(),
                                                            MakeType = t.Truck.MakeType.ToString()
                                                        })
                                                        .OrderBy(t => t.MakeType)
                                                        .ThenByDescending(t => t.CargoCapacity)
                })
                .OrderByDescending(c => c.Trucks.Count())
                .ThenBy(c => c.Name)
                .Take(10)
                .ToArray();
            string result = JsonConvert.SerializeObject(clients, Formatting.Indented);
            return result;
        }
    }
}
