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
                UserInfo userInfo = dbContext.UsersInfo.Where(b => b.UserId == orderDto.UserId).First();
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


        /*
         * Get api/orders/{id}
            * Get all orders of current user
            * Get all orders by user ID -- put it into list -- LIST OF ORDER
            * Map each order from list to orderDto . form list of  orderDto
        */
        [HttpGet]
        [Route("orders/{id}")]
        [Authorize(Roles = "User")]
        public ICommandCommonResult GetUsersOrders(Guid id)
        {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translatorOrder = new OrderModelToOrderDtoTranslator(Configuration, Mapper);
            try
            {
                GetItemsListById<Order, OrderDto> GetOrdersListById = new GetItemsListById<Order, OrderDto>(order, orderDto, repositoryOrder, translatorOrder, id);
                result = GetOrdersListById.Execute();
            }
            catch(Exception e)
            {
                throw e;
            }
            return result;
        }


        /*
         * Get api/orders/
            * ADMIN METHOD
            * Get all orders of ALL USERS  
        */
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("orders")]
        public ICommandCommonResult GetOrders()
        {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translatorOrder = new OrderModelToOrderDtoTranslator(Configuration, Mapper);
            try
            {
                GetItemsListCommand<Order, OrderDto> GetOrdersList = new GetItemsListCommand<Order, OrderDto>(order, orderDto, repositoryOrder, translatorOrder);
                result = GetOrdersList.Execute();
            }
            catch(Exception e)
            {
                throw e;
            }
            return result;
        }


        /*
         * Post api/orders/update
            * Change state for order
        */
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("orders/update")]
        public HttpResponseMessage updateState([FromBody] StateDto stateDto)
        {
            MapperConfiguration = new MapperConfiguration(c => Configuration = c);
            Mapper = MapperConfiguration.CreateMapper();
            var translatorOrder = new OrderDtoToOrderModelTranlator(Configuration, Mapper);
            var translatorOrderLocal = new OrderModelToOrderDtoTranslator(Configuration, Mapper);

            // !! Need to form new orderDto
            Order currentOrder = dbContext.Orders.Where(b => b.OrderId == stateDto.OrderId).First();
            currentOrder.State = stateDto.State;
            orderDto = translatorOrderLocal.Translate(currentOrder);

            try
            {
                UpdateItemCommand<Order,OrderDto> UpdateOrderState  = new UpdateItemCommand<Order, OrderDto>(order, orderDto, repositoryOrder, translatorOrder, stateDto.OrderId);
                result = UpdateOrderState.Execute();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }

    }
}
