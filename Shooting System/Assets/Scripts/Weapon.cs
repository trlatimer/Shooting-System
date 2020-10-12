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
    [SerializeField] public float bulletSpeed = 420f;
    [Tooltip("Distance before a system destroys the object if it hasn't collided with anything")]
    [SerializeField] public float maxShotDistance = 100f;
    [Tooltip("Vector to determine wind direction and amount of effect on projectile")]
    [SerializeField] public Vector3 windEffect;
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
        DrawPrediction();
        ProcessPlayerInput();
    }

    private void ProcessPlayerInput()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && canShoot)
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
        bullet.SetInitialValues(FPCamera, bulletSpeed, maxShotDistance, windEffect);
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

    private void DrawPrediction()
    {
        Vector3 point1 = FPCamera.transform.position;
        LineRenderer predictionLine = GetComponentInChildren<LineRenderer>();
        List<Vector3> tempPoints = new List<Vector3>();
        float tempDistanceTravelled = 0f;
        Vector3 bulletVelocity = FPCamera.transform.forward * bulletSpeed;
        float stepSize = 1.0f / 6;

        while (tempDistanceTravelled <= maxShotDistance)
        {
            for (float step = 0; step < 1; step += stepSize)
            {
                bulletVelocity += Physics.gravity * stepSize * Time.deltaTime; // Gravity
                bulletVelocity += windEffect * stepSize * Time.deltaTime; // Wind
                Vector3 point2 = point1 + bulletVelocity * stepSize * Time.deltaTime;
                tempPoints.Add(point2);
                //if (Physics.Raycast(ray, out hit, (point2 - point1).magnitude) || point2.y <= 0 || distanceTravelled >= maxShotDistance)
                tempDistanceTravelled += Vector3.Distance(point1, point2);
                point1 = point2;
            }
        }

        predictionLine.positionCount = tempPoints.Count;
        predictionLine.SetPositions(tempPoints.ToArray());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 point1 = FPCamera.transform.position;
        float tempDistanceTravelled = 0f;
        float stepSize = 1.0f / 6;
        Vector3 bulletVelocity = FPCamera.transform.forward * bulletSpeed;

        while (tempDistanceTravelled <= maxShotDistance)
        {
            for (float step = 0; step < 1; step += stepSize)
            {
                bulletVelocity += Physics.gravity * stepSize; // Gravity
                bulletVelocity += windEffect * stepSize; // Wind
                Vector3 point2 = point1 + bulletVelocity * stepSize;
                Gizmos.DrawLine(point1, point2);

                //if (Physics.Raycast(ray, out hit, (point2 - point1).magnitude) || point2.y <= 0 || distanceTravelled >= maxShotDistance)

                tempDistanceTravelled += Vector3.Distance(point1, point2);
                point1 = point2;
            }
        }
    }



}
