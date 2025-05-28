using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Movement
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 lastMovedVector;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 5f;
    public GameObject afterImagePrefab;
    public float afterImageSpawnInterval = 0.01f;

    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private float afterImageTimer = 0f;
    private Vector2 dashDirection;

    //References
    Rigidbody2D rb;
    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); //initializing default last moved vector
    }

    void Update()
    {
        if (!isDashing)
        {
            InputManagement();

            if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f && moveDir != Vector2.zero)
            {
                StartCoroutine(Dash());
            }
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        dashDirection = moveDir;
        float timer = 0f;
        afterImageTimer = 0f;

        while (timer < dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;

            afterImageTimer += Time.deltaTime;
            if (afterImageTimer >= afterImageSpawnInterval)
            {
                SpawnAfterImage();
                afterImageTimer = 0f;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        dashCooldownTimer = dashCooldown;
    }

    void SpawnAfterImage()
    {
        GameObject afterImage = Instantiate(afterImagePrefab, transform.position, Quaternion.identity);
        SpriteRenderer sr = afterImage.GetComponent<SpriteRenderer>();
        SpriteRenderer playerSr = GetComponent<SpriteRenderer>();
        if (sr && playerSr)
        {
            sr.sprite = playerSr.sprite;
            sr.flipX = playerSr.flipX;
            sr.color = new Color(1, 1, 1, 0.5f); // semi-transparent
        }

        Destroy(afterImage, 0.1f); // destroy after some time
    }



    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.isPaused)
        {
            return;
        }
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);
        }

        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);
        }

        if (moveDir.x != 0f && moveDir.y != 0f)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }
        
    void Move()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.isPaused || isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    }

}
