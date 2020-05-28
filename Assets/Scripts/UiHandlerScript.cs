using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

[ExecuteInEditMode]
public class UiHandlerScript : MonoBehaviour
{



    public UnityEngine.UI.Image HealthBarPercent;
    [Range(0.0f, 1.0f)]
    public float percent;

    public CanvasGroup ReticleGroup;
    public Image Reticle;

    [Space]
    public CanvasGroup pauseMenu;
    [Space]
    public CanvasGroup DestroyedPanel;
    
    private bool gamePaused = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetReticle();
    }

    // Update is called once per frame
    void Update()
    {
        // HealthBarPercent.fillAmount = percent;
        Color barColor = new Color(1 - HealthBarPercent.fillAmount, HealthBarPercent.fillAmount, 0);
        HealthBarPercent.color = barColor;

        if (Input.GetButtonDown("Cancel"))
        {
            if (gamePaused)
            {
                Time.timeScale = 1;
                pauseMenu.alpha = 0;
                pauseMenu.blocksRaycasts = false;
            } else {
                Time.timeScale = 0;
                pauseMenu.alpha = 1;
                pauseMenu.blocksRaycasts = true;
            }
            gamePaused = !gamePaused;
        }

    }

    public void AimReticle(Vector3 position){
        // Reticle.transform.position = position;
        Reticle.transform.DOMove(position,1);
    }

    public void ResetReticle(){
        ReticleGroup.alpha = 0;
        Reticle.transform.position = Vector3.zero;
    }

    public void UpdateHealth(float percent){
        HealthBarPercent.fillAmount = percent;
    }

    public void PlayerDestroyed(){
        DestroyedPanel.alpha = 1;
        DestroyedPanel.interactable = true;
        DestroyedPanel.blocksRaycasts = true;
     
    }
    public void ReloadLevel(){
        DestroyedPanel.alpha = 0;
        DestroyedPanel.interactable = false;
        DestroyedPanel.blocksRaycasts = false;
        pauseMenu.alpha = 0;
        pauseMenu.blocksRaycasts = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);   
    }
    public void GoToMenu(){
        DestroyedPanel.alpha = 0;
        DestroyedPanel.interactable = false;
        DestroyedPanel.blocksRaycasts = false;
        pauseMenu.alpha = 0;
        pauseMenu.blocksRaycasts = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

}
