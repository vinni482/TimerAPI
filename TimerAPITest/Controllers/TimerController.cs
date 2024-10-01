using Microsoft.AspNetCore.Mvc;
using TimerAPITest.Models;
using TimerAPITest.Repositories;
using TimerAPITest.Services;

namespace TimerAPITest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimerController : ControllerBase
    {
        private readonly ITimerRepository _timerRepository;

        public TimerController(ITimerRepository timerRepository)
        {
            _timerRepository = timerRepository;
        }

        [HttpPost("SetTimer")]
        public async Task<TimerResponseModel> SetTimer(TimerModel timer)
        {
            if (timer == null) 
                throw new ArgumentNullException(nameof(timer));

            var timerId = await _timerRepository.SaveAsync(timer);
            
            return new TimerResponseModel { id = timerId };
        }

        [HttpGet("GetTimerStatus/{id}")]
        public async Task<TimerStatus> GetTimerStatus(Guid id)
        {
            var timer = await _timerRepository.GetByIdAsync(id);
            if (timer == null)
            {
                return new TimerStatus();
            }

            var remainingSeconds = Math.Max((timer.ExpirationTime - DateTime.UtcNow).TotalSeconds, 0);

            return new TimerStatus { id = timer.Id, timeLeft =  remainingSeconds };
        }
    }
}