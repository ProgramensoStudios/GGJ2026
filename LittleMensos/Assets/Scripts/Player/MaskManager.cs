using System;
using System.Collections.Generic;
using UnityEngine;

public enum MaskType { None, Dash, Climb }

public class MaskManager : MonoBehaviour
{
    public static MaskManager Instance { get; private set; }

    [Header("Masks")]
    [SerializeField] private GameObject dashMask;
    [SerializeField] private GameObject climbMask;

    [Header("Mask Data")]
    public MaskType activeMask = MaskType.None;
    private HashSet<MaskType> unlockedMasks = new HashSet<MaskType>();

    public Action<MaskType> OnMaskChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //LoadMasks();
    }

    private void Start()
    {
        UpdateMaskVisuals();
    }

    // Desbloquear máscara nueva
    public void UnlockMask(MaskType mask)
    {
        if (mask == MaskType.None) return;
        if (!unlockedMasks.Contains(mask))
        {
            unlockedMasks.Add(mask);
           // SaveMasks();
            Debug.Log($"<color=green>Mask unlocked: {mask}</color>");
        }
    }

    public void CycleMask()
    {
        List<MaskType> options = new List<MaskType> { MaskType.None };

        if (unlockedMasks.Contains(MaskType.Dash))
            options.Add(MaskType.Dash);

        if (unlockedMasks.Contains(MaskType.Climb))
            options.Add(MaskType.Climb);

        int currentIndex = options.IndexOf(activeMask);
        int nextIndex = (currentIndex + 1) % options.Count;

        activeMask = options[nextIndex];

        UpdateMaskVisuals(); // CLAVE

        Debug.Log($"<color=yellow>Mask active: {activeMask}</color>");
        OnMaskChanged?.Invoke(activeMask);
    }


    void UpdateMaskVisuals()
    {
        // APAGAMOS TODO SIEMPRE
        dashMask.SetActive(false);
        climbMask.SetActive(false);

        // PRENDEMOS SOLO LA ACTIVA
        switch (activeMask)
        {
            case MaskType.Dash:
                dashMask.SetActive(true);
                break;

            case MaskType.Climb:
                climbMask.SetActive(true);
                break;

            case MaskType.None:
            default:
                // no se prende nada
                break;
        }
    }



    // Ver si el jugador tiene una habilidad
    public bool CanDash() => activeMask == MaskType.Dash;
    public bool CanClimb() => activeMask == MaskType.Climb;

    // ==== Persistencia ====
   private void SaveMasks()
    {
        PlayerPrefs.SetInt("MaskDashUnlocked", unlockedMasks.Contains(MaskType.Dash) ? 1 : 0);
        PlayerPrefs.SetInt("MaskClimbUnlocked", unlockedMasks.Contains(MaskType.Climb) ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadMasks()
    {
        unlockedMasks.Clear();
        if (PlayerPrefs.GetInt("MaskDashUnlocked", 0) == 1) unlockedMasks.Add(MaskType.Dash);
        if (PlayerPrefs.GetInt("MaskClimbUnlocked", 0) == 1) unlockedMasks.Add(MaskType.Climb);
    }
}
