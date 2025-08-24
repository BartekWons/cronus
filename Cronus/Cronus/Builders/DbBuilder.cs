using Cronus.Attributes;
using Cronus.Database.Model;
using Cronus.Exceptions;
using Cronus.Mappers;
using System.Reflection;

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
        var tableAttribute = table.GetCustomAttribute<TableAttribute>();
        if (tableAttribute is null)
            throw new AttributeNotFoundException();

        var tableName = tableAttribute.Name;

        var columns = new List<ColumnModel>();

        foreach (var prop in table.GetProperties())
        {
            var notMappedAttribute = prop.GetCustomAttribute<NotMappedAttribute>();
            if (notMappedAttribute is not null)
                continue;

            var columnAttribute = prop.GetCustomAttribute<ColumnAttribute>();
            var columnName = columnAttribute?.Name ?? prop.Name;

            var primaryKeyAttribute = prop.GetCustomAttribute<PrimaryKeyAttribute>();
            bool isPrimaryKey = primaryKeyAttribute is not null;
            // add check if PK is int

            var temp = prop.PropertyType;
            var mapper = new AllowedTypeMapper();
            var type = mapper.Map(temp).ToString();

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

    public Database.Database Build()
    {
        //TODO: add saving structure to file

        //_database.Create();
        return _database;
    }
}
