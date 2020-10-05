using System;
using System.Collections.Generic;


namespace Abc.HabitTracker.Domain.HabitAggregate
{
    public class HabitFactory
    {

        public static HabitEntity CreateNew(string name, List<string> DaysOffs, Guid userID)
        {       
              return new HabitEntity(Guid.NewGuid(), name, DaysOffs, new Streak(),
            new Streak(), DateTime.Now, userID);
        }
    
    }
}