using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPITeaApp.Models.DB;
using WebAPITeaApp.Dto;
using AutoMapper;
using WebAPITeaApp.Commands;
using WebAPITeaApp.Repository;
using WebAPITeaApp.Servicies.Translators;
using NLog;

namespace WebAPITeaApp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/catalog")]
    public class ItemController : ApiController
    {
        private readonly ILogger _logger;
        //Connect to DataBase
        static TeaShopContext dbContext = new TeaShopContext();
        DbRepositorySQL<Item> repository = new DbRepositorySQL<Item>(dbContext);
        Item item = new Item();
        ItemDto itemDto = new ItemDto();
        ICommandCommonResult result;



        public ItemController(ILogger logger)
        {
            _logger = logger;
        }
        protected IMapper Mapper { get; private set; }
        protected IMapperConfiguration Configuration { get; private set; }
        protected MapperConfiguration MapperConfiguration { get; private set; }

        
        /*
         GET: api/items
         method to get all ITEMS from itemsTable
        */
        [HttpGet]
        [Route("items")]
        public ICommandCommonResult GetItems()
        {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translator = new ItemModelToItemDtoTranslator(Configuration, Mapper);
            _logger.Info(DateTime.Now + Environment.NewLine + "ItemController: GET all items method invoked");
            try
            {
                GetItemsListCommand<Item, ItemDto> GetItemsList = new GetItemsListCommand<Item, ItemDto>(item, itemDto, repository, translator);
                result = GetItemsList.Execute();
                _logger.Info(DateTime.Now + Environment.NewLine + "ItemController: all items extracted successfully");
            }
            catch(Exception e)
            {
                _logger.Error(DateTime.Now + Environment.NewLine + "ItemController: items was not extracted");
                throw e;
            }
             return result;
        }

        /*
         GET: api/items/5
         method to get  ITEM by ID from itemsTable
        */
        [HttpGet]
        [Route("items/{id}")]
        public ICommandCommonResult GetItem(Guid id)
        {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translator = new ItemModelToItemDtoTranslator(Configuration, Mapper);
            _logger.Info(DateTime.Now + Environment.NewLine + "ItemController: GET item by id method invoked");
            try
            {
                GetItemCommand<Item, ItemDto> GetItem = new GetItemCommand<Item, ItemDto>(item, itemDto, repository, translator, id);
                result = GetItem.Execute();
                _logger.Info(DateTime.Now + Environment.NewLine + "ItemController: item extracted successfully");
            }
            catch(Exception e)
            {
                _logger.Error(DateTime.Now + Environment.NewLine + "ItemController: item was not extracted");
                throw e;
            } 
            return result;
        }

    }
}
