# Library Management System

## Overview

This project is a RESTful **Library Management System** built using **ASP.NET Core Web API**, **Entity Framework Core**, **SQL Server**, and **ASP.NET Identity**.

The primary goal was to build a secure and maintainable backend that demonstrates authentication, authorization, normalized database design, and business logic for managing books, members, borrowing operations, and system users.

---

# Features

* JWT Authentication using ASP.NET Identity
* Role-Based Authorization (Administrator, Librarian, Staff)
* Book Management (Create, Read, Update, Delete)
* Author, Publisher, Category, Member, and User Management
* Book Borrowing and Return
* Borrowing History for Members and Books
* Active Borrowings
* User Activity Logging
* Book Search by Title, Author, and Category
* Book Filtering by Status
* Pagination for Book Listing
* Book Cover Image Upload
* Hierarchical Categories

---

# Design Choices

## Authentication & User Management

I chose **ASP.NET Identity** instead of implementing authentication manually because it provides secure password hashing, user management, and role management out of the box.

The required system roles (**Administrator**, **Librarian**, and **Staff**) together with the default administrator account are automatically created during application startup using a database initializer.

---

## JWT Authentication

The API uses **JWT Authentication** to keep the application stateless and suitable for both web and mobile clients.

After a successful login, authenticated users receive a JWT token that must be included when accessing protected endpoints.

---

## Role-Based Authorization

Authorization is implemented using **ASP.NET Core Role-Based Authorization**.

The system contains three roles:

* Administrator
* Librarian
* Staff

This ensures that only authorized users can access protected resources.

---

## Database Design

The database was designed using normalization principles.

Instead of storing author names, publishers, or categories directly inside the **Books** table, each entity has its own table and relationship.

This design:

* Reduces duplicated data.
* Improves maintainability.
* Makes future extensions easier.

---

## Data Access

The application uses Entity Framework Core's **DbContext** directly inside the service layer.

Since Entity Framework Core already implements the Repository and Unit of Work patterns, introducing an additional repository layer would add unnecessary abstraction for the current project while providing little practical benefit.

---

## Books & Authors Relationship

Books and Authors have a **many-to-many relationship** implemented using the **BookAuthors** junction table.

This allows:

* One book to have multiple authors.
* One author to write multiple books.

---

## Hierarchical Categories

Categories use a **self-referencing relationship**.

Example:

```text
Programming
├── Backend Development
└── Database
```

This allows adding unlimited category levels without changing the database schema.

---

## Borrowing Design

Borrowing operations are stored inside a dedicated **BorrowTransactions** table instead of modifying book records.

Keeping every borrowing transaction allows the system to provide:

* Borrowing history per member.
* Borrowing history per book.
* Active borrowings.
* Future reporting capabilities.

---

## Book Availability

Each book maintains a **Status** (Available / Borrowed).

The status is automatically updated whenever a borrowing or return operation occurs, allowing availability checks without querying borrowing history.

---

## Activity Logging

Important user operations are stored inside a dedicated **UserActivityLogs** table.

Examples include:

* Book Created
* Book Updated
* Book Deleted
* Borrow Book
* Return Book

Keeping activity logs in a separate table keeps the domain model clean while providing a lightweight audit trail.

---

## Result Pattern

The application uses a custom **Result Pattern** to standardize service responses.

Expected business scenarios return a unified result object instead of throwing exceptions, providing consistent API responses and keeping controller logic simple.

---

## Global Exception Handling

Unexpected exceptions are handled centrally using ASP.NET Core's global exception handling mechanism.

This ensures that unhandled exceptions return consistent HTTP responses while preventing internal implementation details from being exposed to clients.

---

## Service Layer

Business logic is implemented inside **Services** rather than Controllers.

Controllers are responsible only for:

* Receiving HTTP requests.
* Returning HTTP responses.

This separation improves readability, maintainability, and makes business logic easier to modify.

---

## DTO Usage

DTOs are used for every request and response.

This provides several advantages:

* Prevent exposing database entities directly.
* Validate client input.
* Keep API contracts independent from persistence models.

---

## Image Storage

Book cover images are stored on disk while only the image path is stored inside the database.

This keeps the database smaller and makes replacing the storage provider easier in the future.

---

# Running the Project

Before running the application:

1. Configure the **Connection String** inside `appsettings.json`.

2. Configure the **JWT Settings** inside `appsettings.json`.

3. Run the application.

During startup, the application automatically:

* Applies pending Entity Framework Core migrations.
* Creates the required system roles.
* Creates the default administrator account (if it does not already exist).

Default Administrator Credentials

**Email**

```text
admin@library.com
```

**Password**

```text
Admin@123
```

4. Execute the provided **seed-data.sql** script after the application starts successfully.

> **Note:** Before executing the script, update the `USE` statement at the top of the script to match your local database name.

The script inserts sample:

* Categories
* Publishers
* Authors
* Members
* Books
* BookAuthors
* Borrow Transactions

---

# Additional Files

The repository includes:

* Entity Relationship Diagram (ERD)
* SQL Seed Script
* Postman Collection
