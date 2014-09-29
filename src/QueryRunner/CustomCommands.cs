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

        public static readonly RoutedUICommand Execute = new RoutedUICommand(
            "Execute Query",
            "ExecuteQuery",
            typeof (CustomCommands));

        public static readonly RoutedUICommand IncreaseFontSize = new RoutedUICommand(
            "Increase Font Size",
            "IncreaseFontSize",
            typeof(CustomCommands));

        public static readonly RoutedUICommand DecreaseFontSize = new RoutedUICommand(
            "Decrease Font Size",
            "DecreaseFontSize",
            typeof(CustomCommands));
    }
}