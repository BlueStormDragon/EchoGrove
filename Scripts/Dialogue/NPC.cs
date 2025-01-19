using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private GameObject diaTriggerObject;
    private DialogeTrigger diaTrigger;
    private Animator animator;
    private string currentAnim = "";
   
    private void Start()
    {
        diaTrigger = diaTriggerObject.GetComponent<DialogeTrigger>();
        animator = GetComponent<Animator>();
    }

    public void Dialogue(GameObject player)
    {
        ChangeAnimation("Talking");
        diaTrigger.StartDialogue(player);
    }

    public void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if(currentAnim != animation)
        {
            currentAnim= animation;
            animator.CrossFade(animation, crossfade);
        }
    }
}
