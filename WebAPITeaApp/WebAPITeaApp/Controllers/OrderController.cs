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

namespace WebAPITeaApp.Controllers
{
    [RoutePrefix("api")]
    [Authorize]
    public class OrderController : ApiController
    {
        //Connect to DataBase
        static TeaShopContext dbContext = new TeaShopContext();
        DbRepositorySQL<Order> repositoryOrder = new DbRepositorySQL<Order>(dbContext);
        DbRepositorySQL<User> repositoryUser = new DbRepositorySQL<User>(dbContext);
        Order order = new Order();
        OrderDto orderDto = new OrderDto();
        ICommandCommonResult result;

        protected IMapper Mapper { get; set; }
        protected IMapperConfiguration Configuration { get; private set; }
        protected MapperConfiguration MapperConfiguration { get; private set; }


        // POST: api/Order
        [HttpPost]
        [Route("orders")]
        [Authorize(Roles = "User")]
        public HttpResponseMessage AddOrder([FromBody]  OrderDto orderDto)
        {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translatorUser = new UserInfoModelToUserModelTranslator(Configuration, Mapper);
            var translatorOrder = new OrderDtoToOrderModelTranlator(Configuration, Mapper);

            // Get INFO about user from USERINFO table by userGuid, and put it into User Table
            try
            {
                UserInfo userInfo = dbContext.UsersInfo.Where(b => b.UserId == orderDto.UserGuid).First();
                User userEntity = translatorUser.Translate(userInfo);
                repositoryUser.Create(userEntity);
                repositoryUser.Save();
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "No personal information about user");
            }

            // Put DATA in ORDERS table
            try
            {
                CreateItemCommand<Order, OrderDto> CreateOrder = new CreateItemCommand<Order, OrderDto>(order, orderDto, repositoryOrder, translatorOrder);
                result = CreateOrder.Execute();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }


        [HttpGet]
        [Route("orders/{id}")]
        [Authorize(Roles = "User")]
        public List<OrderDto> GetUsersOrders(Guid id)
        {
            List<Order> order = dbContext.Orders.Where(b => b.User.UserId == id).ToList();
            List<OrderDto> orderDtoList = new List<OrderDto>();
            foreach (Order elem in order)
            {
                OrderDto orderDto = new OrderDto();
                orderDto.DateTimeOfOrder = elem.DateTimeProperty;
                orderDto.UserGuid = elem.User.UserId;
                orderDto.State = elem.State;
                orderDto.ItemsList = new List<Item>();
                foreach (Item item in elem.Items)
                {
                    orderDto.ItemsList.Add(item);
                }
                orderDtoList.Add(orderDto);
            }
            return orderDtoList;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("orders")]
        public List<OrderDto> GetOrders()
        {
            List<Order> order = dbContext.Orders.ToList();           
            List<OrderDto> orderDtoList = new List<OrderDto>();
            foreach (Order elem in order)
            {
                OrderDto orderDto = new OrderDto();
                orderDto.DateTimeOfOrder = elem.DateTimeProperty;
                orderDto.UserGuid = elem.User.UserId;
                orderDto.State = elem.State;
                orderDto.ItemsList = new List<Item>();
                foreach (Item item in elem.Items)
                {
                    orderDto.ItemsList.Add(item);
                }
                orderDtoList.Add(orderDto);
            }
            return orderDtoList;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("orders/update")]
        public HttpResponseMessage updateState([FromBody] StateDto stateDto)
        {
            Order orderFromDb = dbContext.Orders.Where(b => b.OrderId == stateDto.OrderId).First();

            orderFromDb.State = stateDto.State;

            List<Order> bufferList = dbContext.Orders.ToList();

            //db.Orders.Add(orderFromDb);
            dbContext.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, "State Updated");
        }

    }
}
