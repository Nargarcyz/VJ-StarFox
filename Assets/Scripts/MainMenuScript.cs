using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera camera;
    public GameObject focusedObject;
    public float distance;
    public GameObject loadingSpinner;
    public CanvasGroup levelSelector;
    public CanvasGroup aboutWindow;
    public CanvasGroup settingsWindow;

    public Button Level1Button;
    public Button Level2Button;

    void Start()
    {
        camera.transform.position = focusedObject.transform.position;
        camera.transform.LookAt(focusedObject.transform,camera.transform.up);
        camera.transform.position = camera.transform.position + new Vector3(0,3,distance);
        Level2Button.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        focusedObject.transform.position = focusedObject.transform.position + focusedObject.transform.forward * 2;
        camera.transform.position = camera.transform.position + focusedObject.transform.forward * 2;
        

        camera.transform.LookAt(focusedObject.transform,camera.transform.up);
        // camera.transform.RotateAround(focusedObject.transform.position, Vector3.Cross(focusedObject.transform.up,focusedObject.transform.rig), 5 * Time.deltaTime);
        camera.transform.RotateAround(focusedObject.transform.position, Vector3.up, 5 * Time.deltaTime);
        // camera.transform.Translate(Vector3.right * Time.deltaTime);
        
        
        if (GameStatus.Instance.Level1Completed)
        {
            Level1Button.GetComponent<Image>().color = Color.green;
            Level2Button.interactable = true;
        }
        if (GameStatus.Instance.Level2Completed)
        {
            Level2Button.GetComponent<Image>().color = Color.green;
        }


        if (levelLoadingOp != null && !levelLoadingOp.isDone)
        {
            if (loadingSpinner.GetComponent<CanvasGroup>().alpha == 1)
            {
                loadingSpinner.transform.Rotate(0,0,10);
            }
        }
    }


    public void ExposeLevels(){
        if (levelSelector.alpha == 1)
        {
            levelSelector.alpha = 0;
            levelSelector.blocksRaycasts = false;
        } else {
            levelSelector.alpha = 1;
            levelSelector.blocksRaycasts = true;
            
        }
    }

    AsyncOperation levelLoadingOp;
    public void LoadLevel (string level){
        StartCoroutine(AsyncLevelLoad(level));
        loadingSpinner.GetComponent<CanvasGroup>().alpha = 1;
        
    }

    IEnumerator AsyncLevelLoad(string level){
        levelLoadingOp = SceneManager.LoadSceneAsync(level);
        levelLoadingOp.allowSceneActivation = true;
        yield return levelLoadingOp;
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void ToggleSettingsWindow(){
        if (settingsWindow.alpha == 1)
        {
            settingsWindow.alpha = 0;
            settingsWindow.blocksRaycasts = false;
        } else {
            settingsWindow.alpha = 1;
            settingsWindow.blocksRaycasts = true;
        }

        aboutWindow.alpha = 0;
        aboutWindow.blocksRaycasts = false;
    }

    public void ToggleAboutWindow(){
        if (aboutWindow.alpha == 1)
        {
            aboutWindow.alpha = 0;
            aboutWindow.blocksRaycasts = false;
        } else {
            aboutWindow.alpha = 1;
            aboutWindow.blocksRaycasts = true;
        }

        settingsWindow.alpha = 0;
        settingsWindow.blocksRaycasts = false;
        

    }

    public void GodModeEnabledByDefault(bool gm){
        Debug.Log("GodMode enabled: " + gm);
        GameStatus.Instance.GodMode = gm;
    }

    public void AllLevelsUnlocked(bool bValue){
        if (bValue)
        {
            Level2Button.interactable = true;
        } else {
            Level2Button.interactable = false;
        }
    }
    
}
