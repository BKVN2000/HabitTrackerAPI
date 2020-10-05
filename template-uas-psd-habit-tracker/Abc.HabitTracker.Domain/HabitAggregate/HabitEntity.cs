using System;
using System.Linq;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.HabitAggregate
{
    public class HabitEntity
    {
        private Guid _habitId;
        private Guid _userID;
        private string _name;
        private List<HabitLog> _logs;
        private List<DayOff> _dayOffs;
        private Streak _currentStreak;
        private Streak _longestStreak;
        private DateTime _createdAt;
        public Guid User { get; private set; }
        public Guid ID
        {
            get
            {
                return _habitId;
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public List<DayOff> DayOffs
        {
            get
            {
                return _dayOffs;
            }
        }
        public string[] DayoffsToString
        {
            get
            {
                return (string[])_dayOffs.Select(d => d.DayName).ToArray();
            }
        }
        public List<HabitLog> Logs
        {
            get
            {
                return _logs;
            }
            set
            {
                _logs = value;
            }
        }

        public Streak CurrentStreak
        {
            get
            {
                return _currentStreak;
            }
        }
        public Streak LongestStreak
        {
            get
            {
                return _longestStreak;
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return _createdAt;
            }
        }

        public int LogCount
        {
            get
            {
                return _logs.Count;
            }
        }
        public HabitEntity(Guid ID, string name, List<string> dayOffs, Streak longestStreak,
            Streak currentStreak, DateTime CreatedAt, Guid userID)
        {
            if (name.Length < 2 || name.Length > 100)
                throw new Exception("Name Invalid");

            this._habitId = ID;
            this._dayOffs = CreateDaysOff(dayOffs);
            this._name = name;
            this._currentStreak = currentStreak;
            this._longestStreak = longestStreak;
            this._createdAt = CreatedAt;
            this.User = userID;
        }
        public static HabitEntity NewHabit(string name, List<string> DaysOffs, Guid userID)
        {
            return new HabitEntity(Guid.NewGuid(), name, DaysOffs, new Streak(),
            new Streak(), DateTime.Now, userID);
        }
        public void IncrementStreak()
        {
            this._currentStreak = this._currentStreak.IncreaseStreak();
        }
        public void CreateStreak()
        {
            this._currentStreak = this._currentStreak.InitializeStreak();
        }
        public HabitLog GetNewestLog()
        {
            return this._logs.OrderByDescending(t => t.log).FirstOrDefault();
        }

        public HabitLog CreateLogHabit()
        {
            HabitLog habitLog = null;
            if (_dayOffs.Any(x => x.Day == (int)DateTime.Now.DayOfWeek))
            {
                habitLog = new HabitLog(DateTime.Now, true);
            }
            else
            {
                habitLog = new HabitLog(DateTime.Now, false);
            }
            _logs.Add(habitLog);
            return habitLog;
        }
        private List<DayOff> CreateDaysOff(List<string> days)
        {
            if (days.Count >= 7)
                throw new Exception("Too much Daysoff");

            List<DayOff> daysOff = new List<DayOff>();

            foreach (string day in days)
            {
                var duplicateItem = daysOff
                    .FirstOrDefault(x => x.DayName.Contains(day));

                if (duplicateItem != null)
                    throw new Exception("Duplicate Day");

                daysOff.Add(new DayOff(day));
            }

            return daysOff;
        }

        public override bool Equals(object obj)
        {
            var habit = obj as HabitEntity;
            if (habit == null) return false;

            return this.ID == habit.ID;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + ID.GetHashCode();
                return hash;
            }
        }
    }
}