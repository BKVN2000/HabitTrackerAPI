using System;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.HabitAggregate
{
    public class HabitLog  
    {
        public Guid ID {get; private set;}
        public bool IsDayOff {get;private set;}     
        private DateTime _log;
        public DateTime log
        {
            get
            {
                return _log;
            }
        }
        public HabitLog(DateTime dateLog,bool isDayOff) : this(Guid.NewGuid(), dateLog, isDayOff) { }
        public HabitLog(Guid ID,DateTime dateLog,bool isDayOff)
        {
            this.ID = ID;
            this._log = dateLog;
            this.IsDayOff = isDayOff;
        }

        public override bool Equals(object obj)
        {
            var HabitLog = obj as HabitLog;
            if (HabitLog == null) return false;

            return this.ID == HabitLog.ID;
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