using FirstWave.Unity.Gui.Panels;
using System.Collections.Generic;
using System.Reflection;

namespace FirstWave.Unity.Gui.Utilities
{
    internal static class VisualTreeHelper
    {
        private static MethodInfo ON_KEY_DOWN;
        private static MethodInfo ON_KEY_PRESSED;
        private static MethodInfo ON_KEY_RELEASED;

        static VisualTreeHelper()
        {
            var controlType = typeof(Control);

            ON_KEY_DOWN = controlType.GetMethod("OnKeyDown", BindingFlags.Instance | BindingFlags.NonPublic);
            ON_KEY_PRESSED = controlType.GetMethod("OnKeyPressed", BindingFlags.Instance | BindingFlags.NonPublic);
            ON_KEY_RELEASED = controlType.GetMethod("OnKeyReleased", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        internal static void FireKeyDown(string key, IEnumerable<Control> tree)
        {
            TraverseTreeKeyHandler(tree, ON_KEY_DOWN, key);
        }

        internal static void FireKeyPressed(string key, IEnumerable<Control> tree)
        {
            TraverseTreeKeyHandler(tree, ON_KEY_PRESSED, key);
        }

        internal static void FireKeyReleased(string key, IEnumerable<Control> tree)
        {
            TraverseTreeKeyHandler(tree, ON_KEY_RELEASED, key);
        }

        private static void TraverseTreeKeyHandler(IEnumerable<Control> tree, MethodInfo toInvoke, string key)
        {
            foreach (var control in tree)
            {
                toInvoke.Invoke(control, new[] { key });

                if (control is Panel)
                    TraverseTreeKeyHandler((control as Panel).Children, toInvoke, key);
                if (control is ContentControl)
                    TraverseTreeKeyHandler(new Control[] { (control as ContentControl).Child }, toInvoke, key);
            }
        }
    }
}
