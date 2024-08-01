The section of the documentation below explains the authentication process of the application. It provides details on how new users can register and obtain authentication tokens, which are necessary for accessing protected resources within the application.

# Endpoints:

### User Register

- URL: /api/v1/account/register
- Método HTTP: POST
- Cabeçalho de Autenticação: Não é necessário autenticação.
- Corpo da Solicitação (JSON):

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
    "ProfileImage": "https://example.com/images/john_doe.jpg"
  },
  "AccessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "RefreshToken": "dGVzdF9yZWZyZXNoX3Rva2Vu...",
  "Roles": [
    {
      "Id": "admin",
      "Name": "Administrator"
    },
    {
      "Id": "user",
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
