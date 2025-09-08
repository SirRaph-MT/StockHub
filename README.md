## Assumptions
- The application uses Entity Framework Core with a SQL Server database.
- The `Product`, `Order`, and `OrderItem` models are pre-defined with appropriate relationships.
- Stock quantities are managed server-side to prevent overselling.
- Concurrent requests are handled using optimistic concurrency with `RowVersion`.

## Tech Stack 
- **Language**: C# (.NET Core)
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Serialization**: System.Text.Json with DTOs to handle circular references
- **Database**: SQL Server (configurable via connection string)
- **Transaction Management**: EF Core transactions for data integrity

## The Highlight
- The API includes CRUD operations for products and a `PlaceOrder` endpoint with stock validation.
- A migration is required to add the `RowVersion` column to the `Product` table for concurrency control.
- DTOs are used to shape API responses and avoid serialization issues.
- The solution is designed for scalability and maintainability, suitable for an enterprise production environment.
