using TMPro;
using UnityEngine;

namespace Tizieria.Manager
{
    /// <summary>
    /// Manage the time of the game
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { private set; get; }

        [SerializeField]
        private AudioSource _source;

        [SerializeField]
        private TMP_Text _startTimerText;

        private float _currentTime;
        private float _lastTime = -1f;

        private int _loop = 0;

        private float _startTimer = 3f;

        /// <summary>
        /// Time is current music time, multiplied by the length of the song by the amount of loops done, we also substract the starting timer
        /// </summary>
        public float Time => _source.time + (_source.clip.length * _loop) - _startTimer;

        public float Length => _source.clip.length;

        public bool _didStart;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!_didStart) return;

            if (_startTimer > 0f)
            {
                // Countdown before game start
                _startTimer -= UnityEngine.Time.deltaTime;
                if (_startTimer <= 0f)
                {
                    _source.Play();
                    _startTimerText.text = string.Empty;
                }
                else
                {
                    _startTimerText.text = $"{Mathf.CeilToInt(_startTimer)}";
                }
            }
            else
            {
                // If current time is inferior of the one checked last Update, it means the song looped
                _lastTime = _currentTime;
                _currentTime = _source.time;

                if (_lastTime > _currentTime)
                {
                    _loop++;
                }
            }

        }

        public void StartTimer()
        {
            _didStart = true;
        }
    }
}
