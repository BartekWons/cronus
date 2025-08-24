namespace Cronus.Interfaces;

internal interface IFileReader<T> where T : class
{
    T Read(string filename);
}
