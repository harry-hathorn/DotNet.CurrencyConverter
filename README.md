# CurrencyConverter
## Setup Instructions
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
```
HMACSHA256(
  base64UrlEncode(header) + "." +
  base64UrlEncode(payload),
  "69fac991-79d0-4cb5-b552-3f23661aa127"
)
```