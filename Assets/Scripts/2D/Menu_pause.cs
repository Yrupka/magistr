using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_pause : MonoBehaviour
{
    private Transform options;
    private Transform pause;
    private bool isPaused = false;
    
    private void Awake()
    {
        pause = transform.Find("Pause");
        options = transform.Find("Options");
        Transform buttons = pause.Find("Buttons");
        buttons.Find("Back").GetComponent<Button>().onClick.AddListener(Resume);
        buttons.Find("Options").GetComponent<Button>().onClick.AddListener(Options);
        buttons.Find("Quit").GetComponent<Button>().onClick.AddListener(Quit);
    }

    private void Start()
    {
        Resume();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    private void Resume()
    {
        pause.gameObject.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    private void Pause()
    {
        pause.gameObject.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }

    private void Options()
    {
        options.gameObject.SetActive(true);
    }

    private void Quit()
    {
        SceneManager.LoadScene("Questions");
    }
}
