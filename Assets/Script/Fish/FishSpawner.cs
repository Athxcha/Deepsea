using System.Collections.Generic;
using UnityEngine;

namespace DeepScan
{
    public class FishSpawner :
        MonoBehaviour
    {
        [System.Serializable]
        public class FishSpawnSetting
        {
            public FishData fish;

            [Min(1)]
            public int amount = 1;
        }


        [Header("Prefab")]

        [SerializeField]
        private FishActor fishPrefab;


        [Header("Spawn Points")]

        [SerializeField]
        private Transform[] spawnPoints;


        [Header("Fish In Map")]

        [SerializeField]
        private FishSpawnSetting[] fish;


        public void SpawnFish()
        {
            if (fishPrefab == null)
            {
                Debug.LogError(
                    "Fish Prefab is missing."
                );

                return;
            }


            List<Transform> available =
                new List<Transform>(
                    spawnPoints
                );


            foreach (
                FishSpawnSetting entry
                in fish)
            {
                if (entry == null ||
                    entry.fish == null)
                    continue;


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


                    FishActor fishActor =
                        Instantiate(
                            fishPrefab,
                            spawn.position,
                            Quaternion.identity
                        );


                    fishActor.Initialize(
                        entry.fish
                    );
                }
            }
        }
    }
}