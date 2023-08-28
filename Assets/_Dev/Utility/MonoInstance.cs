using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class MonoInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = "(MonoInstance) " + typeof(T);
                    }
                }

                return instance;
            }
        }
    }

    public void Ping()
    {
    }
    public virtual void Init(){

    }

    #region override method
    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }
    public virtual void Dispose(){
        Destroy(this.gameObject);
    }
    #endregion
}