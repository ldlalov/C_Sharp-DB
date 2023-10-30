namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.enums;
    using Artillery.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportCountryModel[]), new XmlRootAttribute("Countries"));
            using StringReader reader = new StringReader(xmlString);
            var countryData = (ImportCountryModel[])serializer.Deserialize(reader);
            var countries = new List<Country>();
            foreach (var c in countryData)
            {
                if (!IsValid(c))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                countries.Add(new Country { CountryName = c.CountryName, ArmySize = c.ArmySize });
                sb.AppendLine(string.Format(SuccessfulImportCountry,c.CountryName,c.ArmySize));
            }
            context.Countries.AddRange(countries);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(typeof(ImportManufacturerModel[]), new XmlRootAttribute("Manufacturers"));
            using StringReader reader = new StringReader(xmlString);
            var manufacturersData = (ImportManufacturerModel[])serializer.Deserialize(reader);
            var manufacturers = new List<Manufacturer>();
            foreach (var m in manufacturersData)
            {
                if (!IsValid(m) || manufacturers.Any(x => x.ManufacturerName == m.ManufacturerName))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                string[] manufacturerAddress = m.Founded.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                string resultAddress = manufacturerAddress[manufacturerAddress.Length-2] + ", " + manufacturerAddress[manufacturerAddress.Length - 1];
                manufacturers.Add(new Manufacturer { ManufacturerName = m.ManufacturerName, Founded = resultAddress });
            sb.AppendLine(string.Format(SuccessfulImportManufacturer,m.ManufacturerName, resultAddress));
            }
            context.AddRange(manufacturers);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(typeof(ImportShellModel[]), new XmlRootAttribute("Shells"));
            using StringReader reader = new StringReader(xmlString);
            var shellData = (ImportShellModel[])serializer.Deserialize(reader);
            var shells = new List<Shell>();
            foreach (var s in shellData)
            {
                if (!IsValid(s))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                shells.Add(new Shell { ShellWeight = s.ShellWeight, Caliber = s.Caliber });
                sb.AppendLine(string.Format(SuccessfulImportShell, s.Caliber,s.ShellWeight));
            }
            context.Shells.AddRange(shells);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var gunsData = JsonConvert.DeserializeObject<ImportGunModel[]>(jsonString);
            var guns = new List<Gun>();
            foreach (var g in gunsData)
            {
                if (!IsValid(g) || !Enum.GetValues<GunType>().Any(x => x.ToString() == g.GunType))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                Gun gun = new Gun
                {
                    ManufacturerId = g.ManufacturerId,
                    GunWeight = g.GunWeight,
                    BarrelLength = g.BarrelLength,
                    NumberBuild = g.NumberBuild,
                    Range = g.Range,
                    GunType = (GunType)Enum.Parse(typeof(GunType), g.GunType),
                    ShellId = g.ShellId
                };
                foreach (var c in g.Countries)
                {
                    if (c.Id <= context.Countries.Count())
                    {

                        gun.CountriesGuns.Add(new CountryGun{CountryId = c.Id});
                    }
                }
                guns.Add(gun);
                sb.AppendLine(string.Format(SuccessfulImportGun, g.GunType, g.GunWeight, g.BarrelLength));
            }
            context.AddRange(guns);
            context.SaveChanges();
            return sb.ToString().Trim();
        }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}