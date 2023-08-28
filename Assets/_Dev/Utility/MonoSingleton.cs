using UnityEngine;
using Sirenix.OdinInspector;
/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class MonoSingleton<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
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
                        singleton.name = "(MonoSingleton) " + typeof(T);
                        
                        DontDestroyOnLoad(singleton);
                    }
                }

                return instance;
            }
        }
    }
    public virtual void Init(){

    }

    protected virtual void Awake()
    {
        Init();
    }
    public virtual void Dispose(){
        Destroy(this.gameObject);
    }
}