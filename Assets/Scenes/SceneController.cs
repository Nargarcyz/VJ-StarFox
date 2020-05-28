using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Scene manager active");   
    }

    void OnTriggerEnter(Collider other){
        Debug.Log("END VOLUME: "+other.tag);
        if (other.tag == "Player")
        {
            string name = SceneManager.GetActiveScene().name;
            bool completed = false;
            if (name == "Escenario1")
            {
                GameStatus.Instance.CompletedLevel(0);
                Debug.Log("Escenario1 set as completed");
                completed = true;
            } else if (name == "Escenario2"){
                GameStatus.Instance.CompletedLevel(1);
                Debug.Log("Escenario2 set as completed");
                completed = true;
            }

            if (completed)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }


}
