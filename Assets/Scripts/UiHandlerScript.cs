using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[ExecuteInEditMode]
public class UiHandlerScript : MonoBehaviour
{



    public UnityEngine.UI.Image HealthBarPercent;
    [Range(0.0f, 1.0f)]
    public float percent;

    public CanvasGroup ReticleGroup;
    public Image Reticle;


    // Start is called before the first frame update
    void Start()
    {
        ResetReticle();
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarPercent.fillAmount = percent;
        Color barColor = new Color(1 - percent, percent, 0);
        HealthBarPercent.color = barColor;
    }

    public void AimReticle(Vector3 position){
        // Reticle.transform.position = position;
        Reticle.transform.DOMove(position,1);
    }

    public void ResetReticle(){
        ReticleGroup.alpha = 0;
        Reticle.transform.position = Vector3.zero;
    }

}
