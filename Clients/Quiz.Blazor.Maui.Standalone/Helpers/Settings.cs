namespace Quiz.Blazor.Maui.Standalone.Helpers {

    public static class Settings {
        public static Theme ThemeOption {
            get => (Theme)Preferences.Get(nameof(ThemeOption), HasDefaultThemeOption ? (int)Theme.Default : (int)Theme.Light);
            set => Preferences.Set(nameof(ThemeOption), (int)value);
        }

        public static Theme GetThemeOption(AppTheme requestedTheme) {
            switch (requestedTheme) {
                case AppTheme.Light:
                    return Theme.Light;
                case AppTheme.Dark:
                    return Theme.Dark;
                case AppTheme.Unspecified:
                default:
                    return ThemeOption;
            }
        }

        private static bool HasDefaultThemeOption {
            get {
                var minDefaultVersion = new Version(13, 0);
                if (DeviceInfo.Platform == DevicePlatform.WinUI)
                    minDefaultVersion = new Version(10, 0, 17763, 1);
                else if (DeviceInfo.Platform == DevicePlatform.Android)
                    minDefaultVersion = new Version(9, 0);

                return DeviceInfo.Version >= minDefaultVersion;
            }
        }

    }
}
