using UnityEngine;


public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance
    {
        get;
        protected set;
    }

    public static bool InstanceExists
    {
        get => Instance != null;
    }

    void Awake()
    {
        if (InstanceExists)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = (T)this;
        }

        OnPostAwake();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        OnPostDestroy();
    }

    protected virtual void OnPostAwake() { }
    protected virtual void OnPostDestroy() { }
}
