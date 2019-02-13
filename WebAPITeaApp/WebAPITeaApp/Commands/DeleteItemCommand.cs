using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPITeaApp.Dto;
using WebAPITeaApp.Models.DB;
using WebAPITeaApp.Repository;

namespace WebAPITeaApp.Commands
{
    public class DeleteItemCommand<MODEL> : Command
        where MODEL : Entity
    {
        public MODEL Model { get; set; }
        public DbRepositorySQL<MODEL> Repository { get; set; }
        public Guid Id { get; set; }

        public DeleteItemCommand(MODEL model, DbRepositorySQL<MODEL> rep, Guid id)
        {
            Model = model;
            Repository = rep;
            Id = id;
        }

        public override ICommandCommonResult Execute()
        {
            ICommandCommonResult result = new CommandResult();
            try
            {
                Repository.Delete(Id);
                Repository.Save();
                result.Result = true;
                result.Message = "DB: Item was deleted successfully";
            }
            catch
            {
                result.Result = false;
                result.Message = "DB: Item removing error";
            }
            return result;
        }

    }
}