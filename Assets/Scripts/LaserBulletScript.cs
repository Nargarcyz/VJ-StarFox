using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
using System.Collections.Generic;


public class LaserBulletScript : MonoBehaviour
{
    
    private Vector3 direction;
    private CapsuleCollider collider;
    public VisualEffect sparkEffect;
    [Header("Projectile Parameters")]
    public ParticleSystem explosionEffect;
    public float lifetime = 3;
    public float speed = 100;
    public bool friendly = true;

    public void setSpeed(float speed) {
        this.speed = speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        direction = new Vector3(0,0,1);
        collider = transform.GetComponent<CapsuleCollider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position += transform.forward * speed * Time.deltaTime;
        // transform.position += direction * speed * Time.deltaTime;
    }
    void OnTriggerEnter(Collider other){
        // 
        // Debug.Log(other.tag);
        if (friendly && (other.tag == "Solid" || other.tag == "Enemy"))
        {

                


                
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    VisualEffect sparks = Instantiate(sparkEffect);
                    //    Debug.Log("Point of contact: "+hit.point);
                    //    Debug.Log(hit.normal);
                    Debug.DrawLine(hit.point,hit.point+hit.normal*2,Color.red,4);
                    Debug.DrawRay(transform.position, transform.forward, Color.green);
                    sparks.transform.position = hit.point;
                    sparks.transform.rotation = Quaternion.LookRotation(hit.normal);
                    Destroy(sparks,2);

                }

                if (other.tag == "Enemy")
                {
                    other.gameObject.GetComponent<HealthManager>().DealDamage(10);
                }

                Destroy(this.gameObject);
                

         } 
        else if( !friendly && (other.tag == "Solid" || other.tag == "Player")){
            
                
                
                

                if (other.tag == "Player" && !(other.gameObject.GetComponent<PlayerMovement>().isBarrelRolling()))
                {
                    other.gameObject.GetComponent<PlayerMovement>().DealDamage(1);
                } else if (other.tag == "Solid")
                {
                    
                } else {
                    return;
                }

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    VisualEffect sparks = Instantiate(sparkEffect);
                    //    Debug.Log("Point of contact: "+hit.point);
                    //    Debug.Log(hit.normal);
                    Debug.DrawLine(hit.point,hit.point+hit.normal*2,Color.red,4);
                    Debug.DrawRay(transform.position, transform.forward, Color.green);
                    sparks.transform.position = hit.point;
                    sparks.transform.rotation = Quaternion.LookRotation(hit.normal);
                    Destroy(sparks,2);

                }


                // Destroy(this);
                Destroy(this.gameObject);

        }
        
    }
}
