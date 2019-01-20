using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    private float newFillAmount;
    protected Image healthBar;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, newFillAmount, Time.deltaTime);
    }

    public void SetFillAmount(float healthAsPercentage)
    {
        newFillAmount = healthAsPercentage;
    }
    
    public void SetHealthBar(Image newHealthBar)
    {
        healthBar = newHealthBar;
    }
}   
