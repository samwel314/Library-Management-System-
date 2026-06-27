
use LibraryManagementSystem


-- 1. (Categories)
INSERT INTO Categories (Name, ParentCategoryId)
VALUES  
('Programming', NULL),        -- Id: 1
('Backend Development', 1),   -- Id: 2
('Database', 1);              -- Id: 3

-------------------------------------------------------------------

-- 2.   (Publishers)
INSERT INTO Publishers (Name, Country, ContactInfo)
VALUES  
('Manning Publications', 'USA', 'contact@manning.com'), -- Id: 1
('Microsoft Press', 'USA', 'press@microsoft.com'),      -- Id: 2
('O''Reilly Media', 'USA', 'info@oreilly.com');         -- Id: 3


-- 3.  (Authors)
INSERT INTO Authors (Name, Bio)
VALUES  
('Robert C. Martin', 'Clean Code Author'),         -- Id: 1
('Martin Fowler', 'Software Architecture Expert'), -- Id: 2
('Jon Skeet', 'C# Deep Expert');                   -- Id: 3



-- 4. (Members)
INSERT INTO Members (Name, Phone, Email , CreatedAt)
VALUES  
('Ahmed Ali', '01011111111', 'ahmed@test.com', GETUTCDATE()),
('Mohamed Hassan', '01022222222', 'mohamed@test.com', GETUTCDATE()),
('Sara Mohamed', '01033333333', 'sara@test.com', GETUTCDATE());

-- 5. (Books)
INSERT INTO Books (Title, ISBN, Language, PublicationYear, Edition, Summary, PublisherId, CategoryId, Status)
VALUES  
('Clean Code', '9780132350884', 'English', 2008, '1st', 'Clean Code book', 1, 1, 1), -- Id: 1 (Status: Out)
('Refactoring', '9780201485677', 'English', 1999, '2nd', 'Improving design', 2, 1, 1), -- Id: 2 (Status: Out)
('Clean Architecture', '9780134494166', 'English', 2017, '1st', 'Architecture principles', 1, 2, 0), -- Id: 3 (Status: In)
('C# in Depth', '9781617294532', 'English', 2019, '4th', 'C# advanced concepts', 2, 3, 0), -- Id: 4 (Status: Out)
('Domain-Driven Design', '9780321125217', 'English', 2003, '1st', 'DDD fundamentals', 3, 2, 0); -- Id: 5 (Status: In)

-- 6. (BookAuthors)

INSERT INTO BookAuthors (BookId, AuthorId)
VALUES  
(1, 1), 
(2, 2),
(3, 1), 
(4, 3);

-- 7. (BorrowTransactions)
INSERT INTO BorrowTransactions (BookId, MemberId, BorrowDate, DueDate, ReturnDate)
VALUES  
(1, 1, GETUTCDATE(), DATEADD(day, 7, GETUTCDATE()), NULL),
(2, 2, GETUTCDATE(), DATEADD(day, 5, GETUTCDATE()), NULL);


