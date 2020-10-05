using System;
using System.Collections.Generic;
using Abc.HabitTracker.Domain.UserAggregate;
using Abc.HabitTracker.Domain.HabitAggregate;

namespace Abc.HabitTracker.Domain.DomainEvent.Handler
{
    public class WorkaholicHandler : StreakCreatedHandler
    {
         public WorkaholicHandler(IUserRepository repo,IHabitRepository habitrepo) : base(repo,habitrepo) { }
         public override void Update(StreakCreated ev)
         {
            List<BadgeVO> badges = _userRepository.GetALLBadgeAchieved(ev.User);
            if (badges.Exists(x => x.Name.Equals("Workaholic"))) return;

            Console.WriteLine("IIIIIII");
            List<HabitLog> logs = _habitRepository.FindAllLogsOnHoliday(ev.Habit,ev.User);
                Console.WriteLine("{0}",logs.Count);
            if (logs.Count < 10) return;

            Console.WriteLine("{0}",logs.Count);
            _userRepository.InsertBadge(ev.User,"Workaholic");
         }
    }
}