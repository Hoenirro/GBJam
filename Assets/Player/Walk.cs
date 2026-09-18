using UnityEngine;
using UnityEngine.InputSystem;
public class Walk : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float turnSpeed = 180f;
    private Vector2 moveInput = Vector2.zero;
    public LayerMask interactMask;

    [SerializeField] private GameObject MessageBox;
    [SerializeField] private TMPro.TMP_Text MessageText;

    private Vector3 raycastOrigin;
    public float raycastDistance = 2f;

    void Start()
    {
        MessageBox.SetActive(false);
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

        //Debug.Log("Jump button pressed");

        // Draw the ray in the Scene view for 1 second to debug alignment and distance
        //Debug.DrawRay(raycastOrigin, transform.forward * raycastDistance, Color.red, 1f);

        // Cast the ray
        if (Physics.Raycast(raycastOrigin, transform.forward, out RaycastHit hit, raycastDistance, interactMask))
        {
            string hitObjectTag = hit.collider.tag;

            switch (hitObjectTag)
            {
                case "Door":
                    if (hit.collider.GetComponent<DoorBehaviour>() == null)
                    {
                        //Debug.LogWarning("DoorBehaviour component not found on the door object.");
                        return;
                    }
                    DoorBehaviour doorBehaviour = hit.collider.GetComponent<DoorBehaviour>();
                    MessageBox.SetActive(hit.collider.GetComponent<DoorBehaviour>().needsKey);
                    MessageText.text = "hmmm..\n..a LOCKED Door";
                    transform.position =
                        (doorBehaviour.inside.position - transform.position).magnitude <
                        (doorBehaviour.outside.position - transform.position).magnitude ?
                        doorBehaviour.outside.position : doorBehaviour.inside.position;
                    break;
                case "Interactable":
                    Debug.Log("Hit an interactable object: " + hit.collider.name);
                    break;
                default:
                    Debug.Log("Hit an object with tag: " + hitObjectTag);
                    break;
            }
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