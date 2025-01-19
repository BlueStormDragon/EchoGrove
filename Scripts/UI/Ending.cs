using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField] private GameObject goodEnding, okEnding, bestEnding;

    private void Start()
    {
        goodEnding.SetActive(false);    
        okEnding.SetActive(false);
        bestEnding.SetActive(false);
    }

    public void WhatEnding()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (SaveManager.instance.LoadBerryCount() == 15)
            BestEnding();
        else if (SaveManager.instance.LoadBerryCount() > 11)
            GoodEnding();
        else
            BadEnding();
    }

    void GoodEnding()
    {
        goodEnding.SetActive(true);
        StartCoroutine(EndingScene());
    }

    void BadEnding()
    {
        okEnding.SetActive(true);
        StartCoroutine(EndingScene());
    }
    void BestEnding() 
    {
        bestEnding.SetActive(true);
        StartCoroutine(EndingScene());
    }
    IEnumerator EndingScene()
    {
        while(true)
        {
            yield return new WaitForSeconds(8f);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton0));
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("MainMenu");
        }
    }
    
}
