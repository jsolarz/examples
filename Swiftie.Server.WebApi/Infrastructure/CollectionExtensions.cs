using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Swiftie.Server.WebApi.Infrastructure
{
    /// <summary>
    /// Linq extensions for collections
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Remove an item from a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static ICollection<T> RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            List<T> toRemove = collection.Where(item => predicate(item)).ToList();
            toRemove.ForEach(item => collection.Remove(item));
            return collection;
        }

        /// <summary>
        /// Clears a whole set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbSet"></param>
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}