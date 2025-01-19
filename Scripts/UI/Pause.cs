using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu, optionsMenu, pauseFirstButton, optionsFirstButton, optionsBackButton;
    [SerializeField] private AudioSource aSource;
    [SerializeField] private AudioClip aClick;
    [SerializeField] private PlayerMovement pMovement;

    private bool isPaused, inCutScene = true, controller = false;

    private ControllerCheck controllerCheck = new ControllerCheck();
    void Start()
    {
        pauseMenu.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (!inCutScene && Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            if (isPaused)
            {
                Cursor.visible = false;
                ResumeGame();
            }
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        pMovement.TurnOffWalkingSFX();
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(null);
;
        if(controllerCheck.CheckForController())
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
    }

    public void OptionMenu()
    {
        optionsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        if (controllerCheck.CheckForController())
            EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void CloseOptionMenu()
    {
        optionsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        if (controllerCheck.CheckForController())
            EventSystem.current.SetSelectedGameObject(optionsBackButton);
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        pMovement.TurnOnWalkingSFX();
        optionsMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    } 

    public void TurnOffMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void QuitToMenu()
    {
        SaveManager.instance.SaveGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void OutOfCutscene()
    {
        inCutScene = false;
    }

    [SerializeField] AudioClip hoverSFX1, hoverSFX2, hoverSFX3, hoverSFX4;
    [SerializeField] AudioSource hoverAudioSource;
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
    public void ClickSFX()
    {
        aSource.PlayOneShot(aClick);
    }
}
