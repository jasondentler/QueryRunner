using System.Reflection;
using System.Windows;

namespace DB2Query
{
    public static class WindowExtensions
    {
        public static bool IsModal(this Window window)
        {
            return (bool)typeof(Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(window);
        }
    }
}