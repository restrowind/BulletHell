using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class PlayerCharacter : MonoBehaviour
{
    // 移动参数
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    // 残影参数
    public GameObject ghostPrefab;
    public float ghostInterval = 0.05f;
    public Color ghostColor = new Color(1, 1, 1, 0.5f);
    public int ghostPoolSize = 10; // 预创建的残影对象数量

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDashing;
    private bool canDash = true;
    private SpriteRenderer playerSprite;
    public float timer;
    public Image img;


    private float currentHP;
    [SerializeField] float maxHP;
    [SerializeField] private float takeDamageRate = 1f;
    private float invincibleTime = 0.5f;
    private float extraInvincibleTimeRate = 1f;
    private float invincibleTimer;
    private bool invincible = false;

    private float giveBackDamageRate = 0f;

    public float damageTakeThisTurn = 0f;

   

    private void HandleInvincibleTimer()
    {
        if (invincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                invincible = false;
            }
        }
    }
    public void MultiplaeInvincibleTime(float rate)
    {
        extraInvincibleTimeRate*=rate;
    }



    public Text hpText;
    private bool _isStay;
    private GameObject currentTrigger;

    private Animator player_animator;

    // 对象池
    private Queue<GameObject> ghostPool = new Queue<GameObject>();

    // 阶段切换
    private BattleState _currentState;
    public bool playerCharacterPause = true;


    //采集相关
    [SerializeField] private float baseCollectTime = 5f;
    [SerializeField] private float collectEnhance = 5f;
    private float currentCollectTime;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        player_animator = GetComponent<Animator>();
        currentHP = maxHP;
        UpdateHPUI();

        // 初始化对象池
        for (int i = 0; i < ghostPoolSize; i++)
        {
            GameObject ghost = Instantiate(ghostPrefab);
            ghost.SetActive(false);
            ghostPool.Enqueue(ghost);
        }
        InitData();
    }

    private void InitData()
    {
        currentCollectTime = baseCollectTime / collectEnhance;
    }

    public void SetCollectEnhance(float enhanceRate)
    {
        collectEnhance= enhanceRate;
        currentCollectTime = baseCollectTime / collectEnhance;
    }
    public void MultipleCollectEnhance(float enhanceRate)
    {
        collectEnhance*= enhanceRate;
        currentCollectTime = baseCollectTime / collectEnhance;
    }

    void Update()
    {
        if (!playerCharacterPause)
        {


            UpdateHPUI();

            if (!isDashing)
            {
                // 获取常规移动输入
                moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

                UpdateAnimation();

            }


            // 检测冲刺输入
            if (Input.GetKeyDown(KeyCode.Space) && canDash && (moveInput.x != 0 || moveInput.y != 0))
            {
                StartCoroutine(Dash());
            }


            HandleInvincibleTimer();

        }
    }

    private void UpdateHPUI()
    {
        hpText.text = "HP: " + currentHP.ToString();
    }

    void UpdateAnimation()
    {
        int direction = 0; // Idle

        if (moveInput.y > 0)
            direction = 2; // 向上
        else if (moveInput.y < 0)
            direction = 1; // 向下
        else if (moveInput.x > 0)
            direction = 4; // 向右
        else if (moveInput.x < 0)
            direction = 3; // 向左
        player_animator.SetInteger("MoveDirection", direction);
    }

    void FixedUpdate()
    {
        if(!playerCharacterPause)
        {
            if (!isDashing)
            {
                // 常规移动
                rb.velocity = moveInput * moveSpeed;
            }

            if (_isStay)
            {
                timer -= Time.deltaTime;
                img.fillAmount = timer / currentCollectTime;
                if (timer <= 0)
                {
                    if(currentTrigger.name != "Blank")
                    {
                     StartCoroutine(BlinkAndHide(currentTrigger.transform.GetChild(0).gameObject));
                    }

                    if (currentTrigger.name.Equals("Lumen"))
                    {
                        ResourceCollection.lumen++;
                    }
                    else if (currentTrigger.name.Equals("Vitality"))
                    {
                        ResourceCollection.vitality++;
                    }
                    else if (currentTrigger.name.Equals("Aqua"))
                    {
                        ResourceCollection.aqua++;
                    }
                    timer = currentCollectTime;
                }
            }
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // 存储原始速度并应用冲刺速度
        Vector2 dashDirection = moveInput;
        rb.velocity = dashDirection * dashSpeed;

        // 开始生成残影
        StartCoroutine(CreateGhosts());

        // 冲刺持续时间
        yield return new WaitForSeconds(dashDuration);

        // 恢复常规移动
        isDashing = false;
        rb.velocity = moveInput * moveSpeed;

        // 冷却时间
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator CreateGhosts()
    {
        while (isDashing)
        {
            GameObject ghost = GetGhostFromPool();
            ghost.transform.position = transform.position;
            ghost.transform.rotation = transform.rotation;
            SpriteRenderer ghostSprite = ghost.GetComponent<SpriteRenderer>();

            // 复制玩家精灵和颜色
            ghostSprite.sprite = playerSprite.sprite;
            ghostSprite.color = ghostColor;

            // 启动淡出效果
            StartCoroutine(FadeGhost(ghostSprite));

            yield return new WaitForSeconds(ghostInterval);
        }
    }

    GameObject GetGhostFromPool()
    {
        GameObject ghost;
        if (ghostPool.Count > 0)
        {
            ghost = ghostPool.Dequeue();
        }
        else
        {
            // 如果池子里的对象用完了，额外创建一个（可选）
            ghost = Instantiate(ghostPrefab);
        }

        ghost.SetActive(true);
        return ghost;
    }

    void ReturnGhostToPool(GameObject ghost)
    {
        ghost.SetActive(false);
        ghostPool.Enqueue(ghost);
    }

    IEnumerator FadeGhost(SpriteRenderer ghost)
    {
        float fadeTime = 0.5f;
        float timer = 0;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            Color newColor = ghost.color;
            newColor.a = Mathf.Lerp(ghostColor.a, 0, timer / fadeTime);
            ghost.color = newColor;
            yield return null;
        }

        ReturnGhostToPool(ghost.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {             
        if (collision.CompareTag("收集元素"))
        {
            _isStay = true;
            currentTrigger = collision.gameObject;
            //Debug.Log("111");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        /*if (collision.CompareTag("收集元素"))
        {
            timer = 5;
            _isStay =false;
        }*/
    }
    
    IEnumerator BlinkAndHide(GameObject ghost)
    {
        // 闪烁两次
        for (int i = 0; i < 4; i++)
        {
            ghost.SetActive(!ghost.activeInHierarchy); 
            yield return new WaitForSeconds(0.2f);
        }

        // 确保物体最终隐藏
        ghost.SetActive(false);
    }

    public void SetPlayerPause(bool pause)
    {
        if(pause)
        {
            playerCharacterPause = true;
            timer = 0;
            rb.velocity = Vector2.zero;
            moveInput = Vector2.zero;
            UpdateAnimation();
            img.fillAmount = 1;
        }
        else
        {
            playerCharacterPause = false;
        }
    }

    public void HealByAbs(float amount)
    {
        currentHP=Mathf.Min(currentHP+amount, maxHP);
        UpdateHPUI();
    }

    public void HealByPercentage(float percentage)
    {
        currentHP = Mathf.Min(currentHP + maxHP* percentage, maxHP);
        UpdateHPUI();
    }
    public void HealByWoundPercentage(float percentage)
    {
        currentHP = currentHP + (maxHP - currentHP) * percentage;
        UpdateHPUI();
    }

    public void MultipleTakeDamage(float rate)
    {
        takeDamageRate*=rate;
    }

    public void TakeDamage(float damage)
    {
        if (!invincible)
        {
            currentHP = Mathf.Max(currentHP - damage* takeDamageRate, 0);
            invincible = true;
            invincibleTimer = invincibleTime* extraInvincibleTimeRate;
            damageTakeThisTurn += invincibleTime * extraInvincibleTimeRate;
        }
        CardManager.Instance._boss.DealDamage(giveBackDamageRate* damage * takeDamageRate);
    }

}