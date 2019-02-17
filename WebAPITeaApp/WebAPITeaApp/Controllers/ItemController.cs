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

namespace WebAPITeaApp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/catalog")]
    public class ItemController : ApiController
    {
        //Connect to DataBase
        static TeaShopContext dbContext = new TeaShopContext();
        DbRepositorySQL<Item> repository = new DbRepositorySQL<Item>(dbContext);
        Item item = new Item();
        ItemDto itemDto = new ItemDto();
        ICommandCommonResult result;

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
            try
            {
                GetItemsListCommand<Item, ItemDto> GetItemsList = new GetItemsListCommand<Item, ItemDto>(item, itemDto, repository, translator);
                result = GetItemsList.Execute();
            }
            catch(Exception e)
            {
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
        public ItemDto GetItem(Guid id)
        {
            var bufItem = dbContext.Items.Where(b => b.GuidId == id).First();

            ItemDto bufNote = new ItemDto();
            bufNote.GuidIdOfItem = bufItem.GuidId;
            bufNote.Cost = bufItem.Cost;
            bufNote.Name = bufItem.Name;
            bufNote.Description = bufItem.Description;
            bufNote.ImageLink = bufItem.ImageLink;
            bufNote.CategoryId = bufItem.Category.CategoryId;
            bufNote.ManufacterId = bufItem.Manufacter.ManufacterId;
            //ItemDto recievedFromDBItem = Mapper.Map<Item, ItemDto>(bufItem);
            return bufNote;
        }

    }
}
