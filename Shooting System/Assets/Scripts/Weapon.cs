﻿using System;
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
    [SerializeField] Vector3 windEffect = new Vector3();
    [SerializeField] float gravityMultiplier = 0.5f;
    [SerializeField] Color predictionColor = Color.yellow;
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private IEnumerator Shoot()
    {
        canShoot = false;
        gunshotSource.Play();
        muzzleFlash.Play();
        Projectile bullet = Instantiate(bullets, FPCamera.transform.position, Quaternion.LookRotation(FPCamera.transform.forward));
        bullet.SetInitialValues(FPCamera, bulletSpeed, maxShotDistance, windEffect, gravityMultiplier);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = predictionColor;
        Vector3 point1 = FPCamera.transform.position;
        Vector3 point2 = new Vector3();
        Vector3 predictedBulletVelocity = FPCamera.transform.forward * bulletSpeed;
        float tempDistanceTravelled = 0f;

        // Add placeholder values for more accurate calculation for prediction compared to reality
        float stepSize = 1.0f / 6;
        float tempDeltaTime = 0.033f; // Can't use Time.deltaTime in editor as it crashes Unity

        // Iterate to simulate frames that would occur before hitting the max distance
        while (tempDistanceTravelled <= maxShotDistance)
        {
            // Iterate to simulate the amount of iterations that would occur per frame
            for (float step = 0; step < 1; step += stepSize)
            {
                predictedBulletVelocity += (Physics.gravity * gravityMultiplier) * stepSize * tempDeltaTime; // Gravity
                predictedBulletVelocity += windEffect * stepSize * tempDeltaTime; // Wind
                point2 = point1 + predictedBulletVelocity * stepSize * tempDeltaTime;
                Gizmos.DrawLine(point1, point2);
                tempDistanceTravelled += Vector3.Distance(point1, point2);
                point1 = point2;
            }
        }
    }



}
