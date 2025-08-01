using UnityEngine;

public class DialoguePrefabSpawner : MonoBehaviour
{
    public GameObject dialoguePrefab;
    private static bool spawned = false;

    void Start()
    {
        if (!spawned)
        {
            GameObject instance = Instantiate(dialoguePrefab);
            DontDestroyOnLoad(instance);
            spawned = true;
        }
    }
}
