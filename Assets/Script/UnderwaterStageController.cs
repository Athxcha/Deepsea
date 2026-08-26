using UnityEngine;

namespace DeepScan
{
    public class UnderwaterStageController :
        MonoBehaviour
    {
        [SerializeField]
        private FishSpawner fishSpawner;

        [SerializeField]
        private DiveTimer diveTimer;


        private void Start()
        {
            if (GameSession.Instance ==
                null)
            {
                Debug.LogError(
                    "GameSession does not exist."
                );

                return;
            }


            StageData stage =
                GameSession.Instance
                    .CurrentStage;


            if (stage == null)
            {
                Debug.LogError(
                    "No current StageData."
                );

                return;
            }


            fishSpawner
                .SpawnStageFish(stage);


            diveTimer
                .StartTimer(
                    stage.DiveDuration
                );
        }
    }
}