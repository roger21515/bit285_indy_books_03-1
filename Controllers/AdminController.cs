using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IndyBooks.Models;
using IndyBooks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Controllers
{
    public class AdminController : Controller
    {
        private IndyBooksDataContext _db;
        public AdminController(IndyBooksDataContext db) { _db = db; }

        /***
         * READ       
         */
        [HttpGet]
        public IActionResult Index(long id)
        {
            IQueryable<Book> books = _db.Books.Include(b => b.Author);
            if (id == 0)
            {
                return View("SearchResults", books);
            }

            return View("SearchResults", books.Where(b => b.Id == id));
        }
        /***
         * CREATE
         */
        [HttpGet]
        public IActionResult CreateBook()
        {

            //TODO: Build a new CreateBookViewModel with a complete set of Writers from the database
            var createViewModel = new CreateBookViewModel { Writers = _db.Writers };
            return View("CreateBook", createViewModel); //TODO: pass the ViewModel onto the CreateBook View
        }
        [HttpPost]
        public IActionResult CreateBook(CreateBookViewModel bookVM)
        {
            //TODO: Build the Writer object using the parameter
            Writer writer;
            if (bookVM.AuthorId > 0)
            {
                writer = _db.Writers.Where(w => w.Id.Equals(bookVM.AuthorId)).FirstOrDefault();
            }
            else
            {
                writer = new Writer();
                writer.Name = bookVM.Name;

            }
            //TODO: Build the Book using the parameter data and your newly created author.
            Book book = new Book()
            {
                Name = bookVM.Name,
                BookId = bookVM.BookId,
                Title = bookVM.Title,
                SKU = bookVM.SKU,
                Price = bookVM.Price,
                AuthorId = bookVM.AuthorId,
            };

            //TODO: Add author and book to their DbSets; SaveChanges
            _db.Writers.Add(writer);
            _db.Books.Add(book);
            _db.SaveChanges();
            //TODO: Show the book by passing the Book's id (rather than 1) to the Index Action 
            return RedirectToAction("Index", new { id = bookVM.BookId });
        }
        /***
         * UPDATE (reusing the CreateBook View ) 
         */
        //TODO: Write a method to take a book id, and load book and author info
        //      into the ViewModel for the CreateBook View
        [HttpPut]
        public IActionResult UpdateBook(long BookId, Book book)
        {
            book = _db.Books.Where(w => w.BookId == BookId).FirstOrDefault();
            return View(book);
        }
        /***
         * DELETE
         */
        [HttpGet]
        public IActionResult DeleteBook(long id)
        {
            //TODO: Remove the Book associated with the given id number; Save Changes
            if (id > 0)
            {
                var book = _db.Books.Where(w => w.BookId == id).FirstOrDefault();
                if (book != null)
                {
                    _db.Books.Remove(book);
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Search() { return View(); }
        [HttpPost]
        public IActionResult Search(SearchViewModel search)
        {
            //Full Collection Search
            IQueryable<Book> foundBooks = _db.Books.Include(b => b.Author); // start with entire collection

            //Partial Title Search
            if (search.Title != null)
            {
                foundBooks = foundBooks
                            .Where(b => b.Title.Contains(search.Title))
                            .OrderBy(b => b.Author.Name)
                            ;
            }

            //Author's Last Name Search
            if (search.AuthorName != null)
            {
                //Use the Name property of the Book's Author entity
                foundBooks = foundBooks
                            .Where(b => b.Author.Name.EndsWith(search.AuthorName))
                            ;
            }
            //Priced Between Search (min and max price entered)
            if (search.MinPrice > 0 && search.MaxPrice > 0)
            {
                foundBooks = foundBooks
                            .Where(b => b.Price >= search.MinPrice && b.Price <= search.MaxPrice)
                            ;
            }
            //Composite Search Results
            return View("SearchResults", foundBooks);
        }

    }
}
