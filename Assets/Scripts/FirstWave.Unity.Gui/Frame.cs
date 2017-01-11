using FirstWave.Unity.Core.Input;
using FirstWave.Unity.Data;
using FirstWave.Unity.Gui.Utilities;
using FirstWave.Unity.Gui.Utilities.Parsing;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstWave.Unity.Gui
{
	/// <summary>
	/// Base Container for all my GUI controls. It will take up the entire screen space.
	/// </summary>
    [RequireComponent(typeof(InputManager))]
    [RequireComponent(typeof(GUIManager))]
	public class Frame : MonoBehaviour
	{
		private IList<Control> controls;
		private InputManager inputManager;

		private object viewModel;

		private int prevWidth;
		private int prevHeight;

		void Awake()
		{
			controls = new List<Control>();

			// If there isn't an input manager in the scene create one as it's a safesingleton
			if (!inputManager)
				inputManager = GetComponent<InputManager>();

			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		}

		private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadMode)
		{
			// If we're clearing the current scene altogether, then let's reset the frame
			if (loadMode == LoadSceneMode.Single)
			{
				controls.Clear();
				viewModel = null;
			}

			// Maybe do something with an additive load?
		}

		public void AddControl(Control control)
		{
			controls.Add(control);
		}

		public void Clear()
		{
			controls.Clear();
		}

		public void LoadPage(string view, object viewModel)
		{
			Clear();

			this.viewModel = viewModel;

			// Get the controls in one fell swoop so we're not in the process of parsing while updates are going on
			var parsedControls = XamlProcessor.ParseXaml(view, viewModel);

			foreach (var c in parsedControls)
				controls.Add(c);
		}

		private void CheckInput()
		{
			// We want to enumerate fully here because some of the key presses below can modify the structure
			// of the visual tree and we don't want there to be exceptions
			var castControls = controls.ToList();

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
			if (viewModel is ViewModelBase)
				(viewModel as ViewModelBase).Update();

			CheckInput();
		}

		void OnGUI()
		{
			// If the window was resized and we had done a layout at least once, we need to invalidate everything
			if ((Screen.width != prevWidth && prevWidth > 0) || (Screen.height != prevHeight && prevHeight > 0))
			{
				foreach (var c in controls)
					c.InvalidateLayout(null);
			}

			foreach (var c in controls)
				c.Measure();

			var screenSpace = new Rect(0, 0, Screen.width, Screen.height);

			foreach (var c in controls)
				c.Layout(screenSpace);

            foreach (var c in controls)
                c.DoDraw();

			prevWidth = Screen.width;
			prevHeight = Screen.height;
		}

		#endregion
	}
}
