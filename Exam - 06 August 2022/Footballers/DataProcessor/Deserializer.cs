namespace Footballers.DataProcessor
{
    using AutoMapper;
    using Footballers.Data;
    using Footballers.Data.enums;
    using Footballers.Data.Models;
    using Footballers.DataProcessor.importdto;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FootballersProfile>();
            });
            StringBuilder sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(ImportCoachModel[]),new XmlRootAttribute("Coaches"));
            using StringReader reader = new StringReader(xmlString);
            var coachdData = (ImportCoachModel[])serializer.Deserialize(reader);
            List<Coach> coaches = new List<Coach>();
            foreach (var c in coachdData)
            {
                if (!IsValid(c))
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                    Coach coach = new Coach { Name = c.Name, Nationality = c.Nationality};

                foreach (var f in c.Footballers)
                {
                    if (!IsValid(f))
                    {
                        sb.AppendLine(string.Format(ErrorMessage));
                        continue;
                    }
                    var startDate = DateTime.ParseExact(f.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(f.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (startDate > endDate)
                    {
                        sb.AppendLine(string.Format(ErrorMessage));
                        continue;
                    }
                    Footballer footballer = new Footballer
                    {
                        Name = f.Name,
                        ContractStartDate = startDate,
                        ContractEndDate = endDate, 
                        BestSkillType = (BestSkillType)f.BestSkillType,
                        PositionType = (PositionType)f.PositionType
                    };
                    coach.Footballers.Add(footballer);
                }
                coaches.Add(coach);
                sb.AppendLine(string.Format(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count()));
            }
            context.Coaches.AddRange(coaches);
            context.SaveChanges();
            return sb.ToString().Trim();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            var teamsData = JsonConvert.DeserializeObject<ImportTeamModel[]>(jsonString);
            List<Team> teams = new List<Team>();
            foreach (var t in teamsData)
            {
                if (!IsValid(t) || t.Trophies == 0)
                {
                    sb.AppendLine(string.Format(ErrorMessage));
                    continue;
                }
                Team team = new Team { Name = t.Name, Nationality = t.Nationality};

                foreach (var foo in t.Footballers)
                {
                    if (!IsValid(foo))
                    {
                        sb.AppendLine(string.Format(ErrorMessage));
                        continue;
                    }
                    if (context.Footballers.FirstOrDefault(f => f.Id == foo) == null)
                    {
                        sb.AppendLine(string.Format(ErrorMessage));
                        continue;
                    }
                    team.TeamsFootballers.Add(new TeamFootballer { FootballerId = foo });
                }
                teams.Add(team);
                sb.AppendLine(string.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count()));
            }
            context.Teams.AddRange(teams);
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
