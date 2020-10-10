using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int stepsPerFrame = 6;
    [SerializeField] float bulletSpeed = 420f;
    [SerializeField] float maxShotDistance = 100f;
    [SerializeField] Vector3 windEffect;

    Vector3 bulletVelocity; // Test more to learn about mechancis regarding, tests had this serialized and related to distance travelled

    // Start is called before the first frame update
    void Start()
    {
        bulletVelocity = this.transform.forward * bulletSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 point1 = this.transform.position;

        float distanceTravelled = 0f;
        float stepSize = 1.0f / stepsPerFrame;
        for (float step = 0; step < 1; step += stepSize)
        {
            bulletVelocity += Physics.gravity * stepSize * Time.deltaTime; // Gravity
            //bulletVelocity += windEffect * stepSize * Time.deltaTime; // Wind
            Vector3 point2 = point1 + bulletVelocity * stepSize * Time.deltaTime;
            Ray ray = new Ray(point1, point2 - point1);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, (point2 - point1).magnitude) || point2.y <= 0 || distanceTravelled >= maxShotDistance)
            {
                Debug.Log($"Hit at {hit.transform.name}" + distanceTravelled + " meters");
                // TODO Inform player that something was hit
                Destroy(gameObject);
                return;
            }

            distanceTravelled = (point2 - point1).magnitude;
            point1 = point2;
        }

        this.transform.position = point1;
    }

    public RaycastHit FireRound()
    {
        return new RaycastHit();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 point1 = this.transform.position;
        Vector3 predictedBulletVelocity = bulletVelocity;
        float stepSize = 0.01f;
        for (float step = 0; step < 1; step += stepSize)
        {
            predictedBulletVelocity += Physics.gravity * stepSize;
            predictedBulletVelocity += windEffect * stepSize;
            Vector3 point2 = point1 + predictedBulletVelocity * stepSize;
            Gizmos.DrawLine(point1, point2);
            point1 = point2;
        }
    }
}
