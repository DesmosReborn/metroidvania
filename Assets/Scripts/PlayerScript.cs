using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    private float movementInputDirection;

    private int amountOfJumpsLeft;
    private int facingDirection = 1;
    private float dashTimeLeft;
    private float lastAfterImageXPos;
    private float lastDash = -100;

    private bool isFacingRight = true;
    private bool isWalking;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isDoubleJumping;
    private bool isDashing;
    private bool canDash = true;
    private bool canJump = true;
    private bool canMove = true;
    private bool canFlip = true;

    private Rigidbody2D rb;
    public Animator anim;

    public int amountOfJumps = 2;

    public float movementSpeed = 10.0f;
    public float jumpForce = 16.0f;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier = 0.95f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float wallHopForce;
    public float wallJumpForce;
    public float dashTime;
    public float dashSpeed;
    public float dashCoolDown;
    public float distanceBetweenAfterImages;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckDash();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
    }

    private void CheckIfCanJump()
    {
        if ((isGrounded && rb.velocity.y <= 0))
        {
            amountOfJumpsLeft = amountOfJumps;
            isDoubleJumping = false;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }

    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0);

                if (Mathf.Abs(transform.position.x - lastAfterImageXPos) > distanceBetweenAfterImages)
                {
                    AfterImagePool.Instance.getFromPool();
                    lastAfterImageXPos = transform.position.x;
                }

                dashTimeLeft -= Time.deltaTime;
            } else if (dashTimeLeft <= 0 || isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
                rb.velocity *= 0.5f;
            }
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isDoubleJumping", isDoubleJumping);
        anim.SetBool("isDashing", isDashing);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.C))
        {
            Jump();
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (Time.time >= lastDash + dashCoolDown)
            {
                AttemptToDash();
            }
        }
    }

    private void Jump()
    {
        if (canJump && !isWallSliding)
        {
            if (amountOfJumpsLeft == amountOfJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            } else
            {
                isDoubleJumping = true;
                if ((rb.velocity.x > 0 && facingDirection == -1) || (rb.velocity.x < 0 && facingDirection == 1))
                {
                    rb.velocity = new Vector2(-rb.velocity.x, jumpForce);
                } else
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }
            amountOfJumpsLeft--;
        }
        else if ((isWallSliding && movementInputDirection == 0) || (isWallSliding && !canJump)) //Wall hop
        {
            isWallSliding = false;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if ((isWallSliding || isTouchingWall) && movementInputDirection == -facingDirection && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            isWallSliding = false;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        isWallSliding = false;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        AfterImagePool.Instance.getFromPool();
        lastAfterImageXPos = transform.position.x;
    }

    private void ApplyMovement()
    {
        if (canMove)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
            }
            else if (!isGrounded && !isWallSliding && movementInputDirection != 0)
            {
                Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
                rb.AddForce(forceToAdd);

                if (Mathf.Abs(rb.velocity.x) > movementSpeed)
                {
                    rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
                }
            }
            else if (!isGrounded && !isWallSliding && movementInputDirection == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
            }

            if (isWallSliding)
            {
                if (rb.velocity.y < -wallSlideSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
                }
            }
        }
    }

    private void Flip()
    {
        if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}