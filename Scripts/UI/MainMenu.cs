using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScene, optionsMenuScene, controllerMenuScene, creditMenuScene;
    [SerializeField] private GameObject continueButton, mainButtonFirst, mainButtonSecond, optionsButtonFirst, controllerButtonBack, creditButtonBack, optionsButton, quitButton;

    [SerializeField] private TextMeshProUGUI loadingText;

    [SerializeField] private TextMeshProUGUI contText;

    [SerializeField] private AudioSource aSource;
    [SerializeField] private AudioClip aClick;
    
    private bool newGame = false;

    private ControllerCheck controller = new ControllerCheck();

    void Start()
    {
        if (!PlayerPrefs.HasKey("SpawnPointX"))
        {
            newGame = true;
            GrayOut(continueButton, true);
            mainButtonFirst = mainButtonSecond;
        }
        optionsMenuScene.SetActive(false);
        controllerMenuScene.SetActive(false);
        creditMenuScene.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        if (controller.CheckForController())
            EventSystem.current.SetSelectedGameObject(mainButtonFirst);
        else
            EventSystem.current.SetSelectedGameObject(null);
    }
    public void NewGame()
    {
        MakeButtonsUninteractable();
        StartCoroutine(LoadSceneAsync(true));
        Time.timeScale = 1f;
        PlayerPrefs.DeleteAll();
        SaveManager.instance.DeleteData();
    }

    private void MakeButtonsUninteractable()
    {
        hoverAudioSource.volume = 0;
        GrayOut(continueButton, false);
        GrayOut(mainButtonSecond, false);
        GrayOut(optionsButton, false);
        GrayOut(quitButton, false);
    }

    private IEnumerator LoadSceneAsync(bool newGame)
    {
        StartCoroutine(LoadingText());

        AsyncOperation asyncLoad;

        if (newGame)
            asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        else
            asyncLoad = SceneManager.LoadSceneAsync((int)SaveManager.instance.LoadSpawnPoint().x + 1, LoadSceneMode.Single);

        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if(asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    private IEnumerator LoadingText()
    {
        int loops = 0;
        loadingText.gameObject.SetActive(true);
        while (true)
        {
            yield return new WaitForSeconds(1f);

            loops++;
            if(loops > 3)
            {
                loadingText.text = "Loading";
                loops = 0;
            }
            else
                loadingText.text += " .";

            yield return null;
        }
    }

    public void Mainmenu()
    {
        mainMenuScene.SetActive(true);
        optionsMenuScene.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        if(controller.CheckForController())
            EventSystem.current.SetSelectedGameObject(mainButtonFirst);
    }

    public void OptionsMenu()
    {
        mainMenuScene.SetActive(false);
        optionsMenuScene.SetActive(true);
        creditMenuScene.SetActive(false);
        controllerMenuScene.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        if (controller.CheckForController())
            EventSystem.current.SetSelectedGameObject(optionsButtonFirst);
    }

    public void ControllerMenu() 
    {
        controllerMenuScene.SetActive(true);
        optionsMenuScene.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        if (controller.CheckForController())
            EventSystem.current.SetSelectedGameObject(controllerButtonBack);

    }

    public void CreditMenu()    
    {
        creditMenuScene.SetActive(true);
        optionsMenuScene.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);

        if (controller.CheckForController())
            EventSystem.current.SetSelectedGameObject(creditButtonBack);

    }
    public void Continue()
    {
        Time.timeScale = 1f;
        MakeButtonsUninteractable();
        StartCoroutine(LoadSceneAsync(false));
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    private void GrayOut(GameObject buttonObject, bool makeGray)
    {
        Button button = buttonObject.GetComponent<Button>();
        button.GetComponent<Button>().enabled = false;
        buttonObject.GetComponent<Image>().enabled = false;
        
        if(makeGray)
            contText.color = Color.gray;
    }
 
    public void ClickSFX()
    {
        aSource.PlayOneShot(aClick);
    }

    public void ClickSFXForContinue()
    {
        if(!newGame)
            aSource.PlayOneShot(aClick);
    }

    [SerializeField] AudioClip hoverSFX1, hoverSFX2, hoverSFX3, hoverSFX4;
    [SerializeField] AudioSource hoverAudioSource;
    public void HoverSFXForContinue()
    {
        if (!newGame)
            hoverAudioSource.PlayOneShot(hoverSFX1);
    }

    public void HoverSFX(int pitch)
    {
        switch (pitch)
        {
            case 1:
                hoverAudioSource.PlayOneShot(hoverSFX1);
                break;
            case 2:
                hoverAudioSource.PlayOneShot(hoverSFX2);
                break;
            case 3:
                hoverAudioSource.PlayOneShot(hoverSFX3);
                break;
            case 4:
                hoverAudioSource.PlayOneShot(hoverSFX4);
                break;
            default: 
                Debug.Log("Invalid input. Should only be 1-4"); 
                break;
        }
    }
}

public class ControllerCheck  
{
    public bool CheckForController()
    {
        var controllers = Input.GetJoystickNames();
        if (controllers.Length > 0)
            return true;

        return false;
    }
}
