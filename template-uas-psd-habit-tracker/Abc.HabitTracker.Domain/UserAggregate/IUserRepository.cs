using System;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.UserAggregate
{
    public interface IUserRepository
    {
        UserEntity FindByID(Guid UserID);
        List<BadgeVO> GetALLBadgeAchieved(Guid UserID);
        void InsertBadge(Guid UserID,string BadgeName);
    }
}