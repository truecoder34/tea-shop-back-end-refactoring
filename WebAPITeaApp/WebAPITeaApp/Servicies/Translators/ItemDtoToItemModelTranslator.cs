using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPITeaApp.Dto;
using WebAPITeaApp.Models.DB;
using WebAPITeaApp.Translators;

namespace WebAPITeaApp.Servicies.Translators
{
    public class ItemDtoToItemModelTranslator : AutomapperTranslator<ItemDto, Item>
    {

        public ItemDtoToItemModelTranslator(
            IMapperConfiguration configuration,
            IMapper mapper)
            : base(configuration, mapper)
        {
        }

        public override void Configure()
        {
            base.Configure();

            Mapping
                .ForMember(m => m.GuidId,                   o => o.MapFrom(m => m.GuidId))
                .ForMember(m => m.Cost,                     o => o.MapFrom(m => m.Cost))
                .ForMember(m => m.Name,                     o => o.MapFrom(m => m.Name))
                .ForMember(m => m.Description,              o => o.MapFrom(m => m.Description))
                .ForMember(m => m.ImageLink,                o => o.MapFrom(m => m.ImageLink))
                .ForMember(m => m.Category == null ? (int?)null : m.Category.CategoryId, o => o.MapFrom(m => m.CategoryId))
                .ForMember(m => m.Manufacter == null ? (int?)null : m.Manufacter.ManufacterId,  o => o.MapFrom(m => m.ManufacterId));
        }
    }
}