using MVPathway.Settings.Abstractions;
using Newtonsoft.Json;
using Plugin.Settings;

namespace MVPathway.Settings
{
    public class SettingsRepository : ISettingsRepository
    {
        protected T Get<T>(string key)
        {
            var valueAsJson = CrossSettings.Current.GetValueOrDefault(key, default(string));
            if (string.IsNullOrWhiteSpace(valueAsJson))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(valueAsJson);
        }

        protected void Set<T>(string key, T value)
        {
            var valueAsJson = JsonConvert.SerializeObject(value);
            CrossSettings.Current.AddOrUpdateValue(key, valueAsJson);
        }
    }
}
