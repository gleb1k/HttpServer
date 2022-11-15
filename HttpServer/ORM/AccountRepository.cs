using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    public class AccountRepository<T> : IRepository<T> where T : EntityBase
    {
        private const string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True";
        public void Create(T entity)
        {
            var myORM = new MyORM(_connectionString);
            myORM.Insert(entity);
        }

        public void Delete(T entity)
        {
            var myORM = new MyORM(_connectionString);
            myORM.Delete(entity);
        }

        public T GetById(int id)
        {
            var myORM = new MyORM(_connectionString);
            var table = myORM.Select<T>();
            return table.Where(entity => entity.Id == id).FirstOrDefault();
        }

        public void Update(T entity)
        {
            var myORM = new MyORM(_connectionString);
        }
    }
}
