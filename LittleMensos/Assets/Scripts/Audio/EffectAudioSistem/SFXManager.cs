using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private List<SFXData> sfxList;

    private Dictionary<string, SFXData> sfxDict;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxDict = new Dictionary<string, SFXData>();
        foreach (var sfx in sfxList)
            sfxDict[sfx.id] = sfx;
    }

    public void Play(string id, Vector3 position)
    {
        if (!sfxDict.TryGetValue(id, out var data))
        {
            Debug.LogWarning($"SFX {id} no existe");
            return;
        }

        var go = new GameObject($"SFX_{id}");
        var player = go.AddComponent<SFXPool>();
        player.Play(data, position);
    }
}
