using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mimi : MonoBehaviour
{
    [Header("References")]
    public static float minSpeed = 10f;
    public static float maxSpeed = 25f;
    public static float currentSpeed;
    public float acceleration = 0.05f;
    public bool isAlive;

    public Text gameOverText;

    [SerializeField] public LayerMask groundLayerMask;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider2D;

    public GameObject restartButton;
    public GameObject quitButton;

    private BackgroundMusic8Bit bgMusic;

    [SerializeField] public AudioSource jumpSound;
    [SerializeField] public AudioSource failSound;
    [SerializeField] public AudioSource bonePickedUpSound;

    public static bool gamePaused = false;

    private void Awake()
    { 
        rigidBody2D = transform.GetComponent<Rigidbody2D>();
        boxCollider2D = transform.GetComponent<BoxCollider2D>();
        currentSpeed = minSpeed;
        isAlive = true;
        gameOverText.enabled = false;
        bgMusic = FindFirstObjectByType<BackgroundMusic8Bit>();
    }

    
    void Update()
    {
        if (!gamePaused)
        {
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                float jumpVelocity = 14;
                rigidBody2D.linearVelocity = Vector2.up * jumpVelocity;
                jumpSound.Play();
            }

            transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);
        }

        if (currentSpeed < maxSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
           
        }

        if (isAlive) 
        {
            restartButton.SetActive(false);
            quitButton.SetActive(false);
        } 
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(
            boxCollider2D.bounds.center, boxCollider2D.size, 0f, Vector2.down,
            0.01f, groundLayerMask);
        return (raycastHit2D.collider != null);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isAlive = false;
            Time.timeScale = 0;
            gameOverText.enabled = true;
            restartButton.SetActive(true);
            quitButton.SetActive(true);
            failSound.Play();
            bgMusic.StopMusic();
            gamePaused = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bone"))
        {
             ScoreManager.Instance.AddScore(1);
             Destroy(collision.gameObject);   
             bonePickedUpSound.Play();
             
        }

    }
}
