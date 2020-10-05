using System;
using System.Linq;
using System.Collections.Generic;
using Abc.HabitTracker.Domain.UserAggregate;
using Abc.HabitTracker.Domain.HabitAggregate;

namespace Abc.HabitTracker.Domain.DomainEvent
{
    public class EpicComebackHandler : StreakCreatedHandler
    {
        public EpicComebackHandler(IUserRepository repo,IHabitRepository habitrepo) : base(repo,habitrepo) { }
        public override void Update(StreakCreated ev)
        {
            DateTime start = DateTime.Now;
            List<BadgeVO> badges = _userRepository.GetALLBadgeAchieved(ev.User);
            if (badges.Exists(x => x.Name.Equals("Epic Comeback"))) return;
            if (ev.GetStreak() < 10) return;
            
            HabitEntity habitET = _habitRepository.FindByUserId(ev.User,ev.Habit);
            List<HabitLog> logs = habitET.Logs;
            if (logs.Count() < 10) return;
            
            if (logs.Count == 10)
                start = habitET.CreatedAt;

            List<HabitLog> Firstlogs = _habitRepository.FindFirstLogStreakAndBeforeStreak(ev.User,ev.Habit);
            start = (Firstlogs.Count == 2) ? Firstlogs.ElementAt(1).log : start;

            if (MissingStreakGreaterThanEquals10(start,logs.ElementAt(0).log) >= 10)
                 _userRepository.InsertBadge(ev.User,"Epic Comeback");
        }

        private int MissingStreakGreaterThanEquals10(DateTime start,DateTime end)
        {
            return (int)(end-start).TotalDays;
        }
    }
}