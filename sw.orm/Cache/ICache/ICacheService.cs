using System;
using System.Collections.Generic;
using System.Text;

namespace sw.orm
{
    internal interface ICacheService
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheDurationInSeconds">缓存时长(秒)</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        void Add<V>(string key, V value, bool isSliding, int cacheDurationInSeconds);

        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainsKey<V>(string key);

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        V Get<V>(string key);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="key"></param>
        void Remove<V>(string key);

        /// <summary>
        /// 获取或新增缓存
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="create"></param>
        /// <param name="cacheDurationInSeconds">缓存时长(秒)</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间</param>
        /// <returns></returns>
        V GetOrCreate<V>(string cacheKey, Func<V> create,  bool isSliding = false, int cacheDurationInSeconds = int.MaxValue);
    }
}
