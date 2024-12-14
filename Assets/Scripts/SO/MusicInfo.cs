using UnityEngine;

namespace Tizieria.SO
{
    [CreateAssetMenu(fileName = "MusicInfo", menuName = "ScriptableObjects/MusicInfo")]
    public class MusicInfo : ScriptableObject
    {
        public string Name;
        public int BPM;

        public NoteData[] Notes;
    }

    [System.Serializable]
    public class NoteData
    {
        public int Line;
        public float Index;
    }
}