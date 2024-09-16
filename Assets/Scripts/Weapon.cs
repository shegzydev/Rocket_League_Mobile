using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] WeaponDetails weaponDetails;
    bool reloading;
    float timeSinceFire;
    [SerializeField] Transform firePoint;
    AudioSource fireSource;

    ParticleSystem muzzle;
    List<ParticleSystem> muzzleChildren;

    void Start()
    {
        fireSource = gameObject.AddComponent<AudioSource>();
        muzzle = Instantiate(weaponDetails.muzzleFlash, firePoint.position, Quaternion.AngleAxis(-90, firePoint.up) * firePoint.rotation, firePoint);
        muzzleChildren = muzzle.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    void Update()
    {
        if (TryFire())
        {
            Fire();
        }
    }

    bool TryFire()
    {
        bool final = true;
        timeSinceFire += Time.deltaTime;

        if (reloading) final = false;

        if (timeSinceFire < 1f / weaponDetails.fireRate) final = false;
        else timeSinceFire = 0f;

        return final;
    }

    void Fire()
    {
        var error = 45 * (1 - weaponDetails.accuracy);
        var scatter = Random.Range(-error, error);
        var direction = Quaternion.AngleAxis(scatter, firePoint.up) * Quaternion.AngleAxis(scatter, firePoint.right) * firePoint.forward;

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hitInfo, weaponDetails.range))
        {

        }

        fireSource.PlayOneShot(weaponDetails.fireSound);
        muzzle.Emit(5);
        muzzleChildren.ForEach(x => x.Emit(5));
    }

    IEnumerator Reload()
    {
        reloading = true;

        for (float i = 0; i <= weaponDetails.reloadTime; i += Time.deltaTime)
        {
            yield return null;
        }

        reloading = false;
    }

    private void OnGUI()
    {
        Rect label = new Rect(20, 120, 400, 400);
        GUI.Label(label, $"Rate {weaponDetails.fireRate}\nsound{weaponDetails.fireSound.name}", new GUIStyle { fontSize = 50 });
    }
}
