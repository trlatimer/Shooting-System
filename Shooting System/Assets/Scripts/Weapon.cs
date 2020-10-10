using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera FPCamera;
    [SerializeField] RigidbodyFirstPersonController fpsController;
    [Tooltip("Represents rounds per minute (e.g. 60 would mean you can shoot once every second)")] 
    [SerializeField] float fireRate = 20f;
    [Tooltip("How fast the projectile will travel. Higher value increases distance it will travel before dropping noticably")]
    [SerializeField] float bulletSpeed = 420f;
    [Tooltip("Distance before a system destroys the object if it hasn't collided with anything")]
    [SerializeField] float maxShotDistance = 100f;
    [Tooltip("Vector to determine wind direction and amount of effect on projectile")]
    [SerializeField] Vector3 windEffect;
    [SerializeField] float zoomIn = 30f;
    [SerializeField] float zoomOut = 60f;
    [SerializeField] float zoomOutSensitivity = 2f;
    [SerializeField] float zoomInSensitivity = .5f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] Projectile bullets;

    AudioSource gunshotSource;

    bool canShoot = true;

    private void OnEnable()
    {
        canShoot = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        gunshotSource = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessPlayerInput();

    }

    private void ProcessPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            StartCoroutine(Shoot());
        }

        if (Input.GetMouseButtonDown(1))
        {
            GetComponent<Animator>().SetBool("isAiming", true);
            ZoomIn();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            GetComponent<Animator>().SetBool("isAiming", false);
            ZoomOut();
        }
    }

    private IEnumerator Shoot()
    {
        canShoot = false;
        gunshotSource.Play();
        muzzleFlash.Play();
        Projectile bullet = Instantiate(bullets, FPCamera.transform.position, Quaternion.LookRotation(FPCamera.transform.forward));
        bullet.SetInitialValues(bulletSpeed, maxShotDistance, windEffect);
        yield return new WaitForSeconds(60 / fireRate);
        canShoot = true;
    }

    private void ZoomIn()
    {
        FPCamera.fieldOfView = zoomIn;
        fpsController.mouseLook.XSensitivity = zoomInSensitivity;
        fpsController.mouseLook.YSensitivity = zoomInSensitivity;
    }

    private void ZoomOut()
    {
        FPCamera.fieldOfView = zoomOut;
        fpsController.mouseLook.XSensitivity = zoomOutSensitivity;
        fpsController.mouseLook.YSensitivity = zoomOutSensitivity;
    }

}
