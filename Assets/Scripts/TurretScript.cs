using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TurretScript : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject focusedEntity = null;
    public Transform turretHeadRotor = null;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (focusedEntity != null && turretHeadRotor != null)
        {
            turretHeadRotor.rotation = Quaternion.LookRotation(focusedEntity.transform.position - turretHeadRotor.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            Debug.Log("Player Detected Entering at" + other.transform.position);
            focusedEntity = other.gameObject;
        }
    }

    void OnTriggerLeave(Collider other)
    {
        if (other.tag == "Player" && other.gameObject == focusedEntity)
        {
            Debug.Log("Player Detected Leaving at" + other.transform.position);
            focusedEntity = null;
        }
    }
}

