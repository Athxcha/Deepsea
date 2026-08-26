using System.Collections.Generic;
using UnityEngine;

namespace DeepScan
{
    public class FishSpawner :
        MonoBehaviour
    {
        [SerializeField]
        private FishActor fishPrefab;

        [SerializeField]
        private Transform[] spawnPoints;


        public void SpawnStageFish(
            StageData stage)
        {
            if (stage == null)
                return;


            List<Transform> available =
                new List<Transform>(
                    spawnPoints
                );


            foreach (FishSpawnEntry entry
                     in stage.Fish)
            {
                for (int i = 0;
                     i < entry.amount;
                     i++)
                {
                    if (available.Count == 0)
                    {
                        Debug.LogWarning(
                            "Not enough fish spawn points."
                        );

                        return;
                    }


                    int index =
                        Random.Range(
                            0,
                            available.Count
                        );


                    Transform spawn =
                        available[index];

                    available.RemoveAt(
                        index
                    );


                    FishActor fish =
                        Instantiate(
                            fishPrefab,
                            spawn.position,
                            Quaternion.identity
                        );


                    fish.Initialize(
                        entry.fish
                    );
                }
            }
        }
    }
}