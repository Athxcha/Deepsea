using UnityEngine;

namespace DeepScan
{
    public class LobbyController :
        MonoBehaviour
    {
        public void StartGame()
        {
            if (SceneFlowService.Instance ==
                null)
                return;


            SceneFlowService.Instance
                .StartGame();
        }


        public void OpenLeaderboard()
        {
            if (SceneFlowService.Instance ==
                null)
                return;


            SceneFlowService.Instance
                .OpenLeaderboard();
        }
    }
}