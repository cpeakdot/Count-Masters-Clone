using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cpeak.cPool
{
    public class cPool : MonoBehaviour
    {
        public static cPool instance;

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
            [Tooltip("Set this value if you want your objects to return to the pool after certain amount of time \n 0 means you need to return them manually")]
            public float returnTime;
        }

        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }

            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                GameObject parent = new GameObject(pool.tag);
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                        GameObject newObj = Instantiate(pool.prefab, parent.transform);
                    newObj.SetActive(false);
                    objectPool.Enqueue(newObj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject GetPoolObject(string tag, Vector3 position, Quaternion rotation, bool returnObject = false, float returnTime = 1f)
        {
            // Find the objects pool
            Pool objectsPool = FindPool(tag);
            if (objectsPool == null) 
            {
                Debug.LogWarning("Check out the tag of the object");
                return null;
            };

            // If there aren't any pool objects, instantiate a new one
            if (!poolDictionary.ContainsKey(tag) || poolDictionary[tag].Count == 0)
            {
                 GameObject newobj = Instantiate(objectsPool.prefab, GameObject .Find(objectsPool.tag).transform);

                 newobj.transform.position = position;

                 newobj.transform.rotation = rotation;

                 if(objectsPool.returnTime != 0 || returnObject) ReturnObjectToPool(objectsPool, tag, newobj, returnObject, returnTime);

                 return newobj;

            }
            else
            {
                GameObject newObject = poolDictionary[tag].Dequeue();

                newObject.SetActive(true);

                newObject.transform.position = position;

                newObject.transform.rotation = rotation;

                if (objectsPool.returnTime != 0 || returnObject) ReturnObjectToPool(objectsPool, tag, newObject, returnObject, returnTime);

                return newObject;
            }
        }

        public void ReleaseObject(string tag, GameObject obj)
        {
            poolDictionary[tag].Enqueue(obj);

            obj.SetActive(false);
        }

        private void ReturnObjectToPool(Pool pool, string tag, GameObject prefab, bool customReturnTime = false, float returnTime = 1f)
        {
            if (customReturnTime) StartCoroutine(ReturnObjectRoutine(pool, tag, prefab, true, returnTime));
            else StartCoroutine(ReturnObjectRoutine(pool, tag, prefab));
        }

        private Pool FindPool(string tag)
        {
            foreach (Pool pool in pools)
            {
                if (pool.tag == tag) return pool;
            }
            return null;
        }

        IEnumerator ReturnObjectRoutine(Pool pool, string tag, GameObject prefab, bool customReturnTime = false, float returnTime = 1f)
        {
            float timer = 0;
            if (customReturnTime) timer = returnTime;
            else timer = pool.returnTime;
            yield return new WaitForSecondsRealtime(returnTime);
            ReleaseObject(tag, prefab);
        }
    }
}

