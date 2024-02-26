using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    static T _instance;

    public static T instance
    {
        get
        {
            if (instanceExists)
            {
                return _instance;
            }

            CreateNewInstance();
            return _instance;
        }
    }

    public static bool instanceExists
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }

            return _instance != null;
        }
    }

    protected static void CreateNewInstance()
    {
        GameObject newGO = new(typeof(T).Name);
        _instance = newGO.AddComponent<T>();
    }

}