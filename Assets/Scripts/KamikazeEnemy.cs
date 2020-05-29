using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading;

public class KamikazeEnemy : MonoBehaviour
{

    public GameObject target;
    public Rigidbody rb;
    public float spaceshipSpeed = 20f;

    private int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    public void setSpeed(float speed)
    {
        spaceshipSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {

        float playerDistance = Vector3.Distance(rb.position, target.transform.GetChild(0).transform.position);

        if (count == 11) {
            transform.LookAt(target.transform.position);
            count = 0;
        }

        if (playerDistance < 150)
        {
            rb.position += transform.forward * spaceshipSpeed * Time.deltaTime;
        }
        ++count;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Solid")
        {
            if (other.tag == "Player" && other.gameObject.GetComponent<PlayerMovement>().isBarrelRolling())
            {
                return;
            }
            else if (other.tag == "Player")
            {
                other.gameObject.GetComponent<PlayerMovement>().DealDamage(15);
            }

            this.gameObject.GetComponent<HealthManager>().DealDamage(500);
        }

    }
}
