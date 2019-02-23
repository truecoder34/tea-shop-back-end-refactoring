using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPITeaApp.Models.DB;

namespace WebAPITeaApp.Dto
{
    public class OrderDto : EntityDto
    {
        public Guid UserId { get; set; }
        public DateTime DateTimeProperty { get; set; }

        public ICollection<Item> Items { get; set; }

        public string State { get; set; }

    }
}