using System;
using System.Collections.Generic;

namespace Abc.HabitTracker.Infrastructure.DTOModel
{
    public class UserDTO
    {
        public Guid ID {get;set;}
        public List<BadgeDTO> badges {get;set;}
    }
}