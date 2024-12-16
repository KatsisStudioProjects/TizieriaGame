using UnityEngine;

namespace Tizieria.Manager
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { private set; get; }

        [SerializeField]
        private AudioSource _source;

        private float _currentTime;
        private float _lastTime = -1f;

        private int _loop = 0;

        public float Time => _source.time + (_source.clip.length * _loop);

        public float Length => _source.clip.length;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            _lastTime = _currentTime;
            _currentTime = _source.time;

            if (_lastTime > _currentTime)
            {
                _loop++;
            }
        }
    }
}
