using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossCountDown : MonoBehaviour
{
    public float timer;
    public Text countDownText;
    public GameObject ReStartBtn;

    public void Update()
    {
        timer -= Time.deltaTime;
        countDownText.text = timer.ToString("0");
        if (timer <= 0)
        {
            Time.timeScale = 0;
            ReStartBtn.SetActive(true);
        }
    }

    public void ReStart()
    {
        timer = 30;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
