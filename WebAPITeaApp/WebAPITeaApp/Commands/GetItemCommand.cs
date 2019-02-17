using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPITeaApp.Dto;
using WebAPITeaApp.Models.DB;
using WebAPITeaApp.Repository;
using WebAPITeaApp.Servicies.Translators;
using WebAPITeaApp.Translators;


namespace WebAPITeaApp.Commands
{
    public class GetItemCommand<TEntity, TDto> : Command
        where TEntity : Entity
        where TDto : EntityDto
    {
        private TEntity _model { get; set; }
        private TDto _dto { get; set; }
        private DbRepositorySQL<TEntity> _repositosry { get; set; }
        private readonly AutomapperTranslator<TEntity, TDto> _translator;
        private Guid _id { get; set; }

        public GetItemCommand(TEntity model, TDto dto, DbRepositorySQL<TEntity> rep, AutomapperTranslator<TEntity, TDto> trans, Guid id)
        {
            _model = model;
            _dto = dto;
            _repositosry = rep;
            _translator = trans;
            _id = id;
        }

        //execute method overriding
        public override ICommandCommonResult Execute()
        {
            ICommandCommonResultData<TDto> result = new CommandResult<TDto>();
           
            try
            {
                var bufferEntity = _repositosry.GetNote(_id);
                result.Data = _translator.Translate(bufferEntity);
                result.Result = true;
                result.Message = "DB: data from table invoked successfully";
            }
            catch
            {
                result.Result = false;
                result.Message = "DB: data  Error";
            }

            return result;
        }
    }
}