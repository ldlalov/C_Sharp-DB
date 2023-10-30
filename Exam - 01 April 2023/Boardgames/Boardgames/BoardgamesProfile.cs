namespace Boardgames
{
    using AutoMapper;
    using Boardgames.Data.Models;
    using Boardgames.DataProcessor.ExportDto;
    using System.Xml.Serialization;

    public class BoardgamesProfile : Profile
    {
        // DO NOT CHANGE OR RENAME THIS CLASS!
        public BoardgamesProfile()
        {
            CreateMap<Creator, ExportCreatorsModel>().ForMember(n => n.FullName, dst => dst.MapFrom(n => n.FirstName + " " + n.LastName));
            //var config = new MapperConfiguration(cfg => cfg.CreateMap<Creator,ExportCreatorsModel>());
            CreateMap<Boardgame,ExportBoardgamesModel>();
        }
    }
}