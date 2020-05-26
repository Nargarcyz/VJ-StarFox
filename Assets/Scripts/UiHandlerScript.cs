using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UiHandlerScript : MonoBehaviour
{



    public UnityEngine.UI.Image HealthBarPercent;
    [Range(0.0f, 1.0f)]
    public float percent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HealthBarPercent.fillAmount = percent;
        Color barColor = new Color(1 - percent, percent, 0);
        HealthBarPercent.color = barColor;
    }
}
