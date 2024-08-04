The section of the documentation below explains the authentication process of the application. It provides details on how new users can register and obtain authentication tokens, which are necessary for accessing protected resources within the application.

# Endpoints:

### User Register

- URL: /api/v1/account/register
- HTTP Method: POST
- Authentication Header: Not required.
- Request Body (JSON):

```
{
"UserName": "Nome do Usuário",
"Email": "email@example.com",
"Password": "senha",
"ProfileImage": "base64"
}
```

- **Success Response (200 OK):**

```
{
  "User": {
    "Id": "1a2b3c4d",
    "UserName": "john_doe",
    "Email": "john.doe@example.com",
    "ProfileImageUrl": "https://example.com/images/john_doe.jpg"
  },
  "AccessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "RefreshToken": "dGVzdF9yZWZyZXNoX3Rva2Vu...",
  "Roles": [
    {
      "Name": "Admin"
    },
    {
      "Name": "User"
    }
  ]
}
```

- **Error Response:**

If the registration fails, the response will include error details in the ProblemDetails format.

```
{
    "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/422",
    "Title": "One or more validation errors occurred.",
    "Status": 422,
    "Detail": "See the errors property for details.",
    "Instance": "dc560ab3-9c72-41a8-8649-43477283a637",
    "Errors": {
        "Password": [
            {
                "Code": "ERR_TOO_LOW",
                "Description": "A senha deve ter pelo menos oito caracteres"
            },
            {
                "Code": "ERR_INVALID_PASSWORD",
                "Description": "Senha inválida. A senha deve ter pelo menos dois caracteres especiais"
            }
        ]
    }
}
```

- **Error Response (404 Not Found):**

```
{
    "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/409",
    "Title": "ERR_EMAIL_CONFLICT",
    "Status": 409,
    "Detail": "The email 'app_user545311@test.com' is already registered.",
    "Instance": "4af774bf-ae3a-443e-b9e5-12e742eeabf9"
}
```

### User Login

- URL: /api/v1/account/login
- HTTP Method: POST
- Authentication Header: Not required.
- Request Body (JSON):

```
{
  "UserName": "Username",
  "Password": "password"
}
```

- **Success Response (200 OK):**

```
{
  "User": {
    "Id": "1a2b3c4d",
    "UserName": "john_doe",
    "Email": "john.doe@example.com",
    "ProfileImageUrl": "https://example.com/images/john_doe.jpg"
  },
  "AccessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "RefreshToken": "dGVzdF9yZWZyZXNoX3Rva2Vu...",
  "Roles": [
    {
      "Name": "Admin"
    },
    {
      "Name": "User"
    }
  ]
}
```

- **Error Response:**

If the registration fails, the response will include error details in the ProblemDetails format.

```
{
  "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400",
  "Title": "One or more validation errors occurred.",
  "Status": 400,
  "Detail": "See the errors property for details.",
  "Instance": "de6d585a-6383-4e2f-9e7e-49a8218c295b",
  "Errors": {
    "Password": [
      {
        "Code": "ERR_INVALID_PASSWORD",
        "Description": "The provided password is incorrect."
      }
    ]
  }
}
```

- **Error Response (404 Not Found):**

If the user is not found, the response will include error details in the ProblemDetails format.

```
{
    "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404",
    "Title": "ERR_USER_NOT_FOUND",
    "Status": 404,
    "Detail": "User with email = 'app_user5453111@test.com' was not found.",
    "Instance": "6242a7e7-8d11-47b6-a8e5-576ff3c52134"
}
```
