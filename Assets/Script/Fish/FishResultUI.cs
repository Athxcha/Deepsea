using UnityEngine;

namespace DeepScan
{
    public class FishResultUI :
        MonoBehaviour
    {
        [SerializeField]
        private Transform content;


        private void Start()
        {
            BuildResult();
        }


        public void BuildResult()
        {
            if (GameSession.Instance == null)
            {
                Debug.LogError(
                    "GameSession does not exist."
                );

                return;
            }


            ClearList();


            foreach (
                FishData fish
                in GameSession.Instance.CaughtFish)
            {
                if (fish == null)
                    continue;


                if (fish.ResultPrefab == null)
                {
                    Debug.LogWarning(
                        fish.FishName +
                        " has no ResultPrefab."
                    );

                    continue;
                }


                Instantiate(
                    fish.ResultPrefab,
                    content
                );
            }
        }


        private void ClearList()
        {
            foreach (
                Transform child
                in content)
            {
                Destroy(
                    child.gameObject
                );
            }
        }
    }
}