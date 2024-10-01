using TimerAPITest.Middlewares;
using TimerAPITest.Models;
using TimerAPITest.Repositories;
using TimerAPITest.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

// Configurations
builder.Services.Configure<TimerSettings>(builder.Configuration.GetSection("TimerSettings"));

builder.Services.AddHttpClient();

// DbContext
builder.Services.AddDbContext<AppDbContext>();

// Repositories
builder.Services.AddScoped<ITimerRepository, TimerRepository>();

// Services
builder.Services.AddHostedService<TimerBackgroundService>();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middlewares
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

