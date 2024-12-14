using UnityEngine;
using UnityEngine.UI;

namespace Tizieria.Game
{
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

        public Transform Container => _hitMarker.transform;

        public Vector2 SpawnPos => _spawnPos.position;
    }
}
