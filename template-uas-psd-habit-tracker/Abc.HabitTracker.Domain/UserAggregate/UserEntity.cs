using System;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.UserAggregate
{
    public class UserEntity
    {
        public Guid ID { get; private set; }
        public List<BadgeVO> Badges { get; set; }

        public UserEntity(Guid ID)
        {
            this.ID = ID;
        }

        public override bool Equals(object obj)
        {
            var user = obj as UserEntity;
            if (user == null) return false;

            return this.ID == user.ID;
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