using Microsoft.Extensions.Options;
using TimerAPITest.Enums;
using TimerAPITest.Models;
using TimerAPITest.Repositories;
using TimerAPITest.Repositories.Entities;

namespace TimerAPITest.Services
{
    public class TimerBackgroundService : BackgroundService
    {
        private readonly ILogger<TimerBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private List<TimerDBEntity> _cachedTimers = new List<TimerDBEntity>();
        private readonly TimerSettings _timerSettings;

        public TimerBackgroundService(ILogger<TimerBackgroundService> logger, IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory, IOptions<TimerSettings> timerSettings)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
            _timerSettings = timerSettings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTime lastDataFetchTime = DateTime.MinValue;

            while (!stoppingToken.IsCancellationRequested)
            {
                if ((DateTime.UtcNow - lastDataFetchTime).TotalSeconds >= _timerSettings.DataFetchIntervalSeconds)
                {
                    await UpdateTimersFromDatabase();
                    lastDataFetchTime = DateTime.UtcNow;
                }

                await CheckTimersEverySecond();

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task UpdateTimersFromDatabase()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var timerRepository = scope.ServiceProvider.GetRequiredService<ITimerRepository>();

                _cachedTimers = (await timerRepository.GetActiveTimersAsync(_timerSettings.DataFetchIntervalSeconds)).ToList();
            }
        }

        private async Task CheckTimersEverySecond()
        {
            var timersToCheck = _cachedTimers.ToList();

            foreach (var timer in timersToCheck)
            {
                if (timer.HasExpired)
                {
                    await HandleExpiredTimer(timer);
                    _cachedTimers.Remove(timer);
                }
            }
        }

        private async Task HandleExpiredTimer(TimerDBEntity timer)
        {
            if (string.IsNullOrEmpty(timer.WebhookUrl) || !Uri.IsWellFormedUriString(timer.WebhookUrl, UriKind.Absolute))
            {
                Console.WriteLine($"Invalid URL: {timer.WebhookUrl}");
                timer.Status = TimerStatuses.Error;
            }

            if (timer.Status != TimerStatuses.Error)
            {
                try
                {
                    using var client = new HttpClient();

                    await client.PostAsync(timer.WebhookUrl, null);

                    timer.Status = TimerStatuses.Finished;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the background service.");
                    
                    timer.Status = TimerStatuses.Error;
                }
                
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var timerRepository = scope.ServiceProvider.GetRequiredService<ITimerRepository>();
                await timerRepository.UpdateTimerAsync(timer);
            }
        }
    }
}
