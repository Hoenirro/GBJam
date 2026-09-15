using UnityEngine;
using UnityEngine.InputSystem;
public class Walk : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float turnSpeed = 180f;
    private Vector2 moveInput = Vector2.zero;
    public LayerMask interactMask;


    private Vector3 raycastOrigin;
    public float raycastDistance = 2f;

    void Start()
    {

    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        raycastOrigin = transform.position + Vector3.up * 0.5f + transform.forward * 0.2f;
        if (!value.isPressed)
            return;

        Debug.Log("Jump button pressed");

        // Draw the ray in the Scene view for 1 second to debug alignment and distance
        Debug.DrawRay(raycastOrigin, transform.forward * raycastDistance, Color.red, 1f);

        // Cast the ray
        if (Physics.Raycast(raycastOrigin, transform.forward, out RaycastHit hit, raycastDistance, interactMask))
        {
            // Use GetComponentInParent so child colliders (doorknobs, panels) still register the door
            DoorBehaviour door = hit.collider.GetComponentInParent<DoorBehaviour>();

            if (door != null)
            {
                if (!door.needsKey)
                {
                    // Get the animator from the door object containing the script
                    Animator animator = door.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.SetTrigger("Open");
                        Debug.Log("Door opened successfully.");
                    }
                    else
                    {
                        Debug.LogWarning("DoorBehaviour found, but no Animator component attached to it.");
                    }
                }
                else
                {
                    Debug.Log("The door is locked and requires a key.");
                }
            }
            else
            {
                Debug.Log($"Hit {hit.collider.name}, but it does not have a DoorBehaviour component.");
            }
        }
        else
        {
            Debug.Log("Raycast missed all objects within range.");
        }
    }

    void Update()
    {
        if (moveInput.sqrMagnitude < 0.01f)
            return;
        float turn = moveInput.x * turnSpeed * Time.deltaTime;
        transform.Rotate(0f, turn, 0f);
        float forward = moveInput.y * moveSpeed * Time.deltaTime;
        transform.Translate(Vector3.forward * forward);
    }


}