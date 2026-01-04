# crypto-service-api

This is a minimal, security-focused ASP.NET Core API that provides REST API endpoints for hashing, signing, and verifying.

This project was created for educational purposes only and should not be used for other purposes.

---

## Features

- Hashing using [SHA256](https://en.wikipedia.org/wiki/SHA-2)
- Signing messages and verifying signatures using [ECDSA](https://en.wikipedia.org/wiki/Elliptic_Curve_Digital_Signature_Algorithm) (in conjunction with SHA256)
- [JSON Web Tokens](https://en.wikipedia.org/wiki/JSON_Web_Token) for authentication with role-based authorization (using [HMAC-SHA256](https://en.wikipedia.org/wiki/HMAC))
- Input validation and request size limits
- Logging

---

## Security and technology

- Hash, sign and verify endpoints use the built-in functions from [System.Security.Cryptography](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography?view=net-8.0) of `.NET` and require authentication
  - [SHA256](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha256?view=net-8.0)
  - [ECDSA](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.ecdsa?view=net-8.0)
- Tokens are created using [HmacSha256](https://learn.microsoft.com/en-us/dotnet/api/microsoft.identitymodel.tokens.securityalgorithms.hmacsha256?view=msal-web-dotnet-latest) (token endpoint exists only for local testing)
- Request sizes are limited
- Keys are **demo-only** and intentionally stored **unsecurely** in `appsettings.json`!

---

## Running using Docker

### Prerequisites (install first)

- [Docker](https://docs.docker.com/engine/install/) (tested with Docker Engine on Linux)

### Build the Docker Image

`cd` into the project root and run

```bash
docker build -t crypto-service .
```

Then, run the container with

```bash
docker run -p 8080:8080 crypto-service
```

The API will be available at `http://localhost:8080`.

## API Endpoints

- `GET /health`
- `POST /hash`
- `POST /sign`
- `POST /verify`

## Basic Usage

### Health check (doesn't require authorization):

```bash
curl http://localhost:8080/health
```


### Obtain a JWT (only for testing):

The following commands assume that you have [curl](https://curl.se/) and [jq](https://jqlang.org/) installed. If you don't want to install `jq`, you can copy and paste the relevant fields from the JSON responses.

```bash
TOKEN=$(curl -s -X POST http://localhost:8080/auth/token | jq -r '.token')
```

### Hashing:

```bash
curl -s -X POST http://localhost:8080/crypto/hash -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"Data":"hello"}'
```

### Signing:

```bash
SIGNATURE=$(curl -s -X POST http://localhost:8080/crypto/sign -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"Data":"sign this"}' | jq -r '.signature')
```

### Verifying:

Right signature:

```bash
curl -X POST http://localhost:8080/crypto/verify -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d "{\"Data\":\"sign this\",\"signature\":\"$SIGNATURE\"}"
```

Wrong signature:

```bash
curl -X POST http://localhost:8080/crypto/verify -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d "{\"Data\":\"Different data\",\"signature\":\"$SIGNATURE\"}"
```
