using System;
using UnityEngine;
using System.Collections;

namespace UnityUtil
{
    public class UtilSingletonFolder : UtilObjectSingleton<UtilSingletonFolder>
    {
        override protected void InitInstance()
        {
            gameObject.name = "Singleton Folder";
        }
    }

    public class UtilObjectSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static private T m_sInstance;

        public static T Instance
        {
            get
            {
                if (m_sInstance == null)
                {
                    var obj = (T)FindObjectOfType<T>();
                    if (obj == null)
                    {
                        //m_sInstance = new GameObject("Singleton Object", typeof(T)) as T;
                        GameObject gobj = new GameObject("Singleton Object", typeof(T));
                        if (gobj == null)
                        {
                            Debug.Log("Create failed - singleton GameObject");
                        }
                        m_sInstance = gobj.GetComponent<T>();
                        if (m_sInstance == null)
                        {
                            Debug.Log("Create failed - Singleton Component");
                        }
                        else
                        {
                            m_sInstance.gameObject.transform.SetParent(UtilSingletonFolder.Instance.gameObject.transform);
                            DontDestroyOnLoad(m_sInstance.gameObject);
                            m_sInstance.GetComponent<UtilObjectSingleton<T>>().InitInstance();
                        }
                    }
                    else
                        m_sInstance = obj.GetComponent<T>();
                }
                return m_sInstance;
            }
        }

        static public void DestroySingleton()
        {
            if (m_sInstance == null)
                return;
            Debug.Log("Destory singleton object : " + m_sInstance.gameObject.name);
            Destroy(m_sInstance.gameObject);
        }

        virtual protected void InitInstance()
        {

        }
    }

    public class UtilHalfSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static private T m_sInstance;

        /**
            Returns the instance of this singleton.
        */
        public static T Instance
        {
            get
            {
                if (m_sInstance == null)
                {
                    m_sInstance = (T)FindObjectOfType(typeof(T));

                    if (m_sInstance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                    }
                }

                return m_sInstance;
            }
        }
    }

    public class UtilSingleton<T> where T : UtilSingleton<T>, new()
    {
        static private T m_sInstance = default(T);

        public static T Instance
        {
            get
            {
                if (m_sInstance == null)
                {
                    m_sInstance = new T();
                    m_sInstance.InitInstance();
                }
                return m_sInstance;
            }
        }

        static public void DestroySingleton()
        {
            m_sInstance = default(T);
        }

        virtual protected void InitInstance()
        {

        }
    }
}
