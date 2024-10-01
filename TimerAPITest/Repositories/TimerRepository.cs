using Microsoft.EntityFrameworkCore;
using TimerAPITest.Enums;
using TimerAPITest.Models;
using TimerAPITest.Repositories.Entities;

namespace TimerAPITest.Repositories
{
    public interface ITimerRepository
    {
        Task<Guid> SaveAsync(TimerModel timerModel);

        Task<int> UpdateTimerAsync(TimerDBEntity timer);
        
        /// <summary>
        /// Get Timers with Status != Finished and expire in {expireInSeconds}
        /// </summary>
        /// <param name="expireInSeconds"></param>
        /// <returns></returns>
        Task<List<TimerDBEntity>> GetActiveTimersAsync(int expireInSeconds);
        
        Task<TimerDBEntity> GetByIdAsync(Guid id);
    }

    public class TimerRepository : ITimerRepository
    {
        private readonly AppDbContext _appDbContext;

        public TimerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Guid> SaveAsync(TimerModel timerModel)
        {
            TimerDBEntity timerDBEntity = new TimerDBEntity {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                Hours = timerModel.hours,
                Minutes = timerModel.minutes,
                Seconds = timerModel.seconds,
                WebhookUrl = timerModel.webhookUrl,
                Status = TimerStatuses.Started
            };

            await _appDbContext.Timers.AddAsync(timerDBEntity);
            await _appDbContext.SaveChangesAsync();

            return timerDBEntity.Id;
        }

        public async Task<int> UpdateTimerAsync(TimerDBEntity timer)
        {
            _appDbContext.Timers.Update(timer);
            return await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<TimerDBEntity>> GetActiveTimersAsync(int expireInSeconds)
        {
            return await _appDbContext.Timers.Where(x => x.Status == TimerStatuses.Started && DateTime.UtcNow.AddHours(-x.Hours).AddMinutes(-x.Minutes).AddSeconds(-x.Seconds + expireInSeconds) >= x.DateCreated).ToListAsync();
        }

        public async Task<TimerDBEntity> GetByIdAsync(Guid id)
        {
            return await _appDbContext.Timers.Where(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}
