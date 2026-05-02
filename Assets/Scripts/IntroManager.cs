using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Начальные звуки (сирены, крики и т.д.)")]
    public GameObject[] initialSounds;
    public float delayBetweenSounds = 1f; // Задержка между звуками

    [Header("Взрыв и эффекты")]
    public float delayBeforeExplosion = 15f;
    public GameObject explosionSoundObject;

    [Header("Камера и интерфейс")]
    public Animator cameraAnimator;
    public string shakeTriggerName = "Shake";
    public GameObject darknessUI;

    [Header("Настройки финала")]
    public float timeInDarkness = 10f;
    public string mainMenuSceneName = "MainMenu";

    private void OnEnable()
    {
        StartCoroutine(StartIntroSequence());
    }

    IEnumerator StartIntroSequence()
    {
        // 1. Активируем звуки по очереди с задержкой
        foreach (GameObject sound in initialSounds)
        {
            if (sound != null)
            {
                sound.SetActive(true);
                yield return new WaitForSeconds(delayBetweenSounds);
            }
        }

        // 2. Ждем оставшееся время до взрыва
        // Вычитаем время, которое уже потратили на включение звуков, чтобы общая задержка была точной
        float remainingTime = delayBeforeExplosion - (initialSounds.Length * delayBetweenSounds);
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }
        else
        {
            // Если звуков было слишком много и 15 секунд уже прошло, ждем хотя бы чуть-чуть
            yield return new WaitForSeconds(1f);
        }


        // 4. Запускаем тряску камеры
        if (cameraAnimator != null)
            cameraAnimator.SetTrigger(shakeTriggerName);
        // 3. Активируем звук взрыва
        if (explosionSoundObject != null)
            explosionSoundObject.SetActive(true);
        foreach (GameObject sound in initialSounds)
        {
            if (sound != null)
                sound.SetActive(false); // Отключаем начальные звуки
        }

      

        // 5. Включаем темноту (картинку с AudioSource потери сознания)
        if (darknessUI != null)
            darknessUI.SetActive(true);

        // 6. Ждем в темноте
        yield return new WaitForSeconds(timeInDarkness);

        // 7. Переходим в главное меню
        SceneManager.LoadScene(mainMenuSceneName);
    }
}