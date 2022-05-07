using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace DataAccess
{
    
    public class TwitterData
    {
        private readonly IMemoryCache _memoryCache;

        

        public TwitterData(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
      
    }
}