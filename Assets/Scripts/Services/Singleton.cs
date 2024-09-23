using UnityEngine;

namespace Services
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        protected bool isOrigin;
        public static T Default { get => _instance; }
        public static bool HasDefault { get; private set; }
        protected virtual void Awake()
        {
            Initialization();
        }
        private void Initialization()
        {
            if (!HasDefault)
            {
                _instance = (T)this;
                HasDefault = true;
                isOrigin = true;
            }
            else
            {
                DestroyImmediate(this);
            }
        }

    }
}