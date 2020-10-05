using System;
using System.Collections.Generic;


namespace Abc.HabitTracker.Infrastructure.DTOModel
{
    public class HabitDTO
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Guid UserID { get; set; }
        public List<String> DayOff { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CurrentStreak {get;set;}
        public int LongestStreak {get;set;}
        public List<DateTime> LogHabits {get;set;}
        public int LogCount {get;set;}
    }
}