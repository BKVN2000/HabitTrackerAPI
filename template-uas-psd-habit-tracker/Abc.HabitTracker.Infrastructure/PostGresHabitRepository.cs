using System;
using System.Linq;
using System.Collections.Generic;

using Npgsql;
using NpgsqlTypes;
using Abc.HabitTracker.Infrastructure.DTOModel;
using Abc.HabitTracker.Domain.HabitAggregate;
using Abc.HabitTracker.Domain.UserAggregate;

namespace Abc.HabitTracker.Infrastructure
{
    public class PostGresHabitRepository : IHabitRepository
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public PostGresHabitRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public List<HabitEntity> FindAllByUserId(Guid Userid)
        {
            List<HabitEntity> habits = new List<HabitEntity>();
            string query = @"SELECT a.*,(SELECT COALESCE(MAX(streak),0) FROM streak WHERE streak.habit_id = a.id) 
            as Maximum_streak,(SELECT COALESCE(streak,0) FROM streak WHERE DATE(last_update_at) = CURRENT_DATE 
            AND streak.habit_id = a.id ORDER BY last_update_at DESC LIMIT 1) as Current_streak 
            FROM ""Habit"" a WHERE ""user_id"" = @userID AND ""deleted_at"" is null";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("userID", Userid);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Guid ID = reader.GetGuid(0);
                        string Name = reader.GetString(1);
                        DateTime CreatedAt = (DateTime) reader.GetValue(2);
                        Guid UserID = reader.GetGuid(3);
                        List<string> DayOff = new List<string>((String[])reader.GetValue(4));
                        Streak longestStreak =(reader.IsDBNull(6)) ? new Streak(0) : new Streak(reader.GetInt32(6));
                        Streak currentStreak = (reader.IsDBNull(7)) ? new Streak(0) : new Streak(reader.GetInt32(7));

                        habits.Add(new HabitEntity(ID, Name, DayOff, longestStreak, currentStreak, CreatedAt,UserID));
                    }
                }
            }

            if (habits.Count > 0)
            {
                foreach (HabitEntity habitEntity in habits)
                {
                    habitEntity.Logs = GetLogHabits(habitEntity);
                }
            }
           
            return habits;
        }

        public List<HabitLog> GetLogHabits(HabitEntity habitEntity)
        {
            List<HabitLog> habitlogs = new List<HabitLog>();
            
            string query = "SELECT \"id\",\"created_at\",\"isDayOff\" FROM log_habit WHERE habit_id = @habitId";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitId", habitEntity.ID);
                
               
                NpgsqlDataReader reader = cmd.ExecuteReader();
               
                while (reader.Read())
                {    
                    Guid ID = reader.GetGuid(0);
                    DateTime dateTime =(DateTime)reader.GetValue(1);
                    bool isDayOff = reader.GetBoolean(2);

                    habitlogs.Add(new HabitLog(ID, dateTime, isDayOff)); 
      
                }
                reader.Close();
            }
           
            return habitlogs;
        }

        public HabitEntity Create(UserEntity user, HabitEntity habitEntity)
        { 
            string[] dayOffs = habitEntity.DayoffsToString;

            string query = @"INSERT INTO ""Habit"" (id,name,created_at,user_id,day_off,deleted_at) 
            VALUES (@id,@name,@created_at,@user_id,@day_off,@deleted_at)";
            try
            {
                using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
                {
                    cmd.Parameters.AddWithValue("id", habitEntity.ID);
                    cmd.Parameters.AddWithValue("name", habitEntity.Name);
                    cmd.Parameters.AddWithValue("created_at", habitEntity.CreatedAt);
                    cmd.Parameters.AddWithValue("user_id", user.ID);
                    cmd.Parameters.AddWithValue("day_off", ((object)dayOffs) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("deleted_at",  DBNull.Value);
                    
                    cmd.ExecuteNonQuery();
                    _transaction.Commit();
                }
                return habitEntity;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public HabitEntity FindByUserId(Guid UserId, Guid HabitID)
        {
            HabitEntity habitEntity = null;
            string query = @"SELECT a.*,(SELECT COALESCE(MAX(streak),0) FROM streak WHERE streak.habit_id = a.id) 
            as Maximum_streak,(SELECT COALESCE(streak,0) FROM streak WHERE DATE(last_update_at) = CURRENT_DATE 
            AND streak.habit_id = a.id ORDER BY last_update_at DESC LIMIT 1) as Current_streak 
            FROM ""Habit"" a WHERE ""user_id"" = @userID AND  ""id""  = @HabitId AND ""deleted_at"" is null LIMIT 1";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {                
                cmd.Parameters.AddWithValue("userID", UserId);
                cmd.Parameters.AddWithValue("HabitId", HabitID);      

                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Guid ID = reader.GetGuid(0);
                    string Name = reader.GetString(1);
                    DateTime CreatedAt = (DateTime) reader.GetValue(2);
                    Guid UserID = reader.GetGuid(3);
                    List<string> DayOff = new List<string>((String[])reader.GetValue(4));
                    Streak longestStreak = (reader.IsDBNull(6)) ? new Streak(0) : new Streak(reader.GetInt32(6));
                    Streak currentStreak =(reader.IsDBNull(7)) ? new Streak(0) : new Streak(reader.GetInt32(7));

                    habitEntity = new HabitEntity(ID, Name, DayOff, longestStreak, currentStreak, CreatedAt,UserID);
                }
                reader.Close();
            }
            if (habitEntity != null)
            {
                habitEntity.Logs = GetLogHabits(habitEntity);
            }
                

            return habitEntity;
        }
        public HabitEntity Update(HabitEntity habitEntity)
        {
            string[] dayOffs = habitEntity.DayoffsToString;

            string query = @"UPDATE ""Habit"" SET 
                name = @name, day_off = @dayOffs WHERE id = @HabitID";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("name", habitEntity.Name);
                cmd.Parameters.AddWithValue("dayOffs", ((object)dayOffs) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("HabitID", habitEntity.ID);

                cmd.ExecuteNonQuery();

                _transaction.Commit();
            }
            return habitEntity;
        }
        public HabitEntity Delete(HabitEntity habitEntity)
        {
            //softdelete
            NpgsqlDateTime deletedAt = new NpgsqlDateTime(0);
            string query = @"UPDATE ""Habit"" SET ""deleted_at"" = @DeletedAt WHERE id = @HabitId";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("DeletedAt", deletedAt);
                cmd.Parameters.AddWithValue("HabitId", habitEntity.ID);

                cmd.ExecuteNonQuery();

                _transaction.Commit();
            }
            return habitEntity;
        }

        public HabitLog InsertLog(HabitEntity habitEntity, HabitLog habitLog)
        {            
            string query = @"INSERT INTO log_habit VALUES (@id,@userID,@HabitID,@CreatedAt,@IsDayOff)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id",habitLog.ID);
                cmd.Parameters.AddWithValue("userID", habitEntity.User);
                cmd.Parameters.AddWithValue("HabitID", habitEntity.ID);
                cmd.Parameters.AddWithValue("CreatedAt", habitLog.log);
                cmd.Parameters.AddWithValue("IsDayOff", habitLog.IsDayOff);
                cmd.ExecuteNonQuery();

                 _transaction.Commit();
            }
            
            return habitLog;
        }
        
        public void InsertNewStreak (HabitEntity habitEntity,DateTime firstLog)
        {    
            _transaction = _connection.BeginTransaction();      
            Guid ID = new Guid();
            string query = "";
           

            query = @"SELECT id FROM log_habit ORDER BY created_at DESC LIMIT 2 OFFSET 1";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("createdAt",firstLog);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if(reader.Read())
                {
                    ID = reader.GetGuid(0);
                }
                   
                reader.Close();
            }

            query = @"INSERT INTO ""streak"" (id,user_id,habit_id,streak,first_log_habit_id)
            VALUES (@id,@userID,@HabitID,@streak,@first_log_habit_id)";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id",Guid.NewGuid());
                cmd.Parameters.AddWithValue("userID", habitEntity.User);
                cmd.Parameters.AddWithValue("HabitID", habitEntity.ID);
                cmd.Parameters.AddWithValue("streak", 2);
                cmd.Parameters.AddWithValue("first_log_habit_id",ID);

                cmd.ExecuteNonQuery();
            }
            _transaction.Commit();
        }

        public void UpdateCurrentStreak(HabitEntity habitEntity,int streak)
        {
            
             _transaction = _connection.BeginTransaction();  
            string query = @"UPDATE ""streak"" SET streak = @streakHabit, last_update_at = CURRENT_DATE
                WHERE habit_id = @habitID AND user_id = @userID";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("userID", habitEntity.User);
                cmd.Parameters.AddWithValue("habitID", habitEntity.ID);
                cmd.Parameters.AddWithValue("streakHabit", streak);
                cmd.ExecuteNonQuery();         
            }
             _transaction.Commit();
        } 
        public List<HabitLog> FindAllLogsOnHoliday(Guid habitID,Guid userID)
        {
            List<HabitLog> habitLogs = new List<HabitLog>();
            string query = @"SELECT id,created_at,""isDayOff"" FROM log_habit WHERE 
            habit_id = @habitId AND user_id = @userID AND ""isDayOff"" = true";
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitId", habitID);
                cmd.Parameters.AddWithValue("userID", userID);
               
                NpgsqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Guid ID = reader.GetGuid(0);
                    DateTime dateTime = reader.GetDateTime(1);
                    bool isDayOff = reader.GetBoolean(2);

                    habitLogs.Add(new HabitLog(ID, dateTime, isDayOff));
                }
                
                reader.Close();
            }
            return habitLogs;
        }
        public List<HabitLog> FindFirstLogStreakAndBeforeStreak(Guid Habit, Guid User)
        {
            DateTime dateTime = DateTime.Now;
            string query = @"SELECT id, created_at, ""isDayOff"" FROM log_habit WHERE id IN 
            (SELECT first_log_habit_id FROM streak WHERE habit_id = @HabitID AND user_id = @userID AND 
                DATE(last_update_at) = CURRENT_DATE LIMIT 1)";
            
            List<HabitLog> habitLog = new List<HabitLog>();
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitID", Habit);
                cmd.Parameters.AddWithValue("UserID", User);
                
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                { 
                    Guid ID = reader.GetGuid(0);
                    dateTime = reader.GetDateTime(1);
                    bool isDayOff = reader.GetBoolean(2);
                    habitLog.Add(new HabitLog(ID, dateTime, isDayOff));
                }
                reader.Close();
            }
            
            query = @"SELECT id, created_at, ""isDayOff"" FROM log_habit WHERE
                Date(created_at) < Date(firstLog) ORDER BY created_at DESC LIMIT 1";
            
            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("firstLog", dateTime);
                
                NpgsqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                { 
                    Guid ID = reader.GetGuid(0);
                    dateTime = reader.GetDateTime(1);
                    bool isDayOff = reader.GetBoolean(2);
                    habitLog.Add(new HabitLog(ID, dateTime, isDayOff));
                }
                reader.Close();
            }

            return habitLog;
        }  
    }
}