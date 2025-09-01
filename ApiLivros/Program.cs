using ApiLivros.Data;
using ApiLivros.Models;
using ApiLivros.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Lê dados do aluno do appsettings
var student = builder.Configuration.GetSection("Student").Get<StudentInfo>() ?? new StudentInfo();

// Adiciona serviços do EF Core com banco em memória (prático para trabalho acadêmico)
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("BooksDb"));

// Autenticação Basic: login = último sobrenome; senha = RU
builder.Services
    .AddAuthentication("Basic")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", null);

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Útil para visualizar/testar endpoints

var app = builder.Build();

app.UseDefaultFiles();   // Serve index.html por padrão
app.UseStaticFiles();    // Habilita wwwroot

app.UseSwagger();
app.UseSwaggerUI();

// Middleware de autenticação/autorização
app.UseAuthentication();
app.UseAuthorization();

// ====== SEED: cria livros iniciais e garante seus dados disponíveis ======
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Books.Any())
    {
        db.Books.AddRange(
            new Book {
                Id = 1,
                Author = "LEDUR, Cleverson Lopes",
                Title = "Programação Back End II",
                Publisher = "SAGAH",
                Year = 2019
            },
            new Book {
                Id = 2,
                Author = "FREITAS, Pedro H. Chagas [et al.]",
                Title = "Programação Back End III",
                Publisher = "SAGAH",
                Year = 2021
            },
            new Book {
                Id = 3,
                Author = "DEITEL, Paul J.",
                Title = "Ajax, RICH Internet Applications e desenvolvimento Web para programadores",
                Publisher = "Pearson Prentice Hall",
                Year = 2008
            }
        );
        await db.SaveChangesAsync();
    }
}

// ====== ROTAS HTTP ======

// Seus dados (GET). Sem autenticação para o professor visualizar facilmente.
app.MapGet("/api/me", () => Results.Ok(student))
   .WithSummary("Dados do aluno (SEU NOME, RU, CURSO)");

// Lista todos os livros (GET). Requer autenticação básica.
app.MapGet("/api/books", async (AppDbContext db) =>
    Results.Ok(await db.Books.OrderBy(b => b.Id).ToListAsync()))
   .RequireAuthorization()
   .WithSummary("Lista livros");

// Obtém um livro por id (GET). Requer autenticação.
app.MapGet("/api/books/{id:int}", async (int id, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    return book is null ? Results.NotFound() : Results.Ok(book);
})
.RequireAuthorization()
.WithSummary("Obtém livro por ID");

// Cria um novo livro (POST). Requer autenticação.
// Comentários simples para explicar: o corpo JSON é desserializado em Book e salvo no banco.
app.MapPost("/api/books", async (Book book, AppDbContext db) =>
{
    // Garante que um novo ID seja atribuído se não veio definido
    if (book.Id == 0)
    {
        book.Id = (db.Books.Any() ? db.Books.Max(b => b.Id) : 0) + 1;
    }

    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/api/books/{book.Id}", book);
})
.RequireAuthorization()
.WithSummary("Cria um livro");

// Atualiza um livro existente (PUT). Requer autenticação.
app.MapPut("/api/books/{id:int}", async (int id, Book input, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    // Atualiza campos
    book.Author = input.Author;
    book.Title = input.Title;
    book.Publisher = input.Publisher;
    book.Year = input.Year;

    await db.SaveChangesAsync();
    return Results.Ok(book);
})
.RequireAuthorization()
.WithSummary("Atualiza um livro");

// Exclui um livro (DELETE). Requer autenticação.
app.MapDelete("/api/books/{id:int}", async (int id, AppDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.RequireAuthorization()
.WithSummary("Exclui um livro");

app.Run();