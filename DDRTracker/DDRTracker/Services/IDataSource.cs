using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDRTracker.Services
{
    /// <summary>
    /// Interface that Data Stores are required to implement. This pattern is also known as a repository. 
    /// Useful for storing or managing persistant data.
    /// </summary>
    /// <typeparam name="T">The Model/Item of the Data Store. </typeparam>
    /// <typeparam name="S">The type that uniquely identifies an item in the store. UID.</typeparam>
    public interface IDataSource<T, S>
    {
        /// <summary>
        /// Adds an item into the data store.
        /// </summary>
        /// <param name="item">Model to be added</param>
        /// <returns>Boolean indicating success of operation.</returns>
        Task<bool> AddAsync(T item);

        /// <summary>
        /// Updates an existing item in the store.
        /// </summary>
        /// <param name="item">Model to be updated. Model must have updated values. You should have the id before updating.</param>
        /// <returns>Boolean indicating success of operation. Failure will also occur if the item is not in the store.</returns>
        Task<bool> UpdateAsync(T item);

        /// <summary>
        /// Removes an item in the store.
        /// </summary>
        /// <param name="id">The unique identifier of the model/item you want to delete.</param>
        /// <returns>Boolean indicating success of operation.</returns>
        Task<bool> RemoveAsync(S id);

        /// <summary>
        /// Gets an item from the inventory using the ID.
        /// </summary>
        /// <param name="id">ID of the model/item you want to get.</param>
        /// <returns>Boolean indicating success of operation.</returns>
        Task<T> GetAsync(S id);

        /// <summary>
        /// Gets all the items from the data store.
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <returns>All items in the data store in a collection. If there is nothing, it will return an empty collection.</returns>
        Task<IEnumerable<T>> GetAllAsync(bool forceRefresh = false);
    }
}
