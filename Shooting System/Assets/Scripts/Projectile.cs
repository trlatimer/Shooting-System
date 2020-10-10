using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("Determines how many iterations occur per frame, higher value improves accuracy of trajectory")]
    [SerializeField] int stepsPerFrame = 6;

    float bulletSpeed = 420f;
    float maxShotDistance = 100f;
    Vector3 windEffect;
    Vector3 bulletVelocity;
    float distanceTravelled = 0f;

    public void SetInitialValues(float speed, float range, Vector3 wind)
    {
        bulletSpeed = speed;
        maxShotDistance = range;
        windEffect = wind;
        bulletVelocity = this.transform.forward * bulletSpeed;
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
            bulletVelocity += Physics.gravity * stepSize * Time.deltaTime; // Gravity
            bulletVelocity += windEffect * stepSize * Time.deltaTime; // Wind
            Vector3 point2 = point1 + bulletVelocity * stepSize * Time.deltaTime;
            Ray ray = new Ray(point1, point2 - point1);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, (point2 - point1).magnitude) || point2.y <= 0 || distanceTravelled >= maxShotDistance)
            {
                if (point2.y <= 0)
                {
                    Debug.Log("Bullet fell below zero");
                }
                else if (distanceTravelled >= maxShotDistance)
                {
                    Debug.Log("Bullet hit max distance");
                }
                else
                {
                    Debug.Log($"Hit {hit.transform.name} at " + Vector3.Distance(FindObjectOfType<Camera>().transform.position, hit.transform.position) + " meters");
                    // TODO Inform player that something was hit  
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
