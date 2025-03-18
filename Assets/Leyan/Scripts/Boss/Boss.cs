using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] private float maxHP;
    private float currentHP;
    private int displayedHP => (int)currentHP;

    public BossCountDown bossCountDown;
    [SerializeField] private Image HPBar;

    private float displayFillAmount;
    [SerializeField] private float fillSmoothSpeed = 2f;
    [SerializeField] private float extraDamageRate = 1f; 

    public void DealDamage(float damage)
    {
        currentHP = Mathf.Max(0, currentHP - damage* extraDamageRate); 
    }

    private void Update()
    {
        UpdateFillBar();
    }
    private void UpdateFillBar()
    {
        displayFillAmount = Mathf.Lerp(displayFillAmount, currentHP / maxHP, Time.deltaTime * fillSmoothSpeed);
        HPBar.fillAmount = displayFillAmount;


    }
    private void Start()
    {
        currentHP = maxHP;
    }

    public void MultiplaDamageRate(float rate)
    {
        extraDamageRate *= rate;
    }

}
