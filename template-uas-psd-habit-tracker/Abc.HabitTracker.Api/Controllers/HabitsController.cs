using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Abc.HabitTracker.Domain;
using Abc.HabitTracker.Infrastructure;
using Abc.HabitTracker.Infrastructure.DTOModel;
using Abc.HabitTracker.Domain.HabitAggregate;
using Abc.HabitTracker.Domain.UserAggregate;
using Abc.HabitTracker.Domain.Services;
using Abc.HabitTracker.Domain.DomainEvent;
using Abc.HabitTracker.Domain.DomainEvent.Handler;
namespace Abc.HabitTracker.Api.Controllers
{
    [ApiController]
    public class HabitsController : ControllerBase
    {
        private readonly ILogger<HabitsController> _logger;
        private readonly IHabitRepository _habitRepository;
        private readonly IUserRepository _userRepository;
        private readonly InserLogHabitService _InsertLogHabitService;
        public HabitsController(ILogger<HabitsController> logger)
        {
            PostGresUnitOfWork postGresUnitOfWork = new PostGresUnitOfWork();

            _habitRepository = postGresUnitOfWork.HabitRepository;
            _userRepository = postGresUnitOfWork.UserRepository;
            _InsertLogHabitService = new InserLogHabitService(_habitRepository);

            _logger = logger;
        }

        [HttpGet("api/v1/users/{userID}/habits")]
        public ActionResult<IEnumerable<Habit>> All(Guid userID)
        {
            UserEntity user = _userRepository.FindByID(userID);

            if (user == null)
                return NotFound("User not found");

            List<HabitEntity> habits = _habitRepository.FindAllByUserId(userID);
            List<Habit> habitits = new List<Habit>();

            habitits = habits.Select(HabitEntity => new Habit()
            {
                ID = HabitEntity.ID,
                Name = HabitEntity.Name,
                Logs = (List<DateTime>)HabitEntity.Logs.Select(t => t.log).ToList(),
                UserID = userID,
                CreatedAt = HabitEntity.CreatedAt,
                LogCount = HabitEntity.LogCount,
                DaysOff = HabitEntity.DayoffsToString,
                CurrentStreak = HabitEntity.CurrentStreak.value,
                LongestStreak = HabitEntity.LongestStreak.value
            }).ToList();

            return habitits;
        }

        [HttpGet("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habit> Get(Guid userID, Guid id)
        {
            //mock only. replace with your solution
            UserEntity user = _userRepository.FindByID(userID);

            if (user == null)
                return NotFound("User not found");

            HabitEntity habitEntity = _habitRepository.FindByUserId(userID,id); 
             if (habitEntity == null)
              return NotFound("habit not found");
            
             return new Habit()
              {
                ID = habitEntity.ID,
                Name = habitEntity.Name,
                Logs = (List<DateTime>)habitEntity.Logs.Select(t => t.log).ToList(),
                UserID = userID,
                CreatedAt = habitEntity.CreatedAt,
                LogCount = habitEntity.LogCount,
                DaysOff = habitEntity.DayoffsToString,
                CurrentStreak = habitEntity.CurrentStreak.value,
                LongestStreak = habitEntity.LongestStreak.value
              };
        }

        [HttpPost("api/v1/users/{userID}/habits")]
        public ActionResult<Habit> AddNewHabit(Guid userID, [FromBody] RequestData data)
        {
            UserEntity user = _userRepository.FindByID(userID);
            if (user == null)
                return NotFound("user not found");

            try
            {
              HabitEntity habitEntity = HabitFactory.CreateNew(data.Name, data.DaysOff.ToList(), userID);
              HabitEntity newHabitEntity = _habitRepository.Create(user, habitEntity);
              
              return new Habit()
              {
                  ID = habitEntity.ID,
                  Name = habitEntity.Name,
                  Logs = new List<DateTime>(),
                  LogCount = 0,
                  DaysOff = habitEntity.DayoffsToString,
                  UserID = userID,
                  CreatedAt = habitEntity.CreatedAt,
                  CurrentStreak = habitEntity.CurrentStreak.value,
                  LongestStreak = habitEntity.LongestStreak.value
              };
            }
            catch(Exception ex){
                throw (ex);
            }
        }

        [HttpPut("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habit> UpdateHabit(Guid userID, Guid id, [FromBody] RequestData data)
        {
            //mock only. replace with your solution
            UserEntity user = _userRepository.FindByID(userID);
            if (user == null)
                return NotFound("user not found");

            HabitEntity habitEntity = _habitRepository.FindByUserId(userID,id);
            
            if (habitEntity == null)
              return NotFound("habit not found");

            HabitEntity newHabitEntity = new HabitEntity(habitEntity.ID,data.Name,data.DaysOff.ToList(),
              habitEntity.LongestStreak,habitEntity.CurrentStreak,DateTime.Now,userID);

            HabitEntity updatedHabitEntity = _habitRepository.Update(newHabitEntity);

            updatedHabitEntity = _habitRepository.FindByUserId(userID,id);
            return new Habit()
            {
                ID = updatedHabitEntity.ID,
                Name = updatedHabitEntity.Name,
                Logs = (List<DateTime>)updatedHabitEntity.Logs.Select(t => t.log).ToList(),
                LogCount = updatedHabitEntity.LogCount,
                DaysOff = updatedHabitEntity.DayoffsToString,
                UserID = userID,
                CreatedAt = updatedHabitEntity.CreatedAt,
                CurrentStreak = updatedHabitEntity.CurrentStreak.value,
                LongestStreak = updatedHabitEntity.LongestStreak.value
            };
        }

        [HttpDelete("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habit> DeleteHabit(Guid userID, Guid id)
        {
            //mock only. replace with your solution
            UserEntity user = _userRepository.FindByID(userID);
            if (user == null)
                return NotFound("user not found");

            HabitEntity habitEntity = _habitRepository.FindByUserId(userID,id);            
            if (habitEntity == null)
              return NotFound("habit not found");

            HabitEntity deletedHabitEntity = _habitRepository.Delete(habitEntity);

            return new Habit()
            {
                ID = deletedHabitEntity.ID,
                Name = deletedHabitEntity.Name,
                DaysOff = deletedHabitEntity.DayoffsToString,
                UserID = userID,
                CreatedAt = deletedHabitEntity.CreatedAt
            };
        }

        [HttpPost("api/v1/users/{userID}/habits/{id}/logs")]
        public ActionResult<Habit> Log(Guid userID, Guid id)
        {
            UserEntity user = _userRepository.FindByID(userID);
            if (user == null)
                return NotFound("user not found");

            HabitEntity habitEntity = _habitRepository.FindByUserId(userID,id);            
            if (habitEntity == null)
              return NotFound("habit not found");
            
            _InsertLogHabitService.InsertLog(habitEntity);
            
            using (DomainEvents.Register<StreakCreated>(e => new DominatingHandler(_userRepository,_habitRepository).Update(e)))
            {
                DomainEvents.Raise(new StreakCreated(habitEntity.User,habitEntity.ID,_habitRepository));
            }
            using (DomainEvents.Register<StreakCreated>(e => new WorkaholicHandler(_userRepository,_habitRepository).Update(e)))
            {
                DomainEvents.Raise(new StreakCreated(habitEntity.User,habitEntity.ID,_habitRepository));
            }
            using (DomainEvents.Register<StreakCreated>(e => new EpicComebackHandler(_userRepository,_habitRepository).Update(e)))
            {
                DomainEvents.Raise(new StreakCreated(habitEntity.User,habitEntity.ID,_habitRepository));
            }
            habitEntity = _habitRepository.FindByUserId(userID,id);      
            return new Habit()
            {
                ID = habitEntity.ID,
                Name = habitEntity.Name,
                Logs = (List<DateTime>)habitEntity.Logs.Select(t => t.log).ToList(),
                LogCount = habitEntity.LogCount,
                DaysOff = habitEntity.DayoffsToString,
                UserID = userID,
                CreatedAt = habitEntity.CreatedAt,
                CurrentStreak = habitEntity.CurrentStreak.value,
                LongestStreak = habitEntity.LongestStreak.value
            };
        }

        //mock data only. remove later
        private static readonly Guid AmirID = Guid.Parse("4fbb54f1-f340-441e-9e57-892329464d56");
        private static readonly Guid BudiID = Guid.Parse("0b54c1fe-a374-4df8-ba9a-0aa7744a4531");

        //mock data only. remove later
        private static readonly Habit habitAmir1 = new Habit
        {
            ID = Guid.Parse("fd725b05-a221-461a-973c-4a0899cee14d"),
            Name = "baca buku",
            UserID = AmirID
        };

        //mock data only. remove later
        private static readonly Habit habitAmir2 = new Habit
        {
            ID = Guid.Parse("01169031-752e-4c52-822c-a04d290438ea"),
            Name = "code one simple app prototype",
            DaysOff = new[] { "Sat", "Sun" },
            UserID = AmirID
        };

        //mock data only. remove later
        private static readonly Habit habitBudi1 = new Habit
        {
            ID = Guid.Parse("05fb5a61-aa1f-4a96-b952-378bf73ca713"),
            Name = "100 push-ups, 100 sit-ups, 100 squats",
            LongestStreak = 100,
            CurrentStreak = 10,
            LogCount = 123,
            UserID = BudiID
        };
    }
}
