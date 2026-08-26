using UnityEngine;

namespace DeepScan
{
    [CreateAssetMenu(
        fileName = "Fish_New",
        menuName = "DeepScan/Fish Data"
    )]
    public class FishData : GameData
    {
        [Header("Display")]

        [SerializeField]
        private string fishName;

        [SerializeField]
        private Sprite sprite;

        [TextArea]
        [SerializeField]
        private string description;


        [Header("Scanning")]

        [SerializeField]
        private float scanDuration = 3f;

        [SerializeField]
        private GameObject scanShapePrefab;


        [Header("Movement")]

        [SerializeField]
        private FishMovementData movement;


        [Header("Score")]

        [SerializeField]
        private int discoveryScore = 100;


        public string FishName => fishName;

        public Sprite Sprite => sprite;

        public string Description =>
            description;

        public float ScanDuration =>
            scanDuration;

        public GameObject ScanShapePrefab =>
            scanShapePrefab;

        public FishMovementData Movement =>
            movement;

        public int DiscoveryScore =>
            discoveryScore;
    }
}