using System;
using Microsoft.Extensions.Configuration;

namespace ETicaretAPI.Persistence
{
    // Yalnızca `dotnet ef` (design-time) için connection string çözer.
    // Runtime'da connection string IConfiguration üzerinden DI'dan alınır.
    static class Configuration
    {
        public static string ConnectionString
        {
            get
            {
                var fromEnv = Environment.GetEnvironmentVariable("ETICARETAPI_CONNECTION_STRING");
                if (!string.IsNullOrWhiteSpace(fromEnv))
                    return fromEnv;

                var apiSettingsPath = LocateApiAppSettings();

                ConfigurationManager configurationManager = new();
                configurationManager.SetBasePath(Path.GetDirectoryName(apiSettingsPath)!);
                configurationManager.AddJsonFile(Path.GetFileName(apiSettingsPath));

                return configurationManager.GetConnectionString("PostgreSQL");
            }
        }

        private static string LocateApiAppSettings()
        {
            const string targetRelativePath = "Presentation/ETicaretAPI.API/appsettings.json";
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (current != null)
            {
                var candidate = Path.Combine(current.FullName, targetRelativePath);
                if (File.Exists(candidate))
                    return candidate;

                current = current.Parent;
            }

            throw new FileNotFoundException(
                $"appsettings.json bulunamadı. '{targetRelativePath}' yolunu doğrulayın " +
                "ya da ETICARETAPI_CONNECTION_STRING ortam değişkenini ayarlayın.");
        }
    }
}

