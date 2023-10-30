using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var db = new CarDealerContext();
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //var suppliers = File.ReadAllText("../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(db, suppliers));

            //var parts = File.ReadAllText("../../../Datasets/parts.xml");
            //Console.WriteLine(ImportParts(db, parts));

            //var cars = File.ReadAllText("../../../Datasets/cars.xml");
            //Console.WriteLine(ImportCars(db, cars));

            //var customers = File.ReadAllText("../../../Datasets/customers.xml");
            //Console.WriteLine(ImportCustomers(db, customers));

            //var sales = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(db, sales));

            //string cars = "../../../Datasets/cars-with-distance.xml";
            //File.WriteAllText(cars, GetCarsWithDistance(db));

            string bmwCars = "../../../Datasets/bmw-cars.xml";
            File.WriteAllText(bmwCars,GetCarsFromMakeBmw(db));

            //string suppliers = "../../../Datasets/local-suppliers.xml";
            //File.WriteAllText(suppliers,GetLocalSuppliers(db));

        }
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new { id = s.Id, name = s.Name, parts = s.Parts.Count() })
                .ToArray();
            XDeclaration declaration = new XDeclaration("1.0", "UTF-16", null);
            XElement element = new XElement("suppliers");
            foreach (var sup in suppliers)
            {
                element.Add(
                    new XElement("supplier",
                    new XAttribute("id", sup.id),
                    new XAttribute("name", sup.name),
                    new XAttribute("parts-count", sup.parts))
                    );
            }
            XDocument xDoc = new XDocument(declaration);
            xDoc.Add(element);
            var sw = new StringWriter();
            xDoc.Save(sw);
            return sw.ToString().Trim();
        }
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "BMW ")
                .Select(c => new { id = c.Id, model = c.Model, traveledDistance = c.TraveledDistance })
                .OrderBy(c => c.model)
                .ThenByDescending(c => c.traveledDistance)
                .ToArray();
            XDeclaration declaration = new XDeclaration("1.0", "UTF-16", null);
            XElement element = new XElement("cars");
            foreach (var car in cars)
            {
                element.Add(
                    new XElement("car",
                    new XAttribute("id", car.id), 
                    new XAttribute("model", car.model),
                    new XAttribute("traveled-distance", car.traveledDistance))
                    );
            }
            XDocument xDoc = new XDocument(declaration);
            xDoc.Add(element);
            var sw = new StringWriter();
            xDoc.Save(sw);
            return sw.ToString().Trim();
        }
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .Select(c => new { make = c.Make, model = c.Model, distance = c.TraveledDistance })
                .OrderBy(c => c.make)
                .ThenBy(c => c.model)
                .Take(10)
                .ToArray();
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
            XElement element = new XElement("cars");
            foreach (var car in cars)
            {
                element.Add(
                    new XElement("car",
                    new XElement("make", car.make),
                    new XElement("model", car.model),
                    new XElement("traveled-distance", car.distance))
                    );
            }
            XDocument xDoc = new XDocument(declaration);
            xDoc.Add(element);
            var sw = new StringWriter();
            xDoc.Save(sw);
            return sw.ToString().Trim();

        }
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlData = new XmlSerializer(typeof(SalesDTO[]), new XmlRootAttribute("Sales"));
            using StringReader data = new StringReader(inputXml);
            SalesDTO[] salesInput = (SalesDTO[])xmlData.Deserialize(data);
            List<Sale> sales = new List<Sale>();
            foreach (var sale in salesInput)
            {
                if (context.Cars.FirstOrDefault(c => c.Id == sale.carId) != null)
                {
                    sales.Add(new Sale { CarId = sale.carId, CustomerId = sale.customerId, Discount = sale.discount});
                }
            }
            context.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count()}";
        }
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlData = new XmlSerializer(typeof(CustomersDTO[]), new XmlRootAttribute("Customers"));
            using StringReader data = new StringReader(inputXml);
            CustomersDTO[] customersInput = (CustomersDTO[])xmlData.Deserialize(data);
            List<Customer> customers = new List<Customer>();
            foreach (var cust in customersInput)
            {
                customers.Add(new Customer { Name = cust.name, BirthDate = cust.birthDate, IsYoungDriver = cust.isYoungDriver });
            }
            context.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Count()}";
        }
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlData = new XmlSerializer(typeof(CarDTO[]), new XmlRootAttribute("Cars"));
            using StringReader data = new StringReader(inputXml);
            CarDTO[] carsInput = (CarDTO[])xmlData.Deserialize(data);
            List<Car> cars = new List<Car>();
            foreach (var car in carsInput)
            {
                var currentCar = new Car
                {
                    Make = car.Make,
                    Model = car.Model,
                    TraveledDistance = car.TraveledDistance,
                };
                foreach (var partId in car.Parts)
                {
                    var partCar = new PartCar
                    {
                        PartId = partId.Id,
                    };
                    var part = currentCar.PartsCars.FirstOrDefault(p => p.PartId == partCar.PartId);
                    if (part == null)
                    {
                        currentCar.PartsCars.Add(partCar);
                    }
                }
                cars.Add(currentCar);
            }
            context.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {cars.Count()}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlData = new XmlSerializer(typeof(PartsDTO[]), new XmlRootAttribute("Parts"));
            using StringReader data = new StringReader(inputXml);
            PartsDTO[] partsData = (PartsDTO[])xmlData.Deserialize(data);
            var parts = new List<Part>();
            foreach (var p in partsData)
            {
                if (p.supplierId <= context.Suppliers.Count())
                {
                parts.Add(new Part { Name = p.name, Price = p.price, Quantity = p.quantity, SupplierId = p.supplierId });
                }
            }
            context.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count()}";
        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlData = new XmlSerializer(typeof(SupplierDTO[]), new XmlRootAttribute("Suppliers"));
            using StringReader data = new StringReader(inputXml);
            SupplierDTO[] suppllierData = (SupplierDTO[])xmlData.Deserialize(data);
            var suppliers = new List<Supplier>();
            foreach (var u in suppllierData)
            {
                suppliers.Add(new Supplier { Name = u.name, IsImporter = u.isImporter });
            }
            context.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Count()}";
        }
    }
}