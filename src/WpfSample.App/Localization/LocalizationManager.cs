using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using WpfSample.App.Resources;

namespace WpfSample.App.Localization
{
    public class LocalizationManager : INotifyPropertyChanged
    {
        private CultureInfo currentCulture = null;
        private static readonly LocalizationManager instance = new LocalizationManager();

        #region Properties

        public static LocalizationManager Instance
        {
            get { return instance; }
        }

        public bool IsLocazationRequired { get; set; }

        public bool UseKeySeparator { get; set; } = false;

        public char KeySeparator { get; set; }

        public CultureInfo CurrentCulture
        {
            get => currentCulture;
            set
            {
                if (currentCulture != value)
                {
                    currentCulture = value;
                    var @event = this.PropertyChanged;
                    if (@event != null)
                    {
                        @event.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                    }
                }
            }
        }

        public ResourceManager ResourceManager { get; set; }

        public List<ResourceManager> SecondaryResourceManagers { get; private set; }

        #endregion Properties

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        private LocalizationManager()
        {  
            IsLocazationRequired = true;
            UseKeySeparator = false;
            KeySeparator = '_';

            ResourceManager = null;
            SecondaryResourceManagers = new List<ResourceManager>();
        }

        public string this[string key]
        {
            get => GetString(key);
        }

        public string GetString(string resourceKey)
        {
            if (resourceKey.IndexOf(KeySeparator) == -1)
            {
                return GetString(resourceKey, CurrentCulture);
            }
            else
            {
                var keys = resourceKey.Split(KeySeparator);
                var results = new List<string>();

                foreach (var k in keys)
                {
                    var stringResource = GetString(k, CurrentCulture);
                    results.Add(stringResource);
                }

                return string.Join(" ", results);
            }
        }

        private string GetString(string resourceKey, CultureInfo culture)
        {
            string resourceValue;

            if (ResourceManager != null) 
            {
                resourceValue = GetString(resourceKey, ResourceManager, culture);
                if (resourceValue != null)
                {
                    return resourceValue;
                }
            }

            foreach(var secondaryResourceManager in SecondaryResourceManagers) 
            {
                resourceValue = GetString(resourceKey, secondaryResourceManager, CurrentCulture);
                if (resourceValue != null)
                {
                    return resourceValue;
                }
            }

            return IsLocazationRequired ? $"[{resourceKey}]" : string.Empty;
        }

        private static string GetString(string resourceKey, ResourceManager resourceManager, CultureInfo culture)
        {
            if (resourceManager == null)
            {
                throw new InvalidOperationException("Resource manager is not specified");
            }

            var resourceValue = resourceManager.GetString(resourceKey, culture);
            if (resourceValue != null)
            {
                return resourceValue;
            }

            return resourceManager.GetString(resourceKey);
        }
    }
}
