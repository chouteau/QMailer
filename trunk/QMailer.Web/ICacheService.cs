using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QMailer.Web
{
	public interface ICacheService 
	{
		// Summary:
		//     Gets or sets the cache item at the specified key.
		//
		// Parameters:
		//   key:
		//     A System.String object that represents the key for the cache item.
		//
		// Returns:
		//     The specified cache item.
		object this[string key] { get; }

		// Summary:
		//     Adds the specified item to the System.Web.Caching.Cache object with dependencies,
		//     expiration and priority policies, and a delegate you can use to notify your
		//     application when the inserted item is removed from the Cache.
		//
		// Parameters:
		//   key:
		//     The cache key used to reference the item.
		//
		//   value:
		//     The item to be added to the cache.
		//
		//   absoluteExpiration:
		//     The time at which the added object expires and is removed from the cache.
		//     If you are using sliding expiration, the absoluteExpiration parameter must
		//     be System.Web.Caching.Cache.NoAbsoluteExpiration.
		//
		//
		// Returns:
		//     An System.Object if the item was previously stored in the Cache; otherwise,
		//     null.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     The key or value parameter is set to null.
		//
		//   System.ArgumentOutOfRangeException:
		//     The slidingExpiration parameter is set to less than TimeSpan.Zero or more
		//     than one year.
		//
		//   System.ArgumentException:
		//     The absoluteExpiration and slidingExpiration parameters are both set for
		//     the item you are trying to add to the Cache.
		void Add(string key, object value, DateTime absoluteExpiration);
		//
		// Summary:
		//     Retrieves the specified item from the System.Web.Caching.Cache object.
		//
		// Parameters:
		//   key:
		//     The identifier for the cache item to retrieve.
		//
		// Returns:
		//     The retrieved cache item, or null if the key is not found.
		object Get(string key);
		//
		// Summary:
		//     Removes the specified item from the application's System.Web.Caching.Cache
		//     object.
		//
		// Parameters:
		//   key:
		//     A System.String identifier for the cache item to remove.
		//
		// Returns:
		//     The item removed from the Cache. If the value in the key parameter is not
		//     found, returns null.
		void Remove(string key);

		/// <summary>
		/// Clears all.
		/// </summary>
		void ClearAll();

		/// <summary>
		/// Firsts the or default.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		T FirstOrDefault<T>(Func<T, bool> predicate);

		/// <summary>
		/// Gets the list of.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		IEnumerable<T> GetListOf<T>(Func<T, bool> predicate);

		/// <summary>
		/// Gets the list of.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IQueryable<T> GetListOf<T>();

		/// <summary>
		/// Gets the item count.
		/// </summary>
		/// <value>The item count.</value>
		int ItemCount { get; }

	}
}
