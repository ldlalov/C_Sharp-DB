using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<PartImportDto, Part>();
            CreateMap<CarInput, Car>();
            //CreateMap<CustumerCarsSums, Customer>();
            //CreateMap<CustumerCarsSums, Car>();
        }
    }
}
