using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPITeaApp.Dto;
using WebAPITeaApp.Models.DB;
using AutoMapper;
using WebAPITeaApp.Commands;
using WebAPITeaApp.Repository;
using WebAPITeaApp.Servicies.Translators;
using System.Web.Http.Cors;

namespace WebAPITeaApp.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    [Authorize(Roles = "User")]
    public class UserInfoController : ApiController
    {
        static TeaShopContext dbContext = new TeaShopContext();
        DbRepositorySQL<User> repositoryUser = new DbRepositorySQL<User>(dbContext);
        DbRepositorySQL<UserInfo> repositoryUserInfo = new DbRepositorySQL<UserInfo>(dbContext);
        ICommandCommonResult result;
        User user = new User();
        UserDto userDto = new UserDto();
        UserInfo userInfo = new UserInfo();
        UserInfoDto userInfoDto = new UserInfoDto();

        protected IMapper Mapper { get; set; }
        protected IMapperConfiguration Configuration { get; private set; }
        protected MapperConfiguration MapperConfiguration { get; private set; }


        /*
         GET: api/user/{Guid}
          method to get personal info about user
        */
        [HttpGet]
        [Route("user/{id}")]
        public HttpResponseMessage GetUserInfo(Guid id)
        {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translatorUserInfo = new UserInfoModelToUserInfoDto(Configuration, Mapper);

            //UserInfoDto infoAboutUser = new UserInfoDto();
            //UserInfo infoFromDb = new UserInfo();
            try
            {
                GetItemCommand<UserInfo, UserInfoDto> GetUserInfoCommand = new GetItemCommand<UserInfo, UserInfoDto>(userInfo, userInfoDto, repositoryUserInfo, translatorUserInfo, id);
                result = GetUserInfoCommand.Execute();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
                throw e;
            }
            
            //if (infoFromDb != null)
            //{
            //    infoAboutUser = Mapper.Map<UserInfo, UserInfoDto>(infoFromDb);
            //}
            //return result;
        }

        /*
         Post api/user
         Add new personal data about user
         On Save btn clicked - ADD NEW
       */
       [HttpPost]
       [Route("user")]
       public HttpResponseMessage PostUserInfo([FromBody] UserInfoDto userInfoDto)
       {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translatorUserInfo = new UserInfoDtoToUserInfoModelTranslator(Configuration, Mapper);
            try
            {
                // UPDATE COMMAND
                UpdateItemCommand<UserInfo, UserInfoDto> UpdateUserInfo = new UpdateItemCommand<UserInfo, UserInfoDto>(userInfo, userInfoDto, repositoryUserInfo, translatorUserInfo, userInfoDto.GuidId);
                result = UpdateUserInfo.Execute();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch
            {
                CreateItemCommand<UserInfo, UserInfoDto> CreateUserInfo = new CreateItemCommand<UserInfo, UserInfoDto>(userInfo, userInfoDto, repositoryUserInfo, translatorUserInfo);
                result = CreateUserInfo.Execute();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
       }
    }
}
