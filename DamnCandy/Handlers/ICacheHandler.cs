using DamnCandy.Operations;

namespace DamnCandy.Handlers;

public interface ICacheHandler
{
    Task SaveBytesAsync(Guid guid, byte[] bytes);

    Task<T> LoadAsync<T>(Guid guid);
}