using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    // Start is called before the first frame update

    private float rotationSpeed = 200;
    public float projectileSpeed = 100;
    public float shotDelay = 0.1f;
    private float lastShotTime = 0;


    [Space]
    public GameObject projectile = null;
    [Space]


    public Transform turretHeadRotor = null;
    public Transform restPosition = null;

    public Transform leftBlaster;
    public Transform rightBlaster;

    private GameObject focusedEntity = null;
    private Vector3 lastKnownLocation = Vector3.zero;
    void Start()
    {
        if(projectile != null)
        {
            projectile.GetComponent<LaserBulletScript>().setSpeed(projectileSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (turretHeadRotor != null)
        {
            

            if (focusedEntity != null)
            {
                float playerDistance = Vector3.Distance(turretHeadRotor.position, focusedEntity.transform.position);
                float travelTime = playerDistance / (projectileSpeed * Time.fixedDeltaTime);

                Vector3 playerVelocity = focusedEntity.GetComponent<PlayerMovement>().velocity;
                Vector3 playerVelocityVector = focusedEntity.GetComponent<PlayerMovement>().velocityVector;

                lastKnownLocation = focusedEntity.transform.position - turretHeadRotor.position + playerVelocityVector * travelTime;



                Debug.Log("Velocity: " + playerVelocityVector);



                //lastKnownLocation = focusedEntity.transform.position + focusedEntity.GetComponent<Rigidbody>().velocity;


                //lastKnownLocation = focusedEntity.transform.position - turretHeadRotor.position;          
                
            } else
            {
                if (restPosition == null)
                {
                    lastKnownLocation = Vector3.zero;
                } else
                {
                    lastKnownLocation = restPosition.position - turretHeadRotor.position;
                }
            }

            Quaternion locationRotation = Quaternion.LookRotation(lastKnownLocation);

            if (transform.rotation != locationRotation)
            {
                turretHeadRotor.rotation = Quaternion.RotateTowards(turretHeadRotor.rotation, locationRotation, rotationSpeed * Time.deltaTime);
            }

            if(focusedEntity != null)
            {
                if (Time.time - lastShotTime >= shotDelay)
                {
                    ShootLaser(leftBlaster);
                    ShootLaser(rightBlaster);
                    lastShotTime = Time.time;
                }
            }

        }
    }

    void ShootLaser(Transform src)
    {
        GameObject laser = Instantiate(projectile);

        
        laser.transform.position = src.position;
        laser.transform.rotation = src.rotation;
        Destroy(laser, 3);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            //Debug.Log("Player Detected Entering at" + other.transform.position);
            focusedEntity = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.gameObject == focusedEntity)
        {
            //Debug.Log("Player Detected Leaving at" + other.transform.position);
            focusedEntity = null;
        }
    }
}

