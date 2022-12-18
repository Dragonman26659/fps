using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    [Header("Gun Variables")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 5f;
    public float ImpactForce = 10f;
    private float nextTimeToFire = 0.25f;
    public bool isAutomatic = false;
    public int bulletsPerShot = 1;
    public float bulletSpread = 1f;



    [Header("Gun Objects")]
    public Camera fpscam;
    public ParticleSystem muzzleFlash;
    public GameObject surfaceImpactEffect;
    public GameObject NpcHealthImpactEffect;
    public Animator animator;
    public AudioSource ShootSound;
    public AudioSource ReloadSound;
    public AudioSource MeleSound;

    [Header("Ammunition")]
    public int MaxAmmo = 10;
    public int AmmoPickupAmount = 10;
    public float reloadTime = 2f;
    public int reloadAmmount = 1;
    public float ReloadStartDelay = 0f;
    private float reloadTimer = 0f;
    private int CurrentAmmo;
    private int ammoStore;
    public KeyCode ReloadKey = KeyCode.R;
    private bool isReloading = false;
    public Text AmmoText;
    public int StartAmmo = 10;
    private bool hasStartedReloading = false;

    [Header("Recoil")]
    //hitfire Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;




    void Start()
    {
        //ammo presets
        CurrentAmmo = StartAmmo;
    }



    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
        animator.SetBool("hasStartedReloading", false);
    }


    void Update()
    {

        AmmoText.text = CurrentAmmo + "/" + ammoStore;
        if (isReloading && ammoStore != 0)
        {
            return;
        }
        if (reloadTimer > 0f)
        {
            reloadTimer -= Time.deltaTime;
        }

        if (!hasStartedReloading)
        {
            animator.SetBool("hasStartedReloading", false);
        }
        if (CurrentAmmo >= MaxAmmo || ammoStore <= 0)
        {
            Debug.Log("Ammo reload finished");
            hasStartedReloading = false;
            animator.SetBool("hasStartedReloading", false);
        }

        //Reload
        if (Time.time >= nextTimeToFire && !isReloading && ammoStore > 0)
        {
            if (CurrentAmmo <= 0 || Input.GetKey(ReloadKey) || hasStartedReloading)
            {
                if (!hasStartedReloading)
                {
                    hasStartedReloading = true;
                    animator.SetBool("hasStartedReloading", true);
                    reloadTimer = ReloadStartDelay;
                }

                if(CurrentAmmo >= MaxAmmo)
                {
                    CurrentAmmo = MaxAmmo;
                }

                else if (reloadTimer <= 0f)
                {
                    
                    if(ammoStore >= 1)
                    {
                        int reloadAmmo = MaxAmmo - CurrentAmmo;
                        if (reloadAmmo <= reloadAmmount)
                        {
                            if (ammoStore < reloadAmmo)
                            {
                                StartCoroutine(Reload(ammoStore));
                                ammoStore = 0;
                            }
                            else if (reloadAmmo <= ammoStore)
                            {
                                StartCoroutine(Reload(reloadAmmo));
                                ammoStore -= reloadAmmo;
                            }
                        }
                        else if (reloadAmmo > reloadAmmount)
                        {
                            StartCoroutine(Reload(reloadAmmount));
                            ammoStore -= reloadAmmount;
                        }
                        else
                        {
                            Debug.LogError("Ammo Error");
                        }
                    }
                    else
                    {
                        if (CurrentAmmo >= MaxAmmo || ammoStore <= 0)
                        {
                            Debug.Log("Ammo reload finished");
                            hasStartedReloading = false;
                            animator.SetBool("hasStartedReloading", false);
                        }
                        else
                        {
                            Debug.LogError("Ammo Error");
                        }
                    }
                }
            }

        }



        //Shoot on input
        if (CurrentAmmo >= 1)
        {
            if (isAutomatic)
            {
                if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f/fireRate;
                    Shoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f/fireRate;
                    Shoot();
                }
            }
        }
    }



    //ShootSound function
    void Shoot()
    {
        if (hasStartedReloading)
        {
            hasStartedReloading = false;
        }

        muzzleFlash.Play();
        animator.SetTrigger("shoot");
        ShootSound.Play();

        CurrentAmmo --;


        RaycastHit hit;
        foreach(int value in Enumerable.Range(0, bulletsPerShot))
        {
            if (Physics.Raycast(fpscam.transform.position,fpscam.transform.forward + new Vector3(0, Random.Range(-bulletSpread, bulletSpread), Random.Range(-bulletSpread, bulletSpread)), out hit, range))
            {

                NpcHealth NpcHealth = hit.transform.GetComponent<NpcHealth>();
                if (NpcHealth != null)
                {
                    NpcHealth.TakeDamage(damage);
                    GameObject bloodGO = Instantiate(NpcHealthImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(bloodGO, 2f);
                }
                if (NpcHealth == null)
                {
                    GameObject ImpactGO = Instantiate(surfaceImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(ImpactGO, 2f);
                }
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * ImpactForce);
                }
            }
        }
    }


    //ReloadSound Function
    IEnumerator Reload (int Ammount)
    {
        isReloading = true;
        ReloadSound.Play();
        Debug.Log(ammoStore);
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime);
        animator.SetBool("Reloading", false);
        CurrentAmmo += Ammount;

        if (ammoStore <= -1)
        {
            ammoStore = 0;
        }
        isReloading = false;
    }


    public void AmmoPickup()
    {
        ammoStore += AmmoPickupAmount;
    }
}
