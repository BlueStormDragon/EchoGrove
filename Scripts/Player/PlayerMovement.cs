using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 2, rotateSpeed = 1;

    CharacterController controller;
    private Vector3 input;
    private Animator animator;
    private string currentAnim = "";
    private float startingYLevel, currentYLevel;
    private bool stopMovement, previouslyStopped = true, gravityOn = true, playWalkingSFX = true;

    private AudioSource audioSource;
    [SerializeField] private AudioClip sfxOnRoad, sfxOffRoad, sfxStone;

    private DialogManager diaManager;

    private int currentGround = 0, previousGround = 0;

    void Start()
    {
        //testing
        Application.targetFrameRate = 60;

        diaManager = GetComponent<DialogManager>();
        audioSource = GetComponent<AudioSource>();  
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        startingYLevel = transform.position.y;
        
        ChangeAnimation("Idle");

        StartCoroutine(PlayWalkingSounds());
        StartCoroutine(Gravity());
    }

    //executes player inputs
    private void FixedUpdate()
    {
        if (!stopMovement)
        {
            GatherInput();
        }

        if (!stopMovement && input != Vector3.zero)
        {
            ChangeAnimation("Walking");
            
            LookTowards();
            PerforMovements();

            if (previouslyStopped)
            {
                previouslyStopped = false;
            }

            CurrentGround();
        }
        else
        {
            if(!stopMovement || diaManager.InDialogue())
            {
                ChangeAnimation("Idle");
            }
            previouslyStopped = true;
            audioSource.Pause();
        }
    }

    public void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (currentAnim != animation)
        {
            currentAnim = animation;
            animator.CrossFade(animation, crossfade);
        }
    }

    private void GatherInput()
    {
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (input.sqrMagnitude > 1f)
            input.Normalize();
    }

    private void LookTowards()
    {
        if (input.sqrMagnitude == 0)
            return;

        var rot = Quaternion.LookRotation(input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotateSpeed * 360 * Time.deltaTime);
    }

    private void PerforMovements()
    {
        controller.Move(input.ToIso() * speed * Time.deltaTime);
    }

    public void StopMovement()
    {
        stopMovement = true;
        
    }
    public void StartMovement()
    {
        stopMovement = false;
    }

    //level 2 uses different walking sound effects
    private bool inLevel2;
    private IEnumerator PlayWalkingSounds()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            
            if(playWalkingSFX)
            {
                if (GetMainRoadIndex((int)SaveManager.instance.LoadSpawnPoint().x, currentGround))
                {
                    if (inLevel2)
                    {
                        audioSource.clip = sfxStone;
                        audioSource.Play();
                    }
                    audioSource.clip = sfxOnRoad;
                    audioSource.Play();
                }
                else
                {
                    audioSource.clip = sfxOffRoad;
                    audioSource.Play();
                }
            }
        }
    }
    
    //checks the texture index of the terrain, returns true if the texture is a main road
    private bool GetMainRoadIndex(int level, int groundIndex)
    {
        switch(level)
        {
            case 0:
                inLevel2 = false;
                if (groundIndex == 3 || groundIndex == 4 || groundIndex == 7)
                    return true;
                else
                    return false;

            case 1:
                inLevel2 = true;
                if (groundIndex == 2 || groundIndex == 4)
                    return true;
                else
                    return false;

            case 2:
                inLevel2 = false;
                if (groundIndex == 5 || groundIndex == 6 || groundIndex == 7)
                    return true;
                else
                    return false;
            default:
                Debug.Log("wrong level number");
                return false; ;
        }
    }

    //gets which terrain texture the player is walking on 
    private void CurrentGround()
    {
        previousGround = currentGround;
        currentGround = TerrainSurface.GetMainTexture(transform.position);
    }

    //every 60 frame check if Bo is in the air and if he is move him down to the floor
    private IEnumerator Gravity()
    {
        while (true)
        {
            startingYLevel = transform.position.y;

            while (gravityOn && currentYLevel != startingYLevel)
            {
                transform.position = Vector3.MoveTowards(transform.position , 
                    new Vector3(transform.position.x , startingYLevel , transform.position.z), 0.01f);
                yield return null;
            }
            yield return new WaitForSeconds(1);
        }
    }



    public void TurnOnGravity()
    {
        gravityOn = true;
    }
    public void TurnOffGravity()
    {
        gravityOn = false;
    }

    public void TurnOnWalkingSFX()
    {
        playWalkingSFX = true;
    }
    public void TurnOffWalkingSFX()
    {
        audioSource.Stop();
        playWalkingSFX = false;
    }
}

public static class Isometric
{
    private static Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => isoMatrix.MultiplyPoint3x4(input);
}
