using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossCountDown : MonoBehaviour, IBattlePhaseDependent
{
    public float timer;
    public Text countDownText;
    public GameObject ReStartBtn;
    private BattleState _currentState;
    public int roundCount = 0; // ��¼�غ���

    public void Update()
    {
        if (_currentState != BattleState.BulletPhase) return;

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
        ReStartBtn.SetActive(false);
        roundCount++; // ÿ�����¿�ʼ���ӻغ���
    }

    public void SetState(BattleState newState)
    {
        _currentState = newState;
    }
}