using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPITeaApp.Dto;
using WebAPITeaApp.Models.DB;
using WebAPITeaApp.Repository;

namespace WebAPITeaApp.Commands
{
    public class DeleteItemCommand<TEntity> : Command
        where TEntity : Entity
    {
        private TEntity _model { get; set; }
        private DbRepositorySQL<TEntity> _repository { get; set; }
        private Guid _id { get; set; }
        

        public DeleteItemCommand(TEntity model, DbRepositorySQL<TEntity> rep, Guid id)
        {
            _model = model;
            _repository = rep;
            _id = id;
        }

        public override ICommandCommonResult Execute()
        {
            ICommandCommonResult result = new CommandResult();
            try
            {
                _repository.Delete(_id);
                _repository.Save();
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