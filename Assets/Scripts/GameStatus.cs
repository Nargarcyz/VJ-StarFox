using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : MonoBehaviour
{

    public static GameStatus Instance;
    public bool Level1Completed = false;
    public bool Level2Completed = false;
    // Start is called before the first frame update
    void Awake(){
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }
    void Update(){
        Debug.Log("Level status: Level 1 = " + Level1Completed + " Level 2 = " + Level2Completed);
    }

    public void CompletedLevel(int levelId){
        switch (levelId)
        {
            case 0:
                Level1Completed = true;
                break;
            case 1:
                Level2Completed = true;
            break;
            default:
            break;
        }
        
    }
}
