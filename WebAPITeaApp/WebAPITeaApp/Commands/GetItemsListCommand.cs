using AutoMapper;
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
    public class GetItemsListCommand<TEntity, TDto> : Command
        where TEntity : Entity
        where TDto : EntityDto
    {
        private TEntity _model { get; set; }
        private TDto _dto { get; set; }
        private DbRepositorySQL<TEntity> _repository { get; set; }
        private readonly AutomapperTranslator<TEntity,TDto> _translator;


        public GetItemsListCommand(TEntity model, TDto dto, DbRepositorySQL<TEntity> rep, AutomapperTranslator<TEntity, TDto> trans)
        {
            _model = model;
            _dto = dto;
            _repository = rep;
            _translator = trans;
        }

        // execute method realization
        public override ICommandCommonResult Execute()
        {
            ICommandCommonResultData < List <TDto>> result = new CommandResult<List<TDto>>();
            result.Data = new List<TDto>();
            IEnumerable<TEntity> bufferList = new List<TEntity>();
           try
            {
                result.Result = true;
                result.Message = "DB: data from table invoked successfully";
                bufferList = _repository.GetList();
                //result.Data = Repository.GetList();
                foreach (TEntity elem in bufferList)
                {
                    TDto itemDto = _translator.Translate(elem);
                    result.Data.Add(itemDto);
                }
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