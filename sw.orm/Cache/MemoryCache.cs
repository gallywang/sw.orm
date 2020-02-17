#if NET40
using System.Web;
using System.Web.Caching;
#else
using Microsoft.Extensions.Caching.Memory;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace sw.orm
{
    internal class SWMemoryCache : ICacheService
    {
        public void Add<V>(string key, V value)
        {
            MemoryCacheHelper<V>.GetInstance().Add(key, value);
        }

        public void Add<V>(string key, V value, bool isSliding, int cacheDurationInSeconds)
        {
            MemoryCacheHelper<V>.GetInstance().Add(key, value, cacheDurationInSeconds, isSliding);
        }

        public bool ContainsKey<V>(string key)
        {
            return MemoryCacheHelper<V>.GetInstance().ContainsKey(key);
        }

        public V Get<V>(string key)
        {
            return MemoryCacheHelper<V>.GetInstance().Get(key);
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, bool isSliding = false, int cacheDurationInSeconds = 86400)
        {
            var cacheManager = MemoryCacheHelper<V>.GetInstance();
            if (cacheManager.ContainsKey(cacheKey))
            {
                return cacheManager[cacheKey];
            }
            else
            {
                var result = create();
                cacheManager.Add(cacheKey, result, cacheDurationInSeconds, isSliding);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            MemoryCacheHelper<V>.GetInstance().Remove(key);
        }
    }

    internal class MemoryCacheHelper<V>
    {

        #region 全局变量
        private static MemoryCacheHelper<V> _instance = null;
        private static readonly object _instanceLock = new object();

#if NET40
#else
        static MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
#endif



        #endregion

        #region 构造函数

        private MemoryCacheHelper() { }

        #endregion

        #region  属性

        /// <summary>         
        ///根据key获取value     
        /// </summary>         
        /// <value></value>      
        public V this[string key]
        {
            get { return (V)Get(key); }
        }

        #endregion

        #region 公共函数

        /// <summary>         
        /// key是否存在       
        /// </summary>         
        /// <param name="key">key</param>         
        /// <returns> ///  存在<c>true</c> 不存在<c>false</c>.        /// /// </returns>         
        public bool ContainsKey(string key)
        {
#if NET40
             if (key != null)
            {
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                object value = objCache[key];
                if(value == null)
                {
                    return false;
                }
                return true;
            }
            return false;
#else
            object val = null;
            if (key != null && cache.TryGetValue(key, out val))
            {
                return true;
            }
            return false;
#endif


        }

        /// <summary>         
        /// 获取缓存值         
        /// </summary>         
        /// <param name="key">key</param>         
        /// <returns></returns>         
        public V Get(string key)
        {
#if NET40
            if (key != null)
            {
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                return (V)objCache[key];
            }
            else
            {
                return default(V);
            }
#else
            object val = null;
            if (key != null && cache.TryGetValue(key, out val))
            {
                return (V)val;
            }
            else
            {
                return default(V);
            }
#endif


        }

        /// <summary>         
        /// 获取实例 （单例模式）       
        /// </summary>         
        /// <returns></returns>         
        public static MemoryCacheHelper<V> GetInstance()
        {
            if (_instance == null)
                lock (_instanceLock)
                    if (_instance == null)
                        _instance = new MemoryCacheHelper<V>();
            return _instance;
        }

        /// <summary>         
        /// 插入缓存(默认最大长度,默认设置绝对过期时间)        
        /// </summary>         
        /// <param name="key"> key</param>         
        /// <param name="value">value</param>          
        public void Add(string key, V value)
        {
#if NET40
 System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, value);
#else
            Add(key, value, int.MaxValue, false);
#endif


        }

        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheDurationInSeconds">缓存时长(秒)</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        public void Add(string key, V value, int cacheDurationInSeconds, bool isSliding)
        {
#if NET40
 if (key != null)
            {
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                if (isSliding)
                {
                    objCache.Insert(key, value, null, DateTime.MaxValue, TimeSpan.FromSeconds(cacheDurationInSeconds));
                }
                else
                {
                    objCache.Insert(key, value, null, System.DateTime.Now.AddSeconds(cacheDurationInSeconds), TimeSpan.Zero);
                }
            }
#else
            if (key != null)
            {
                MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions();
                if (isSliding)
                {
                    memoryCacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(cacheDurationInSeconds));
                }
                else
                {
                    memoryCacheEntryOptions.SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(cacheDurationInSeconds));
                }

                cache.Set(key, value, memoryCacheEntryOptions);
            }
#endif


        }

        /// <summary>         
        /// 删除缓存         
        /// </summary>         
        /// <param name="key">key</param>         
        public void Remove(string key)
        {
#if NET40
 if (key != null)
            {
                System.Web.Caching.Cache _cache = HttpRuntime.Cache;
                _cache.Remove(key);
            }
#else
            if (key != null)
            {
                cache.Remove(key);
            }
#endif


        }
        #endregion
    }
}
