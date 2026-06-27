using LibraryManagement.Api.Data;
using LibraryManagement.Api.DTOs;
using LibraryManagement.Api.Entities;
using LibraryManagement.Api.Services.Interfaces;
using LibraryManagement.Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagement.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly LibraryDbContext _db;

        public CategoryService(LibraryDbContext db)
        {
            _db = db;
        }

        public async Task<ResultT<int?>> CreateAsync(CategoryRequestDto requestDto , CancellationToken cancellation)
        {
            requestDto.Name = requestDto.Name.Trim();   
            var isSameNameExist = await _db.Categories.AnyAsync(c => c.Name == requestDto.Name , cancellation);
            if (isSameNameExist)
                return ResultT<int?>.Failure("Category name already exists.", ErrorType.Conflict);  
            if (requestDto.ParentId != null )
            {
                var isParentExist = await _db.Categories.AnyAsync(c => c.Id == requestDto.ParentId , cancellation);
                if  (!isParentExist)
                    return ResultT<int?>.Failure($"no vaild parent found for this category", ErrorType.NotFound);
            }
            var category = new Category 
            { 
                Name = requestDto.Name  ,
                ParentCategoryId = requestDto.ParentId
            };
            await _db.Categories.AddAsync(category , cancellation);   
            await _db.SaveChangesAsync(cancellation);
            return ResultT<int?>.Success(category.Id , "category created successfuly"); 
        }
        public async Task<ResultT<IEnumerable<CategoryLookupDto>>> GetAllLookupAsync( CancellationToken cancellation)
        {
           var categories = await _db.Categories.AsNoTracking().Select( c=>  new  CategoryLookupDto
           {
               Id = c.Id ,  
               Name = c.Name ,  
           }).ToListAsync   (cancellation); 
           return ResultT<IEnumerable<CategoryLookupDto>>.Success(categories);  
        }
        public async Task<ResultT<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken cancellation)
        {
            var categories= await _db.Categories.AsNoTracking().Select(c => new CategoryDto
            {
            
                Id = c.Id ,
                Name= c.Name ,  
                Parent = c.ParentCategory != null ?
                new CategoryLookupDto
                {
                    Id = c.ParentCategory.Id ,
                    Name = c.ParentCategory.Name ,
                }  : null
            }).ToListAsync (cancellation); 
            return ResultT<IEnumerable<CategoryDto>>.Success(categories);
        }
        public async Task<ResultT<CategoryDto>> GetByIdAsync(int id , CancellationToken cancellation)
        {
            var category = await _db.Categories.AsNoTracking().Select(c => new CategoryDto
            {

                Id = c.Id,
                Name = c.Name,
                Parent = c.ParentCategory != null ?
                new CategoryLookupDto
                {
                    Id = c.ParentCategory.Id,
                    Name = c.ParentCategory.Name,
                } : null
            }).FirstOrDefaultAsync( c => c.Id == id ,  cancellation);
            if (category == null)
                return ResultT<CategoryDto>.Failure("this category is not found" , ErrorType.NotFound);

            return ResultT<CategoryDto>.Success(category);
        }
        public async Task<Result> UpdateAsync(int id, CategoryRequestDto requestDto, CancellationToken cancellation)
        {
            requestDto.Name = requestDto.Name.Trim();
            var category = await _db.Categories.FindAsync(id , cancellation); 
            if (category == null )
                return Result.Failure($"Category not found", ErrorType.NotFound);

            var isSameNameExist = await 
                _db.Categories.AnyAsync(c => c.Name == requestDto.Name && c.Id != id , cancellation);
            if (isSameNameExist)
                return Result.Failure("Category name already exists.", ErrorType.Conflict);
    
            if (requestDto.ParentId != null)
            {
                if (requestDto.ParentId == category.Id)
                    return Result.Failure($"A category cannot be its own parent. ", ErrorType.Validation);

                var isParentExist = await _db.Categories.AnyAsync(c => c.Id == requestDto.ParentId, cancellation);
                if (!isParentExist)
                    return Result.Failure($"No valid parent category found.", ErrorType.NotFound);
            }
            category.Name = requestDto.Name;    
            category.ParentCategoryId = requestDto.ParentId;
            await _db.SaveChangesAsync(cancellation);
            return Result.Success("Category updated successfuly");
        }
        public async Task<Result> DeleteAsync(int id, CancellationToken cancellation)
        {

            var category = await _db.Categories.FindAsync(id, cancellation);
            if (category == null)
                return Result.Failure($"Category not found", ErrorType.NotFound);
            var isParent = await _db.Categories.AnyAsync(c=> c.ParentCategoryId == id, cancellation);
            if (isParent)
                return Result.Failure($"Cannot delete because it has subcategories ", ErrorType.Validation);
            var hasBooks = await _db.Books.AnyAsync(b => b.CategoryId == id, cancellation);
            if (hasBooks)
                return Result.Failure($"Cannot delete because it has books", ErrorType.Validation);

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync(cancellation);

            return Result.Success("Category deleted successfuly");
        }
    }
}
