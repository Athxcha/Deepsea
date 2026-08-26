using UnityEngine;

namespace DeepScan
{
    public class ScanPoint : MonoBehaviour
    {
        private ScanPointPool pool;

        private float lifetime;
        private bool useLifetime;

        public void Activate(
            ScanPointPool owner,
            Vector3 worldPosition,
            Transform parent,
            float duration)
        {
            pool = owner;

            transform.SetParent(parent, true);
            transform.position = worldPosition;

            lifetime = duration;
            useLifetime = duration > 0f;

            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!useLifetime)
                return;

            lifetime -= Time.deltaTime;

            if (lifetime <= 0f)
            {
                Release();
            }
        }

        public void Release()
        {
            if (!gameObject.activeSelf)
                return;

            pool.Release(this);
        }
    }
}