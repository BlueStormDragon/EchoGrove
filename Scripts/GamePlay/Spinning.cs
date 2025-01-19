using UnityEngine;

public class Spinning : MonoBehaviour
{
    private int rotateSpeed = 46; 

    private void Update()
    {
        gameObject.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.Self);
    }
}
