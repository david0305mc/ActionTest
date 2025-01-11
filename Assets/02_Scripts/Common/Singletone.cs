using UnityEngine;

public class Singletone<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T istance;

    public static T Instance
    {
        get
        {
            if (istance == null)
            {
                istance = (T)FindAnyObjectByType(typeof(T));

                if (istance == null)
                {
                    var singletonObject = new GameObject();
                    istance = singletonObject.AddComponent<T>();

                    DontDestroyOnLoad(singletonObject);
                }
            }
            return istance;
        }
    }
}
