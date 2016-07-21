using FirstWave.Unity.Gui;
using FirstWave.Unity.Gui.Controls;
using FirstWave.Unity.Gui.Enums;
using FirstWave.Unity.Gui.Panels;
using FirstWave.Unity.Gui.Primitives;
using FirstWave.UPF.Test;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestStackPanel : MonoBehaviour
{
	private TestViewViewModel viewModel;

	void Start()
	{
		var frame = FindObjectOfType<Frame>();
		if (frame == null)
		{
			Debug.Log("Need a frame to properly test");
			return;
		}

		viewModel = new TestViewViewModel();

		frame.LoadPage("UPF/Combat", viewModel);
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(0, 0, 50, 30), "Inc"))
			viewModel.Party[0].HP = (int.Parse(viewModel.Party[0].HP) + 1).ToString();
			//viewModel.Message = "Hello World";
	}
}
