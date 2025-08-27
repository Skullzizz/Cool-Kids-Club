using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject model;
    [Range(0, 100)] public int shootDamage;
    [Range(5, 1000)] public int shootDist;
    [Range(0.1f, 3)] public float shootRate;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootVol;

    public Sprite weaponIcon;

}
