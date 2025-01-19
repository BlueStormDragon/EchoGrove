using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogeTrigger : MonoBehaviour
{

    [SerializeField] private List<DialougeString> dialogueStrings = new List<DialougeString>();
    [SerializeField] private Transform NPCTransform;
    [SerializeField] private GameObject camPos;

    [Tooltip("x is which level (1-3), and y is its index (1-?)"),SerializeField] private Vector2 dialogueIndex;
    public bool hasSpoken;

    private void Start()
    {
        dialogueIndex -= Vector2.one;
        hasSpoken = SaveManager.instance.LoadUsedDialogue((int)dialogueIndex.x, (int)dialogueIndex.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpoken)
        {
            other.gameObject.GetComponent<DialogManager>().DialogueStart(dialogueStrings, NPCTransform, camPos.transform.position, hasSpoken);
            hasSpoken = true;
            SaveManager.instance.SetUsedDialogue((int)dialogueIndex.x, (int)dialogueIndex.y);

            SaveManager.instance.SaveGame();
        }
    }
    public void StartDialogue(GameObject player)
    {
        player.gameObject.GetComponent<DialogManager>().DialogueStart(dialogueStrings, NPCTransform, camPos.transform.position, hasSpoken);
    }
}
 
[System.Serializable]
public class DialougeString
{
    public string text; // insert dialouge here
    public bool isEnd;

    [Header("Trigger Event")]
    public UnityEvent startDialougeEvent;
    public UnityEvent endDialougeEvent;
}



