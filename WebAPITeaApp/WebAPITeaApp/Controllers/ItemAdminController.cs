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
    // [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    [Authorize(Roles ="Admin")]
    public class ItemAdminController : ApiController
    {
        private readonly ILogger _logger;
        //Connect to DataBase
        static TeaShopContext dbContext = new TeaShopContext();
        // repository object 
        DbRepositorySQL<Item> repository = new DbRepositorySQL<Item>(dbContext);
        Item item = new Item();
        ICommandCommonResult result;
        

        public ItemAdminController(ILogger logger)
        {
            _logger = logger;
        }

        protected IMapper Mapper { get; private set; }
        protected IMapperConfiguration Configuration { get; private set; }
        protected MapperConfiguration MapperConfiguration { get; private set; }

        // Add new Item
        [HttpPost]
        [Route("items")]
        public HttpResponseMessage AddItem([FromBody] ItemDto itemDto)
        {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translator = new ItemDtoToItemModelTranslator(Configuration, Mapper);
            _logger.Info(DateTime.Now + Environment.NewLine + "ItemAdminController: POST add item method invoked");

            // Call command create
            try
            {
                CreateItemCommand<Item, ItemDto> CreateItem = new CreateItemCommand<Item, ItemDto>(item, itemDto, repository, translator);
                result = CreateItem.Execute();
                _logger.Info(DateTime.Now + Environment.NewLine + "ItemAdminController: item created successfully");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
                _logger.Error(DateTime.Now + Environment.NewLine + "ItemAdminController: item was not created");
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }

        // UPDATE CURRENT ITEM
        // PUT: api/Admin/5
        [HttpPut]
        [Route("items/{id}")]
        public HttpResponseMessage UpdateItem(Guid id, [FromBody] ItemDto itemDto)
        {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translator = new ItemDtoToItemModelTranslator(Configuration, Mapper);
            _logger.Info(DateTime.Now + Environment.NewLine + "ItemAdminController: PUT update item method invoked");
            try
            {
                UpdateItemCommand<Item, ItemDto> UpdateItem = new UpdateItemCommand<Item, ItemDto>(item, itemDto, repository, translator, id);
                result = UpdateItem.Execute();
                _logger.Info(DateTime.Now + Environment.NewLine + "ItemAdminController: item updated successfully");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
                _logger.Error(DateTime.Now + Environment.NewLine + "ItemAdminController: item was not updated");
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }

        // DELETE CURRENT ITEM
        // DELETE: api/Admin/5
        [HttpDelete]
        [Route("items/{id}")]
        public HttpResponseMessage DeleteItem(Guid id)
        {
            _logger.Info(DateTime.Now + Environment.NewLine + "ItemAdminController: DELETE delete item method invoked");
            try
            {
                DeleteItemCommand<Item> DeleteItem = new DeleteItemCommand<Item>(item, repository, id);
                result = DeleteItem.Execute();
                _logger.Info(DateTime.Now + Environment.NewLine + "ItemAdminController: item deleted successfully");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
                _logger.Error(DateTime.Now + Environment.NewLine + "ItemAdminController: item was not deleted");
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }

    }
}
