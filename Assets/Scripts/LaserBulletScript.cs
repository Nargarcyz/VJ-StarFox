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
        
        if(other.tag == "Solid"){


            //VisualEffect sparks = Instantiate(sparkEffect);
            //RaycastHit hit;
            //if (Physics.Raycast(transform.position, transform.forward, out hit))
            //{
            //    Debug.Log("Point of contact: "+hit.point);
            //    Debug.Log(hit.normal);
            //    Debug.DrawLine(hit.point,hit.point+hit.normal*2,Color.red,4);
            //    Debug.DrawRay(transform.position, transform.forward, Color.green);
            //    sparks.transform.position = hit.point;
            //    sparks.transform.rotation = Quaternion.LookRotation(hit.normal);

            //}

            Destroy(this);
            Destroy(this.gameObject);

            
        }
    }
}
