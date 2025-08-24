namespace Cronus.Builders;

public interface IBuilder<T> where T : class
{
    Task<T> Build();
}
