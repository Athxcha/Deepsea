using UnityEngine;

namespace DeepScan
{
    public abstract class GameData : ScriptableObject
    {
        [SerializeField]
        private string id;

        public string ID => id;
    }
}