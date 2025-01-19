using UnityEngine;
using System.Collections.Generic;

public class Hands : MonoBehaviour
{
    [SerializeField] private GameObject blockManagerObject;
    private BlockManager blockManager;

    public List<GameObject> blocks = new List<GameObject>();

    void Start()
    {
        blockManager = blockManagerObject.GetComponent<BlockManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            blocks.Add(other.gameObject);

            blockManager.HandsOnBlock(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            blocks.Remove(other.gameObject);
            if(blocks.Count < 1)
            {
                blockManager.PlayerLetGO();
                blockManager.HandsOffBlock();
            }
            else
                blockManager.HandsOnBlock(blocks[0]);
        }
    }

    public GameObject NearestBlock(GameObject player)
    {
        if (blocks.Count > 1)
        {
            float currentDistance = float.MaxValue;
            float shortestDistance = float.MaxValue;
            int blockIndex = -1;
            for(int i = 0; i < blocks.Count; i++)
            {
                currentDistance = Vector3.Distance(player.transform.position, blocks[i].transform.position);
                if(currentDistance < shortestDistance )
                {
                    shortestDistance = currentDistance;
                    blockIndex = i;
                }
            }
            return blocks[blockIndex];
        }
        else if(blocks.Count == 1)
            return blocks[0];
        else
            return null;
    }
}
