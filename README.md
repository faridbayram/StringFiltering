# Heavy String Filtering API

## Overview

This project is a .NET Core Web API designed to accept very large text strings in chunks, store them, and process the full text in a background queue with memory-efficient filtering. It uses clean architecture, async streaming, and in-memory processing for optimal performance.

---

## Features

- Accepts chunked string uploads via `/api/upload`
- Assembles and filters full strings asynchronously
- Uses in-memory background queue (`ConcurrentQueue`)
- Filtering with Levenshtein or Jaro-Winkler similarity
- Clean architecture and extensible strategy pattern
- Fully in-memory (no DB, no external services)

---

## Technologies

- .NET Core 8 Web API
- BackgroundService
- ConcurrentQueue
- Unit Testing with xUnit
- Clean Architecture (Domain, Application, Infrastructure, API layers)

---

## How to Run

```bash
git clone git@github.com:faridbayram/StringFiltering.git
cd StringFiltering\StringFiltering.API
dotnet build
dotnet run
```

## How to Test

```bash
dotnet test
```

## ️ Architecture

The project follows a clean, modular structure:

```text
StringFiltering/
│
├── StringFiltering.API/
│   └── UploadController.cs
│
├── StringFiltering.Application/
│   ├── Common/
│   ├── Dtos/
│   ├── Exceptions/
│   ├── Extensions/
│   ├── Factories/
│   ├── Implementations/
│   ├── Results/
│   ├── Services/
│   ├── Utilities/
│   ├── Validators/
│
├── StringFiltering.Infrastructure/
│   ├── Factories/
│   └── Utilities/
│
└── StringFiltering.UnitTests/
│   ├── BackgroundQueueTests.cs
│   └── LevenshteinFilteringHelperTests.cs
```

- **Separation of concerns** between layers
- **Dependency inversion** through interfaces
- **Strategy pattern** for selecting filtering algorithms

##  Filtering Logic

- Filter words are predefined inside the application (not sent from client).
- Dynamically supporting different filtering algorithms and strategies:
    - **Levenshtein Distance**
    - **Jaro-Winkler Similarity** - Created but not implemented
- Words with ≥ 80% similarity to any filter word will be removed.
- Similarity threshold is configurable (default: 80%).
- Filtering runs asynchronously in a background service.