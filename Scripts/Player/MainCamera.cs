using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothTime;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 offset;
    private Vector3 playerPosition;

    private bool moveCamera = true;

    private void Start()
    {
        offset = transform.position - player.position;
    }

    private void FixedUpdate()   
    {
        if(moveCamera)
        {
            playerPosition = player.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref currentVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
        }
    }

    public void StopCameraMovement()
    {
        moveCamera = false;
    }
    public void StartCameraMovement()
    {
        moveCamera = true;
    }
}
