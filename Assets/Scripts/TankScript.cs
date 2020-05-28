using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TankScript : MonoBehaviour
{

    public Transform CannonBase;
    public Transform Cannon;
    public Transform missileExit;

    [Space]
    public GameObject projectile = null;
    [Space]

    public float projectileSpeed = 100;
    public float shotDelay = 2000f;
    private float lastShotTime = 0;

    private GameObject focusedEntity = null;
    // private Vector3 lastKnownLocation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if(projectile != null)
        {
            LaserBulletScript lbs = projectile.GetComponent<LaserBulletScript>();
            if (lbs != null)
            {
                lbs.setSpeed(projectileSpeed);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Cannon != null && CannonBase != null)
        {
            if (focusedEntity != null)
            {
                if (RaycastCheckIsPlayer(focusedEntity) != "Player") return;

                Vector3 lastKnownLocationBase = focusedEntity.transform.position - CannonBase.position;// + playerVelocityVector * travelTime;
                Vector3 lastKnownLocationCannon = focusedEntity.transform.position - Cannon.position;// + playerVelocityVector * travelTime;

                Quaternion locationRotationBase = Quaternion.LookRotation(Vector3.Normalize(lastKnownLocationBase));
                Quaternion locationRotationCannon = Quaternion.LookRotation(Vector3.Normalize(lastKnownLocationCannon));

                if (CannonBase.rotation != locationRotationBase)
                {
                    Quaternion rot = Quaternion.RotateTowards(CannonBase.rotation, locationRotationBase, 200 * Time.deltaTime);
                    // CannonBase.rotation = new Quaternion(0,0,rot.z,0);
                    CannonBase.rotation = rot;
                    // CannonBase.Rotate(Vector3.up, -90);
                }

                if (Cannon.rotation != locationRotationCannon)
                {
                    Quaternion rot = Quaternion.RotateTowards(Cannon.rotation, locationRotationCannon*Quaternion.Euler(-90,0,0), 100 * Time.deltaTime);
                    // CannonBase.rotation = new Quaternion(0,0,rot.z,0);
                    
                    Cannon.rotation = rot;
                    // Cannon.Rotate(new Vector3(180,0,0));
                    // CannonBase.Rotate(Vector3.up, -90);
                }

                
                if(Time.time > lastShotTime){
                    GameObject missile = Instantiate(projectile);
                    missile.GetComponent<HomingMissileScript>().setTarget(focusedEntity);
                    missile.transform.position = missileExit.position;
                    Destroy(missile, 10);
                    lastShotTime = Time.time + shotDelay;
				}


            }   
        }
    }

    string RaycastCheckIsPlayer(GameObject player){

        LayerMask mask = (1<<9);
        mask |= (1<<10);
        RaycastHit hit;
        string returnTag = "null";
        if (Physics.Raycast(Cannon.position, Vector3.Normalize(player.transform.position - Cannon.position), out hit, 1000f,mask))
        {
            Debug.DrawRay(Cannon.position, Vector3.Normalize(player.transform.position - Cannon.position)*Vector3.Distance(Cannon.position,hit.transform.position),Color.red,0.0f);
            if(hit.transform.tag == "Player"){
                returnTag =  "Player";
            }
        } 
        return returnTag;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            // Debug.Log("Player Detected Entering at" + other.transform.position);

            if (RaycastCheckIsPlayer(other.gameObject) == "Player")
            {
                focusedEntity = other.gameObject;
            }
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
