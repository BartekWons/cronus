using Cronus.Database.Model;
using Cronus.Interfaces;
using Newtonsoft.Json;

namespace Cronus.Utils;

internal class DatabaseModelFileHandler : IFileReader<DatabaseModel>, IFileSaver
{
    private DatabaseModel _model;

    internal DatabaseModelFileHandler(DatabaseModel model)
    {
        _model = model;
    }

    public DatabaseModel Read(string filename)
    {
        return Read(filename);
    }

    public async Task SaveAsync(string filename)
    {
        if (!Directory.Exists("DB"))
            Directory.CreateDirectory("DB");
        var json = JsonConvert.SerializeObject(_model.Tables, Formatting.Indented);
        var path = Path.Combine("DB", Path.ChangeExtension(filename, ".json"));
        await File.WriteAllTextAsync(path, json);
    }
}
