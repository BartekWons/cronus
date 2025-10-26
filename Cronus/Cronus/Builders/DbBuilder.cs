using Cronus.Database.Model;
using Cronus.Exceptions;
using Cronus.Utils;

namespace Cronus.Builders;

public class DbBuilder : IBuilder<Database.Database>
{
    private Database.Database _database = new();
    private DbBuilder() { }

    public static DbBuilder CreateBuilder()
    {
        return new DbBuilder();
    }

    public DbBuilder AddConnectionString(string connectionString)
    {
        _database.Config.AddConnectionString(connectionString);
        return this;
    }

    public DbBuilder AddTable(Type table)
    {
        var tableProp = new TypeAttributeHelper(table);
        var tableName = tableProp.GetTableName();

        var tableModel = new TableModel
        {
            Name = tableName,
            Columns = [],
            ForeignKeys = [],
        };

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

            var propType = helper.GetPropertyType();

            tableModel.Columns.Add(new ColumnModel
            {
                Name = columnName,
                IsPrimaryKey = isPrimaryKey,
                Type = propType,
            });

            var joinAttr = helper.GetJoinColumn();

            if (joinAttr is not null)
            {
                string fkColumn = joinAttr.Name ?? prop.Name;

                var baseName = fkColumn.EndsWith("Id") ? fkColumn.Substring(0, fkColumn.Length - 2) : fkColumn;

                var referencedTable = baseName + "s";

                var referencedColumn = fkColumn;

                tableModel.ForeignKeys.Add(new ForeignKeyModel
                {
                    Column = fkColumn,
                    ReferencedTable = referencedTable,
                    ReferencedColumn = referencedColumn,
                    CascadeDelete = true,
                });
            }
        }

        if (!tableModel.Columns.Any(col => col.IsPrimaryKey == true))
        {
            throw new AttributeNotFoundException("Table must contain Primary Key attribute");
        }

        _database.Model.TablesSchema.Add(tableModel);

        // create empty list of data for this table
        if (!_database.Model.Data.ContainsKey(tableName))
        {
            _database.Model.Data[tableName] = [];
        }

        return this;
    }

    public async Task<Database.Database> Build()
    {
        if (string.IsNullOrEmpty(_database.Config.ConnectionString))
            throw new InvalidConnectionStringException("Connection string is null or empty");

        var handler = new DatabaseModelFileHandler(_database.Model);
        await handler.SaveAsync(_database.Config.ConnectionString);

        return _database;
    }
}
