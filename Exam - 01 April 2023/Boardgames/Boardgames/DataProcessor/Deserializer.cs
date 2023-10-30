namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Net;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            var xmlData = new XmlSerializer(typeof(ImportCreatorsModel[]), new XmlRootAttribute("Creators"));
            using StringReader creatorsData = new StringReader(xmlString);
            ImportCreatorsModel[] importCreators = (ImportCreatorsModel[])xmlData.Deserialize(creatorsData);
            //var importCreatorsModel = new ImportCreatorsModel();
            //var config = new MapperConfiguration(cfg =>
            //        cfg.CreateMap<Creator, ImportCreatorsModel>());
            //var mapper = config.CreateMapper();
            var creators = new List<Creator>();
            foreach (var item in importCreators)
            {
                Creator creator = new Creator { FirstName = item.FirstName, LastName = item.LastName };
                if (!IsValid(creator))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;   
                }
                foreach (var bg in item.Boardgames)
                {
                    if (!IsValid(bg))
                    {
                            sb.AppendLine(string.Format(ErrorMessage));
                            continue;
                    }
                    Boardgame boardgame = new Boardgame { Name = bg.Name, Rating = bg.Rating, YearPublished = bg.YearPublished, CategoryType = (CategoryType)bg.CategoryType, Mechanics = bg.Mechanics };
                    creator.Boardgames.Add(boardgame);
                }
                creators.Add(creator);
                sb.AppendLine(string.Format(SuccessfullyImportedCreator, creator.FirstName,creator.LastName,creator.Boardgames.Count()));
            }
            context.AddRange(creators);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var sellersData = JsonConvert.DeserializeObject<ImportSellersModel[]>(jsonString);
            List<Seller> sellers = new List<Seller>();
            foreach (var s in sellersData)
            {
                if (!IsValid(s))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                Seller seller = new Seller 
                { 
                    Name = s.Name,
                    Address = s.Address,
                    Country = s.Country,
                    Website = s.Website 
                };
                foreach (var bg in s.Boardgames)
                {
                    if (bg > context.Boardgames.Count())
                    {
                        sb.AppendLine(string.Format(ErrorMessage));
                        continue;
                    }
                    var boardgameSeller = new BoardgameSeller { BoardgameId = bg};
                    seller.BoardgamesSellers.Add(boardgameSeller);
                }
                sellers.Add(seller);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, seller.Name,seller.BoardgamesSellers.Count()));
            }
            context.Sellers.AddRange(sellers);
            context.SaveChanges();
            return sb.ToString();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
