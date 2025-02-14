using UnityEngine;

public class MovingState : PlayerState
{
    public override void Tick()
    {
        base.Tick();
        if (PlayerScript.playerInput.currentControlScheme == "Keyboard&Mouse")
        {
            Vector2 moveInput = PlayerScript.moveAction.ReadValue<Vector2>();
            // Apply movement
            if (moveInput != Vector2.zero)
            {
                // Get the camera's forward and right vectors
                Vector3 cameraForward = PlayerScript.playerCamera.transform.forward;
                Vector3 cameraRight = PlayerScript.playerCamera.transform.right;

                // Flatten the vectors to the ground plane
                cameraForward.y = 0;
                cameraRight.y = 0;

                // Normalize the vectors
                cameraForward.Normalize();
                cameraRight.Normalize();

                // Calculate the movement direction
                Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

                // Move the player
                PlayerScript.rb.linearVelocity = new Vector3(moveDirection.x * PlayerScript.speed,
                    PlayerScript.rb.linearVelocity.y, 
                    moveDirection.z * PlayerScript.speed);
            }
        }
        else if (PlayerScript.playerInput.currentControlScheme == "Gamepad")
        {
            Vector2 input = PlayerScript.moveAction.ReadValue<Vector2>();
            // Get the camera's forward and right vectors
            Vector3 cameraForward = PlayerScript.playerCamera.transform.forward;
            Vector3 cameraRight = PlayerScript.playerCamera.transform.right;

            // Flatten the vectors to the ground plane
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Normalize the vectors
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate the movement direction
            Vector3 moveDirection = (cameraForward * input.y + cameraRight * input.x).normalized;

            // Move the player
            PlayerScript.rb.linearVelocity = new Vector3(moveDirection.x * PlayerScript.speed,
                PlayerScript.rb.linearVelocity.y,
                moveDirection.z * PlayerScript.speed);
        }
        
        // Change to idle state if there is no input
        if (PlayerScript.moveAction.ReadValue<Vector2>() == Vector2.zero)
        {
            PlayerScript.ChangeState(PlayerScript.GetComponent<IdleState>());
        }
    }
}
