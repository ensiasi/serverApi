# TestApi — a minimal .NET 10 web API for deployment practice

A small ASP.NET Core minimal API used to practice deploying a real app onto a Linux
server as a systemd service (Phase 1 of the DevOps roadmap).

## Endpoints

| Method | Path      | Purpose                                              |
|--------|-----------|------------------------------------------------------|
| GET    | `/`       | Liveness greeting; shows machine name and UTC time   |
| GET    | `/health` | Health check (used later by proxies / k8s probes)    |
| GET    | `/notes`  | Returns an in-memory list of notes (JSON)            |
| POST   | `/notes`  | Adds a note. Body: `{ "text": "..." }`               |

## Run locally

Requires the .NET 10 SDK.

```bash
dotnet run
```

By default it listens on a local port (shown in the console output, typically
http://localhost:5000). Test it:

```bash
curl http://localhost:5000/
curl http://localhost:5000/health
curl http://localhost:5000/notes
curl -X POST http://localhost:5000/notes -H "Content-Type: application/json" -d '{"text":"hello"}'
```

## Publish for deployment

This produces a self-contained-ish framework-dependent build to copy to the server
(the server needs the ASP.NET Core 10 runtime installed):

```bash
dotnet publish -c Release -o ./publish
```

The runnable output lands in `./publish`, with the entry DLL `TestApi.dll`.
Run it with: `dotnet TestApi.dll`

## Controlling the listening address

When run as a service behind a reverse proxy, bind it explicitly to localhost on a
fixed port using the ASPNETCORE_URLS environment variable, e.g.:

```bash
ASPNETCORE_URLS=http://127.0.0.1:5000 dotnet TestApi.dll
```

Binding to 127.0.0.1 (not 0.0.0.0) means only the local reverse proxy can reach it,
not the public internet directly — which is what you want once a proxy + TLS is in front.
