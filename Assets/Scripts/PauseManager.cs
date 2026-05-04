using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Сюда кидаем панель PauseMenu
    public static bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
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
        Time.timeScale = 1f; // Запускаем время
        isPaused = false;

        // Прячем курсор обратно
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Останавливаем время в игре
        isPaused = true;

        // Показываем курсор, чтобы нажимать кнопки
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // Важно вернуть время в норму перед сменой сцены!
        SceneManager.LoadScene("MainMenu"); // Напиши тут имя своей сцены с меню
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Игра закрыта");
    }
}