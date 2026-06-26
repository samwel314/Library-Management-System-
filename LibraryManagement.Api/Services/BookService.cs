using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Entities;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using static System.Net.Mime.MediaTypeNames;

namespace LibraryManagement.Api.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _db;
        private readonly IFileStorageService _fileStorageService;

        public BookService(LibraryDbContext db, IFileStorageService fileStorageService)
        {
            _db = db;
            _fileStorageService = fileStorageService;
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
            var bookAuthors = new List<BookAuthor>();
            foreach (var id in requestDto.AuthorsIds)
                bookAuthors.Add(new BookAuthor
                {
                    AuthorId = id,  
                }); 

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

            await _db.SaveChangesAsync(cancellation);
            return ResultT<int?>.Success(book.Id, "book created successfuly");
        }
    }
}
