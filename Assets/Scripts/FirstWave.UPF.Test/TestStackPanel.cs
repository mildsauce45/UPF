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
