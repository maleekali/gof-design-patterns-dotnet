# Abstract Factory — Intermediate: Import Preview Service

A practical C#/.NET example of the **Abstract Factory** pattern using CSV and JSON import families, validation, error reports, and Redis-backed preview caching.

> Learning goal: understand when a family of related products is useful, then apply it to a realistic processing pipeline without making the pattern an excuse for needless abstractions.

## What it does

- Accepts CSV or JSON files through an ASP.NET Core minimal API.
- Selects a format-specific factory.
- Uses the factory to create a parser, validator, and error-report builder belonging to that input-format family.
- Normalizes records into one application model.
- Returns record counts, a sample of valid records, validation issues, and a downloadable-style error report in the same format family.
- Caches the preview in Redis using a SHA-256 content hash and a cache schema version.
- Degrades gracefully if Redis is temporarily unavailable: the import can still be processed, but it will not benefit from the cache.
- Includes unit tests that do not require Redis.

## Pattern structure

```text
                        ImportPreviewService
                                |
                                v
                    IImportFormatFactoryResolver
                                |
                 +--------------+--------------+
                 |                             |
                 v                             v
           CsvImportFactory              JsonImportFactory
                 |                             |
        +--------+--------+           +--------+--------+
        |        |        |           |        |        |
        v        v        v           v        v        v
      Parser  Validator  Report      Parser  Validator  Report

                ImportPreviewService
                         |
                         v
                 IImportPreviewCache
                         |
                         v
                       Redis
```

Each factory creates three related products for its input family. The parsers translate source files into a common raw-document model; validators apply the required-field and business rules; report builders render diagnostics as CSV or JSON.

## Requirements

- .NET 10 SDK
- Docker Desktop or Docker Engine with Docker Compose

.NET 10 is an LTS release. See the [official .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).

## Run locally

Start Redis from the repository root:

```bash
docker compose up -d redis
```

Start the API:

```bash
dotnet run --project src/ImportPreview.Api --urls http://localhost:5080
```

Health check:

```bash
curl http://localhost:5080/health
```

### Preview a CSV file

```bash
curl -X POST http://localhost:5080/imports/preview \
  -F "format=csv" \
  -F "file=@samples/customers.csv"
```

### Preview a JSON file

```bash
curl -X POST http://localhost:5080/imports/preview \
  -F "format=json" \
  -F "file=@samples/customers.json"
```

The `format` field is optional when the uploaded file has a `.csv` or `.json` extension.

The response includes `cacheHit`. Repeating the exact same file with the same format should return a cache hit while the Redis entry remains valid. The cache key includes the format and processing cache version, so identical bytes interpreted as different formats do not collide.

## Run tests

```bash
dotnet test AbstractFactory.Intermediate.sln
```

The unit tests use an in-memory test cache and do not require Docker or a running Redis instance.

## Input schema

Both formats are normalized to this common record shape:

| Field | Rules |
|---|---|
| `id` | Required, non-empty string |
| `name` | Required, non-empty string |
| `email` | Required, basic email-format validation |
| `amount` | Required, invariant-culture decimal, zero or greater |

CSV files must contain the `id`, `name`, `email`, and `amount` headers. JSON files must contain an array of objects with those fields.

Example CSV:

```csv
id,name,email,amount
1001,Ada Lovelace,ada@example.com,125.50
1002,Grace Hopper,not-an-email,40.00
```

Example JSON:

```json
[
  { "id": "1001", "name": "Ada Lovelace", "email": "ada@example.com", "amount": 125.50 },
  { "id": "1002", "name": "Grace Hopper", "email": "not-an-email", "amount": 40.00 }
]
```

## Important design decisions

- **Abstract Factory is used because each input family provides multiple related products**, not just because there are different parser classes.
- Business-level validation is intentionally kept similar across formats; each concrete validator also applies format-specific structural checks.
- The parser, validator, report builder, and Redis cache have separate responsibilities.
- Redis is a cache, not the source of truth. If Redis is unavailable, the service continues without caching.
- The API caps uploaded file size at 5 MiB and limits the returned sample and validation diagnostics to keep responses bounded.
- The content hash avoids treating two different files with the same filename as the same import.
- `preview:v1` is a cache schema/version component. Increment it when processing logic changes in a way that makes old cached previews incompatible.
- This project demonstrates caching, not durable job processing. It does not promise atomicity between Redis and another system.

## Deliberate limitations

- Parsing is performed synchronously within the HTTP request and is intended for small-to-medium preview files.
- JSON Lines, Excel files, background jobs, authentication, and persistent import history are out of scope.
- Email validation is intentionally basic and is not a guarantee that an address exists.
- Only the first 100 validation issues and first 10 valid records are returned. The valid/invalid counts still cover all parsed rows.
- The Redis password in `docker-compose.yml` is for local development only. Never reuse it in a production deployment.

## Folder layout

```text
src/ImportPreview.Api/
  Caching/       Redis preview cache
  Contracts/     Factories, parser, validator, report and DTO interfaces
  Endpoints/     HTTP upload endpoint
  Factories/     CSV and JSON concrete factories
  Parsing/       CSV and JSON parsers
  Reporting/     CSV and JSON error-report builders
  Services/      Import orchestration and cache-key generation
  Validation/    Shared rules and format-specific validators
tests/ImportPreview.Tests/
samples/
docs/architecture.md
```

## Next improvements

- Add integration tests using a disposable Redis container.
- Add a per-file import ID and background processing for larger files.
- Add configurable field mappings and schema versions.
- Add metrics for cache hit rate, parsing duration, and validation errors.
- Compare this implementation with a simpler factory or DI registration that does not use Abstract Factory.
