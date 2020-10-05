using System;

namespace Abc.HabitTracker.Infrastructure
{
    public interface UnitOfWork : IDisposable
    {
        void Commit();
        void Rollback();
    }
}