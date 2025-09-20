using Cronus.Database;
using Cronus.Database.Helpers;
using Cronus.Database.Model;
using Cronus.Exceptions;

namespace Cronus.Builders;

public class DbBuilder : IBuilder<Database.Database>
{
    private Database.Database _database = new();
    private DbBuilder() { }

    public static DbBuilder CreateBuilder()
    {
        return new DbBuilder();
    }

    public DbBuilder AddTable(Type table)
    {
        var tableProp = new TypeAttributeHelper(table);
        var tableName = tableProp.GetTableName();

        var columns = new List<ColumnModel>();

        foreach (var prop in table.GetProperties())
        {
            var helper = new PropertyAttributeHelper(prop);

            if (helper.IsNotMapped())
                continue;

            var columnName = helper.GetColumnName();
            bool isPrimaryKey = helper.IsPrimaryKey();

            if (isPrimaryKey && !helper.IsPrimaryKeyInteger())
            {
                throw new AttributeNotFoundException("Primary Key is not an integer");
            }

            var type = helper.GetPropertyType();

            columns.Add(new ColumnModel
            {
                Name = columnName,
                IsPrimaryKey = isPrimaryKey,
                Type = type,
            });
        }

        bool hasPrimaryKey = false;
        foreach (var column in columns)
        {
            if (column.IsPrimaryKey) { hasPrimaryKey = true; break; }
        }

        if (!hasPrimaryKey)
            throw new AttributeNotFoundException("Table must contain Primary Key attribute");

        _database.Model.Tables.Add(new TableModel
        {
            Name = tableName,
            Columns = columns,
        });

        return this;
    }

    public DbBuilder AddConnectionString(string connectionString)
    {
        _database.Config.ConnectionString = connectionString;
        return this;
    }

    public async Task<Database.Database> Build()
    {
        if (String.IsNullOrEmpty(_database.Config.ConnectionString))
            throw new InvalidConnectionStringException("Connection string is null or empty");
        var handler = new DatabaseModelFileHandler(_database.Model);
        await handler.SaveAsync(_database.Config.ConnectionString);
            return _database;
    }
}
