using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeepScan
{
    public class SceneFlowService : MonoBehaviour
    {
        public static SceneFlowService Instance
        {
            get;
            private set;
        }

        [Header("Scene Names")]

        [SerializeField]
        private string lobbyScene = "Lobby";

        [SerializeField]
        private string underwaterScene = "Underwater";

        [SerializeField]
        private string loadingScene = "Loading";


        public string PendingScene
        {
            get;
            private set;
        }

        public string LoadingMessage
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


        public void StartDive(StageData stage)
        {
            if (stage == null)
            {
                Debug.LogError(
                    "StartDive failed: StageData is null."
                );

                return;
            }

            if (GameSession.Instance == null)
            {
                Debug.LogError(
                    "StartDive failed: GameSession does not exist."
                );

                return;
            }

            GameSession.Instance.StartStage(stage);

            PendingScene = underwaterScene;

            LoadingMessage = "DESCENDING...";

            SceneManager.LoadScene(loadingScene);
        }


        public void Surface()
        {
            PendingScene = lobbyScene;

            LoadingMessage = "SURFACING...";

            SceneManager.LoadScene(loadingScene);
        }


        public void CompleteLoading()
        {
            if (string.IsNullOrEmpty(PendingScene))
            {
                Debug.LogError(
                    "PendingScene is empty."
                );

                return;
            }

            SceneManager.LoadScene(PendingScene);
        }
    }
}