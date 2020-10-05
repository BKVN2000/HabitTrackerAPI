using System;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.HabitAggregate
{
    public class DayOff
    {
        public int Day{get; private set;} 
        public string DayName{get; private set;} 
        private static readonly Dictionary<int, string> dayFormats = new Dictionary<int, string>() {
            { 0, "Sun"},
            { 1, "Mon"},
            { 2, "Tue"},
            { 3, "Wed"},
            { 4, "Thu"},
            { 5, "Fri"},
            { 6, "Sat"},
        };

        public DayOff(string DayName)
        {
            int day = CheckFormatAndGetDay(DayName);
            if (day == -1)
                throw new Exception("Invalid Day Format");

            this.Day = day;
            this.DayName = DayName;
        }

        public int CheckFormatAndGetDay(string dayName)
        {
            if (dayName.Trim() != "")
            {
                foreach (KeyValuePair<int, string> item in dayFormats)
                {
                    if (dayName.Equals(item.Value))
                    {
                        return item.Key;
                    }
                }
            }
            return -1;
        }

        public override bool Equals(object obj)
        {
            var day = obj as DayOff;
            if (day == null) return false;

            return (day.Day == this.Day && day.DayName == this.DayName);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Day.GetHashCode();
                return hash;
            }
        }
    }
}