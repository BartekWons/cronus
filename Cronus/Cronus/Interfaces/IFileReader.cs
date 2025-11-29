namespace Cronus.Interfaces;

internal interface IFileReader<T> where T : class
{
    Task<T?> ReadAsync(string filename);
}
