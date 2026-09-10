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


        // =====================================================
        // STAGE COMPATIBILITY
        // เก็บไว้ให้โค้ดเก่าที่ยังเรียก CurrentStage
        // =====================================================

        public StageData CurrentStage
        {
            get;
            private set;
        }


        // =====================================================
        // FISH
        // =====================================================

        private readonly List<FishData> caughtFish =
            new List<FishData>();


        // ระบบใหม่
        public IReadOnlyList<FishData> CaughtFish =>
            caughtFish;


        // ระบบเก่า
        // SubmitTerminal ยังเรียก ScannedFish
        public IReadOnlyList<FishData> ScannedFish =>
            caughtFish;


        // =====================================================
        // SCORE
        // =====================================================

        public int BaseScore
        {
            get;
            private set;
        }


        public int CorrectAnswers
        {
            get;
            private set;
        }


        public int Multiplier
        {
            get;
            private set;
        } = 1;


        public int FinalScore
        {
            get;
            private set;
        }


        // ระบบเก่ายังเรียก Score
        public int Score
        {
            get
            {
                if (FinalScore > 0)
                    return FinalScore;

                return BaseScore;
            }
        }


        // =====================================================
        // STATE
        // =====================================================

        public bool FishSubmitted
        {
            get;
            private set;
        }


        // ระบบเก่ายังเรียก DataSubmitted
        public bool DataSubmitted =>
            FishSubmitted;


        public bool LeaderboardSubmitted
        {
            get;
            private set;
        }


        // =====================================================
        // UNITY
        // =====================================================

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


        // =====================================================
        // NEW GAME
        // =====================================================

        public void StartNewGame()
        {
            caughtFish.Clear();

            BaseScore = 0;

            CorrectAnswers = 0;

            Multiplier = 1;

            FinalScore = 0;

            FishSubmitted = false;

            LeaderboardSubmitted = false;
        }


        // =====================================================
        // OLD STAGE SYSTEM COMPATIBILITY
        //
        // SceneFlowService / StageTerminal เก่ายังใช้ตัวนี้
        // =====================================================

        public void StartStage(StageData stage)
        {
            CurrentStage = stage;

            StartNewGame();

            Debug.Log(
                "Game started with stage compatibility mode."
            );
        }


        // =====================================================
        // REGISTER FISH
        // =====================================================

        public bool RegisterFish(FishData fish)
        {
            if (fish == null)
                return false;


            // สำคัญ:
            // ไม่เช็ค Contains
            // เพราะต้องจับปลา Species เดิมหลายตัวได้
            caughtFish.Add(fish);


            Debug.Log(
                "Fish caught: " +
                fish.FishName
            );


            return true;
        }


        // =====================================================
        // HAS CAUGHT
        // =====================================================

        public bool HasCaught(FishData fish)
        {
            if (fish == null)
                return false;


            return caughtFish.Contains(fish);
        }


        // ระบบ Quiz เก่าอาจเรียก HasScanned
        public bool HasScanned(FishData fish)
        {
            return HasCaught(fish);
        }


        // =====================================================
        // SUBMIT FISH
        // ระบบใหม่
        // =====================================================

        public int SubmitFish()
        {
            if (FishSubmitted)
                return BaseScore;


            BaseScore = 0;


            foreach (FishData fish in caughtFish)
            {
                if (fish == null)
                    continue;


                BaseScore +=
                    fish.DiscoveryScore;
            }


            FishSubmitted = true;


            Debug.Log(
                "Fish Score = " +
                BaseScore
            );


            return BaseScore;
        }


        // =====================================================
        // OLD SUBMIT SYSTEM COMPATIBILITY
        //
        // SubmitTerminal เก่ายังเรียก SubmitData()
        // =====================================================

        public int SubmitData()
        {
            return SubmitFish();
        }


        // =====================================================
        // QUIZ
        // =====================================================

        public void RegisterCorrectAnswer()
        {
            CorrectAnswers++;


            Debug.Log(
                "Correct Answers = " +
                CorrectAnswers
            );
        }


        // =====================================================
        // OLD QUIZ COMPATIBILITY
        //
        // ถ้ามี QuizController เก่าที่ยังเรียก AddQuizScore
        // จะไม่ compile error
        // =====================================================

        public void AddQuizScore(int amount)
        {
            // ระบบใหม่ไม่ได้ใช้ BonusScore แล้ว
            // เก็บ method ไว้เพื่อให้ code เก่า compile ผ่าน
            Debug.Log(
                "AddQuizScore compatibility call: " +
                amount
            );
        }


        // =====================================================
        // FINAL SCORE
        //
        // 0 correct = x1
        // 1 correct = x2
        // 2 correct = x4
        // 3 correct = x8
        // =====================================================

        public int CalculateFinalScore()
        {
            switch (CorrectAnswers)
            {
                case 1:
                    Multiplier = 2;
                    break;

                case 2:
                    Multiplier = 4;
                    break;

                case 3:
                    Multiplier = 8;
                    break;

                default:
                    Multiplier = 1;
                    break;
            }


            FinalScore =
                BaseScore *
                Multiplier;


            Debug.Log(
                "Final Score = " +
                BaseScore +
                " x " +
                Multiplier +
                " = " +
                FinalScore
            );


            return FinalScore;
        }


        // =====================================================
        // LEADERBOARD
        // =====================================================

        public void MarkLeaderboardSubmitted()
        {
            LeaderboardSubmitted = true;
        }
    }
}