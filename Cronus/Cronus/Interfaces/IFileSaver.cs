namespace Cronus.Interfaces;

internal interface IFileSaver
{
    Task SaveAsync(string filename);
}
