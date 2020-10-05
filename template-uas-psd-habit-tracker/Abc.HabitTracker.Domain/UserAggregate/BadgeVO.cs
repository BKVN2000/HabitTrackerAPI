using System;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.UserAggregate
{
    //sebenernya mau ganti nama ko tp males gantinya hehe tp ini badge Entity kok
    public class BadgeVO
    {
        public Guid ID {get;private set;}
        public string Name {get;private set;}
        public string Description {get;private set;}
        public DateTime created_at {get;private set;}

        public BadgeVO(Guid ID,string Name,string Description,DateTime created_at){
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.created_at = created_at;
        }

        public override bool Equals(object obj)
        {
            var badge = obj as BadgeVO;
            if (badge == null) return false;

            return this.ID == badge.ID;
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