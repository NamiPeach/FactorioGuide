namespace ClientApp.Services;

/// <summary>Server address. The trailing slash matters for relative URLs.</summary>
public static class ApiConfig
{
    // The Android emulator reaches the PC's localhost as 10.0.2.2; the Windows app uses localhost.
    public static string BaseUrl =>
        DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5003/"
            : "http://localhost:5003/";
}
