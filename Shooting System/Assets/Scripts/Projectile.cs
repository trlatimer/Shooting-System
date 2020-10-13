using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("Determines how many iterations occur per frame, higher value improves accuracy of trajectory")]
    [SerializeField] int stepsPerFrame = 6;
    [SerializeField] float drag = 0.95f;

    float bulletSpeed = 420f;
    float maxShotDistance = 100f;
    Vector3 windEffect;
    Vector3 bulletVelocity;
    float gravityMultiplier = 0.5f;
    float distanceTravelled = 0f;
    HitLog hitLog;
    Camera mainCamera;

    private void Start()
    {
        hitLog = FindObjectOfType<HitLog>();
    }

    public void SetInitialValues(Camera camera, float speed, float range, Vector3 wind, float gravityMult)
    {
        mainCamera = camera;
        bulletSpeed = speed;
        maxShotDistance = range;
        windEffect = wind;
        bulletVelocity = this.transform.forward * bulletSpeed;
        gravityMultiplier = gravityMult;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessParabolicRayCast();
    }

    private void ProcessParabolicRayCast()
    {
        Vector3 point1 = this.transform.position;

        float stepSize = 1.0f / stepsPerFrame;
        for (float step = 0; step < 1; step += stepSize)
        {
            bulletVelocity += (Physics.gravity * gravityMultiplier)* stepSize * Time.deltaTime; // Gravity
            bulletVelocity += windEffect * stepSize * Time.deltaTime; // Wind
            Vector3 point2 = point1 + bulletVelocity * stepSize * Time.deltaTime;
            Ray ray = new Ray(point1, point2 - point1);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, (point2 - point1).magnitude) || point2.y <= 0 || distanceTravelled >= maxShotDistance)
            {
                if (point2.y <= 0)
                {
                    hitLog.AppendMessage("Bullet fell below zero");
                }
                else if (distanceTravelled >= maxShotDistance)
                {
                    hitLog.AppendMessage("Bullet hit max distance");
                }
                else
                {
                    hitLog.AppendMessage($"Hit {hit.transform.name} at " + Vector3.Distance(FindObjectOfType<Camera>().transform.position, hit.transform.position) + " meters");
                }
                Destroy(gameObject);
                return;
            }
            distanceTravelled += Vector3.Distance(point1, point2);
            point1 = point2;
        }

        this.transform.position = point1;
    }
}
