using System.Collections;
using TMPro;
using UnityEngine;

namespace DeepScan
{
    public class LoadingController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text loadingText;

        [SerializeField]
        private float loadingTime = 1.5f;


        private void Start()
        {
            if (SceneFlowService.Instance == null)
            {
                Debug.LogError(
                    "SceneFlowService does not exist!"
                );

                return;
            }

            if (loadingText != null)
            {
                loadingText.text =
                    SceneFlowService.Instance
                        .LoadingMessage;
            }

            StartCoroutine(LoadNextScene());
        }


        private IEnumerator LoadNextScene()
        {
            yield return new WaitForSeconds(
                loadingTime
            );

            SceneFlowService.Instance
                .CompleteLoading();
        }
    }
}