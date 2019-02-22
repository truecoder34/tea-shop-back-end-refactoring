﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPITeaApp.Dto;
using WebAPITeaApp.Models.DB;
using WebAPITeaApp.Translators;

namespace WebAPITeaApp.Servicies.Translators
{
    public class UserInfoModelToUserModelTranslator : AutomapperTranslator<UserInfo, User>
    {
        public UserInfoModelToUserModelTranslator(
           IMapperConfiguration configuration,
           IMapper mapper)
           : base(configuration, mapper)
        {
        }

        public override void Configure()
        {
            base.Configure();

            Mapping
                .ForMember(m => m.UserId,           o => o.MapFrom(m => m.UserId))
                .ForMember(m => m.Name,             o => o.MapFrom(m => m.Name))
                .ForMember(m => m.Surname,          o => o.MapFrom(m => m.Surname))
                .ForMember(m => m.Email,            o => o.MapFrom(m => m.Email));
       
        }
    }
}