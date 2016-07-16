using FirstWave.Messaging;
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



		public object XamlProcesser { get; private set; }

		void Awake()
		{
			panels = new List<Panel>();

			inputManager = FindObjectOfType<InputManager>();

			// If there isn't an input manager in the scene create one as it's a safesingleton
			if (!inputManager)
				inputManager = InputManager.Instance;

			DontDestroyOnLoad(gameObject);
		}

		public void AddPanel(Panel panel)
		{
			panels.Add(panel);
		}

		public void Clear()
		{
			panels.Clear();
		}

		/// <summary>
		/// The guts of this probably need to be refactored into their own special little class
		/// </summary>		
		public void LoadPage(string view, object viewModel)
		{
			Clear();

			XamlProcessor.ParseXaml(panels, view, viewModel);
		}

		private void CheckInput()
		{
			// We want to enumerate fully here because some of the key presses below can modify the structure
			// of the visual tree and we don't want there to be exceptions
			var castControls = panels.OfType<Control>().ToList();

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

		void OnLevelWasLoaded()
		{
			panels.Clear();
		}

		void OnDestroy()
		{
			Messenger.Default.Unregister(this);
		}

		#endregion
	}
}
