using UnityEngine;
using System.Collections;
using FirstWave.Unity.Gui;
using FirstWave.Unity.Gui.Panels;
using FirstWave.Unity.Gui.Primitives;

public class TestSecondGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var frame = FindObjectOfType<Frame>();

        var sp = new StackPanel();
        sp.HorizontalAlignment = FirstWave.Unity.Gui.Enums.HorizontalAlignment.Center;
        sp.VerticalAlignment = FirstWave.Unity.Gui.Enums.VerticalAlignment.Top;

        sp.AddChild(new TextBlock("This is the second scene."));

        frame.AddPanel(sp);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
