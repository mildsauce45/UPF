using FirstWave.Unity.Gui;
using FirstWave.Unity.Gui.Controls;
using FirstWave.Unity.Gui.Enums;
using FirstWave.Unity.Gui.Panels;
using FirstWave.Unity.Gui.Primitives;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestStackPanel : MonoBehaviour
{
    private Image img;

    void Start()
    {
        var frame = FindObjectOfType<Frame>();
        if (frame == null)
        {
            Debug.Log("Need a frame to properly test");
            return;
        }

        //var menu = new Menu();
        //menu.AddItem("Start New Game", () => SceneManager.LoadScene("TestLoadNewScene"));
        //menu.AddItem("Continue Existing Game", () => Debug.Log("Continued Existing Game"));

        var sp = new StackPanel();
        sp.HorizontalAlignment = HorizontalAlignment.Center;
        sp.VerticalAlignment = VerticalAlignment.Center;

        //sp.AddChild(menu);

        img = new Image("Icons/fireball");

        sp.AddChild(new TextBlock("Foo"));
        sp.AddChild(img);

        frame.AddPanel(sp);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 60, 25), "Change"))
        {
            img.Source = "Icons/book";
        }
    }
}
