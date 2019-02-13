using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebAPITeaApp.Dto;
using WebAPITeaApp.Models.DB;
using WebAPITeaApp.Repository;

namespace WebAPITeaApp.Commands
{
    public class UpdateItemCommand<DTO, MODEL> : Command
        where DTO : EntityDto
        where MODEL : Entity
    {
        public DTO Dto { get; set; }
        public MODEL Model { get; set; }
        public DbRepositorySQL<MODEL> Repository { get; set; }
        public Guid Id { get; set; }

        public UpdateItemCommand(DTO dto, MODEL model, DbRepositorySQL<MODEL> rep, Guid id)
        {
            Dto = dto;
            Model = model;
            Repository = rep;
            Id = id;
        }

        // execute method realization
        public override ICommandCommonResult Execute()
        {
            ICommandCommonResultData<Guid> result = new CommandResult<Guid>();
            // Transform from DTO type to MODEL type
            MODEL itemToUpdate = Mapper.Map<DTO, MODEL>(Dto);
            //Repository.
            try
            {
                Repository.Update(itemToUpdate, Id);
                Repository.Save();
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