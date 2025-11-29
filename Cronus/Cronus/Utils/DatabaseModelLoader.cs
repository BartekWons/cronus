using Cronus.DataAccess.Model;
using Cronus.Interfaces;
using Newtonsoft.Json;

namespace Cronus.Utils;

internal class DatabaseModelLoader : IFileReader<DatabaseModel>, IFileSaver
{
    private DatabaseModel _model;

    internal DatabaseModelLoader(DatabaseModel model)
    {
        _model = model;
    }

    public async Task<DatabaseModel?> ReadAsync(string filename)
    {
        var path = GetPath(filename);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Path not found: {path}");
        }

        var content = await File.ReadAllTextAsync(path);

        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        try
        {
            var model = JsonConvert.DeserializeObject<DatabaseModel>(content);
            return model;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task SaveAsync(string filename)
    {
        if (!Directory.Exists("DB"))
        {
            Directory.CreateDirectory("DB");
        }

        var json = JsonConvert.SerializeObject(_model, Formatting.Indented);
        var path = GetPath(filename);

        await File.WriteAllTextAsync(path, json);
    }

    private string? GetPath(string filename)
    {
        return Path.Combine("DB", Path.ChangeExtension(filename, ".json"));
    }
}
