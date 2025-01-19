using System.Collections;
using UnityEngine;

public class PickUpBlueberry : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private PlayerMovement pMovement;

    private void Start()
    {
        pMovement = player.GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(PickUp());
        }
    }

    private IEnumerator PickUp()
    {
        pMovement.StopMovement();
        pMovement.ChangeAnimation("PickingBerry");
        GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(1f);
        
        BerryManager.instance.GetBerry(gameObject);
        pMovement.StartMovement();
        Destroy(gameObject);
    }
}