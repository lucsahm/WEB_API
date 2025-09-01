namespace ApiLivros.Models;

// Modelo simples de Livro. Isso será mapeado pelo Entity Framework para o banco (InMemory).
public class Book
{
    public int Id { get; set; }           // Identificador único (chave primária)
    public string Author { get; set; } = "";
    public string Title { get; set; } = "";
    public string Publisher { get; set; } = "";
    public int Year { get; set; }         // Ano de publicação
}