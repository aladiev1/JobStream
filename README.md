# JobStream

Asynchronous job processing platform built with ASP.NET Core and .NET, designed to process external API data and generate CSV exports using a background worker architecture.

---

## 🚀 Overview

JobStream simulates a real-world data pipeline:

- Accepts jobs via REST API
- Stores jobs in SQL Server
- Processes jobs asynchronously via a background worker
- Fetches data from an external API (Weatherstack)
- Generates CSV exports
- Handles retries and failure states

---

## 🏗️ Architecture

```
API → SQL Server → Worker → Weatherstack API → CSV Output
```

### Components

- **JobStream.Api**
  - REST endpoints for job creation and retrieval

- **JobStream.Worker**
  - Background service that processes jobs asynchronously

- **JobStream.Application**
  - Interfaces and business logic contracts

- **JobStream.Infrastructure**
  - EF Core persistence, external API integration, CSV export

- **SQL Server**
  - Stores job state and metadata

---

## ⚙️ Features

- Asynchronous job processing using background workers
- External API integration (Weatherstack)
- CSV file generation and export
- Retry logic with configurable max attempts
- Error tracking and logging
- Clean layered architecture (API / Application / Infrastructure)

---

## 📡 API Endpoints

### Create Job

```http
POST /api/jobs
```

```json
{
  "location": "Baltimore",
  "format": "csv",
  "priority": "High"
}
```

### Get All Jobs

```http
GET /api/jobs
```

### Get Job By ID

```http
GET /api/jobs/{id}
```

---

## 🧪 Example Workflow

1. Send POST request to create a job
2. Job is stored in SQL Server with status Pending
3. Worker picks up job
4. Calls Weatherstack API
5. Generates CSV file
6. Updates job to Completed

---

## 📁 Example Output

```csv
Location,Temperature,WeatherDescription
Baltimore,72,Partly cloudy
```

---

## 🔐 Configuration

Weatherstack API Key
Stored securely using .NET User Secrets

```bash
dotnet user-secrets set "Weatherstack:ApiKey" "YOUR_API_KEY" --project src/JobStream.Worker/JobStream.Worker.csproj
```

---

## 🛠️ Tech Stack
- C#
- ASP.NET Core
- Entity Framework Core
- SQL Server
- Background Services (IHostedService)
- HttpClientFactory
- REST APIs

---

## 🧠 Key Concepts Demonstrated
- Asynchronous processing
- Dependency Injection
- Repository pattern
- External API integration
- Fault tolerance & retry handling
- Separation of concerns (clean architecture)

---

## 📸 Screenshots

### Create Job (API)

![Create Job](screenshots/create-job.png)

---

### Worker Processing Jobs

![Worker Processing](screenshots/worker-processing.png)

---

### Generated CSV Output

![CSV Output](screenshots/csv-output.png)

---

## 📌 Future Improvements
- Replace polling with queue system (Azure Queue / RabbitMQ)
- Add file download endpoint
- Add job prioritization handling
- Add authentication & rate limiting
- Dockerize application

---

## 🧑‍💻 Author
Anna Aladiev

---