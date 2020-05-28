using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HealthManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxHp = 100;
    private float currentHp;
    public GameObject explosionEffect;

    void Start()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHp <= 0)
        {
            GameObject explosion = Instantiate(explosionEffect);
            explosion.transform.position = transform.position;
            explosion.GetComponent<VisualEffect>().Play();
            Destroy(this.gameObject);
        }
    }

    public void DealDamage(int damage){
        currentHp -= damage;
    }
}
