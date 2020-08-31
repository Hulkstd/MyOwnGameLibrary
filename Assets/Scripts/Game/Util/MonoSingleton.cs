﻿using UnityEngine;

namespace Game.Util
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        private static readonly object Lock = new object();
        private static bool _applicationIsQuitting;

        protected virtual bool ShouldRename => false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.Log(
                        $"[MonoSingleton]Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                    return _instance;
                }

                lock (Lock)
                {
                    if (_instance)
                        return _instance;

                    _instance = (T) FindObjectOfType(typeof(T));

                    if (!_instance)
                    {
                        _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                        if (!_instance)
                        {
                            Debug.LogError("[MonoSingleton]Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                        }

                        Debug.Log($"[MonoSingleton]An instance of {typeof(T)} is needed in the scene, so '{_instance.name}' was created with DontDestroyOnLoad.");
                    }

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError(
                            $"[MonoSingleton]Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                    }

                    return _instance;
                }
            }
        }

        #region Mono

        protected virtual void Awake()
        {
            if (_instance &&
                _instance != this)
            {
                Debug.LogWarning($"{typeof(T)} already exist!");
                Destroy(gameObject);
                return;
            }
            
            if (!_instance)
            {
                _instance = (T) this;
            }
            
            if (ShouldRename)
            {
                name = typeof(T).ToString();
            }
            
            DontDestroyOnLoad(gameObject);
        }
        
        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        #endregion

        public void EmptyMethod()
        {
        }
    }
}
