using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool _isGrounded;
    private Vector3 _velocity;
    public CharacterController controller;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    public float jumpHeight = 3f;

    public float speed = 12f;

    // Update is called once per frame
    private void Update()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0) _velocity.y = -2f;

        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        var playerTransform = transform;
        var move = playerTransform.right * x + playerTransform.forward * z;

        controller.Move(move * (speed * Time.deltaTime));

        if (Input.GetButtonDown("Jump") && _isGrounded) _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        _velocity.y += gravity * Time.deltaTime;
        controller.Move(_velocity * Time.deltaTime);
    }
}