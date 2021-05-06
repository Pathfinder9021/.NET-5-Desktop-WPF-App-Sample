using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace WpfSample.App.Configuration
{
    public class SkinEngine
    {
        private readonly ResourceDictionary resources;

        public string Current { get; private set; }

        public SkinEngine(Application app)
        {
            resources = app.Resources;
        }

        public void ApplySkin(string skinName)
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                ClearResources();

                switch (skinName)
                {
                    case Constants.Skins.Dark:
                        ApplyEmbededResources("Resources/Skins/Dark.xaml");
                        break;
                    case Constants.Skins.Light:
                        ApplyEmbededResources("Resources/Skins/Light.xaml");
                        break;
                    default:
                        var path = $"{Environment.CurrentDirectory}/Skins/{skinName}.xaml";
                        ApplyExternalResources(path);
                        break;
                }

                ApplyEmbededResources("Resources/Styles/Styles.xaml");

                Current = skinName;
            });
        }

        public void ApplyEmbededResources(string uri)
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri(uri, UriKind.Relative)
            };

            foreach (var mergeDict in dict.MergedDictionaries)
            {
                resources.MergedDictionaries.Add(mergeDict);
            }

            foreach (var key in dict.Keys)
            {
                resources[key] = dict[key];
            }
        }

        public void ApplyExternalResources(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                var dict = XamlReader.Load(streamReader.BaseStream) as ResourceDictionary;
                resources.MergedDictionaries.Add(dict);
            }
        }

        private void ClearResources()
        {
            var resourcesNotToRemove = new string[] 
            {
                "ViewModelLocator"
            };

            // Clear resources
            foreach (var key in resources.Keys)
            {
                if(!resourcesNotToRemove.Contains(key))
                {
                    resources.Remove(key);
                }
            }

            // Clear merged dictionaries
            var mergedDictionariesToRemove = resources.MergedDictionaries
                .Where(x => !resourcesNotToRemove.Contains(x.Source.ToString()))
                .ToList();

            mergedDictionariesToRemove.ForEach(x => resources.MergedDictionaries.Remove(x));
        }
    }
}
