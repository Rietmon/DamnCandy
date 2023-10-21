using System;
using System.Threading.Tasks;
using DamnCandy.Operations;

namespace DamnCandy.Handlers
{
    public interface ICacheHandler
    {
        /// <summary>
        /// Write bytes to file in folder with Guid path
        /// </summary>
        /// <param name="guid">Guid of cache</param>
        /// <param name="bytes">Bytes of value</param>
        /// <returns>Async task</returns>
        Task SaveBytesAsync(Guid guid, byte[] bytes);

        /// <summary>
        /// Load target value from file in folder with Guid path
        /// </summary>
        /// <param name="guid">Guid of cache</param>
        /// <typeparam name="T">Type of target value</typeparam>
        /// <returns>ASync task with value</returns>
        Task<T> LoadAsync<T>(Guid guid);
    }
}