namespace Cronus.Interfaces
{
    internal interface IQueryParser
    {
        IQuery ParseQuery(string query);
    }
}
