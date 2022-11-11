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
    private int CurrentAmmo;
    private int ammoStore;
    public KeyCode ReloadKey = KeyCode.R;
    private bool isReloading = false;
    public Text AmmoText;
    public int StartAmmo = 10;

    [Header("Recoil")]
    //hitfire Recoil
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;




    [Header("Mele attack")]
    public float MeleDamage = 10f;
    public float MeleRange = 3f;
    public float MeleCooldown = 1f;
    public KeyCode meleKey = KeyCode.Mouse1;
    private float timeBeforeNextHit = 0f;





    void Start()
    {
        //ammo presets
        CurrentAmmo = StartAmmo;
    }



    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }


    void Update()
    {

        AmmoText.text = CurrentAmmo + "/" + ammoStore;
        if (timeBeforeNextHit >= 0f)
        {
            timeBeforeNextHit -= Time.deltaTime;
        }


        //Mele hit
        if (Input.GetKeyDown(meleKey) && timeBeforeNextHit <= 0f)
        {
            Mele();
        }

        if (isReloading && ammoStore != 0)
        {
            return;
        }


        //Reload
        if(Time.time >= nextTimeToFire && !isReloading)
        {
            if (CurrentAmmo <= 0 || Input.GetKey(ReloadKey))
            {
                if(CurrentAmmo >= MaxAmmo)
                {
                    return;
                }
                else
                {
                    
                    if(ammoStore >= 1)
                    {
                        int reloadAmmo = MaxAmmo - CurrentAmmo;
                        if (ammoStore < reloadAmmo)
                        {
                            StartCoroutine(Reload(ammoStore));
                            ammoStore = 0;
                        }
                        else if(reloadAmmo <= ammoStore)
                        {
                                StartCoroutine(Reload(reloadAmmo));
                                ammoStore -= reloadAmmo;
                        }
                        else
                        {
                            Debug.LogError("Ammo Error");
                        }
                    }
                    else
                    {
                        return;
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




    void Mele()
    {
        animator.SetTrigger("mele");
        MeleSound.Play();

        RaycastHit MeleHit;
        if (timeBeforeNextHit <= 0f)
        {
            if(Physics.Raycast(fpscam.transform.position,fpscam.transform.forward , out MeleHit, MeleRange))
            {
                    NpcHealth NpcHealth = MeleHit.transform.GetComponent<NpcHealth>();
                    if (NpcHealth != null)
                    {
                        NpcHealth.TakeDamage(MeleDamage);
                        GameObject bloodGO = Instantiate(NpcHealthImpactEffect, MeleHit.point, Quaternion.LookRotation(MeleHit.normal));
                        Destroy(bloodGO, 2f);
                        timeBeforeNextHit = MeleCooldown;
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
