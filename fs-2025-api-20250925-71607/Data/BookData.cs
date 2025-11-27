using fs_2025_api_20250925_71607.Models;

namespace fs_2025_api_20250925_71607.Data
{
    public class BookData
    {
        private readonly List<Book> _books = new()
        {
            new() { Id=1,  Title="To Kill a Mockingbird",        Isbn="9780061120084", PublicationDate=new DateOnly(1960,7,11), Author="Harper Lee" },
            new() { Id=2,  Title="1984",                         Isbn="9780451524935", PublicationDate=new DateOnly(1949,6,8),  Author="George Orwell" },
            new() { Id=3,  Title="Pride and Prejudice",          Isbn="9780141439518", PublicationDate=new DateOnly(1813,1,28), Author="Jane Austen" },
            new() { Id=4,  Title="The Great Gatsby",             Isbn="9780743273565", PublicationDate=new DateOnly(1925,4,10), Author="F. Scott Fitzgerald" },
            new() { Id=5,  Title="Moby-Dick",                    Isbn="9781503280786", PublicationDate=new DateOnly(1851,10,18), Author="Herman Melville" },
            new() { Id=6,  Title="Brave New World",              Isbn="9780060850524", PublicationDate=new DateOnly(1932,1,1),  Author="Aldous Huxley" },
            new() { Id=7,  Title="The Catcher in the Rye",       Isbn="9780316769488", PublicationDate=new DateOnly(1951,7,16), Author="J.D. Salinger" },
            new() { Id=8,  Title="The Hobbit",                   Isbn="9780547928227", PublicationDate=new DateOnly(1937,9,21), Author="J.R.R. Tolkien" },
            new() { Id=9,  Title="Crime and Punishment",         Isbn="9780486415871", PublicationDate=new DateOnly(1866,1,1),  Author="Fyodor Dostoevsky" },
            new() { Id=10, Title="One Hundred Years of Solitude",Isbn="9780060883287", PublicationDate=new DateOnly(1967,5,30), Author="Gabriel García Márquez" }
        };

        public IEnumerable<Book> GetAll() => _books;
        public Book? GetById(int id) => _books.FirstOrDefault(b => b.Id == id);
    }
}