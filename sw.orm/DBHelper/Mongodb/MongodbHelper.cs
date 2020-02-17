using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace sw.orm
{
    internal class MongodbHelper
    {
        private IMongoDatabase mongodb;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionStrings"></param>
        public MongodbHelper(string connectionStrings)
        {
            var mongoUrl = new MongoUrlBuilder(connectionStrings);
            string databaseName = mongoUrl.DatabaseName;
            MongoClient client = new MongoClient(mongoUrl.ToMongoUrl());
            mongodb = client.GetDatabase(databaseName);
        }

        public IMongoDatabase DB
        {
            get { return mongodb; }
        }
    }
}
