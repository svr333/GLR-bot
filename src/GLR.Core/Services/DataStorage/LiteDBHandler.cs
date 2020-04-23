using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LiteDB;

namespace GLR.Core.Services.DataStorage
{
    public class LiteDBHandler
    {
        private string _dbFileName = "Storage.db";

        public void Store<T>(T item)
        {
            using (var db = new LiteDatabase(_dbFileName))
            {
                var collection = db.GetCollection<T>();
                collection.Insert(item);
            }
        }

        public void Update<T>(T item)
        {
            using (var db = new LiteDatabase(_dbFileName))
            {
                var collection = db.GetCollection<T>();
                collection.Update(item);
            }
        }

        public IEnumerable<T> RestoreMany<T>(Expression<Func<T, bool>> predicate)
        {
            using (var db = new LiteDatabase(_dbFileName))
            {
                var collection = db.GetCollection<T>();
                return collection.Find(predicate);
            }
        }

        public T RestoreSingle<T>(Expression<Func<T, bool>> predicate)
            => RestoreMany(predicate).FirstOrDefault();

        public bool Exists<T>(Expression<Func<T, bool>> predicate)
        {
            using (var db = new LiteDatabase(_dbFileName))
            {
                var collection = db.GetCollection<T>();
                return collection.Exists(predicate);
            }
        }
    }
}
