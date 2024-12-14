using System.Collections.Generic;
using Tizieria.Game;
using UnityEngine;

namespace Tizieria.Manager
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { private set; get; }

        [SerializeField]
        private GameObject _notePrefab;

        private Queue<NoteData> _unspawnedNotes;
        private readonly List<NoteData> _spawnedNotes = new();

        private float _currFallDuration = 1f;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            
        }

        private void SpawnNote(int lineId, double currentTime)
        {
            var line = ResourceManager.Instance.Lines[lineId];

            var note = Instantiate(_notePrefab, line.Container);
            note.transform.position = line.SpawnPos;

            _spawnedNotes.Add(new()
            {
                RT = note.transform,
                Time = currentTime,
                Lane = lineId,
            });
        }

        private void TrySpawningNotes(double currentTime)
        {
            if (_unspawnedNotes.Count == 0) return;

            var closestUnspawnedNote = _unspawnedNotes.Peek();

            if (currentTime > closestUnspawnedNote.Time - _currFallDuration)
            {
                _unspawnedNotes.Dequeue();
                SpawnNote(closestUnspawnedNote.Lane, currentTime - (closestUnspawnedNote.Time - _currFallDuration));
                TrySpawningNotes(currentTime);
            }
        }
    }

    public class NoteData
    {
        public Transform RT;
        public int Lane;
        public double Time;
    }
}
