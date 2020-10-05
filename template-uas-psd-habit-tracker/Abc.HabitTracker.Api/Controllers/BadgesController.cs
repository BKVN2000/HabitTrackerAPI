using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Abc.HabitTracker.Infrastructure;
using Abc.HabitTracker.Infrastructure.DTOModel;

using Abc.HabitTracker.Domain.UserAggregate;

namespace Abc.HabitTracker.Api.Controllers
{
    [ApiController]
    public class BadgesController : ControllerBase
    {
        private readonly ILogger<BadgesController> _logger;
        private readonly IUserRepository _userRepository;
        public BadgesController(ILogger<BadgesController> logger)
        {
            PostGresUnitOfWork postGresUnitOfWork = new PostGresUnitOfWork();
            _userRepository = postGresUnitOfWork.UserRepository;
            _logger = logger;
        }

        [HttpGet("api/v1/users/{userID}/badges")]
        public ActionResult<IEnumerable<Badge>> All(Guid userID)
        {
            UserEntity user = null;
            user = _userRepository.FindByID(userID);

            if (user == null)
                return NotFound("User not found");

            List<Badge> badges = new List<Badge>();
            
            badges = user.Badges.Select(badge => new Badge()
            {
                ID = badge.ID,
                Name = badge.Name,
                Description = badge.Description,
                UserID = userID,
                CreatedAt = badge.created_at
            }).ToList();

            return badges;
        }
    }
}
