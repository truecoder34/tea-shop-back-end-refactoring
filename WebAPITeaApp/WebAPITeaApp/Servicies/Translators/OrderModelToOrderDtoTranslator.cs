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
    public class OrderModelToOrderDtoTranslator : AutomapperTranslator<Order, OrderDto>
    {
        public OrderModelToOrderDtoTranslator(
            IMapperConfiguration configuration,
            IMapper mapper)
            : base(configuration, mapper)
        {
        }

        public override void Configure()
        {
            base.Configure();

            Mapping
                .ForMember(m => m.DateTimeProperty,          o => o.MapFrom(m => m.DateTimeProperty))
                .ForMember(m => m.UserId,                 o => o.MapFrom(m => m.User.UserId))
                .ForMember(m => m.State,                    o => o.MapFrom(m => m.State))
                .ForMember(m => m.Items,                o => o.MapFrom(m => m.Items));
        }
    }
}