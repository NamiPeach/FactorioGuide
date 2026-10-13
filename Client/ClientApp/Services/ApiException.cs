namespace ClientApp.Services;

/// <summary>An error from the API that is safe to show to the user as is.</summary>
public class ApiException(string message) : Exception(message);
