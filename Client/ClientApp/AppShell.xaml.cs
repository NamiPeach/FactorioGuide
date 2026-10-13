namespace ClientApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Pushed on top of the Items page, so "action" is a route, not a ShellContent
        Routing.RegisterRoute("action", typeof(Views.ActionPage));
    }
}
