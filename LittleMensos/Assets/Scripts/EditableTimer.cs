using UnityEngine;
using UnityEngine.UI;
using System;

public class EditableTimer : MonoBehaviour
{
    [Header("Configuración del Timer")]
    [Tooltip("Duración del temporizador en segundos")]
    [SerializeField] private float timerDuration;

    [Tooltip("Tiempo que se agrega cuando se llama AddTime()")]
    [SerializeField] private float timeAdded;

    [Header("UI")]
    [Tooltip("Imagen con Fill Radial")]
    [SerializeField] private Image timerFillImage;

    [Header("Eventos del Timer")]
    public static Action onTimerEnd;

    [Header("Referencias")]
    [SerializeField] private Canvas endGame;
    [SerializeField] private TriggerCutscene triggerCutscene;

    private float remainingTime;
    private bool isRunning;

    // =========================
    // UNITY LIFECYCLE
    // =========================

    private void OnEnable()
    {
        if (triggerCutscene != null)
            triggerCutscene.startGame += StartTimer;
    }

    private void OnDisable()
    {
        if (triggerCutscene != null)
            triggerCutscene.startGame -= StartTimer;
    }

    private void Update()
    {
        if (!isRunning) return;

        remainingTime -= Time.deltaTime;

        // Fill radial (1 → 0)
        float fillAmount = Mathf.Clamp01(remainingTime / timerDuration);
        timerFillImage.fillAmount = fillAmount;

        // Feedback visual de urgencia (opcional)
        if (fillAmount <= 0.2f)
            timerFillImage.color = Color.red;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            timerFillImage.fillAmount = 0f;
            isRunning = false;

            EndTime();
            onTimerEnd?.Invoke();

            Time.timeScale = 0.1f;
        }
    }

    // =========================
    // TIMER CONTROLS
    // =========================

    public void StartTimer()
    {
        Debug.Log("⏱️ Timer iniciado");

        remainingTime = timerDuration;
        isRunning = true;

        timerFillImage.fillAmount = 1f;
        timerFillImage.color = Color.white;
    }

    [ContextMenu("Reset Timer")]
    public void ResetTimer()
    {
        remainingTime = timerDuration;
        isRunning = false;

        timerFillImage.fillAmount = 1f;
        timerFillImage.color = Color.white;
    }

    public void AddTime()
    {
        remainingTime = Mathf.Min(remainingTime + timeAdded, timerDuration);
    }

    // =========================
    // END GAME
    // =========================

    private void EndTime()
    {
        Debug.Log("⏰ TIEMPO TERMINADO");

        if (endGame != null)
            endGame.gameObject.SetActive(true);
    }
}
