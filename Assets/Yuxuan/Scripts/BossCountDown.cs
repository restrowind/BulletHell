using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossCountDown : MonoBehaviour
{
    private float timer;
    public Text countDownText;
    public GameObject ReStartBtn;
    private BattleState _currentState;
    public int roundCount = 0; // 记录回合数

    [SerializeField] private float baseBulletStateTimeLength = 30f;
    [SerializeField] private float lengthExtraRate = 1f;
    private float currentBulletStateTimeLength;

    private void Start()
    {
        UpdateLength();
        timer = currentBulletStateTimeLength;
        countDownText.text = timer.ToString("0");
    }
    public void Update()
    {
        if (BattleManager.Instance._currentState != BattleState.BulletPhase) return;

        timer -= Time.deltaTime;
        countDownText.text = timer.ToString("0");
        if (timer <= 0)
        {
            BattleManager.Instance.MoveToNextState();
            timer = currentBulletStateTimeLength;
        }
    }

    public void ReStart()
    {
        timer = currentBulletStateTimeLength;
        Time.timeScale = 1;
        roundCount++; // 每次重新开始增加回合数
    }

    public void SetLengthReduceRate(float newRate)
    {
        lengthExtraRate=newRate;
        UpdateLength();
    }

    public void MultipleLengthReduceRate(float rate)
    {
        lengthExtraRate *= rate;
        UpdateLength();
    }
    private void UpdateLength()
    {
        currentBulletStateTimeLength = lengthExtraRate * baseBulletStateTimeLength;
    }

}