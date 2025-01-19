using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SelectedButton : MonoBehaviour
{
    [SerializeField] GameObject[] buttons = new GameObject[4];
    private int selectedIndex = 0;
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    public void NextSelectedButton()
    {
        selectedIndex++;
        EventSystem.current.SetSelectedGameObject(buttons[selectedIndex]);
    }
}
