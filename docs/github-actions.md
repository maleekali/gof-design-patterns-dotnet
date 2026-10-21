# GitHub Actions

This repository uses GitHub Actions to automate build, test, and security checks.

## Workflows

- `.github/workflows/ci.yml`: discovers every `.sln` and `.slnx` file and runs restore, Release build, and tests for each solution. The matrix is capped at four concurrent solutions to avoid exhausting runner capacity as the repository grows.
- `.github/workflows/codeql.yml`: runs CodeQL analysis for C# on pushes, pull requests, manual runs, and a weekly schedule.
- `.github/workflows/dependency-review.yml`: reviews dependency changes in pull requests and fails when a newly introduced vulnerable dependency is reported at high or critical severity.

## Requirements

- The projects currently target .NET 10.
- Repository Actions must be enabled.
- Code scanning and dependency review availability can depend on repository visibility and GitHub plan/settings.
- Keep unit tests independent of infrastructure wherever possible. If integration tests require Redis, RabbitMQ, Kafka, or SFTP, add a separate workflow or job that explicitly starts those services and runs the relevant test category.

## Adding a new pattern

1. Add the pattern's `.sln` under `src/`.
2. Ensure its projects target a supported .NET SDK and restore without interactive prompts.
3. Add unit tests to the solution.
4. The CI workflow discovers the new solution automatically; no workflow edit is needed.
5. If integration tests need external services, document and wire up those services explicitly instead of silently relying on a developer machine.

## Local equivalents

From the directory containing a solution:

```bash
dotnet restore YourSolution.sln
dotnet build YourSolution.sln --configuration Release --no-restore
dotnet test YourSolution.sln --configuration Release --no-build
```

## Security notes

- Never commit passwords, access tokens, private keys, or real connection strings.
- Prefer repository variables for non-sensitive configuration and GitHub Actions secrets for credentials.
- Review third-party actions and pin them to full commit SHAs when your repository's security policy requires immutable action references.
