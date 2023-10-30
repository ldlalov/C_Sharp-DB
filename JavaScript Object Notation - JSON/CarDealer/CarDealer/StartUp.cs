using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var db = new CarDealerContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            string inputJson = File.ReadAllText("../../../Datasets/suppliers.json");
            Console.WriteLine(ImportSuppliers(db, inputJson));

            inputJson = File.ReadAllText("../../../Datasets/parts.json");
            Console.WriteLine(ImportParts(db, inputJson));

            inputJson = File.ReadAllText("../../../Datasets/cars.json");
            Console.WriteLine(ImportCars(db, inputJson));

            //inputJson = File.ReadAllText("../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(db, inputJson));

            //inputJson = File.ReadAllText("../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(db, inputJson));

            string customers = "../../../Datasets/ordered-customers.json";
            File.WriteAllText(customers, GetOrderedCustomers(db));

            string toyotas = "../../../Datasets/toyota-cars.json";
            File.WriteAllText(toyotas, GetCarsFromMakeToyota(db));

            string suppliers = "../../../Datasets/local-suppliers.json";
            File.WriteAllText(suppliers, GetLocalSuppliers(db));

            string cars = "../../../Datasets/cars-and-parts.json";
            File.WriteAllText(cars, GetCarsWithTheirListOfParts(db));

            string sales = "../../../Datasets/customers-total-sales.json";
            File.WriteAllText(sales, GetTotalSalesByCustomer(db));



        }
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {

            var customers = context.Customers
                .Where(c => c.Sales.Count() > 0)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Select(c => c.Car.PartsCars.Sum(d => d.Part.Price) - c.Discount),
                })
                .ToArray();

            var test = new List<CustumerCarsSums>();
            foreach (var c in customers)
            {
                var obj = new CustumerCarsSums { fullName = c.fullName, boughtCars = c.boughtCars, spentMoney = c.spentMoney.Sum() };
                test.Add(obj);
            }
            var final = test.OrderByDescending(x => x.spentMoney);
            string result = JsonConvert.SerializeObject(final, Formatting.Indented);
            return result;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance,
                    },
                    parts = c.PartsCars
                            .Select(p => new { p.Part.Name, Price = p.Part.Price.ToString("0.00") })
                }).ToArray();
            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return result;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new { s.Id, s.Name, PartsCount = s.Parts.Count })
                .ToArray();
            string result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            return result;
        }
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .Select(c => new { c.Id, c.Make, c.Model, c.TraveledDistance })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ToList();
            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return result;
        }
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(u => new { u.Name, BirthDate = u.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), u.IsYoungDriver })
                .ToArray();
            string result = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return result;

        }
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sale = JsonConvert.DeserializeObject<Sale[]>(inputJson);
            context.AddRange(sale);
            context.SaveChanges();
            return $"Successfully imported {sale.Length}.";
        }
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);
            context.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Length}.";
        }
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsInput = JsonConvert.DeserializeObject<CarInput[]>(inputJson);
            List<Car> cars = new List<Car>();
            foreach (var car in carsInput)
            {
                var currentCar = new Car
                {
                    Make = car.Make,
                    Model = car.Model,
                    TraveledDistance = car.TraveledDistance,
                };
                foreach (var partId in car.PartsId)
                {
                    var partCar = new PartCar
                    {
                        PartId = partId
                    };
                    currentCar.PartsCars.Add(partCar);
                }
                cars.Add(currentCar);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();
            return $"Successfully imported {carsInput.Length}.";
        }
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson);
            var realParts = parts.Where(x => context.Suppliers.Any(y => y.Id == x.SupplierId)).OrderBy(x => x.Name);
            context.AddRange(realParts);
            context.SaveChanges();
            return $"Successfully imported {realParts.Count()}.";
        }
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);
            context.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Length}.";
        }

    }
    public class CustumerCarsSums
    {
        public string fullName { get; set; }
        public int boughtCars { get; set; }
        public decimal spentMoney { get; set; }
    }

}