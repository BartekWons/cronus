# Cronus

Cronus is a lightweight, local relational database library for .NET, implemented in **C#**.  
It is designed as an alternative to full-scale database systems for small and offline-ready applications, providing a simple yet structured way to store and query relational data.

The library combines a **custom ORM layer** with a **SQL-inspired query language**, while storing all data persistently in a single **JSON file**.

---

## Features

- Local, file-based relational database (JSON storage)
- Strongly typed ORM (Object-Relational Mapping)
- Custom SQL-like query language
- Fluent API for database configuration
- CRUD operations:
  - Insert
  - Select
  - Update
  - Delete
- Data filtering and querying
- One-to-many (1:N) table relationships
- Automatic primary key handling (auto-increment)
- Schema and data validation
- Lightweight and infrastructure-free

---

## Technology Stack

- **Language:** C#
- **Platform:** .NET 8.0
- **Serialization:** Newtonsoft.Json
- **Query Parsing:** Sprache
- **Testing:** NUnit, Moq
- **CI:** GitHub Actions

---

## Architecture Overview

Cronus follows a layered architecture:

- **ORM Layer** – Maps C# classes to database tables using attributes
- **Query Parser** – Parses SQL-like queries into executable operations
- **Database Engine** – Manages in-memory data and JSON persistence
- **Builder & Runtime** – Handles schema registration and runtime execution

The database operates on a single active session in memory and writes changes atomically back to disk.

---

## Supported Data Types

Cronus enforces strong typing and supports the following data types:

- `string`
- `bool`
- `int`
- `double`

Additionally:
- `float` → cast to `double`
- `char` → cast to `string`

Invalid type assignments result in a critical error and prevent data persistence.

---

## ORM Mapping Example

```csharp
[Table("Orders")]
public class Order
{
    [PrimaryKey]
    [Column("OrderId")]
    public int OrderId { get; set; }

    [JoinColumn("CustomerId")]
    public int CustomerId { get; set; }

    public string OrderDate { get; set; }

    [Column("TotalAmount")]
    public double TotalAmount { get; set; }

    [NotMapped]
    public Customer? Customer { get; set; }

    [OneToMany("OrderId")]
    public List<OrderItem> Items { get; set; } = new();
}
```

## Builder Configuration

Before working with the database, it is necessary to configure the runtime environment using the `DbBuilder`.  
This step defines the database location and registers all entity tables that will be managed by Cronus.

### Example: Builder Setup

```csharp
using Cronus.Builders;
using Demo.Model;

var builder = DbBuilder.CreateBuilder();

// Define database file
builder.AddConnectionString("Example_database");

// Register entity tables
builder
    .AddTable(typeof(Customer))
    .AddTable(typeof(Order))
    .AddTable(typeof(OrderItem))
    .AddTable(typeof(Product));

// Build runtime instance
var runtime = await builder.BuildRuntimeAsync();