using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    private Rigidbody2D rb;

    [SerializeField]
    private float jumpHeight = 7;
    private bool jumping = false;
    private bool ceiling = false;
    private bool grounded = true;

    [SerializeField]
    private int maxJump = 3;

    [SerializeField]
    private int jumpCount = 0;

    [SerializeField]
    private int groundLayer;

    [SerializeField]
    private int raycastDistance = 1;


    [SerializeField]
    private float moveDown = -1;

    [SerializeField]
    private float moveSpeed = 1;

    private bool isMoving = false;
    private InputAction.CallbackContext context;

    [SerializeField]
    private int called = 0;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    // Start is called before the first frame update
    void Start()
    {
        jumping = false;
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * raycastDistance, Color.yellow);
    }

    void FixedUpdate()
    {
        if (jumping)
        {
            Jump();
        }

        if (isMoving)
        {
            MoveWithPosition();
            //print("moving");

        }

    }

    private void MoveWithVelocity()
    {

        rb.AddForce(new Vector3(context.ReadValue<Vector2>().x, 0) * moveSpeed * Time.deltaTime);

        //print(rb.velocity);
    }

    private void MoveWithPosition()
    {
        if (GroundedRaycast().collider == null)
        {
            rb.MovePosition(transform.position + new Vector3(context.ReadValue<Vector2>().x, moveDown).normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            rb.MovePosition(transform.position + new Vector3(context.ReadValue<Vector2>().x, 0) * moveSpeed * Time.deltaTime);
        }
       
    }

    private void Jump()
    {
        jumpCount++;
        // ForceMode2D.Impulse means that all force is applied in one hit not over time
        //could devide by zero so idk
        //might change to just an array
        rb.AddForce((Vector2.up) * (jumpHeight / jumpCount), ForceMode2D.Impulse);
        jumping = false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if ( context.performed)
        {
            RaycastHit2D hit = GroundedRaycast();

            if (hit.collider != null && jumpCount < maxJump)
            {
                jumping = true;
                jumpCount = 0;
                print("Jump");
            }
            else if (jumpCount > 0 && jumpCount < maxJump)
            {
                jumping = true;
            }
            else
            {
                print("dont jump");
                jumpCount = 0;
            }

        }
    }

    public void Move(InputAction.CallbackContext newContext)
    {
        ++called;
        context = newContext;
        
            if (newContext.performed )
            {
                RaycastHit2D hit = GroundedRaycast();
                if (hit.collider != null || jumpCount==1)
                {
                    isMoving = !isMoving;
                    print("Move pressed");
                }
            }
        
    }

    private RaycastHit2D GroundedRaycast()
    {
        int layerMask = 1 << groundLayer;
        return Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), raycastDistance, layerMask);

    }
}
