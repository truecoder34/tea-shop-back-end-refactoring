using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPITeaApp.Models.DB;
using WebAPITeaApp.Dto;
using System.Data.Entity.Migrations;
using AutoMapper;
using WebAPITeaApp.Models;
using WebAPITeaApp.Commands;
using WebAPITeaApp.Repository;
using WebAPITeaApp.Servicies.Translators;
using NLog;


namespace WebAPITeaApp.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    [Authorize(Roles = "User")]
    public class UserInfoController : ApiController
    {
        private readonly ILogger _logger;
        static TeaShopContext dbContext = new TeaShopContext();
        DbRepositorySQL<User> repositoryUser = new DbRepositorySQL<User>(dbContext);
        DbRepositorySQL<UserInfo> repositoryUserInfo = new DbRepositorySQL<UserInfo>(dbContext);
        ICommandCommonResult result;
        User user = new User();
        UserDto userDto = new UserDto();
        UserInfo userInfo = new UserInfo();
        UserInfoDto userInfoDto = new UserInfoDto();

        public UserInfoController(ILogger logger)
        {
            _logger = logger;
        }

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

            _logger.Info(DateTime.Now + Environment.NewLine + "UserInfoController: GET user info method invoked");

            try
            {
                GetItemCommand<UserInfo, UserInfoDto> GetUserInfoCommand = new GetItemCommand<UserInfo, UserInfoDto>(userInfo, userInfoDto, repositoryUserInfo, translatorUserInfo, id);
                result = GetUserInfoCommand.Execute();
                _logger.Info(DateTime.Now + Environment.NewLine + "UserInfoController: user info extracted successfully");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
                _logger.Error(DateTime.Now + Environment.NewLine + "UserInfoController: item was not extracted successfully");
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
                throw e;
            }
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

            _logger.Info(DateTime.Now + Environment.NewLine + "UserInfoController: POST user info method invoked");

            try
            {
                // UPDATE COMMAND
                UpdateItemCommand<UserInfo, UserInfoDto> UpdateUserInfo = new UpdateItemCommand<UserInfo, UserInfoDto>(userInfo, userInfoDto, repositoryUserInfo, translatorUserInfo, userInfoDto.GuidId);
                result = UpdateUserInfo.Execute();
                _logger.Info(DateTime.Now + Environment.NewLine + "UserInfoController: user info updated successfully");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch
            {
                CreateItemCommand<UserInfo, UserInfoDto> CreateUserInfo = new CreateItemCommand<UserInfo, UserInfoDto>(userInfo, userInfoDto, repositoryUserInfo, translatorUserInfo);
                result = CreateUserInfo.Execute();
                _logger.Info(DateTime.Now + Environment.NewLine + "UserInfoController: user info added successfully");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
       }
    }
}
