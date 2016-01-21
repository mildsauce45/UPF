using FirstWave.Unity.Gui;
using FirstWave.Unity.Gui.Controls;
using FirstWave.Unity.Gui.Enums;
using FirstWave.Unity.Gui.Panels;
using FirstWave.Unity.Gui.Primitives;
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

        var menu = new Menu();
        menu.AddItem("Start New Game", () => menu.AddItem("A child", () => { }));
        menu.AddItem("Continue Existing Game", () => Debug.Log("Continued Existing Game"));

        var sp = new StackPanel();
        sp.HorizontalAlignment = HorizontalAlignment.Center;
        sp.VerticalAlignment = VerticalAlignment.Center;

        sp.AddChild(menu);

        frame.AddPanel(sp);
    }
}
