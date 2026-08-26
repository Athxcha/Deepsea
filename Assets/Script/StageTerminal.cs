using UnityEngine;

namespace DeepScan
{
    public class StageTerminal :
        MonoBehaviour
    {
        [SerializeField]
        private StageData stage;


        private bool playerInside;


        private void Update()
        {
            if (!playerInside)
                return;


            if (Input.GetKeyDown(
                KeyCode.E))
            {
                SceneFlowService.Instance
                    .StartDive(stage);
            }
        }


        private void OnTriggerEnter(
            Collider other)
        {
            if (other.CompareTag(
                "Player"))
            {
                playerInside = true;

                Debug.Log(
                    "Press E to dive: " +
                    stage.StageName
                );
            }
        }


        private void OnTriggerExit(
            Collider other)
        {
            if (other.CompareTag(
                "Player"))
            {
                playerInside = false;
            }
        }
    }
}