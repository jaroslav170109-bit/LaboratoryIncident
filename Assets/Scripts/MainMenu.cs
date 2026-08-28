using UnityEngine;
using UnityEngine.SceneManagement;
using System.Data.SqlClient; // Добавили для работы с SQL из Visual Studio

public class MainMenu : MonoBehaviour
{
    // Строка подключения к файлу базы данных .mdf из Visual Studio
    private string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\jaroslav.karelkin\LaboratoryIncident\Database folder\GameStats.mdf"";Integrated Security=True;Connect Timeout=30";

    // Метод для кнопки Старт
    public void PlayGame()
    {
        // 1. Записываем факт нажатия кнопки в SQL базу
        RecordPlayInDatabase();

        // 2. Переходим на сцену с названием Level 1
        SceneManager.LoadScene("Level 1");
    }

    private void RecordPlayInDatabase()
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                // Вставляем текущее время нажатия в таблицу Plays
                string query = "INSERT INTO Plays (ClickTime) VALUES (GETDATE())";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            Debug.Log("Нажатие 'Play' зафиксировано в SQL базе данных!");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("БД не ответила, но игра продолжается: " + e.Message);
        }
    }

    // Метод для кнопки Выход
    public void QuitGame()
    {
        Debug.Log("Игра закрылась!");
        Application.Quit();
    }
}