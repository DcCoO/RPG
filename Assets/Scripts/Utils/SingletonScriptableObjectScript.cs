using UnityEngine;

namespace BitBox.Utils
{
    //[CreateAssetMenu(fileName = "SingletonScriptableObjectScript", menuName = "Scriptable Objects/SingletonScriptableObjectScript")]
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<T>(typeof(T).Name);
                    if (_instance == null)
                    {
                        Debug.LogError("Couldn't find an asset called " + typeof(T).Name);
                        return null;
                    }
                }
                return _instance;
            }
        }
    }
}
