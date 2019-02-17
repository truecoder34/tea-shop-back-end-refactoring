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
    public class CreateItemCommand<TDto, TEntity> : Command
        where TDto : EntityDto
        where TEntity : Entity
    {
        private TDto _dto { get; set; }
        private TEntity _model { get; set; }
        private DbRepositorySQL<TEntity> _repository { get; set; }
        private readonly AutomapperTranslator<TDto, TEntity> _translator;
        

        public CreateItemCommand(TDto dto, TEntity model, DbRepositorySQL<TEntity> rep, AutomapperTranslator<TDto, TEntity> trans)
        {
            _dto = dto;
            _model = model;
            _repository = rep;
            _translator = trans;
        }

        // execute method realization
        public override ICommandCommonResult Execute()
        {
            ICommandCommonResultData<Guid> result = new CommandResult<Guid>();
            // Transform from DTO type to MODEL type
            TEntity itemToCreate = _translator.Translate(_dto);
            //Repository.
            try
            {
                _repository.Create(itemToCreate);
                _repository.Save();
                result.Result = true;
                result.Message = "DB: Item created successfully";
                result.Data = itemToCreate.GuidId;
                // возращать гуид
            }
            catch
            {
                result.Result = false;
                result.Message = "DB: Item creation Error";
            }
           
            return result;
        }       
    }
}