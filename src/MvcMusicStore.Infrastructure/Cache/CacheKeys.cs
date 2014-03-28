namespace MvcMusicStore.Infrastructure.Cache
{
    public static class CacheKeys
    {
        public static string UserOrderKey(string userId)
        {
            return string.Format("user:{0}:orders", userId);
        }
    }
}
