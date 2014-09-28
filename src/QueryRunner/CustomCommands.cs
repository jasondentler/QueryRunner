using System.Windows.Input;

namespace QueryRunner
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand OpenConnection = new RoutedUICommand
            (
            "Open Connection",
            "OpenConnection",
            typeof(CustomCommands)
            );

        public static readonly RoutedUICommand CloseConnection = new RoutedUICommand(
            "Close Connection",
            "CloseConnection",
            typeof (CustomCommands));
    }
}