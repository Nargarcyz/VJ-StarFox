using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[ExecuteInEditMode]
public class UiHandlerScript : MonoBehaviour
{

    public Image HealthBarPercent;
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
        HealthBarPercent.color = new Color(1-percent,percent,0);
    }
}
