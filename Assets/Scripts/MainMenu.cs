using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для смены сцен

public class MainMenu : MonoBehaviour
{
    // Метод для кнопки Старт
    public void PlayGame()
    {
        // Переходим на сцену с названием Level 1
        SceneManager.LoadScene("Level 1");
    }

    // Метод для кнопки Выход
    public void QuitGame()
    {
        Debug.Log("Игра закрылась!"); // Это будет видно только в редакторе
        Application.Quit(); // Это сработает в скомпилированной игре (.exe)
    }
}