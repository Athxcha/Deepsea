using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeepScan
{
    public static class LeaderboardStorage
    {
        private const string DataKey =
            "DeepSeaLeaderboard";

        private const string PlayerNumberKey =
            "DeepSeaNextPlayerNumber";


        [Serializable]
        public class Entry
        {
            public string playerName;
            public int score;
        }


        [Serializable]
        private class SaveData
        {
            public List<Entry> entries =
                new List<Entry>();
        }


        public static void AddScore(
            int score)
        {
            SaveData data =
                LoadData();


            int playerNumber =
                PlayerPrefs.GetInt(
                    PlayerNumberKey,
                    1
                );


            Entry entry =
                new Entry
                {
                    playerName =
                        "PLAYER " +
                        playerNumber,

                    score = score
                };


            data.entries.Add(
                entry
            );


            playerNumber++;


            PlayerPrefs.SetInt(
                PlayerNumberKey,
                playerNumber
            );


            SaveDataToPrefs(
                data
            );
        }


        public static List<Entry>
            GetEntries()
        {
            SaveData data =
                LoadData();


            data.entries.Sort(
                (a, b) =>
                    b.score.CompareTo(
                        a.score
                    )
            );


            return data.entries;
        }


        public static void ResetLeaderboard()
        {
            PlayerPrefs.DeleteKey(
                DataKey
            );

            PlayerPrefs.DeleteKey(
                PlayerNumberKey
            );

            PlayerPrefs.Save();
        }


        private static SaveData LoadData()
        {
            if (!PlayerPrefs.HasKey(
                    DataKey))
            {
                return new SaveData();
            }


            string json =
                PlayerPrefs.GetString(
                    DataKey
                );


            SaveData data =
                JsonUtility
                    .FromJson<SaveData>(
                        json
                    );


            if (data == null)
                data = new SaveData();


            return data;
        }


        private static void SaveDataToPrefs(
            SaveData data)
        {
            string json =
                JsonUtility.ToJson(
                    data
                );


            PlayerPrefs.SetString(
                DataKey,
                json
            );


            PlayerPrefs.Save();
        }
    }
}