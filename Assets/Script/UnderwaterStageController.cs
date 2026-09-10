using UnityEngine;

namespace DeepScan
{
    public class UnderwaterStageController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FishSpawner fishSpawner;
        [SerializeField] private DiveTimer diveTimer;

        [Header("Game")]
        [SerializeField] private float diveDuration = 180f;


        private void Start()
        {
            if (GameSession.Instance == null)
            {
                Debug.LogError("GameSession does not exist.");
                return;
            }

            if (fishSpawner == null)
            {
                Debug.LogError("FishSpawner is missing.");
                return;
            }

            if (diveTimer == null)
            {
                Debug.LogError("DiveTimer is missing.");
                return;
            }


            fishSpawner.SpawnFish();

            diveTimer.StartTimer(diveDuration);
        }
    }
}