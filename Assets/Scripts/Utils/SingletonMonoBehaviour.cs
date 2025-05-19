using UnityEngine;

namespace BitBox.Utils
{
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(Instance.gameObject);
            Instance = (T)this;
        }

        protected virtual void OnDestroy() => Instance = null;
    }
}