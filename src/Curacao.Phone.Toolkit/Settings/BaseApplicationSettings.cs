using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Curacao.Phone.Toolkit.Settings
{
    [PublicAPI]
    public abstract class BaseApplicationSettings<TSettings> where TSettings : class
    {
        private const string SettingsKey = "settings";

        protected BaseApplicationSettings()
        {
            Settings = LoadSettings();
        }

        private TSettings LoadSettings()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(SettingsKey))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(SettingsKey, SerializeToStrng(GetDefaultSettings()));
            }
            var settingsJsonString = (string) IsolatedStorageSettings.ApplicationSettings[SettingsKey];
            var settings = DeserializeFromString(settingsJsonString);

            if (settings == null)
            {
               settings = GetDefaultSettings();
            }

            RestoreMissedValues(settings);
            return settings;
        }

        protected abstract void RestoreMissedValues([NotNull] TSettings settings);

        [NotNull]
        protected abstract TSettings GetDefaultSettings();

        [PublicAPI, NotNull]
        public TSettings Settings { get; private set; }

        [CanBeNull]
        private static TSettings DeserializeFromString(string favsJsonString)
        {
            var deserializedFavs = JsonConvert.DeserializeObject<TSettings>(favsJsonString);
            return deserializedFavs;
        }

        private static string SerializeToStrng(TSettings favs)
        {
            return JsonConvert.SerializeObject(favs);
        }

        [PublicAPI]
        public void SaveSettings()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(SettingsKey))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(SettingsKey, SerializeToStrng(Settings));
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings[SettingsKey] = SerializeToStrng(Settings);
            }
        }
    }
}