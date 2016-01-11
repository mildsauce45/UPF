using FirstWave.Unity.Core.Input;
using FirstWave.Unity.Gui.Panels;
using FirstWave.Unity.Gui.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FirstWave.Unity.Gui
{
    /// <summary>
    /// Base Container for all my GUI controls. It will take up the entire screen space.
    /// </summary>
    public class Frame : MonoBehaviour
    {
        private IList<Panel> panels;
        private InputManager inputManager;

        void Awake()
        {
            panels = new List<Panel>();
            inputManager = FindObjectOfType<InputManager>();
        }

        public void AddPanel(Panel panel)
        {
            panels.Add(panel);
        }

        private void CheckInput()
        {
            var castControls = panels.OfType<Control>();

            // We're going to tunnel through each control in the frame and inform them of any key events
            // There are three events we're concerned with, KeyDown, KeyPressed, KeyReleased
            /// TODO: Find a better way to traverse the tree, probably caching the tree as flat list
            foreach (var key in inputManager.allKeys)
            {
                if (inputManager.KeyDown(key))
                    VisualTreeHelper.FireKeyDown(key, castControls);

                if (inputManager.KeyPressed(key))
                    VisualTreeHelper.FireKeyPressed(key, castControls);

                if (inputManager.KeyReleased(key))
                    VisualTreeHelper.FireKeyReleased(key, castControls);
            }
        }

        #region Unity Engine

        void Update()
        {
            foreach (var p in panels)
                p.Measure();

            var screenSpace = new Rect(0, 0, Screen.width, Screen.height);

            foreach (var p in panels)
                p.Layout(screenSpace);

            CheckInput();
        }

        void OnGUI()
        {
            foreach (var p in panels)
                p.Draw();
        }

        #endregion
    }
}
