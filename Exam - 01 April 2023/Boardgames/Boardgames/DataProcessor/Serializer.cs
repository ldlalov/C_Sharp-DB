namespace Boardgames.DataProcessor
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;


    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BoardgamesProfile>();
            });
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            StringBuilder sb = new StringBuilder();
            ExportCreatorsModel[] test = context.Creators
                .Where(c => c.Boardgames.Count > 0)
                .ProjectTo<ExportCreatorsModel>(config)
                .OrderByDescending(c => c.BoardgamesCount)
                .ThenBy(c => c.FullName)
                .ToArray();
            XmlSerializer serializer = new XmlSerializer(typeof(ExportCreatorsModel[]), new XmlRootAttribute("Creators"));
            using StringWriter stringWriter = new StringWriter(sb);
            serializer.Serialize(stringWriter, test,namespaces);
            return sb.ToString().Trim();
            
            //var creators = context.Creators
            //    .ToArray()
            //    .Where(c => c.Boardgames.Count() > 0)
            //    .Select(c => new
            //    {
            //        CreatorName = c.FirstName + " " + c.LastName,
            //        Boardgames = c.Boardgames
            //                     .Select(bg => new
            //                     {
            //                         BoardgameName = bg.Name,
            //                         BoardgameYearPublished = bg.YearPublished
            //                     })
            //                     .OrderBy(bg => bg.BoardgameName)
            //    })
            //    .OrderByDescending(c => c.Boardgames.Count())
            //    .ThenBy(c => c.CreatorName)
            //    .ToArray();
            //XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
            //XElement element = new XElement("Creators");
            //foreach (var cr in creators)
            //{
            //    XElement creator = new XElement("Creator",
            //        new XAttribute("BoardgamesCount", cr.Boardgames.Count()),
            //        new XElement("CreatorName", cr.CreatorName)
            //        );
            //    XElement games = new XElement("Boardgames");
            //    foreach (var bg in cr.Boardgames)
            //    {

            //        XElement game = new XElement("Boardgame",
            //                        new XElement("BoardgameName", bg.BoardgameName),
            //                        new XElement("BoardgameYearPublished", bg.BoardgameYearPublished));
            //        games.Add(game);
            //    }
            //    creator.Add(games);
            //    element.Add(creator);
            //}
            //XDocument xDoc = new XDocument(declaration);
            //xDoc.Add(element);
            //var sw = new StringWriter();
            //xDoc.Save(sw);
            //return sw.ToString().Trim();
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            var sellers = context.Sellers
                    .ToArray()
                    .Where(s => s.BoardgamesSellers.Any(bg => bg.Boardgame.YearPublished >= year && bg.Boardgame.Rating <= rating))
                    .Select(s => new { Name = s.Name, Website = s.Website, 
                        Boardgames = s.BoardgamesSellers
                                      .Where(bg => bg.Boardgame.YearPublished >= year && bg.Boardgame.Rating <= rating)
                                      .Select(bg => new {Name = bg.Boardgame.Name,
                                                         Rating = bg.Boardgame.Rating,
                                                         Mechanics = bg.Boardgame.Mechanics,
                                                         Category = bg.Boardgame.CategoryType.ToString()})
                                      .OrderByDescending(bg => bg.Rating)
                                      .ThenBy(bg => bg.Name)
                    })
                    .OrderByDescending(s => s.Boardgames.Count())
                    .ThenBy(s => s.Name)
                    .Take(5)
                    .ToArray();

            string result = JsonConvert.SerializeObject(sellers, Formatting.Indented);
            return result;
        }
    }
}