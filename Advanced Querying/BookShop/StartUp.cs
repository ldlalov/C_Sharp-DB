namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using System;
    using System.Globalization;
    using System.Text;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

    public class StartUp
    {
        public static void Main()
        {
            var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);
            Console.WriteLine(GetAuthorNamesEndingIn(db, "e"));
            //IncreasePrices(db);
        }
        //16
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Copies < 4200)
                .ToList();
            foreach (var b in books)
            {
                context.Books.Remove(b);
            }
                context.SaveChanges();
            return books.Count();
        }
        //15
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var b in books)
            {
                b.Price += 5;
            }
                context.SaveChanges();

        }
        //14
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();
            var categories = context.Categories
                .Select(c => new
                {
                    c.Name,
                    books = c.CategoryBooks.Select(b => new { b.Book.Title, b.Book.ReleaseDate })
                    .OrderByDescending(b => b.ReleaseDate)
                    .Take(3).ToList()
                })
                .OrderBy(c => c.Name)
                .ToArray();

            foreach (var c in categories)
            {
                result.AppendLine($"--{c.Name}");
                foreach (var b in c.books)
                {
                    result.AppendLine($"{b.Title} ({b.ReleaseDate.Value.Year})");
                }
            }
            return result.ToString().TrimEnd();

        }
        //13
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();
            var books = context.Categories
                .Select(b => new
                {
                    name = b.Name,
                    profit = b.CategoryBooks.Sum(b => b.Book.Price * b.Book.Copies),
                })
                .OrderByDescending(b => b.profit)
                .ToArray();

            foreach (var b in books)
            {
                result.AppendLine($"{b.name} ${b.profit}");
            }
            return result.ToString().TrimEnd();

        }
        //12
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();
            var authors = context.Authors
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    copies = a.Books.Sum(b => b.Copies),
                }
                )
                .OrderByDescending(b => b.copies)
                .ToArray();

            foreach (var b in authors)
            {
                result.AppendLine($"{b.FirstName} {b.LastName} - {b.copies}");
            }
            return result.ToString().TrimEnd();

        }
        //11
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            StringBuilder result = new StringBuilder();
            var books = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .ToArray();

            return books.Count();

        }
        //10
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder result = new StringBuilder();
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new {b.BookId,b.Title,b.Author.FirstName,b.Author.LastName})
                .OrderBy(b => b.BookId)
                .ToArray();

            foreach (var b in books)
            {
                result.AppendLine($"{b.Title} ({b.FirstName} {b.LastName})");
            }
            return result.ToString().TrimEnd();

        }
        //9
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder result = new StringBuilder();
            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => new { b.Title })
                .OrderBy(b => b.Title)
                .ToArray();

            foreach (var b in books)
            {
                result.AppendLine($"{b.Title}");
            }
            return result.ToString();

        }
        //8
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            //tringBuilder result = new StringBuilder();
            var autors = context.Authors
                .Where(fn => EF.Functions.Like( fn.FirstName,$"%{input}"))
                .Select(a => new {a.FirstName,a.LastName})
                .OrderBy(a => a.FirstName)
                .ToArray();

            var result = string.Join(Environment.NewLine, autors.Select(a => $"{a.FirstName} {a.LastName}"));
            //foreach (var a in autors)
            //{
            //    result.AppendLine($"{a.FirstName} {a.LastName}");
            //}
            return result;

        }
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder result = new StringBuilder();
            var books = context.Books
                .Where(b => b.ReleaseDate.Value < DateTime.ParseExact(date,"dd-MM-yyyy",CultureInfo.InvariantCulture))
                .Select(b => new {b.Title,b.EditionType,b.Price,b.ReleaseDate})
                .OrderByDescending(b => b.ReleaseDate)
                .ToList();

            foreach (var b in books)
            {
                result.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:F2}");
            }
            return result.ToString().TrimEnd();

        }
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToLower().Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();
            StringBuilder result = new StringBuilder();
            var books = context.Books
                .Where(b => b.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            foreach (var b in books)
            {
                result.AppendLine(b);
            }
            return result.ToString().TrimEnd();

        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder result = new StringBuilder();
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => new { b.BookId, b.Title })
                .OrderBy(b => b.BookId)
                .ToList();

            foreach (var b  in books)
            {
                result.AppendLine(b.Title);
            }
            return result.ToString().TrimEnd();

        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();
            var books = context.Books
                .Where(books => books.Price >= 40)
                .Select(b => new { b.Title, b.Price })
                .OrderByDescending(b => b.Price)
                .ToList();
            foreach (var b in books)
            {
                result.AppendLine($"{b.Title} - ${b.Price:F2}");
            }
            return result.ToString().TrimEnd();

        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder result = new StringBuilder();
            var converter = Enum.Parse<EditionType>("Gold");
            var goldenBooks = context.Books
                .Where(books => books.EditionType == converter && books.Copies < 5000)
                .Select(b => new { b.BookId, b.Title })
                .OrderBy(b => b.BookId)
                .ToList();
            foreach (var gb in goldenBooks)
            {
                result.AppendLine(gb.Title);
            }

            return result.ToString().TrimEnd();

        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var converter = Enum.Parse<AgeRestriction>(command, true);
            StringBuilder result = new StringBuilder();
            var titles = context.Books
                .Where(x => x.AgeRestriction == converter)
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();
            foreach (var title in titles)
            {
                result.AppendLine(title);
            }
            return result.ToString().TrimEnd();
        }
    }
}


