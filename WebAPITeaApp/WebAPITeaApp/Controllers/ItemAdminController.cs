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

namespace WebAPITeaApp.Controllers
{
    // [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    [Authorize(Roles ="Admin")]
    public class ItemAdminController : ApiController
    {
        //Connect to DataBase
        static TeaShopContext dbContext = new TeaShopContext();
        // repository object 
        DbRepositorySQL<Item> repository = new DbRepositorySQL<Item>(dbContext);
        Item item = new Item();
        ICommandCommonResult result;

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

            // Call command create
            try
            {
                CreateItemCommand<Item, ItemDto> CreateItem = new CreateItemCommand<Item, ItemDto>(item, itemDto, repository, translator);
                result = CreateItem.Execute();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
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

            try
            {
                UpdateItemCommand<Item, ItemDto> UpdateItem = new UpdateItemCommand<Item, ItemDto>(item, itemDto, repository, id, translator);
                result = UpdateItem.Execute();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }

        // DELETE CURRENT ITEM
        // DELETE: api/Admin/5
        [HttpDelete]
        [Route("items/{id}")]
        public HttpResponseMessage DeleteItem(Guid id)
        {
            DeleteItemCommand<Item> DeleteItem = new DeleteItemCommand<Item>(item, repository, id);
            try
            {
                result = DeleteItem.Execute();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }

    }
}
