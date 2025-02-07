# DotNet.CurrencyConverter
This project is to showcase various features available in .Net Development. The project was developed using TDD, and includes Domain Driven Design, Clean Architecture, and SOLID principles. 
The project is a currency converter API that allows users to search and convert currencies from one to another. Various features include

## Features

- **Docker Compose**: Runs the API, Seq, and Redis for local development.
- **Distributed Caching with Redis**: Utilizes Redis for distributed caching to improve performance and scalability.
- **Retry Policy, Exponential Backoff, and Circuit Breaker with Polly**: Implements robust retry policies, exponential backoff, and circuit breaker patterns using Polly.
- **JWT Authentication**: Secures the API with JWT authentication.
- **API Rate Limiter**: Prevents abuse by limiting the rate of API requests.
- **Distributed Tracing with OpenTelemetry**: Provides distributed tracing capabilities using OpenTelemetry.
- **Result pattern**: Uses the result pattern for clean error handling.
- **Factory pattern**: Allows for easy extention for new kinds of currency providers.
- **Structured Logging with Serilog**: Implements structured logging with Serilog for better log management and analysis.
- **Correlated Visibility Across Entire Request Life**: Ensures correlated visibility across the entire request lifecycle.
- **Unit Tests with 100% Coverage**: Achieves 100% unit test coverage to ensure code quality and reliability.
- **Integration Tests with Test Containers and WireMockServer**: Uses test containers and WireMockServer for comprehensive integration testing.
- **GitHub Actions for Complete CI/CD**: Configures complete CI/CD pipelines for development, test, and production environments, targeting AWS Kubernetes.
- **API Versioning**: Supports API versioning to manage changes and backward compatibility.
- **Support for Horizontal Scaling**: The project uses Docker with the ability easily spin up new instances.

## Test Results

You can view the test results report on GitHub Pages:

https://harry-hathorn.github.io/DotNet.CurrencyConverter.TestResults/

![Test Results Report](https://github.com/harry-hathorn/CurrencyConverter/blob/main/Test%20Coverage.png)


## Setup Instructions
### Local Docker Setup

Ensure that you have docker desktop installed and running. Once running you can open the solution and start the solution using docker compose.
This will launch the API, Redis Server and Seq.

API: https://localhost:8081/
Seq: 
Redis: 

### Development JWT Token

When running the application locally, you can use the following JWT token for authorization as a bearer token:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6Ijc4MWFhZmZmLWYzOTUtNDFjNy04MDkyLTlkMDEyZjhmOGEwZiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6InVzZXIiLCJhdWQiOiJ1c2VycyIsImlzcyI6ImN1cnJlbmN5LmNvbnZlcnRlciIsImV4cCI6MTczODg3MDQ1NSwiaWF0IjoxNzM4ODcwNDU1LCJuYmYiOjE3Mzg4NzA0NTV9.VC02bvEeqB6yNTBXyw6JC3utXI-ooPCXpWp4pHbelLA
```

You can insert the above sample into https://jwt.io/ and adjust the expiration time and insert the JWT secret for local development.

#### JWT Token Header
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```
#### JWT Token Payload


Note that the expiration exp, iat, nbf should be changed to the current epoch unix timestamp, and remains valid for an hour.
```json
{
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "781aafff-f395-41c7-8092-9d012f8f8a0f",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "user",
  "aud": "users",
  "iss": "currency.converter",
  "exp": 1738870455,
  "iat": 1738870455,
  "nbf": 1738870455
}
```
#### JWT Token Signature

The signature is created using the following:
```typescript
HMACSHA256(
  base64UrlEncode(header) + "." +
  base64UrlEncode(payload),
  "69fac991-79d0-4cb5-b552-3f23661aa127"
)
```

### Kubernetes Setup Instructions

The following table provides the details for setting up Kubernetes deployments for different environments. Ensure that the specified environment variables are set for each deployment.

| Environment | Namespace                | Deployment Name                          | Environment Variables                        |
|-------------|---------------------------|------------------------------------------|----------------------------------------------|
| Development | currencyconverter-dev     | currencyconverter-dev-deployment         | `Authentication__JwtSecret={kubernetes-secret}`, `ASPNETCORE_ENVIRONMENT=DEV` |
| Test        | currencyconverter-test    | currencyconverter-test-deployment        | `Authentication__JwtSecret={kubernetes-secret}`, `ASPNETCORE_ENVIRONMENT=Test` |
| Production  | currencyconverter         | currencyconverter-production-deployment  | `Authentication__JwtSecret={kubernetes-secret}`, `ASPNETCORE_ENVIRONMENT=Production` |

## CI/CD Setup

In order to use the github actions you will need to have an EKS user and EKS cluster setup and configured. You will need to add github secret variables for AWS Access Key, AWS Access Id and Jwt Secret.

## Assumptions Made

The primary assumptions made are that there is infrastrure setup and ready for development, test and production cloud enviroments.

These include:
- **There is an external Authentication Service that Generates Valid JWT and User Management**
- **Production Ready Redis Servers**
- **Production Ready Seq Applications**
- **Github Actions Are configured for AWS Enviroment with EKS clusters**
- **Kuberenetes deployments with correct secretes and environment configuration are set up**
- **Network Configuration**: Assumes that the necessary network configurations (e.g., VPC, subnets, security groups) are in place for the cloud environments.
- **Load Balancer**: Assumes that a load balancer is set up to distribute traffic to the API instances.
- **Compliance Requirements**: Assumes that the deployment meets any relevant compliance requirements (e.g., GDPR, HIPAA).
- **Scalability**: Docker images are deployed in a scalable way with infrastructure capable of scaling horizontally to handle increased load.

## Possible Future Enhancements

- **Support for Mass Transit and Message broker for integration with additional microservices**
- **Inclusion of a Database for richer features and user experiences**
- **Inclusion of a domain aggregates for enhanced functionlity**
- **Outbox patter for domain events**
- **Integration with an AI Provider to give richer user experience for further operations around currencies**