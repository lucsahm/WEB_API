# API Livros (C#/.NET 8)

API REST simples com **CRUD de livros** (GET/POST/PUT/DELETE), **autenticação Basic** e **página web** para operar.

## Rodar
```bash
cd /workspaces/WEB_API/ApiLivros
dotnet restore
export ASPNETCORE_URLS=http://0.0.0.0:5000
dotnet run
```

# Credenciais

Defina no appsettings.json:
```
"Auth": { "AllowedUsername": "SEU_SOBRENOME", "AllowedPassword": "SEU_RU" }
```

# Rotas

* / – página web (CRUD)
* /api/me – dados do aluno (sem auth)
* /api/books – livros (Basic Auth)
* /swagger – docs

# Exemplos rápidos (cURL)
```
# GET
curl -u 'SEU_SOBRENOME:SEU_RU' http://localhost:5000/api/books

# POST
curl -u 'SEU_SOBRENOME:SEU_RU' -H 'Content-Type: application/json' \
  -d '{"id":0,"author":"MARINHO, Antonio Lopes","title":"Desenvolvimento de Aplicações para Internet","publisher":"Pearson Education do Brasil","year":2016}' \
  http://localhost:5000/api/books

# DELETE (id 1)
curl -u 'SEU_SOBRENOME:SEU_RU' -X DELETE http://localhost:5000/api/books/1
```
