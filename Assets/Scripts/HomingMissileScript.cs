using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HomingMissileScript : MonoBehaviour
{
    public GameObject target;
    public Rigidbody rb;
    public float missileSpeed = 100f;
    public bool friendly = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        
    }

    public void setTarget(GameObject target){
        this.target = target;
    }

    public void setSpeed(float speed){
        missileSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.forward * missileSpeed * Time.deltaTime;
        if (target != null)
        {
            
            if (!friendly && target.GetComponent<PlayerMovement>().isBarrelRolling())
            {
                target = null;
                return;
            } else {
                transform.LookAt(target.transform.position);
            }

        }

    }

    void OnTriggerEnter(Collider other){
        if (friendly)
        {
            if (other.tag == "Enemy" || other.tag == "Solid")
            {
                if (other.tag == "Enemy")
                {
                    other.gameObject.GetComponent<HealthManager>().DealDamage(25);
                }
                Destroy(this.gameObject);
            }
        } else {
            if (other.tag == "Player" || other.tag == "Solid")
            {
                if (other.tag == "Player" && other.gameObject.GetComponent<PlayerMovement>().isBarrelRolling())
                {
                    return;
                } else if (other.tag == "Player"){
                    other.gameObject.GetComponent<PlayerMovement>().DealDamage(5);
                }
                Debug.Log("Hit :" + other.tag);

                Destroy(this.gameObject);
            }
        }
        

    }
}
