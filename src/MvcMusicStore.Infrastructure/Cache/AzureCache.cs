using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationServer.Caching;
using MvcMusicStore.Infrastructure.Core;

namespace MvcMusicStore.Infrastructure.Cache
{
    public class AzureCache : ICache 
    {
        readonly DataCache _cache;

        public AzureCache()
        {
            _cache = new DataCache("default");
        }


        public bool Exists<T>(string key)
        {
            var result = (T)_cache.Get(key);

            return result is T;
        }

        public T Get<T>(string key)
        {
            var result= (T)_cache.Get(key);

            if (result is T)
            {
                return result;
            }

            return default(T); 
        }

        public void Put<T>(string key,object value)
        {
            _cache.Put(key,value);
        }

        public void Put<T>(string key, object value, string region)
        {
            _cache.Put(key, value, region);
        }

        public void Put<T>(string key, object value,TimeSpan timeOut)
        {
            _cache.Put(key, value, timeOut);
        }

        public void Put<T>(string key, object value, TimeSpan timeOut, string region)
        {
            _cache.Put(key, value, timeOut, region);
        }

        public T MakeCached<T>(string key, Func<string, T> resultFunc)
        {
            return MakeCached(key, resultFunc, TimeSpan.FromDays(365), null);
        }

        public T MakeCached<T>(string key, Func<string, T> resultFunc, string region)
        {
            return MakeCached(key, resultFunc, TimeSpan.FromDays(365), region);
        }

        public T MakeCached<T>(string key, Func<string, T> resultFunc, TimeSpan absoluteExpiry)
        {
             return MakeCached(key, resultFunc, TimeSpan.FromDays(365), null);
        }

        public T MakeCached<T>(string key, Func<string, T> resultFunc, TimeSpan absoluteExpiry, string region)
        {
            var value = Get<T>(key);

            if (!Equals(default(T), value)) 
                return value;
            
            value = resultFunc(key);

            if (Equals(default(T), value)) 
                return value;

            if (String.IsNullOrWhiteSpace(region))
            {
                Put<T>(key, value, absoluteExpiry);
            }
            else
            {
                Put<T>(key, value, absoluteExpiry, region);
            }

            return value;
        }

        public IEnumerable<string> GetAllKeysByRegion(string region)
        {
            return _cache.GetObjectsInRegion(region).Select(kpv => kpv.Key);
        }
    }
}
