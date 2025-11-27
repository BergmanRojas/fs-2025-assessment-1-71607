using fs_2025_api_20250925_71607.Data;
using fs_2025_api_20250925_71607.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace fs_2025_api_20250925_71607.Endpoints
{
    public static class BookEndPoints
    {
        public static void AddBookEndPoints(this WebApplication app)
        {
            app.MapGet("/books", (BookData data) =>
                Results.Ok(data.GetAll()));

            app.MapGet("/books/{id:int}",
                Results<Ok<Book>, NotFound> (int id, BookData data) =>
                {
                    var book = data.GetById(id);
                    return book is null ? TypedResults.NotFound() : TypedResults.Ok(book);
                });
        }
    }
}