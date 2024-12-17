using UnityEngine;
using UnityEngine.UI;

namespace Tizieria.Game
{
    /// <summary>
    /// Store information about a rhythm line
    /// </summary>
    public class Line : MonoBehaviour
    {
        [SerializeField]
        private Image _hitMarker;

        [SerializeField]
        private Transform _spawnPos;

        public void Click()
        {
            _hitMarker.color = Color.white;
        }

        public void Release()
        {
            _hitMarker.color = Color.black;
        }

        /// <summary>
        /// Location where the hit marker is
        /// </summary>
        public Transform Container => _hitMarker.transform;

        /// <summary>
        /// Location where notes spawn
        /// </summary>
        public Vector2 SpawnPos => _spawnPos.position;
    }
}
