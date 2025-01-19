using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovementForTrailer : MonoBehaviour
{
    [SerializeField] private bool freeMove = true, freeRotate =  true;

    [SerializeField] private float camSpeed = 0.2f, turnSpeed = 2f;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private GameObject lookAt;

    [SerializeField] private List<GameObject> camPos = new List<GameObject>();

    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }

    [Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
    const string yAxis = "Mouse Y";
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(StartCameraMovement());
        }
    }
    
    void FixedUpdate()
    {
        if(freeMove)
            rb.MovePosition(transform.position + (transform.forward * Input.GetAxis("Vertical") * camSpeed) + (transform.right * Input.GetAxis("Horizontal") * camSpeed));
       
        if(freeRotate)
            CamLookAt();
    }

    void CamLookAt()
    {
        rotation.x += Input.GetAxis(xAxis) * sensitivity;
        rotation.y += Input.GetAxis(yAxis) * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        transform.localRotation = xQuat * yQuat;
    }

    private IEnumerator StartCameraMovement()
    {
        for (int i = 0; i < camPos.Count; i++)
        {
            yield return StartCoroutine(CameraMovement(camPos[i].transform.position));
        }
    }

    private IEnumerator CameraMovement(Vector3 targetPos)
    {
        //Quaternion targetRotation = Quaternion.LookRotation(lookAt.transform.position - transform.position);

        //transform.LookAt(new Vector3(lookAt.transform.position.x, lookAt.transform.position.y, lookAt.transform.position.z));
        // || transform.rotation != targetRotation 
        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, camSpeed * Time.deltaTime);

            //targetRotation = Quaternion.LookRotation(lookAt.transform.position - transform.position);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
