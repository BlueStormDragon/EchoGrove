using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject dialougeParent, player;
    [SerializeField] private TMP_Text dialougeText;
    [SerializeField] private float typingSpeed = 0.05f, turnSpeed = 2f, diaRange = 2f;

    private int currentDialogueIndex = 0;
    private List<DialougeString> dialogueList = new List<DialougeString>();

    private PlayerMovement playerMovement;

    private Transform playerCamera;
    private Quaternion camRotation;
    private MainCamera mainCamera;

    private bool inDialogue = false;

    [SerializeField] private Transform dialoguePos;
    #endregion

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        dialougeParent.SetActive(false);
        playerCamera = Camera.main.transform;
        camRotation = playerCamera.rotation;
        mainCamera = playerCamera.GetComponent<MainCamera>();
    }

    private void Update()
    {
        if(!inDialogue && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton0)))
        {
            Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, diaRange);
            if (hit.collider != null && hit.collider.tag == "NPC")
            {
                hit.collider.GetComponent<NPC>().Dialogue(transform.gameObject);
            }
        }
    }

    public void DialogueStart(List<DialougeString> textToPrint, Transform NPC, Vector3 camPos, bool hasSpoken)
    {
        inDialogue = true;
        dialougeParent.SetActive(true);
        playerMovement.StopMovement();
        mainCamera.StopCameraMovement();

        //the triggered dialogues
        if(!hasSpoken)
            StartCoroutine(TurnCameraTowardsNPC(NPC, camPos));
        //the manually activated dialogues
        else
            StartCoroutine(TurnCameraTowardsNPC(NPC, dialoguePos.position));

        dialogueList = textToPrint;
        currentDialogueIndex = 0;

        StartCoroutine(PrintDialogue());
    }

    public IEnumerator TurnCameraTowardsNPC(Transform NPC, Vector3 targetPos)
    {
        Quaternion targetRotation = Quaternion.LookRotation(NPC.position - playerCamera.position);
        float elapsedTime = 0;

        transform.LookAt(new Vector3(NPC.transform.position.x, transform.position.y, NPC.transform.position.z));

        while (playerCamera.position != targetPos && camRotation != targetRotation)
        {
            playerCamera.position = Vector3.MoveTowards(playerCamera.position, targetPos, turnSpeed * Time.deltaTime);
            targetRotation = Quaternion.LookRotation(NPC.position - playerCamera.position);
            elapsedTime += Time.deltaTime * turnSpeed;
            playerCamera.rotation = Quaternion.Slerp(camRotation, targetRotation, elapsedTime);
            
            yield return null;
        } 
            playerCamera.rotation = targetRotation;
    }

    private IEnumerator ReturnCamera()
    {
        float elapsedTime = 0F;

        while (elapsedTime < 2f)
        {
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, camRotation, elapsedTime);
            elapsedTime += Time.deltaTime * turnSpeed;
            yield return null;
        }
        playerCamera.rotation = camRotation;
    }

    private IEnumerator PrintDialogue()
    {
        while(currentDialogueIndex < dialogueList.Count)
        {
            DialougeString line = dialogueList[currentDialogueIndex];

            line.startDialougeEvent?.Invoke();
            
            yield return StartCoroutine(TypeText(line.text));
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton0));
            
            line.endDialougeEvent?.Invoke();
        }
        DialogueStop();
    }

    private IEnumerator TypeText(string text)
    {
        yield return null;

        float ogSpeed = typingSpeed, timer;
        bool fastText = false;
        dialougeText.text = "";

        foreach (char letter in text.ToCharArray()) 
        {
            dialougeText.text += letter;

            if (!fastText) //A timer. Used to use WaitForSeconds() but we need to
            {             //check for a button press to turn on the fast text speed
                timer = 0;
                while (timer < typingSpeed)
                {
                    timer += Time.deltaTime;
                    
                    if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.JoystickButton0))
                    {
                        fastText = true;
                        typingSpeed = 0.01f;
                        break;
                    }
                    yield return null;
                }
            }
            else
                yield return new WaitForSeconds(typingSpeed);

            //Another buttoncheck because the first one didnt always catch the input.
            if (!fastText && (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.JoystickButton0)))
            {
                fastText = true;
                typingSpeed = 0.01f;
            }

            yield return null;
        }
        
        typingSpeed = ogSpeed;
        fastText = false;

        if (dialogueList[currentDialogueIndex].isEnd)
        {
            yield return new WaitUntil(() => (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton0)));
            DialogueStop();
            yield return StartCoroutine(ReturnCamera());
        }
        currentDialogueIndex++;
    }

    private void DialogueStop()
    {
        StopAllCoroutines();
        playerMovement.StartMovement();
        mainCamera.StartCameraMovement();
        dialougeText.text = "";
        dialougeParent.SetActive(false);
        inDialogue = false;
    }

    public bool InDialogue()
    {
        return inDialogue;
    }
}       
