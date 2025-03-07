# Marten

Showcase of Marten library usage.

## Prerequisites

- [↑ .NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [↑ Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Run application

Run infrastructure:

```bash
docker compose --file infrastructure.yaml up --detach
```

Run application:

```bash
dotnet run
```

Shut down infrastructure:

```bash
docker compose --file infrastructure.yaml down
```
