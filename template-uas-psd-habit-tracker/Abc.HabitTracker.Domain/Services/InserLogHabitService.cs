using System;
using System.Linq;
using Abc.HabitTracker.Domain.HabitAggregate;
using Abc.HabitTracker.Domain.DomainEvent;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.Services
{
    public class InserLogHabitService
    {
        IHabitRepository _habitRepository;

        public InserLogHabitService(IHabitRepository habitRepository)
        {
            this._habitRepository = habitRepository;
        }

        public HabitEntity InsertLog(HabitEntity habitEntity)
        {
            //kalau return satu berarti kemarin terakhir bikin log
            HabitLog hl = habitEntity.GetNewestLog();
            DateTime lastLog = (hl== null) ? DateTime.Now : hl.log;
           
            int compareTodayWithLastLog = (int)(DateTime.Now - lastLog).TotalDays;
           
            
            HabitLog Log = habitEntity.CreateLogHabit();
            HabitLog newLogHabit = _habitRepository.InsertLog(habitEntity, Log);

            //lebih dari seminggu atau pertama kali insert log
            if (compareTodayWithLastLog > 7 || compareTodayWithLastLog == 0)
                return habitEntity;

            //kurang dari seminggu tapi dia bolos
            else if (compareTodayWithLastLog != 1 && !CheckDayOffBetweenDate(habitEntity, lastLog))
            {
                return habitEntity;
            }

            //insert new Streak 
            if (habitEntity.CurrentStreak.value == 0)
            {
                habitEntity.CreateStreak();
                _habitRepository.InsertNewStreak(habitEntity, lastLog);
            }

            //update streak yang sedang berjalan
            else if (habitEntity.CurrentStreak.value >= 2)
            {
                habitEntity.IncrementStreak();
                _habitRepository.UpdateCurrentStreak(habitEntity, habitEntity.CurrentStreak.value);
            }

            return habitEntity;
        }

        private bool CheckDayOffBetweenDate(HabitEntity habitEntity, DateTime lastLog)
        {
            if (habitEntity.DayOffs.Count == 0)
                return true;

            int dayNow = (int)habitEntity.GetNewestLog().log.DayOfWeek;
            int dayLastLog = ((int)lastLog.DayOfWeek + 1) % 7;

            while (dayLastLog != dayNow)
            {
                if (!habitEntity.DayOffs.Exists(x => x.Day == dayLastLog))
                    return false;

                dayLastLog = (dayLastLog + 1) % 7;
            }

            return true;
        }
    }
}
