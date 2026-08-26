using UnityEngine;

namespace DeepScan
{
    [CreateAssetMenu(
        fileName = "Movement_New",
        menuName = "DeepScan/Movement Data"
    )]
    public class FishMovementData : ScriptableObject
    {
        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private float horizontalRange = 3f;

        [SerializeField]
        private float verticalRange = 1f;

        public float Speed => speed;

        public float HorizontalRange =>
            horizontalRange;

        public float VerticalRange =>
            verticalRange;
    }
}