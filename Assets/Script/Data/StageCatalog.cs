using UnityEngine;

namespace DeepScan
{
    [CreateAssetMenu(
        fileName = "StageCatalog",
        menuName = "DeepScan/Stage Catalog"
    )]
    public class StageCatalog :
        ScriptableObject
    {
        [SerializeField]
        private StageData[] stages;

        public StageData[] Stages =>
            stages;
    }
}