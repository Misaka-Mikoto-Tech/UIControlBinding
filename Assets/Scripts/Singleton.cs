using UnityEngine;
using System.Collections;

namespace SDGame
{
    public abstract class Singleton<T> where T : new()
    {
        private static T _instance;
        static object _lock = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new T();
                    }
                }
                return _instance;
            }
        }

//#if UNITY_EDITOR
        /// <summary>
        /// 编辑器模式下方便释放单例对象
        /// </summary>
        virtual public void Release()
        {
            _instance = default(T);
        }
//#endif
    }

    public class UnitySingleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        //obj.hideFlags = HideFlags.DontSave;
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        _instance = (T)obj.AddComponent(typeof(T));
                    }
                }
                return _instance;
            }
        }
        public virtual void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}