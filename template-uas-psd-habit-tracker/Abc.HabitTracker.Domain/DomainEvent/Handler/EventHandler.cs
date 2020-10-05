using System;
using Abc.HabitTracker.Domain.UserAggregate;
using Abc.HabitTracker.Domain.HabitAggregate;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.DomainEvent
{
    public abstract class StreakCreatedHandler 
    {
        protected IUserRepository _userRepository {get; private set;}
        protected IHabitRepository _habitRepository {get; private set;}
        public StreakCreatedHandler(IUserRepository repo, IHabitRepository habitRepo)
        {
            _userRepository = repo;
            _habitRepository = habitRepo;
        }
        public abstract void Update(StreakCreated ev);
    }
}