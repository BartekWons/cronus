namespace Cronus.Interfaces;

internal interface IMapper<T,U>
{
    T Map(U value);
}
