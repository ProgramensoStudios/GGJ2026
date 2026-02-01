using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private DinamicAudio dynamicAudio;
    [SerializeField] public bool isInDanger;

    public Image imgUI;
    public Sprite noMask;
    public Sprite maskDash;
    public Sprite maskClimb;

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
        SFXManager.Instance.Play("Whoosh", transform.position);
        if (unlockedMasks.Contains(MaskType.Dash))
            options.Add(MaskType.Dash);

        if (unlockedMasks.Contains(MaskType.Climb))
            options.Add(MaskType.Climb);

        if (options.Count <= 1)
            return;

        int currentIndex = options.IndexOf(activeMask);
        int nextIndex = (currentIndex + 1) % options.Count;

        activeMask = options[nextIndex];

        UpdateMaskVisuals(); // CLAVE

        Debug.Log($"<color=yellow>Mask active: {activeMask}</color>");
       // OnMaskChanged?.Invoke(activeMask);
    }


    public void UpdateMaskVisuals()
    {
        // APAGAMOS TODO SIEMPRE
        dashMask.SetActive(false);
        climbMask.SetActive(false);

        // PRENDEMOS SOLO LA ACTIVA
        switch (activeMask)
        {
            case MaskType.Dash:
                dashMask.SetActive(true);
                imgUI.sprite = maskDash;
                if (isInDanger)
                {
                    dynamicAudio.AddLayerSound(5);
                    dynamicAudio.LessLayerSound(0);
                    dynamicAudio.LessLayerSound(1);
                    dynamicAudio.LessLayerSound(2);
                    dynamicAudio.LessLayerSound(4);
                    dynamicAudio.LessLayerSound(3);
                    dynamicAudio.LessLayerSound(6);
                }
                else
                {
                    dynamicAudio.AddLayerSound(3);
                    dynamicAudio.LessLayerSound(0);
                    dynamicAudio.LessLayerSound(1);
                    dynamicAudio.LessLayerSound(2);
                    dynamicAudio.LessLayerSound(4);
                    dynamicAudio.LessLayerSound(5);
                    dynamicAudio.LessLayerSound(6);
                }
                break;

            case MaskType.Climb:
                climbMask.SetActive(true);
                imgUI.sprite = maskClimb;
                if (isInDanger)
                {
                    dynamicAudio.AddLayerSound(5);
                    dynamicAudio.LessLayerSound(0);
                    dynamicAudio.LessLayerSound(1);
                    dynamicAudio.LessLayerSound(2);
                    dynamicAudio.LessLayerSound(4);
                    dynamicAudio.LessLayerSound(3);
                    dynamicAudio.LessLayerSound(6);
                }
                else
                {
                    dynamicAudio.AddLayerSound(3);
                    dynamicAudio.LessLayerSound(0);
                    dynamicAudio.LessLayerSound(1);
                    dynamicAudio.LessLayerSound(2);
                    dynamicAudio.LessLayerSound(4);
                    dynamicAudio.LessLayerSound(5);
                    dynamicAudio.LessLayerSound(6);
                }
                break;

            case MaskType.None:
                imgUI.sprite = noMask;
                if (isInDanger)
                {
                    dynamicAudio.AddLayerSound(6);
                    dynamicAudio.LessLayerSound(0);
                    dynamicAudio.LessLayerSound(1);
                    dynamicAudio.LessLayerSound(2);
                    dynamicAudio.LessLayerSound(4);
                    dynamicAudio.LessLayerSound(3);
                    dynamicAudio.LessLayerSound(5);
                }
                else
                {
                    dynamicAudio.AddLayerSound(4);
                    dynamicAudio.LessLayerSound(0);
                    dynamicAudio.LessLayerSound(1);
                    dynamicAudio.LessLayerSound(2);
                    dynamicAudio.LessLayerSound(3);
                    dynamicAudio.LessLayerSound(5);
                    dynamicAudio.LessLayerSound(6);
                }
                break;
        }
        OnMaskChanged?.Invoke(activeMask);
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
