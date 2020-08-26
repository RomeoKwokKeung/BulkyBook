using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    //generic method definitions for Get, GetAll, GetFirstOrDefault, Add, Remove, RemoveRange
    //<T> generic type
    public interface IRepository<T> where T : class
    {
        //base on the id we can retrieve a record from the database
        T Get(int id);
        //retrieve a list of record, based on number of requirements
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            //result is IOrderedQueryable
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            //includeProperties will be used for eager loading (foreigner key) 
            string includeProperties = null
            );

        //only return one object
        T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null);

        void Add(T entity);
        //base on an id to remove an object
        void Remove(int id);
        //base on a complete entity to remove an object
        void Remove(T entity);
        //remove a complete range of entities
        void RemoveRange(IEnumerable<T> entity);
    }
}
