using UnityEngine;

public class FollowVision : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distance = 0.6f;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private Vector3 initialRotation = new Vector3(0, 0, 0);
    [SerializeField] private float centerThreshold = 0.2f;
    private bool isCentered;

    /*    private void OnBecameInvisible()
        {
            isCentered = false;

        }*/




    private void Update()
    {
        UpdateIfShouldMove();

        if (!isCentered)
        {
            Vector3 targetPosition = FindTargetPosition();
            MoveTowards(targetPosition);
            //if (ReachedPosition(targetPosition))
            //{
            //    isCentered = true;
            //}
        }
        updateRotation1();
    }

    private Vector3 FindTargetPosition()
    {
        return cameraTransform.position + (cameraTransform.forward * distance);
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        transform.position += (targetPosition - transform.position) * 0.025f;
    }

    private bool ReachedPosition(Vector3 targetPosition)
    {
        return Vector3.Distance(targetPosition, transform.position) < 0.1f;
    }

    private void UpdateIfShouldMove()
    {
        Vector3 visionDirection = cameraTransform.forward.normalized;
        Vector3 directionToObject = (cameraTransform.position - transform.position).normalized;

        float angleDistanceToTarget = System.MathF.Abs(Vector3.Dot(visionDirection, directionToObject));

        if (angleDistanceToTarget >= centerThreshold)
        {
            isCentered = false;
        }
    }


    private void updateRotation()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = cameraTransform.position - transform.position;
        Debug.Log("FolowVision.cs: updateRotation() targetDirection: " + targetDirection.ToString());



        Quaternion initialRotationOffset = Quaternion.Euler(initialRotation);
        targetDirection = initialRotationOffset * targetDirection;

        // The step size is equal to speed times frame time.
        float singleStep = rotationSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);

    }



    private void updateRotation1()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = cameraTransform.position - transform.position;
        /*
                Quaternion initialRotationOffset = Quaternion.Euler(initialRotation);
                targetDirection = initialRotationOffset * targetDirection;
        */
        targetDirection += initialRotation;

        // The step size is equal to speed times frame time.
        float singleStep = rotationSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

}
