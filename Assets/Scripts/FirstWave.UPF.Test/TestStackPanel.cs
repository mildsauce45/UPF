using FirstWave.Unity.Gui;
using FirstWave.UPF.Test;
using UnityEngine;

public class TestStackPanel : MonoBehaviour
{
	void Start()
	{
		var frame = FindObjectOfType<Frame>();
		if (frame == null)
		{
			Debug.Log("Need a frame to properly test");
			return;
		}

		frame.LoadPage("UPF/Combat", new TestViewViewModel());
	}
}
