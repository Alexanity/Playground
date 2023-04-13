using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public string MenuName;
    private bool cameraFrozen = false;
    private FirstPersonLook cameraScript;

    void Start()
    {
        // Get the FirstPersonLook script from the camera object
        cameraScript = Camera.main.GetComponent<FirstPersonLook>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        cameraFrozen = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        cameraFrozen = true;
    }

    void LateUpdate()
    {
        if (cameraFrozen && cameraScript != null)
        {
            // Freeze the camera
            cameraScript.enabled = false;
        }
        else if (cameraScript != null)
        {
            // Unfreeze the camera
            cameraScript.enabled = true;
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        Resume();
        SceneManager.LoadScene(MenuName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
