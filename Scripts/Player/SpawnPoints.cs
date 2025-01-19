using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [Tooltip("x is the level (1-3) and y is the index (1-?)"), SerializeField] private Vector2 spawnPoint;

    private void Start()
    {
        //the spawnPoint vector2 are one unity larger because it makes 
        //it easier when inputing the values in the inspector
        spawnPoint -= Vector2.one;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SaveManager.instance.SetSpawnPoint((int)spawnPoint.x, (int)spawnPoint.y);
        }
    }
}

