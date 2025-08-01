using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    void Awake()
    {
        MakePersistent("timer");
        MakePersistent("dialogue");
    }

    void MakePersistent(string objectName)
    {
        var obj = GameObject.Find(objectName);
        if (obj != null)
        {
            // Prevent duplicates using FindObjectsByType
            var foundObjs = Object.FindObjectsByType(obj.GetType(), FindObjectsSortMode.None);
            if (foundObjs.Length > 1)
            {
                Destroy(obj);
                return;
            }
            DontDestroyOnLoad(obj);
        }
    }
}
