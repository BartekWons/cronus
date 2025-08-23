using Cronus.Database.Model;

namespace Cronus.Builders;

public class DbBuilder : IBuilder<DatabaseModel>
{
    private DatabaseModel _databaseModel = new();
    private DbBuilder() { }

    public static DbBuilder CreateBuilder()
    {
        return new DbBuilder();
    }

    public DbBuilder AddTable(Type table)
    {

        return this;
    }

    public DatabaseModel Build()
    {
        //TODO: add saving structure to file
        return _databaseModel;
    }
}
