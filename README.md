# Timer API Service

This project is an ASP.NET Core 6.0 application that provides a simple timer-based system. It allows users to set timers and receive a webhook notification when the timer expires. The service can handle a large number of timers, caching them in memory and updating their statuses periodically. The project also ensures that timers are accurately checked and processed even after server restarts.

## Features

- **Set a Timer**: Define a timer with a specified duration (hours, minutes, and seconds).
- **Timer Status**: Retrieve the status of a timer and see how many seconds are remaining.
- **Webhook Notification**: Send a webhook POST request to a provided URL when the timer expires.
- **Persistent Timer Handling**: Timers continue functioning after server restarts.
- **Efficient Timer Management**: Optimized to check and update timer statuses every second while retrieving timer data from the database at specified intervals.

## Endpoints

### 1. Set a Timer

- **Endpoint**: `POST /api/Timer/SetTimer`
- **Request**:
  ```json
  {
    "hours": 0,
    "minutes": 1,
    "seconds": 0,
    "webhookUrl": "https://webhook.site/bae113b1-82af-4d11-9d3e-4389572a1981"
  }
  ```
- **Response**:
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
  ```

This endpoint creates a new timer and returns the unique ID of the timer.

### 2. Get Timer Status

- **Endpoint**: `GET /api/Timer/GetTimerStatus/{id}`
- **Response**:
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "timeLeft": 45
  }
  ```

This endpoint returns the current status of the timer (`Started` or `Finished`) and the remaining seconds until expiration. If the time has already expired, it returns `timeLeft` as `0`.

## How It Works

1. **Timer Creation**: When a timer is created, it's stored in the database and scheduled for expiration based on the provided time values.
2. **Timer Checking**: The `TimerBackgroundService` runs continuously, checking the status of timers every second and marking them as finished when they expire.
3. **Webhook Notification**: Once a timer expires, an HTTP POST request is sent to the provided `webhookUrl` to notify external systems.
4. **Data Refresh**: Every configured number of seconds (as per `FetchIntervalSeconds` in `appsettings.json`), the service retrieves the active timers from the database, ensuring scalability for handling a large number of timers efficiently.

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- A SQL database (LocalDB or other)

### Installation

1. Install dependencies:
   ```bash
   dotnet restore
   ```

2. Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TimerDb;Trusted_Connection=True;"
   }
   ```

3. Apply the database migrations:
   ```bash
   dotnet ef database update
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

### Configuration

You can configure the following settings in `appsettings.json`:

- **Timer Fetch Interval**: This controls how often the background service fetches timers from the database (in seconds).
  ```json
  "TimerSettings": {
    "FetchIntervalSeconds": 60
  }
  ```

### Example Usage

1. Set a new timer:
   ```bash
   curl -X POST "https://localhost:5001/api/Timer/SetTimer" -H "Content-Type: application/json" -d '{
     "hours": 0,
     "minutes": 1,
     "seconds": 0,
     "webhookUrl": "https://example.com/webhook"
   }'
   ```

2. Get the status of a timer:
   ```bash
   curl -X GET "https://localhost:5001/api/Timer/GetTimerStatus/{id}"
   ```

## Technologies Used

- **ASP.NET Core 6.0**
- **Entity Framework Core (Code First)**
- **SQL Server (LocalDB)**
- **Background Services**
- **HttpClient**

## License

This project is licensed under the MIT License.
