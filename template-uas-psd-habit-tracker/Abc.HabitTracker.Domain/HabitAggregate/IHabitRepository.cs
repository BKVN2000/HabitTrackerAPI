using System;
using System.Collections.Generic;
using Abc.HabitTracker.Domain.UserAggregate;

namespace Abc.HabitTracker.Domain.HabitAggregate
{
    public interface IHabitRepository
    {
        List<HabitEntity> FindAllByUserId(Guid UserId);
        HabitEntity FindByUserId(Guid UserId,Guid HabitID);
        HabitEntity Create(UserEntity user,HabitEntity habitEntity);
        HabitEntity Update(HabitEntity habitEntity);
        HabitEntity Delete(HabitEntity habitEntity);
        HabitLog InsertLog(HabitEntity habitEntity, HabitLog habitLog);
        void InsertNewStreak (HabitEntity habitEntity,DateTime firstLog);
        void UpdateCurrentStreak(HabitEntity habitEntity,int streak);
        List<HabitLog> FindAllLogsOnHoliday(Guid habitID,Guid userID);    
        List<HabitLog> FindFirstLogStreakAndBeforeStreak(Guid Habit, Guid UserID);
    }
}