using BookApp;
using BookApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Настраиваем порты для приложения
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

// Добавляем конфигурацию для подключения к базе данных SQLite
builder.Services.AddDbContext<BookContext>(options =>
    options.UseSqlite("Data Source=books.db"));

var app = builder.Build();

// Перенаправление на HTTPS (если используется)
app.UseHttpsRedirection();

// Настраиваем конечные точки API для работы с книгами

// Получить все книги
app.MapGet("/books", async (BookContext db) =>
    await db.Books.ToListAsync());

// Получить книгу по ID
app.MapGet("/books/{id}", async (int id, BookContext db) =>
    await db.Books.FindAsync(id) is Book book
        ? Results.Ok(book)
        : Results.NotFound("Книга не найдена"));

// Добавить новую книгу
app.MapPost("/books", async (Book book, BookContext db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/books/{book.Id}", book);
});

// Обновить книгу
app.MapPut("/books/{id}", async (int id, Book updatedBook, BookContext db) =>
{
    var book = await db.Books.FindAsync(id);

    if (book == null) return Results.NotFound("Книга не найдена");

    book.Title = updatedBook.Title;
    book.Author = updatedBook.Author;
    book.Pages = updatedBook.Pages;

    await db.SaveChangesAsync();
    return Results.Ok(book);
});

// Удалить книгу
app.MapDelete("/books/{id}", async (int id, BookContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book == null) return Results.NotFound("Книга не найдена");

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.Ok("Книга удалена");
});

// Запуск приложения
app.Run();
app.MapGet("/", () => Results.File("wwwroot/index.html"));
