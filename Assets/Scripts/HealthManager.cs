using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxHp = 100;
    private float currentHp;

    void Start()
    {
        currentHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void DealDamage(int damage){
        currentHp -= damage;
    }
}
