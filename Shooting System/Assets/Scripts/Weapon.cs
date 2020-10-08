using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera FPCamera;
    [Tooltip("Represents rounds per minute (e.g. 60 would mean you can shoot once every second)")] 
    [SerializeField] float fireRate = 20f;

    bool canShoot = true;

    private void OnEnable()
    {
        canShoot = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            StartCoroutine(Shoot());
        } 

        if (Input.GetMouseButtonDown(1))
        {
            GetComponent<Animator>().SetBool("isAiming", true);
        } else if (Input.GetMouseButtonUp(1))
        {
            GetComponent<Animator>().SetBool("isAiming", false);
        }
    }

    private IEnumerator Shoot()
    {
        canShoot = false;
        Debug.Log("Shot!");
        // play audio
        // play muzzle flash
        // process parabolic raycast with gravity and wind drop
        yield return new WaitForSeconds(60 / fireRate);
        canShoot = true;
    }
}
