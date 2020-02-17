using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace sw.orm
{
    internal class MongodbClient : DBClient
    {
        /// <summary>
        /// 数据库处理类
        /// </summary>
        private MongodbHelper mongodbHelper;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _coon;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="strConn"></param>
        public MongodbClient(string strConn)
        {
            _coon = strConn;
            mongodbHelper = new MongodbHelper(strConn);
        }

        #region 插入

        /// <summary>
        /// 插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public override int Insert<T>(T tParameter)
        {
            try
            {
                mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).InsertOne(tParameter);
                return 1;
            }
            catch(Exception)
            {
                //TODO        
            }
            return 0;
        }

        /// <summary>
        /// 采用事务插入，效率最高
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public override int Insert<T>(List<T> tParameter)
        {
            try
            {
                if(tParameter == null || tParameter.Count == 0)
                {
                    return 0;
                }
                mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).InsertMany(tParameter);
                return tParameter.Count;
            }
            catch (Exception)
            {
                //TODO
            }
            return 0;
        }

        #endregion

        #region 更新

        /// <summary>
        /// 更新(以主键为条件更新)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public override int Update<T>(T tParameter)
        {
            //修改条件;原有的修改方法为读取mongodb数据的_id，此处修改为根据对象id获取
            //注意：mongo db中每一条数据会自带一个_id属性
            FilterDefinition<T> filter = MongodbCreate.GetDefaultFilter<T>(tParameter);

            //要修改的字段
            var list = MongodbCreate.GetUpdateFields<T>(tParameter);
            var updatefilter = Builders<T>.Update.Combine(list);
            UpdateResult result = mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).UpdateOne(filter, updatefilter);
            return 1;

        }

        /// <summary>
        /// 更新(以主键为条件更新)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        /// <returns></returns>
        public override int Update<T>(List<T> tParameter)
        {
            if (tParameter != null && tParameter.Count > 0)
            {
                string sql = string.Empty;
                for (int i = 0; i < tParameter.Count; i++)
                {
                    FilterDefinition<T> filter = MongodbCreate.GetDefaultFilter<T>(tParameter[i]);

                    //要修改的字段
                    var list = MongodbCreate.GetUpdateFields<T>(tParameter[i]);
                    var updatefilter = Builders<T>.Update.Combine(list);
                    UpdateResult result = mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).UpdateOne(filter, updatefilter);
                }
            }
            return SWErrorCode.ParamsEmpty;
        }

        /// <summary>
        /// 根据条件更新指定列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldEx">更新列</param>
        /// <param name="fiterEx">更新条件</param>
        /// <returns></returns>
        public override int Update<T>(Expression<Func<T, T>> fieldEx, Expression<Func<T, bool>> fiterEx)
        {
            if (fieldEx != null)
            {
                var list = MongodbCreate.GetUpdateFields<T>(fieldEx);
                var updatefilter = Builders<T>.Update.Combine(list);
                UpdateResult result = UpdateAsync(fiterEx, updatefilter).Result;
                return 1;
            }
            return SWErrorCode.ParamsEmpty;
        }

        /// <summary>
        /// 异步更新，更新指定字段(仅更新单条记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        private async Task<UpdateResult> UpdateAsync<T>(Expression<Func<T, bool>> expression,
            UpdateDefinition<T> update)
        {
            return await mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).UpdateOneAsync(expression, update).ConfigureAwait(false);
        }

        /// <summary>
        /// 异步更新，更新指定字段(更新多条记录)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        private async Task<UpdateResult> UpdateManyAsync<T>(Expression<Func<T, bool>> expression,
            UpdateDefinition<T> update)
        {
            return await mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).UpdateManyAsync(expression, update).ConfigureAwait(false);
        }

        #endregion

        #region 删除

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override int Delete<T>(string id)
        {
            //原方法根据Mongodb中自动生成的数据_id进行删除，这里修改为根据数据记录的ID(每一条数据必定存在ID字段)
            FilterDefinition<T> filter = Builders<T>.Filter.Eq("ID", id);
            DeleteResult result = mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).DeleteOne(filter);
            return 1;
        }

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public override int Delete<T>(List<string> listId)
        {
            if(listId == null || listId.Count == 0)
            {
                return SWErrorCode.ParamsEmpty;
            }

            FilterDefinition<T> filter = null;

            for (int i = 0;i < listId.Count; i++)
            {
                if(filter == null)
                {
                    filter = Builders<T>.Filter.Eq(Const.DEFAULT_FIELD, listId[i]);
                }
                else
                {
                    filter = Builders<T>.Filter.Eq(Const.DEFAULT_FIELD, listId[i]) | filter;
                }
                
            }

            DeleteResult result = mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).DeleteOne(filter);
            return 1;
        }

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override int Delete<T>(int id)
        {
            FilterDefinition<T> filter = Builders<T>.Filter.Eq(Const.DEFAULT_FIELD, id);
            DeleteResult result = mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).DeleteOne(filter);
            return 1;
        }

        /// <summary>
        /// 以ID删除数据(数据列中必须包含ID列)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listId"></param>
        /// <returns></returns>
        public override int Delete<T>(List<int> listId)
        {
            if (listId == null || listId.Count == 0)
            {
                return SWErrorCode.ParamsEmpty;
            }

            FilterDefinition<T> filter = null;
            for (int i = 0; i < listId.Count; i++)
            {
                if (filter == null)
                {
                    filter = Builders<T>.Filter.Eq(Const.DEFAULT_FIELD, listId[i]);
                }
                else
                {
                    filter = Builders<T>.Filter.Eq(Const.DEFAULT_FIELD, listId[i]) | filter;
                }

            }

            DeleteResult result = mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).DeleteOne(filter);
            return 1;
        }

        /// <summary>
        /// 删除记录:不带参数时，以主键作为删除条件，未设置主键时不可删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        public override int Delete<T>(T tParameter)
        {
            FilterDefinition<T> filter = MongodbCreate.GetDefaultFilter<T>(tParameter);
            DeleteResult result = mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).DeleteOne(filter);
            return 1;
        }

        /// <summary>
        /// 删除记录:不带参数时，以主键作为删除条件，未设置主键时不可删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tParameter"></param>
        public override int Delete<T>(List<T> tParameter)
        {
            if (tParameter != null && tParameter.Count > 0)
            {
                string sql = string.Empty;
                for (int i = 0; i < tParameter.Count; i++)
                {
                    FilterDefinition<T> filter = MongodbCreate.GetDefaultFilter<T>(tParameter[i]);
                    DeleteResult result = mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).DeleteOne(filter);
                }
            }
            return SWErrorCode.ParamsEmpty;
        }

        /// <summary>
        /// 根据条件删除，条件为空时删除所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fiterEx"></param>
        /// <returns></returns>
        public override int Delete<T>(Expression<Func<T, bool>> fiterEx)
        {
            if (fiterEx != null)
            {
                //TODO 待封装，将lambda表达式转换为FilterDefinition<T>
            }
            return SWErrorCode.ParamsEmpty;
        }

        #endregion

        #region 查询

        /// <summary>
        /// 获取单条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override T GetModel<T>(Expression<Func<T, bool>> expression)
        {
            return GetOneAsync(expression).Result;
        }

        /// <summary>
        /// 获取单条记录(数据表中必须包含ID字段)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override T GetModel<T>(string id)
        {
            if(!string.IsNullOrEmpty(id))
            {
                return GetOneAsync<T>(id).Result;
            }
            return default(T); 
        }

        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override List<T> GetAll<T>(Expression<Func<T, object>> orderBy, AscOrDesc ascOrDesc = AscOrDesc.Asc)
        {
            string orderName = SqlOrder.GetOrderByName<T>(orderBy);

            //排序约束Ascending正序;Descending倒序
            SortDefinition<T> sort = null;
            if (!string.IsNullOrEmpty(orderName))
            {
                //排序生成器
                SortDefinitionBuilder<T> builderSort = Builders<T>.Sort;
                if (ascOrDesc == AscOrDesc.Asc)
                {
                    sort = builderSort.Ascending(orderName);
                }
                else
                {
                    sort = builderSort.Descending(orderName);
                }
            }
            return PageListAsync(null, null, null, sort).Result;
        }
        public override List<T> GetAll<T>(string orderFieldName, AscOrDesc ascOrDesc = AscOrDesc.Asc)
        {
            //排序约束Ascending正序;Descending倒序
            SortDefinition<T> sort = null;
            if (!string.IsNullOrEmpty(orderFieldName))
            {
                //排序生成器
                SortDefinitionBuilder<T> builderSort = Builders<T>.Sort;
                if (ascOrDesc == AscOrDesc.Asc)
                {
                    sort = builderSort.Ascending(orderFieldName);
                }
                else
                {
                    sort = builderSort.Descending(orderFieldName);
                }
            }
            return PageListAsync(null, null, null, sort).Result;
        }

        public override List<T> GetAll<T>()
        {
            return GetListAsync<T>().Result;
        }

        /// <summary>
        /// 查询数据记录(条件为空获取所有)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override int GetCount<T>(Expression<Func<T, bool>> expression)
        {
            return Convert.ToInt32(CountAsync(expression));
        }

        public override int GetCount<T>()
        {
            return Convert.ToInt32(CountAsync<T>(null));
        }

        /// <summary>
        /// 查询数据记录(条件为空获取所有)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override int GetCount<T>(Expression<Func<T, T>> expression)
        {
            throw new Exception(message: "sw.mongodb not support expression<func<T, T>> filter;");
        }

        public override List<T> GetModelList<T>()
        {
            return GetListAsync<T>().Result;
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression)
        {
            return GetListAsync(expression).Result;
        }

        /// <summary>
        /// 根据条件获取实体对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            string orderName = SqlOrder.GetOrderByName<T>(orderBy);

            //排序约束Ascending正序;Descending倒序
            SortDefinition<T> sort = null;
            if (!string.IsNullOrEmpty(orderName))
            {
                //排序生成器
                SortDefinitionBuilder<T> builderSort = Builders<T>.Sort;
                if (ascOrDesc == AscOrDesc.Asc)
                {
                    sort = builderSort.Ascending(orderName);
                }
                else
                {
                    sort = builderSort.Descending(orderName);
                }
            }
            return PageListAsync(pageIndex, pageSize, expression, sort).Result;
        }

        /// <summary>
        /// 根据条件获取实体对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, T>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            throw new Exception(message: "sw.mongodb not support expression<func<T, T>> filter;");
        }

        /// <summary>
        /// 获取前n条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            string orderName = SqlOrder.GetOrderByName<T>(orderBy);

            //排序约束Ascending正序;Descending倒序
            SortDefinition<T> sort = null;
            if (!string.IsNullOrEmpty(orderName))
            {
                //排序生成器
                SortDefinitionBuilder<T> builderSort = Builders<T>.Sort;
                if (ascOrDesc == AscOrDesc.Asc)
                {
                    sort = builderSort.Ascending(orderName);
                }
                else
                {
                    sort = builderSort.Descending(orderName);
                }
            }
            return PageListAsync(1, top, expression, sort).Result;
        }

        /// <summary>
        /// 获取前n条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            throw new Exception(message: "sw.mongodb not support expression<func<T, T>> filter;");
        }

        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            count = Convert.ToInt32(CountAsync(expression));

            string orderName = SqlOrder.GetOrderByName<T>(orderBy);

            //排序约束Ascending正序;Descending倒序
            SortDefinition<T> sort = null;
            if (!string.IsNullOrEmpty(orderName))
            {
                //排序生成器
                SortDefinitionBuilder<T> builderSort = Builders<T>.Sort;
                if (ascOrDesc == AscOrDesc.Asc)
                {
                    sort = builderSort.Ascending(orderName);
                }
                else
                {
                    sort = builderSort.Descending(orderName);
                }
            }
            return PageListAsync(pageIndex, pageSize, expression, sort).Result;
        }

        public override List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            throw new Exception(message: "sw.mongodb not support expression<func<T, T>> filter;");
        }

        /// <summary>
        /// 根据条件获取实体对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            //排序约束Ascending正序Descending倒序
            SortDefinition<T> sort = null;
            if (!string.IsNullOrEmpty(orderFieldName))
            {
                //排序生成器
                SortDefinitionBuilder<T> builderSort = Builders<T>.Sort;
                if (ascOrDesc == AscOrDesc.Asc)
                {
                    sort = builderSort.Ascending(orderFieldName);
                }
                else
                {
                    sort = builderSort.Descending(orderFieldName);
                }
            }
            return PageListAsync(pageIndex, pageSize, expression, sort).Result;
        }

        /// <summary>
        /// 根据条件获取实体对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public override List<T> GetModelList<T>(Expression<Func<T, T>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            throw new Exception(message: "sw.mongodb not support expression<func<T, T>> filter;");
        }

        /// <summary>
        /// 获取前n条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, bool>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            //TODO 暂时通过分页进行取第一页前n条，待进一步优化

            //排序约束Ascending 正序Descending 倒序
            SortDefinition<T> sort = null;
            if (!string.IsNullOrEmpty(orderFieldName))
            {
                //排序生成器
                SortDefinitionBuilder<T> builderSort = Builders<T>.Sort;
                if (ascOrDesc == AscOrDesc.Asc)
                {
                    sort = builderSort.Ascending(orderFieldName);
                }
                else
                {
                    sort = builderSort.Descending(orderFieldName);
                }
            }
            return PageListAsync(1, top, expression, sort).Result;
        }

        /// <summary>
        /// 获取前n条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="orderBy"></param>
        /// <param name="ascOrDesc"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        public override List<T> GetTopModelList<T>(Expression<Func<T, T>> expression, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            throw new Exception(message: "sw.mongodb not support expression<func<T, T>> filter;");
        }

        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            count = Convert.ToInt32(CountAsync(expression));
            //排序约束Ascending正序Descending倒序
            SortDefinition<T> sort = null;
            if (!string.IsNullOrEmpty(orderFieldName))
            {
                //排序生成器
                SortDefinitionBuilder<T> builderSort = Builders<T>.Sort;
                if (ascOrDesc == AscOrDesc.Asc)
                {
                    sort = builderSort.Ascending(orderFieldName);
                }
                else
                {
                    sort = builderSort.Descending(orderFieldName);
                }
            }
            return PageListAsync<T>(pageIndex, pageSize, expression, sort).Result;
        }

        public override List<T> GetModelListWithCount<T>(Expression<Func<T, T>> expression, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            throw new Exception(message: "sw.mongodb not support expression<func<T, T>> filter;");
        }

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override bool Exists<T>(Expression<Func<T, bool>> expression)
        {
            T result = GetOneAsync<T>(expression).Result;
            if(result != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override bool Exists<T>(Expression<Func<T, T>> expression)
        {
            throw new Exception(message: "sw.mongodb not support expression<func<T, T>> filter;");
        }

        /// <summary>
        /// 异步获取单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> expression)
        {
            FilterDefinitionBuilder<T> builderFilter = Builders<T>.Filter;
            FilterDefinition<T> filter = builderFilter.Empty;
            if (expression != null)
            {
                filter = expression;
            }
            return await mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 异步获取单个对象
        /// </summary>
        /// <param name="id">objectId</param>
        /// <param name="collectionName">表名</param>
        /// <returns></returns>
        public async Task<T> GetOneAsync<T>(string id)
        {
            return await mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).Find(new BsonDocument("ID", id)).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        private long CountAsync<T>(Expression<Func<T, bool>> expression)
        {
            FilterDefinitionBuilder<T> builderFilter = Builders<T>.Filter;
            FilterDefinition<T> filter = builderFilter.Empty;
            if (expression != null)
            {
                filter = expression;
            }
            //总条目数
            return mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).Find<T>(filter).CountDocuments();
        }

        /// <summary>
        /// 异步查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>> expression = null)
        {
            FilterDefinitionBuilder<T> builderFilter = Builders<T>.Filter;
            FilterDefinition<T> filter = builderFilter.Empty;
            if (expression != null)
            {
                filter = expression;
            }
            return await mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).Find(expression).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 异步获取分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="expression"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        private async Task<List<T>> PageListAsync<T>(int? pageIndex, int? pageSize, Expression<Func<T, bool>> expression,
            SortDefinition<T> sort)
        {
            FilterDefinitionBuilder<T> builderFilter = Builders<T>.Filter;
            FilterDefinition<T> filter = builderFilter.Empty;
            if (expression != null)
            {
                filter = expression;
            }
            var temp = mongodbHelper.DB.GetCollection<T>(GetTableName<T>()).Find(filter);

            if(sort != null)
            {
                temp.Sort(sort);
            }

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                temp.Skip((pageIndex - 1) * pageSize)
                        .Limit(pageSize);
            }

            return
                await
                    temp.ToListAsync()
                        .ConfigureAwait(false);
        }

        public override List<T> GetModelList<T>(Expression<Func<T, bool>> expression, SWPaging paging)
        {
            string orderFieldName = null;
            AscOrDesc ascOrDesc = AscOrDesc.Desc;
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                orderFieldName = paging.Sort;
                if (!string.IsNullOrEmpty(orderFieldName))
                {
                    ascOrDesc = Const.ASC.ToLower().Equals(paging.Order.ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                }
                else
                {
                    //TODO Log
                    orderFieldName = Const.DEFAULT_FIELD;
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelList<T>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }
        public override List<T> GetModelListWithCount<T>(Expression<Func<T, bool>> expression, out int count, SWPaging paging)
        {
            string orderFieldName = null;
            AscOrDesc ascOrDesc = AscOrDesc.Desc;
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                orderFieldName = paging.Sort;
                if (!string.IsNullOrEmpty(orderFieldName))
                {
                    ascOrDesc = Const.ASC.ToLower().Equals(paging.Order.ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                }
                else
                {
                    //TODO Log
                    orderFieldName = Const.DEFAULT_FIELD;
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelListWithCount<T>(expression, out count, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }
        public override int GetCount<T>(Where<T> where)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetCount<T>(expression);
        }
        public override List<T> GetModelList<T>(Where<T> where)
        {
            if (where == null || where.GetExpression() == null)
            {
                return GetModelList<T>();
            }
            return GetModelList<T>(where.GetExpression());
        }
        public override List<T> GetModelList<T>(Where<T> where, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelList<T>(expression, orderBy, ascOrDesc, pageSize, pageIndex);
        }
        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelListWithCount<T>(expression, out count, orderBy, ascOrDesc, pageSize, pageIndex);
        }
        public override List<T> GetTopModelList<T>(Where<T> where, Expression<Func<T, object>> orderBy = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetTopModelList<T>(expression, orderBy, ascOrDesc, top);
        }
        public override List<T> GetModelList<T>(Where<T> where, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelList<T>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }
        public override List<T> GetModelList<T>(Where<T> where, SWPaging paging)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }

            string orderFieldName = null;
            AscOrDesc ascOrDesc = AscOrDesc.Desc;
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                orderFieldName = paging.Sort;
                if (!string.IsNullOrEmpty(orderFieldName))
                {
                    ascOrDesc = Const.ASC.ToLower().Equals(paging.Order.ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                }
                else
                {
                    //TODO Log
                    orderFieldName = Const.DEFAULT_FIELD;
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelList<T>(expression, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }
        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? pageSize = null, int? pageIndex = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetModelListWithCount<T>(expression, out count, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }
        public override List<T> GetModelListWithCount<T>(Where<T> where, out int count, SWPaging paging)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }

            string orderFieldName = null;
            AscOrDesc ascOrDesc = AscOrDesc.Desc;
            int? pageSize = null;
            int? pageIndex = null;
            if (paging != null)
            {
                orderFieldName = paging.Sort;
                if (!string.IsNullOrEmpty(orderFieldName))
                {
                    ascOrDesc = Const.ASC.ToLower().Equals(paging.Order.ToLower()) ? AscOrDesc.Asc : AscOrDesc.Desc;
                }
                else
                {
                    //TODO Log
                    orderFieldName = Const.DEFAULT_FIELD;
                }
                pageSize = paging.PageSize;
                pageIndex = paging.PageIndex;
            }
            return GetModelListWithCount<T>(expression, out count, orderFieldName, ascOrDesc, pageSize, pageIndex);
        }
        public override List<T> GetTopModelList<T>(Where<T> where, string orderFieldName = null, AscOrDesc ascOrDesc = AscOrDesc.Asc, int? top = null)
        {
            Expression<Func<T, bool>> expression = null;
            if (where != null && where.GetExpression() != null)
            {
                expression = where.GetExpression();
            }
            return GetTopModelList<T>(expression, orderFieldName, ascOrDesc, top);
        }

        #endregion

        public override List<T> GetProcDataList<T>(string sql, List<SWDbParameter> sqlParameter)
        {
            //TODO
            throw new Exception(message: "sw.mongodb not support stored procedure;");
        }

        public override DataSet ExecSqlWithDataSet(string sql, List<SWDbParameter> sqlParameter)
        {
            //TODO
            throw new Exception(message: "sw.mongodb not support stored procedure;");
        }

        private string GetTableName<T>()
        {
            //注：typeof 运算符比对象实例上的 GetType 方法要快
            Type model = typeof(T);
            EntityInfo entityInfo = new SWMemoryCache().GetOrCreate(model.FullName, EntityGenerator.GetEntityInfo<T>, true);

            //数据表名(类名)
            return entityInfo.DbTableName;
        }
    }
}
