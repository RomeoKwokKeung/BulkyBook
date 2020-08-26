using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    //store procedure
    public interface ISP_Call : IDisposable
    {
        //return a signle value like Count or any first value (int value / bool value)
        //use dapper to pass parameters
        T Single<T>(string procedureName, DynamicParameters param = null);

        //just execute, dont need to retrieve anything like add / delete
        void Execute(string procedureName, DynamicParameters param = null);

        //retrieve one complete row / one complete record
        T OneRecord<T>(string procedureName, DynamicParameters param = null);

        //get a list of record
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);

        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);
    }
}
