using System.Collections.Generic;
using UnityEngine;

namespace DeepScan
{
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance
        {
            get;
            private set;
        }


        public StageData CurrentStage
        {
            get;
            private set;
        }


        private readonly List<FishData>
            scannedFish = new();


        public IReadOnlyList<FishData>
            ScannedFish => scannedFish;


        public int Score
        {
            get;
            private set;
        }


        public bool DataSubmitted
        {
            get;
            private set;
        }


        private void Awake()
        {
            if (Instance != null &&
                Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }


        public void StartStage(StageData stage)
        {
            CurrentStage = stage;

            scannedFish.Clear();

            Score = 0;

            DataSubmitted = false;
        }


        public bool RegisterFish(
            FishData fish)
        {
            if (fish == null)
                return false;

            if (CurrentStage == null)
                return false;

            if (!CurrentStage.ContainsFish(fish))
            {
                Debug.LogWarning(
                    "Fish does not belong to current stage."
                );

                return false;
            }

            if (scannedFish.Contains(fish))
                return false;

            scannedFish.Add(fish);

            Debug.Log(
                "Fish scanned: " +
                fish.FishName
            );

            return true;
        }


        public bool HasScanned(
            FishData fish)
        {
            return scannedFish.Contains(fish);
        }


        public int SubmitData()
        {
            if (DataSubmitted)
                return 0;

            int gainedScore = 0;

            foreach (FishData fish
                     in scannedFish)
            {
                gainedScore +=
                    fish.DiscoveryScore;
            }

            Score += gainedScore;

            DataSubmitted = true;

            return gainedScore;
        }


        public void AddQuizScore(
            int amount)
        {
            Score += amount;
        }
    }
}