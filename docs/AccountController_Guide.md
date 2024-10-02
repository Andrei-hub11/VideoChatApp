The section of the documentation below explains the authentication process for the application. It outlines how users can register, log in, and manage their profiles. This includes obtaining authentication tokens required for accessing protected resources within the application. Detailed information on request formats, response examples, and error handling is provided to ensure a smooth integration with the authentication system.

# Endpoints:

### Get User Profile

- URL: /api/v1/profile
- HTTP Method: GET
- Authentication Header: Required (Bearer token).
- Description: Retrieves the profile information of the currently authenticated user

- **Success Response (200 OK):**

```
{
  "Id": "1a2b3c4d",
  "UserName": "john_doe",
  "Email": "john.doe@example.com",
  "ProfileImagePath": "users/images/293ba48b-669b-4a91-9d2f-f2eefd92b90a.jpg"
}
```

If the update fails, the response will include errors details like this:

- **Error Response (401 Unauthorized):**

```
{
  "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401",
  "Title": "You do not have access to this feature, or have not yet logged in",
  "Status": 401,
  "Detail": "An authentication error has occurred. Please check your credentials and try again.",
  "Instance": "55405af7-3693-452c-b299-6094b20c625d"
}
```

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
"ProfileImage": "base64_string_of_image"
}
```

- **Success Response (200 OK):**

```
{
  "User": {
    "Id": "1a2b3c4d",
    "UserName": "john_doe",
    "Email": "john.doe@example.com",
    "ProfileImagePath": "users/images/293ba48b-669b-4a91-9d2f-f2eefd92b90a.jpg"
  },
  "AccessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "RefreshToken": "dGVzdF9yZWZyZXNoX3Rva2Vu...",
  "Roles": [
        "User"
    ]
}
```

If the registration fails, the response will include errors details like this:

- **Error Response (422 Unprocessable Entity):**

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

- **Error Response (409 Conflict):**

```
{
    "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/409",
    "Title": "ERR_DUPLICATE_EMAIL",
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
    "ProfileImagePath": "users/images/293ba48b-669b-4a91-9d2f-f2eefd92b90a.jpg"
  },
  "AccessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "RefreshToken": "dGVzdF9yZWZyZXNoX3Rva2Vu...",
  "Roles": [
        "User"
    ]
}
```

- **Error Response:**

If the login fails, the response will include errors details like this:

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

```
{
    "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404",
    "Title": "ERR_USER_NOT_FOUND",
    "Status": 404,
    "Detail": "User with email = 'app_user5453111@test.com' was not found.",
    "Instance": "6242a7e7-8d11-47b6-a8e5-576ff3c52134"
}
```

### Refresh Access Token

- URL: /api/v1/token-renew
- HTTP Method: POST
- Authentication Header: Required (Bearer token).
- Request Body (JSON):

```
{
  "RefreshToken": "current_refresh_token"
}
```

- **Success Response (200 OK):**

```
{
  "AccessToken": "new_access_token"
}
```

- **Error Response (422 Unprocessable Entity):**

```
{
  "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/422",
  "Title": "One or more validation errors occurred.",
  "Status": 422,
  "Detail": "See the 'Errors' property for details.",
  "Instance": "d296d9aa-953c-4564-a72e-dd93e60ff78f",
  "Errors": {
    "RefreshToken": [
      {
        "Code": "ERR_VALIDATION_FAILURE",
        "Description": "RefreshToken not provided"
      }
    ]
  }
}
```

### Update User Profile

- URL: /api/v1/account/profile
- HTTP Method: POST
- Authentication Header: Required (Bearer token).
- Request Body (JSON):

```
{
  "UserName": "new_username",
  "ProfileImage": "base64_string_of_image"
}
```

- **Success Response (200 OK):**

```
{
  "Id": "1a2b3c4d",
  "UserName": "john_doe",
  "Email": "john.doe@example.com",
  "ProfileImagePath": "users/images/293ba48b-669b-4a91-9d2f-f2eefd92b90a.jpg"
}
```

- **Error Response (401 Unauthorized):**

If the update fails, the response will include errors details like this:

```
{
  "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401",
  "Title": "You do not have access to this feature, or have not yet logged in",
  "Status": 401,
  "Detail": "An authentication error has occurred. Please check your credentials and try again.",
  "Instance": "55405af7-3693-452c-b299-6094b20c625d"
}
```

- **Error Response (404 Not Found):**

```
{
  "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404",
  "Title": "ERR_USER_NOT_FOUND",
  "Status": 404,
  "Detail": "User with id = '3f8bb3c1-a42e-4ed6-92b3-90c1b216365' was not found.",
  "Instance": "13f05fff-c52c-465c-b47e-933c6d3ec23f"
}
```

- **Error Response (422 Unprocessable Entity):**

```
{
  "Type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/422",
  "Title": "One or more validation errors occurred.",
  "Status": 422,
  "Detail": "See the 'Errors' property for details.",
  "Instance": "1f87e0f0-5625-4762-b307-e33e5d5b1d35",
  "Errors": {
    "UserName": [
      {
        "Code": "ERR_IS_NULL_OR_EMPTY",
        "Description": "UserName cannot be null or empty"
      }
    ]
  }
}
```
