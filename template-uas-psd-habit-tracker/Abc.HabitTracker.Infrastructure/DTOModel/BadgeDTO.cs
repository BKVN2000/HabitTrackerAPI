using System;
using System.Collections.Generic;

namespace Abc.HabitTracker.Infrastructure.DTOModel
{
    public class BadgeDTO
    {
        public Guid ID {get;set;}
        public string Name {get;set;}
        public string Descripton {get;set;}       
        public DateTime CreatedAt {get;set;}  
    }
}