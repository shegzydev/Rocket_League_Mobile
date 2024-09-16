using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weaponstats", menuName = "Weapons")]
public class WeaponDetails : ScriptableObject
{
    [Header("")]
    public float fireRate;
    [Range(0, 1)] public float damage;
    public float range;
    [Range(0, 1)] public float accuracy;
    public float reloadTime;

    public int catridgeSize;

    public AudioClip fireSound;
    public AudioClip reloadSound;

    public ParticleSystem muzzleFlash;
}
