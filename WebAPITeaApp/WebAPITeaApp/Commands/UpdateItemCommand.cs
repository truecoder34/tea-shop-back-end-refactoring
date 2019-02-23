using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebAPITeaApp.Dto;
using WebAPITeaApp.Models.DB;
using WebAPITeaApp.Repository;
using WebAPITeaApp.Translators;

namespace WebAPITeaApp.Commands
{
    public class UpdateItemCommand<TEntity,TDto> : Command
        where TEntity : Entity
        where TDto : EntityDto
    {
        private TDto _dto { get; set; }
        private TEntity _model { get; set; }
        private DbRepositorySQL<TEntity> _repository { get; set; }
        private Guid _id { get; set; }
        private readonly AutomapperTranslator<TDto, TEntity> _translator;

        public UpdateItemCommand(TEntity model, TDto dto, DbRepositorySQL<TEntity> rep,  AutomapperTranslator<TDto, TEntity> trans, Guid id)
        {
            _dto = dto;
            _model = model;
            _repository = rep;
            _translator = trans;
            _id = id;
        }

        // execute method realization
        public override ICommandCommonResult Execute()
        {
            ICommandCommonResultData<Guid> result = new CommandResult<Guid>();
            // Transform from DTO type to MODEL type
            TEntity itemToUpdate = _translator.Translate(_dto);
            //Repository.
            try
            {
                _repository.Update(itemToUpdate, _id);
                _repository.Save();
                result.Result = true;
                result.Message = "DB: Item updated successfully";
                result.Data = itemToUpdate.GuidId;
            }
            catch
            {
                result.Result = false;
                result.Message = "DB: Item update Error";
            } 
            return result;
        }
    }
}