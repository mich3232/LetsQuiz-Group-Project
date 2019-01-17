using System.Collections.Generic;
using UnityEngine;

namespace _LetsQuiz
{
    #region pooled object class

    // Identifies the pool that a GameObject came from
    public class PooledObject : MonoBehaviour
    {
        public SimpleObjectPool objectPool;
    }

    #endregion pooled object class

    // Simple object pooling class
    public class SimpleObjectPool : MonoBehaviour
    {
        #region variables

        [Header("Component")]
        public GameObject prefab;

        private Stack<GameObject> _inactivePrefabInstances = new Stack<GameObject>();

        #endregion variables

        #region methods

        #region get object

        // Retrieve an instance of the prefab from the pool
        public GameObject GetObject()
        {
            GameObject spawnedGameObject = null;

            if (_inactivePrefabInstances.Count > 0)
                spawnedGameObject = _inactivePrefabInstances.Pop();
            else
            {
                spawnedGameObject = Instantiate(prefab);

                var pooledObject = spawnedGameObject.AddComponent<PooledObject>();
                pooledObject.objectPool = this;
            }

            spawnedGameObject.SetActive(true);

            return spawnedGameObject;
        }

        #endregion get object

        #region return object

        // Return an instance of the prefab to the pool
        public void ReturnObject(GameObject gameObjectToReturn)
        {
            var pooledObject = gameObjectToReturn.GetComponent<PooledObject>();

            if (pooledObject && pooledObject.objectPool == this)
            {
                gameObjectToReturn.SetActive(false);
                _inactivePrefabInstances.Push(gameObjectToReturn);
            }
            else
            {
                Debug.LogWarning(gameObjectToReturn.name + " was returned to a pool.");
                Destroy(gameObjectToReturn);
            }
        }

        #endregion return object

        #endregion methods
    }
}