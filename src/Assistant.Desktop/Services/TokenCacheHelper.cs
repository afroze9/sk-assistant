using System.IO;

using Microsoft.Identity.Client;

namespace Assistant.Desktop.Services;

public class TokenCacheHelper
{
    private static readonly string CacheFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "sk_msal_cache.bin");
    
    private static readonly object FileLock = new object();
    
    public static void EnableSerialization(ITokenCache tokenCache)
    {
        tokenCache.SetBeforeAccess(BeforeAccessNotification);
        tokenCache.SetAfterAccess(AfterAccessNotification);
    }

    public static bool CanTryAutoLogin() => File.Exists(CacheFilePath);
    
    private static void BeforeAccessNotification(TokenCacheNotificationArgs args)
    {
        lock (FileLock)
        {
            if (File.Exists(CacheFilePath))
            {
                byte[] protectedData = File.ReadAllBytes(CacheFilePath);
                args.TokenCache.DeserializeMsalV3(protectedData);
            }
        }
    }

    private static void AfterAccessNotification(TokenCacheNotificationArgs args)
    {
        // if the cache state has changed, persist the changes
        if (args.HasStateChanged)
        {
            lock (FileLock)
            {
                byte[]? protectedData = args.TokenCache.SerializeMsalV3();
                File.WriteAllBytes(CacheFilePath, protectedData);
            }
        }
    }
}