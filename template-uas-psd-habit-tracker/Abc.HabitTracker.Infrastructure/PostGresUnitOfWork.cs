using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using System.Text.Json;
using System.Text.Json.Serialization;
using Abc.HabitTracker.Domain.HabitAggregate;
using Abc.HabitTracker.Domain.UserAggregate;
using Abc.HabitTracker.Infrastructure;

namespace Abc.HabitTracker.Infrastructure
{
    public class PostGresUnitOfWork : UnitOfWork
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;
        private IHabitRepository _habitRepository;
        private IUserRepository  _userRepository;

        public IHabitRepository HabitRepository
        {
            get
            {
                if (_habitRepository == null)
                {
                    _habitRepository = new PostGresHabitRepository(_connection, _transaction);
                }
                return _habitRepository;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new PostGresUserRepository(_connection, _transaction);
                }
                return _userRepository;
            }
        }

        public PostGresUnitOfWork()
        {
            _connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=AAAaaa123;Database=HabitTracker;Port=5432;Pooling=true;");
            _connection.Open();
            _transaction = _connection.BeginTransaction();            
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _connection.Close();
                }

                disposed = true;
            }
        }
    }
}