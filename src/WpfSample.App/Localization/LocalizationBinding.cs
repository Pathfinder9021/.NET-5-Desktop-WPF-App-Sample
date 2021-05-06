using System.Windows.Data;

namespace WpfSample.App.Localization
{
    /// <summary>
    /// Returns a localized string resource.
    /// </summary>
    /// <remarks>
    /// E.g.
    /// <list type="number">
    ///    <item>xmlns:locazation="clr-namespace:WpfSample.App.Localization"</item>
    ///    <item>Text="{locazation:Localize TitleBarCaption}" </item>
    /// </list>
    /// </remarks>
    public class LocalizeExtension : Binding
    {
        public LocalizeExtension(string name) : base("[" + name + "]")
        {
            Mode = BindingMode.OneWay;
            Source = LocalizationManager.Instance;
        }
    }
}
