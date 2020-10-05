using System;
using System.Collections.Generic;
using Npgsql;
using NpgsqlTypes;
using Abc.HabitTracker.Domain.UserAggregate;
using Abc.HabitTracker.Infrastructure.DTOModel;

namespace Abc.HabitTracker.Infrastructure
{
    public class PostGresUserRepository : IUserRepository
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public PostGresUserRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }
            
        public UserEntity FindByID(Guid Userid)
        {
            UserEntity user = null;
            Guid ID;
            string query = "SELECT * FROM \"User\" WHERE id = @userid";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("userid", Userid);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        ID = reader.GetGuid(0);
                        user = new UserEntity(ID);
                    }
                }
            }
            if (user!= null)
                user.Badges = GetALLBadgeAchieved(user.ID);

            return user;
        }

        public List<BadgeVO> GetALLBadgeAchieved(Guid user)
        {
            List<BadgeVO> badges = new List<BadgeVO>();
            string query = "SELECT a.* ,b.created_at FROM \"Badge\" a JOIN \"BadgeAchievement\" b ON a.id = b.badge_id WHERE b.user_id = @userid";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("userid", user);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Guid ID = reader.GetGuid(0);
                        string Name = reader.GetString(1);
                        string Description = reader.GetString(2);
                        DateTime CreatedAt = reader.GetDateTime(4);

                        badges.Add(new BadgeVO(ID,Name,Description,CreatedAt));
                    }
                }
                reader.Close();
            }
            return badges;
        }

        public void InsertBadge(Guid UserID,string BadgeName)
        {
            Guid badgeID = new Guid();
            string query = "";
            query = @"SELECT ID FROM ""Badge"" WHERE ""name"" = @badgeName";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("badgeName", BadgeName);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    badgeID = reader.GetGuid(0);
                }

                else
                    throw new Exception("Badge Name not Found pls check User Repository on insertBadge method");
                
                reader.Close();
            }

            query = @"INSERT INTO ""BadgeAchievement"" VALUES(@UserID,@BadgeID)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                 _transaction = _connection.BeginTransaction();      
                cmd.Parameters.AddWithValue("UserID",UserID);                
                cmd.Parameters.AddWithValue("BadgeID", badgeID);

                cmd.ExecuteNonQuery();
                _transaction.Commit();
            }
        }
    }
}
