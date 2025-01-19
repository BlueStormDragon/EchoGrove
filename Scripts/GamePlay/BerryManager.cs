using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BerryManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> berries;
    [SerializeField] TextMeshProUGUI berryText;

    public static BerryManager instance;

    private int berryCount = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DestroyAlreadyPickedBerries();
        BerryCountOnSpawn();
    }

    public void GetBerry(GameObject berry)
    {
        BerryIsPicked(berry);

        berryCount++;
        berryText.text = berryCount.ToString() + "/15";
        SaveManager.instance.SetBerryCount(berryCount); 
    }

    private void BerryCountOnSpawn()
    {
        berryCount = SaveManager.instance.GetBerryCount();
        berryText.text = berryCount.ToString() + "/15";
    }

    private void DestroyAlreadyPickedBerries()
    {
        for(int i = 0; i < berries.Count; i++)
        {
            if(SaveManager.instance.IsBerryPicked((int)SaveManager.instance.LoadSpawnPoint().x, i))
            {
                berries[i].SetActive(false);
            }
        }
    }

    public void BerryIsPicked(GameObject berry)
    {
        SaveManager.instance.SetPickedBerry((int)SaveManager.instance.LoadSpawnPoint().x, berries.IndexOf(berry));
    }
}
