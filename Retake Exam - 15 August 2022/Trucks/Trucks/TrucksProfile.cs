﻿namespace Trucks
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
    using Trucks.Data.Models;
    using Trucks.DataProcessor.ExportDto;
    using Trucks.DataProcessor.ImportDto;

    public class TrucksProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE OR RENAME THIS CLASS
        public TrucksProfile()
        {
            CreateMap<Truck, ExportTrucksModel>();
            CreateMap<Despatcher, ExportrDespatchersModel>();
            CreateMap<Truck, ImportTrucksModel>();
            CreateMap<Client,ImportClientsModel>();
        }
    }
}
