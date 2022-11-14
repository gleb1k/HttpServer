﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.ORM
{
    public class MyORM
    {
        public IDbConnection _connection = null;
        public IDbCommand _cmd = null;
        public MyORM(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _cmd = _connection.CreateCommand();
        }
        //Колво строк
        public int ExecuteNonQuery(string query)
        {
            int noOfAffectedRows = 0;
            using (_connection)
            {
                _cmd!.CommandText = query;
                _connection!.Open();
                noOfAffectedRows = _cmd.ExecuteNonQuery();
            }
            return noOfAffectedRows;
        }
        //Добавить параметры
        public MyORM AddParameter<T>(string name, T value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = name;
            param.Value = value;
            _cmd!.Parameters.Add(param);
            return this;
        }
        //Много штук

        public IEnumerable<T> ExecuteQuery<T>(string query)
        {
            IList<T> list = new List<T>();
            Type type = typeof(T);

            using (_connection)
            {
                _cmd!.CommandText = query;
                _connection.Open();
                var reader = _cmd.ExecuteReader();
                while (reader.Read())
                {
                    T obj = (T)Activator.CreateInstance(type);
                    type.GetProperties().ToList().ForEach(p =>
                    {
                        p.SetValue(obj, reader[p.Name]);
                    });
                    list.Add(obj);
                }
            }
            return list;
        }
        public IEnumerable<T> Select<T>()
        {
            IList<T> list = new List<T>();
            Type t = typeof(T);

            using (_connection)
            {
                //НУЖНО ЧТОБ ТАБЛИЦА НАЗЫВАЛАСЬ Accounts (иначе не робит)
                string sqlExpression = $"SELECT * FROM {t.Name}s";

                _cmd.CommandText = sqlExpression;

                _connection.Open();
                var reader = _cmd.ExecuteReader();
                while (reader.Read())
                {
                    T obj = (T)Activator.CreateInstance(t);
                    t.GetProperties().ToList().ForEach(x =>
                    x.SetValue(obj, reader[x.Name]));

                    list.Add(obj);
                }
            }
            return list;
        }

        //Первый столбец первой попавшей строки
        public T ExectureScalar<T>(string query)
        {
            T result = default;
            using (_connection)
            {
                _cmd.CommandText = query;
                _connection.Open();
                result = (T)_cmd.ExecuteScalar();
            }
            return result;
        }
    }
}