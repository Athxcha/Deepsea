using System.Collections.Generic;
using UnityEngine;

namespace DeepScan
{
    public class ScanPointPool : MonoBehaviour
    {
        [SerializeField]
        private ScanPoint prefab;

        [SerializeField]
        private int initialSize = 500;

        private readonly Queue<ScanPoint> pool =
            new Queue<ScanPoint>();

        private void Awake()
        {
            for (int i = 0; i < initialSize; i++)
            {
                CreateNewPoint();
            }
        }

        private ScanPoint CreateNewPoint()
        {
            ScanPoint point =
                Instantiate(prefab, transform);

            point.gameObject.SetActive(false);

            pool.Enqueue(point);

            return point;
        }

        public ScanPoint Get(
            Vector3 position,
            Transform parent = null,
            float lifetime = 0f)
        {
            if (pool.Count == 0)
            {
                CreateNewPoint();
            }

            ScanPoint point = pool.Dequeue();

            point.Activate(
                this,
                position,
                parent,
                lifetime
            );

            return point;
        }

        public void Release(ScanPoint point)
        {
            point.transform.SetParent(
                transform,
                false
            );

            point.gameObject.SetActive(false);

            pool.Enqueue(point);
        }
    }
}