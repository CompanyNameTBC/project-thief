using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private FieldOfView fieldOfView;
    public float movementSpeed = 5;
    public Rigidbody2D rb;

    Vector2 movement;

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // TODO: Set aim direction to the direction the player is moving in
        fieldOfView.SetAimDirection(Vector3.right);
        fieldOfView.SetOrigin(transform.position);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }
}
