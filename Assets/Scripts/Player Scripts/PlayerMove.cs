using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Animator anim;
    private CharacterController charController;
    private CollisionFlags collisionFlags = CollisionFlags.None;

    private float moveSpeed = 5f;
    private bool canMove;
    private bool finished_Movement = true;

    private Vector3 target_Pos = Vector3.zero;
    private Vector3 player_Move = Vector3.zero;

    private float player_ToPointDistance;

    private float gravity = 9.8f;
    private float height;

    // Awake is called before the OnEnable
    void Awake()
    {
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        /*MoveThePlayer();
        charController.Move(player_Move);*/
        CalculateHeight();
        CheckIfFinishedMovement();
        
    }

    // Determine is it grounded - smooth player movements
    bool IsGrounded()
    {
        return collisionFlags == CollisionFlags.CollidedBelow ? true : false;
    }

    // Determine the gravity for player - smooth player movements
    void CalculateHeight()
    {
        if (IsGrounded())
        {
            height = 0f;
        }
        else
        {
            height -= gravity * Time.deltaTime;
        }
    }

    void CheckIfFinishedMovement()
    {
        // If we DID NOT finished movement
        if (!finished_Movement)
        {
            /*  IF any animation in base layer is not in transition AND
                animator current state not stand AND
                current animation that is being played is about to finished or finish 
                (Normalized time of the animation is represented from 0 to 1 
                0 is the beginning of the animation
                0.5 is the middle of the animation
                1 is the end of the animation)
            */
            
            if (!anim.IsInTransition(0) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Stand") &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                finished_Movement = true;
            }
        }
        else
        {
            MoveThePlayer();
            player_Move.y = height * Time.deltaTime;
            collisionFlags = charController.Move(player_Move);
        }
    }
    
    // Move player
    void MoveThePlayer()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            // Casting a ray
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider is TerrainCollider)
                {
                    player_ToPointDistance = Vector3.Distance(transform.position, hit.point);
                    if (player_ToPointDistance >= 1.0f)
                    {
                        canMove = true;
                        target_Pos = hit.point;
                    }
                }
            }
        } // if MouseButtonDown
        
        if (canMove)
        {
            anim.SetFloat("Walk", 1.0f);
            Vector3 target_Temp = new Vector3(target_Pos.x, transform.position.y, target_Pos.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target_Temp - transform.position), 15.0f * Time.deltaTime );
            player_Move = transform.forward * moveSpeed * Time.deltaTime;
            if (Vector3.Distance(transform.position, target_Pos) <= 0.1f)
            {
                canMove = false;
            }
        }
        else
        {
            player_Move.Set(0f,0f,0f);
            anim.SetFloat("Walk", 0f);
        }
    }
}
