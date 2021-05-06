namespace WpfSample.App
{
    public class GeneralOptions
    {
        public const string SectionName = "General";

        public string Language { get; set; }

        public string Skin { get; set; }
    }

    public class SystemIntegrationOptions
    {
        public const string SectionName = "SystemIntegration";

        public bool ShowTrayIcon { get; set; }

        public bool MinimizeToTrayOnStartup { get; set; }
    }

    public class NotificationOptions
    {
        public const string SectionName = "Notifications";
    }

    public class PerformanceOptions
    {
        public const string SectionName = "Performance";

        public bool EnableAnimations { get; set; }
    }
}
