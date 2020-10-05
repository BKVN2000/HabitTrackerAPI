using System;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.HabitAggregate
{
    public class Streak
    {
        private int _value;
        public int value
        {
            get
            {
                return _value;
            }
        }

        public Streak()
        {
            this._value = 0;
        }

        public Streak(int value)
        {
            if (value < 0)
            {
                throw new Exception("Streak cannot be negative");
            }
            this._value = value;
        }
        public Streak IncreaseStreak()
        {
            return new Streak(this._value + 1);
        }
        public Streak InitializeStreak()
        {
            return new Streak(2);
        }
        public Streak ResetStreak()
        {
            return new Streak();
        }

        public override bool Equals(object obj)
        {
            var streakValue = obj as Streak;
            if (streakValue == null) return false;

            return this._value == streakValue._value;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _value.GetHashCode();
                return hash;
            }
        }
    }
}