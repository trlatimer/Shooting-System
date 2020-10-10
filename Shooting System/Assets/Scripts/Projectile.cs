using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Tooltip("Determines how many iterations occur per frame, higher value improves accuracy of trajectory")]
    [SerializeField] int stepsPerFrame = 6;
    [Tooltip("How fast the projectile will travel. Higher value increases distance it will travel before dropping noticably")]
    [SerializeField] float bulletSpeed = 420f;
    [Tooltip("Distance before a system destroys the object if it hasn't collided with anything")]
    [SerializeField] float maxShotDistance = 100f;
    [Tooltip("Vector to determine wind direction and amount of effect on projectile")]
    [SerializeField] Vector3 windEffect;

    Vector3 bulletVelocity;
    float distanceTravelled = 0f;

    // Start is called before the first frame update
    void Start()
    {
        bulletVelocity = this.transform.forward * bulletSpeed;
    }

    // Update is called once per frame
    void Update()
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

    public RaycastHit FireRound()
    {
        return new RaycastHit();
    }
}
