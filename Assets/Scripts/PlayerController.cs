using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody body;
    CapsuleCollider mainCollider;
    BoxCollider slideCollider;

    Vector3 startGamePosition;
    Quaternion startGameRotation;

    float pointStart;
    float pointFinish;
    private float laneOffset;
    
    [SerializeField]
    private float laneChangeSpeed = 15f;
    
    [SerializeField]
    private float jumpForce = 10f;
    [SerializeField]
    private float jumpGravityBase = -20;
    private float jumpGravity => jumpGravityBase * (0.875f + SectionGenerator.Instance.speed / 40f);

    private bool isMoving = false;
    private bool isJumping = false;
    
    private float lastVectorX;
    
    private Vector3 defaultGravity = new Vector3(0, -9.81f, 0);

    Coroutine moveCoroutine;
    private void Start()
    {
        laneOffset = MapGenerator.Instance.laneOffset;
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody>();
        mainCollider = GetComponent<CapsuleCollider>();
        slideCollider = GetComponent<BoxCollider>();
        startGamePosition = transform.position;
        startGameRotation = transform.rotation;

        SwipeManager.Instance.SwipeEvent += MovePlayer;
    }

    private void Jump()
    {
        isJumping = true;
        body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, jumpGravity, 0);
        StartCoroutine(StopJumpCoroutine());
    }

    public void StartSlide()
    {
        slideCollider.enabled = true;
        mainCollider.enabled = false;
    }
    public void EndSlide()
    {
        mainCollider.enabled = true;
        slideCollider.enabled = false;
    }
    private void Slide()
    {
        animator.SetTrigger("Slide");
    }

    private void MovePlayer(bool[] swipes)
    {
        
        if (swipes[(int)SwipeManager.Direction.Left] && pointFinish > -laneOffset)
        {
            MoveHorizontal(-laneChangeSpeed);
        }
        if (swipes[(int)SwipeManager.Direction.Right] && pointFinish < laneOffset)
        {
            MoveHorizontal(laneChangeSpeed);
        }
        if (swipes[(int)SwipeManager.Direction.Up] && isJumping == false)
        {
            Jump();
        }
        if (swipes[(int)SwipeManager.Direction.Down])
        {
            if (isJumping)
            {
                body.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);
            }
            else
            {
                Slide();
            }
            
        }
    }

    private void MoveHorizontal(float speed)
    {
        pointStart = pointFinish;
        pointFinish += Mathf.Sign(speed) * laneOffset;
        
        if (isMoving)
        {
            StopCoroutine(moveCoroutine);
            isMoving = false;
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(speed));
    }

    IEnumerator MoveCoroutine(float vectorX)
    {
        isMoving = true;
        while (Mathf.Abs(pointStart - transform.position.x) < laneOffset)
        {
            yield return new WaitForFixedUpdate();
            lastVectorX = vectorX;
            body.velocity = new Vector3(vectorX, body.velocity.y, 0);
            float x = Mathf.Clamp(transform.position.x, Mathf.Min(pointStart, pointFinish), Mathf.Max(pointStart, pointFinish));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        body.velocity = Vector3.zero;
        transform.position = new Vector3(pointFinish, transform.position.y, transform.position.z);

        if (transform.position.y > 1)
        {
            body.velocity = new Vector3(body.velocity.x, -10, body.velocity.z);
        }

        isMoving = false;
    }

    IEnumerator StopJumpCoroutine()
    {
        do
        {
            yield return new WaitForFixedUpdate();
        }
        while (body.velocity.y != 0);
        isJumping = false;
        Physics.gravity = defaultGravity;
    }

    public void StartGame()
    {
        if (SectionGenerator.Instance.isRunning) return;
        animator.SetTrigger("Run");
    }

    public void StartRun()
    {
        SectionGenerator.Instance.StartLevel();
        animator.applyRootMotion = false;
    }

    public void ResetGame()
    {
        body.velocity = Vector3.zero;
        pointStart = 0;
        pointFinish = 0;
        animator.applyRootMotion = true;
        animator.SetTrigger("Idle");
        transform.position = startGamePosition;
        transform.rotation = startGameRotation;
        SectionGenerator.Instance.ResetLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;
        if (tag == "RampTrigger")
        {
            body.constraints |= RigidbodyConstraints.FreezePositionZ;
        }
        if (tag == "Lose")
        {
            ResetGame();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string tag = other.gameObject.tag;
        if (tag == "RampTrigger")
        {
            body.constraints &= ~RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Ground")
        {
            body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
        }
        if (tag == "Bounce")
        {
            MoveHorizontal(-lastVectorX);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "RampPlane")
        {
            if (body.velocity.x == 0 && isJumping == false)
            {
                body.velocity = new Vector3(body.velocity.x, -10, body.velocity.z);
            }
        }
    }
}
