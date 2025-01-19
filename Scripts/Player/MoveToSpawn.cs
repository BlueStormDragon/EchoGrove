using System.Collections.Generic;
using UnityEngine;

public class MoveToSpawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;

    private void Start()
    {
        if (PlayerPrefs.HasKey("SpawnPointY"))
        {
            int y = (int)SaveManager.instance.LoadSpawnPoint().y;
            if(y > -1)
            {
                Vector3 targetPos = new Vector3(spawnPoints[PlayerPrefs.GetInt("SpawnPointY")].transform.position.x,
                transform.position.y, spawnPoints[PlayerPrefs.GetInt("SpawnPointY")].transform.position.z);

                transform.position = targetPos;
            }
        }
    }
}
