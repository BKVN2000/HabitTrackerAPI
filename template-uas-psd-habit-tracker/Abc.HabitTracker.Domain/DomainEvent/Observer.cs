using System;
using System.Collections.Generic;

namespace Abc.HabitTracker.Domain.DomainEvent
{
    public interface IObserver<T>
    {        
        void Update(T ev);
    }
    
    public interface IObserveable<T> 
    {        
        void Attach (IObserver<T> obs);
        void Broadcast(T ev);
    }
}
