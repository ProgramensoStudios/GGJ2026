using UnityEngine;

[CreateAssetMenu(fileName = "SFXData", menuName = "Scriptable Objects/SFXData")]
public class SFXData : ScriptableObject
{
    public string id;
    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 2f)] public float pitch = 1f;

    public bool loop;
}
