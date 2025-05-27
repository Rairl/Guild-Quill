using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace TopDown
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f; // Speed multiplier for movement
        private Vector2 movementInput; // Stores the current movement direction (x or y only)

        [Header("Animation Settings")]
        [SerializeField] private Animator anim; // Reference to the Animator
        private string lastDirection = "Down"; // Keeps track of the last direction moved

        private Rigidbody2D rb; // Reference to Rigidbody2D

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D on the GameObject
        }

        private void Update()
        {
            HandleAnimations(); // Update animation every frame
        }

        private void FixedUpdate()
        {
            // Apply movement using Rigidbody2D
            rb.velocity = movementInput * moveSpeed;
        }

        /// <summary>
        /// Handles animation logic based on current movement input and direction.
        /// </summary>
        private void HandleAnimations()
        {
            if (anim == null) return; // Exit if no animator is assigned

            if (movementInput == Vector2.zero)
            {
                // If not moving, play Idle animation based on last direction
                anim.Play("Idle" + lastDirection);
            }
            else
            {
                // If moving, update last direction and play Walking animation
                UpdateDirection(movementInput);
                anim.Play("Walking" + lastDirection);
            }
        }

        /// <summary>
        /// Updates the lastDirection string based on current input vector.
        /// </summary>
        /// <param name="input">Vector2 movement input</param>
        private void UpdateDirection(Vector2 input)
        {
            // Use axis with greater magnitude to determine direction
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                lastDirection = input.x > 0 ? "Right" : "Left";
            }
            else
            {
                lastDirection = input.y > 0 ? "Up" : "Down";
            }
        }

        private void OnMove(InputValue value)
        {
            Vector2 input = value.Get<Vector2>();

            // Restrict movement to only horizontal or vertical axis (no diagonal)
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                movementInput = new Vector2(Mathf.Sign(input.x), 0f); // Move only on X
            }
            else if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                movementInput = new Vector2(0f, Mathf.Sign(input.y)); // Move only on Y
            }
            else
            {
                movementInput = Vector2.zero; // No movement
            }
        }
    }
}
