using System;
using Abc.HabitTracker.Domain.HabitAggregate;
using Abc.HabitTracker.Domain.UserAggregate;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.DomainEvent
{
    public class StreakCreated
    {
        public Guid User { get; private set; }
        public Guid Habit { get; private set; }
        public int CurrentStreak {get ;private set;}
        private IHabitRepository _habitRepository;
        public StreakCreated(Guid User,Guid habit,IHabitRepository habitRepository)
        {
            this.User = User;
            this.Habit = habit;
            this._habitRepository = habitRepository;
        }

        public int GetStreak()
        {
            HabitEntity habitEntity = _habitRepository.FindByUserId(User,Habit);

            if (habitEntity == null)
                return 0;

            return habitEntity.CurrentStreak.value;
        }
    }
}