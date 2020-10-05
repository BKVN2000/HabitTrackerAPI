using System;
using System.Collections.Generic;
using Abc.HabitTracker.Domain.HabitAggregate;
using Abc.HabitTracker.Domain.UserAggregate;

namespace Abc.HabitTracker.Domain.DomainEvent.Handler
{
    public class DominatingHandler : StreakCreatedHandler
    {
        public DominatingHandler(IUserRepository repo,IHabitRepository habitrepo) : base(repo,habitrepo) { }
        
        public override void Update(StreakCreated ev)
        {
            List<BadgeVO> badges = _userRepository.GetALLBadgeAchieved(ev.User);
            if (badges.Exists(x => x.Name.Equals("Dominating"))) return;
            if (ev.GetStreak() < 4) return;

            _userRepository.InsertBadge(ev.User,"Dominating");
        }
    }
}