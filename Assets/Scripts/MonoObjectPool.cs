using System.Collections.Generic;
using UnityEngine;

namespace PathfindingDemo
{
    public class MonoObjectPool<T> where T : MonoBehaviour
    {
        private readonly T prefab;
        private readonly Queue<T> pool = new();

        public MonoObjectPool(T prefab, int initialSize)
        {
            this.prefab = prefab;
            PopulatePool(initialSize);
        }

        private void PopulatePool(int objectsAmount)
        {
            for (int i = 0; i < objectsAmount; i++)
            {
                T newObject = MonoBehaviour.Instantiate(prefab);
                newObject.gameObject.SetActive(false);
                pool.Enqueue(newObject);
            }
        }

        public T GetFreeObject()
        {
            if (pool.Count == 0)
            {
                PopulatePool(10);
            }

            T newObject = pool.Dequeue();
            newObject.gameObject.SetActive(true);
            return newObject;
        }

        public void ReturnObjectToPool(T poolObject)
        {
            poolObject.gameObject.SetActive(false);
            pool.Enqueue(poolObject);
        }
    }
}
