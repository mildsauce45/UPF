using FirstWave.Messaging;
using FirstWave.Unity.Core.Input;
using FirstWave.Unity.Data;
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
		private IList<Control> controls;
		private InputManager inputManager;
		
		private object viewModel;		

		private int prevWidth;
		private int prevHeight;

		void Awake()
		{
            controls = new List<Control>();

			inputManager = FindObjectOfType<InputManager>();

			// If there isn't an input manager in the scene create one as it's a safesingleton
			if (!inputManager)
				inputManager = InputManager.Instance;

			DontDestroyOnLoad(gameObject);
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

			XamlProcessor.ParseXaml(controls, view, viewModel);
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
				c.Draw();

			prevWidth = Screen.width;
			prevHeight = Screen.height;
		}

		void OnLevelWasLoaded()
		{
            controls.Clear();
			viewModel = null;
		}

		void OnDestroy()
		{
			Messenger.Default.Unregister(this);
		}

		#endregion
	}
}
