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
        var path = GetPath(filename);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Path not found: {path}");
        }

        var content = File.ReadAllText(path);
        var model = JsonConvert.DeserializeObject<DatabaseModel>(content);
        
        if (model is null)
        {
            throw new InvalidDataException("Deserialization failed");
        }

        return model;
    }

    public async Task SaveAsync(string filename)
    {
        if (!Directory.Exists("DB"))
        {
            Directory.CreateDirectory("DB");
        }

        var json = JsonConvert.SerializeObject(_model.TablesSchema, Formatting.Indented);
        var path = GetPath(filename);

        await File.WriteAllTextAsync(path, json);
    }

    private string? GetPath(string filename)
    {
        return Path.Combine("DB", Path.ChangeExtension(filename, ".json"));
    }
}
