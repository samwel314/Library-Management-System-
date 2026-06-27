using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Entities;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using System.Diagnostics.CodeAnalysis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace LibraryManagement.Api.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _db;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUserActivityLogService _activityLog;

        public BookService(LibraryDbContext db, IFileStorageService fileStorageService, IUserActivityLogService activityLog)
        {
            _db = db;
            _fileStorageService = fileStorageService;
            _activityLog = activityLog;
        }

        public async Task<ResultT<int?>> CreateAsync(CreateBookDto requestDto, CancellationToken cancellation)
        {
            requestDto.Title = requestDto.Title.Trim();
            requestDto.ISBN = requestDto.ISBN.Trim();
            requestDto.Language = requestDto.Language.Trim();
            requestDto.Edition = requestDto.Edition?.Trim();
            requestDto.Summary = requestDto.Summary?.Trim();

            requestDto.AuthorsIds = requestDto.AuthorsIds.Distinct().ToList();
            if (!requestDto.AuthorsIds.Any())
                return ResultT<int?>.Failure("Booke should belong to at least one author", ErrorType.Validation);

            var isExistedISBN = await _db.Books.AnyAsync(b => b.ISBN == requestDto.ISBN, cancellation);
            if (isExistedISBN)
                return ResultT<int?>.Failure("ISBN already exists.", ErrorType.Conflict);

            var isExistedPublisher = await _db.Publishers.AnyAsync(p => p.Id == requestDto.PublisherId, cancellation); 
            if (!isExistedPublisher)
                return ResultT<int?>.Failure("Publisher not found", ErrorType.NotFound);

            var isExistedCategory = await _db.Categories.AnyAsync(c => c.Id == requestDto.CategoryId, cancellation);
            if (!isExistedCategory)
                return ResultT<int?>.Failure("Category not found", ErrorType.NotFound);
            
            var existingAuthorsCount = await _db.Authors.CountAsync(a => requestDto.AuthorsIds.Contains(a.Id), cancellation);

            if (existingAuthorsCount!= requestDto.AuthorsIds.Count)
                return ResultT<int?>.Failure("Some authors ids not found", ErrorType.NotFound);

            /// 
            var extension = Path.GetExtension(requestDto.CoverImage.FileName).ToLower();

            if  (!FileStorageService.Allows.Contains(extension))
            {
                return ResultT<int?>.Failure($"Allow extensions {string.Join(',', FileStorageService.Allows)}",
                    ErrorType.Validation);
            }
            var coverImageUrl = await _fileStorageService.SaveImageAsync(requestDto.CoverImage ,cancellation);
            var bookAuthors = requestDto.AuthorsIds.Select(id => new BookAuthor
            {
                AuthorId = id
            }).ToList(); 

            var book = new Book 
            {
                Title = requestDto.Title,
                ISBN = requestDto.ISBN ,
                Language  = requestDto.Language, 
                Summary = requestDto.Summary ,  
                Edition = requestDto.Edition ,  
                PublicationYear = requestDto.PublicationYear ,  
                CategoryId  = requestDto.CategoryId ,   
                PublisherId = requestDto.PublisherId ,  
                CoverImageUrl = coverImageUrl ,
                BookAuthors = bookAuthors   
             };
             await  _db.Books.AddAsync(book , cancellation);
            await _activityLog.LogAsync($"Book Created", cancellation);
            await _db.SaveChangesAsync(cancellation);
            return ResultT<int?>.Success(book.Id, "book created successfuly");
        }

        public async Task<ResultT<IEnumerable<BookDto>>> GetAllAsync(BookFilterDto bookFilter , CancellationToken cancellation)
        {
            var booksQuery = _db.Books.AsNoTracking();
            if (bookFilter.Status != null)
                booksQuery = booksQuery.Where(b => b.Status == bookFilter.Status); 
            if (!string.IsNullOrWhiteSpace(bookFilter.Title))
                booksQuery = booksQuery.Where(b => b.Title.Contains( bookFilter.Title));
            if (!string.IsNullOrWhiteSpace(bookFilter.Category))
                booksQuery = booksQuery.Where(b => b.Category.Name.Contains(bookFilter.Category));
            if (!string.IsNullOrWhiteSpace(bookFilter.Author))
                booksQuery = booksQuery.Where(b =>
                    b.BookAuthors.Any(a => a.Author.Name.Contains(bookFilter.Author)));
            if (bookFilter.Page <= 0)
                bookFilter.Page = 1;
            if (bookFilter.PageSize < 5)
                bookFilter.PageSize = 5;


            var books =  await booksQuery.Select(b => new BookDto
            {
                Id = b.Id,  
                Title   = b.Title,  
                Authors = b.BookAuthors.Select(a => new AuthorLookupDto
                {
                    Id = a.Author.Id,  
                    Name = a.Author.Name,  
                }).ToList(),    
                Category = new CategoryLookupDto
                {
                    Id = b.Category.Id,
                    Name = b.Category.Name, 
                } , 
                Publisher = new PublisherLookupDto
                {
                    Id = b.Publisher.Id,
                    Name = b.Publisher.Name,
                },
                Status = b.Status , 
                CoverImageUrl = b.CoverImageUrl!
            }).Skip((bookFilter.Page - 1) * bookFilter.PageSize)
                    .Take(bookFilter.PageSize).ToListAsync(cancellation); 

            return ResultT<IEnumerable<BookDto>>.Success(books); ;
        }

        public async Task<ResultT<BookDetailsDto>> GetByIdAsync(int id, CancellationToken cancellation)
        {
            var book = await _db.Books.Where(b => b.Id == id).Select(b => new BookDetailsDto
            {
                Id = b.Id , 
                Title = b.Title ,   
                Edition = b.Edition , 
                Summary = b.Summary ,   
                Status = b.Status , 
                ISBN    = b.ISBN ,  
                Category = new CategoryDto
                {
                    Id = b.Category.Id, 
                    Name = b.Category.Name,
                    Parent = b.Category.ParentCategory != null ?  new CategoryLookupDto
                    {
                        Id = b.Category.ParentCategory.Id,
                        Name = b.Category.ParentCategory.Name,
                    } 
                    : null
                }, 
                Language = b.Language , 
                CoverImageUrl = b.CoverImageUrl! ,
                PublicationYear = b.PublicationYear ,   
                Publisher = new PublisherDto
                {
                    Id = b.Publisher.Id,    
                    Name = b.Publisher.Name,
                    ContactInfo = b.Publisher.ContactInfo,
                    Country = b.Publisher.Country , 
                }, 
                Authors = b.BookAuthors.Select(a => new AuthorLookupDto
                {
                    Id = a.Author.Id , 
                    Name = a.Author.Name,   
                }).ToList(),    
            }).FirstOrDefaultAsync(cancellation);
            if (book == null)
                return ResultT<BookDetailsDto>.Failure("this book is not found", ErrorType.NotFound);

            return ResultT<BookDetailsDto>.Success(book);
        }

        public async Task<Result> UpdateAuthorsAsync(int id, UpdateBookAuthorsDto requestDto, CancellationToken cancellation)
        {
            requestDto.AuthorIds = requestDto.AuthorIds.Distinct().ToList();
            if (!requestDto.AuthorIds.Any())
                return Result.Failure("Booke should belong to at least one author", ErrorType.Validation);
            var isFound = await _db.Books.AnyAsync(b => b.Id == id, cancellation);
            if (!isFound )
                return Result.Failure("Book not found", ErrorType.NotFound);

            var existingAuthorsCount = await _db.Authors.CountAsync(a => requestDto.AuthorIds.Contains(a.Id), cancellation);

            if (existingAuthorsCount != requestDto.AuthorIds.Count)
                return Result.Failure("Some authors ids not found", ErrorType.NotFound);
            var oldList = await _db.BookAuthors.Where(ba => ba.BookId == id).ToListAsync(cancellation);
            _db.BookAuthors.RemoveRange(oldList);
            var bookAuthors = requestDto.AuthorIds.Select(aid => new BookAuthor
            {
                BookId = id,
                AuthorId = aid
            });

            await _db.BookAuthors.AddRangeAsync(bookAuthors, cancellation);
            await _activityLog.LogAsync($"Book Updated (Id: {id}) - > Authors", cancellation);

            await _db.SaveChangesAsync(cancellation);
            return Result.Success("book updated successfuly"); 
        }

        public async Task<Result> UpdateBasicInfoAsync(int id, UpdateBookBasicInfoDto requestDto, CancellationToken cancellation)
        {
            requestDto.Title = requestDto.Title.Trim();
            requestDto.ISBN = requestDto.ISBN.Trim();
            requestDto.Language = requestDto.Language.Trim();
            requestDto.Edition = requestDto.Edition?.Trim();
            requestDto.Summary = requestDto.Summary?.Trim();
            var book =await _db.Books.FindAsync(id, cancellation); 
            if (book == null)
                return Result.Failure("Book not found", ErrorType.NotFound);

            var isExistedISBN = await _db.Books.AnyAsync(b => b.ISBN == requestDto.ISBN && b.Id != id, cancellation);
            if (isExistedISBN)
                return Result.Failure("ISBN already exists.", ErrorType.Conflict);

            var isExistedPublisher = await _db.Publishers.AnyAsync(p => p.Id == requestDto.PublisherId, cancellation);
            if (!isExistedPublisher)
                return Result.Failure("Publisher not found", ErrorType.NotFound);

            var isExistedCategory = await _db.Categories.AnyAsync(c => c.Id == requestDto.CategoryId, cancellation);
            if (!isExistedCategory)
                return Result.Failure("Category not found", ErrorType.NotFound);

            book.Title = requestDto.Title;
            book.ISBN = requestDto.ISBN;
            book.Language = requestDto.Language;
            book.Summary = requestDto.Summary;
            book.Edition = requestDto.Edition;
            book.PublicationYear = requestDto.PublicationYear;
            book.CategoryId = requestDto.CategoryId;
            book.PublisherId = requestDto.PublisherId;
            await _activityLog.LogAsync($"Book Updated (Id: {id}) - > BasicInfo", cancellation);
            await _db.SaveChangesAsync (cancellation);
            return Result.Success("book updated successfuly"); 
        }

        public async Task<Result> UpdateCoverImageAsync(int id, UpdateBookCoveImageDto requestDto, CancellationToken cancellation)
        {

            var extension = Path.GetExtension(requestDto.CoverImage.FileName).ToLower();

            if (!FileStorageService.Allows.Contains(extension))
            {
                return Result.Failure($"Allow extensions {string.Join(',', FileStorageService.Allows)}",
                    ErrorType.Validation);
            }
            var book = await _db.Books.FindAsync(id, cancellation);
            if (book == null)
                return Result.Failure("Book not found", ErrorType.NotFound);
            var coverImageUrl = await _fileStorageService.SaveImageAsync(requestDto.CoverImage, cancellation);
            _fileStorageService.DeleteImage(book.CoverImageUrl!);        
            book.CoverImageUrl = coverImageUrl;
            await _activityLog.LogAsync($"Book Updated (Id: {id}) - > cover Image", cancellation);
            await _db.SaveChangesAsync(cancellation);
            return Result.Success("book updated successfuly");
        }
        public async Task<Result> DeleteAsync(int id, CancellationToken cancellation)
        {

            var book = await _db.Books.FindAsync(id, cancellation);
            if (book == null)
                return Result.Failure($"book not found", ErrorType.NotFound);
            var hasBorrowingHistory = await _db.BorrowTransactions.AnyAsync(b => b.BookId == id, cancellation);
            if (hasBorrowingHistory)
                return Result.Failure($"this book has browing history", ErrorType.Validation);

            _fileStorageService.DeleteImage(book.CoverImageUrl!);
            _db.Books.Remove(book);
            await _activityLog.LogAsync($"Book Deleted (Id: {id})", cancellation);
            await _db.SaveChangesAsync(cancellation);

            return Result.Success("book deleted successfuly");
        }
        public async Task<ResultT<IEnumerable<BookBorrowTransactionDto>>> GetBorrowHistoryAsync(
    int id,
    CancellationToken cancellation)
        {
            var bookExists = await _db.Books.AnyAsync(b => b.Id == id, cancellation);

            if (!bookExists)
                return ResultT<IEnumerable<BookBorrowTransactionDto>>
                    .Failure("Book not found", ErrorType.NotFound);

            var bookBorrowTransactions = await _db.BorrowTransactions
                .AsNoTracking()
                .Where(bt => bt.BookId == id)
                .OrderByDescending(bt => bt.BorrowDate)
                .Select(bt => new BookBorrowTransactionDto
                {
                    Id = bt.Id,
                    BorrowDate = bt.BorrowDate,
                    DueDate = bt.DueDate,
                    ReturnDate = bt.ReturnDate,
                    Member = new MemberLookupDto
                    {
                        Id = bt.MemberId,
                        Name = bt.Member.Name
                    }
                })
                .ToListAsync(cancellation);

            return ResultT<IEnumerable<BookBorrowTransactionDto>>
                .Success(bookBorrowTransactions);
        }
    }


    
}

