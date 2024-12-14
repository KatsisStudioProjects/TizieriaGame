using System;
using System.Collections.Generic;
using System.Linq;
using Tizieria.SO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Tizieria.Manager
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { private set; get; }

        [SerializeField]
        private MusicInfo _info;

        [SerializeField]
        private GameObject _notePrefab;

        private Queue<PreloadedNotedata> _unspawnedNotes;
        private readonly List<NoteData> _spawnedNotes = new();

        private float _currFallDuration = 1f;

        private void Awake()
        {
            Instance = this;

            _unspawnedNotes
                = new Queue<PreloadedNotedata>(_info.Notes
                    .OrderBy(note => note.Index)
                    .Select(note => new PreloadedNotedata() { Lane = note.Line, Time = note.Index * _info.BPM })
                );
        }

        private void Update()
        {
            
        }

        private void SpawnNote(int lineId, float currentTime)
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

        private void TrySpawningNotes(float currentTime)
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
        public float Time;
    }

    public class PreloadedNotedata
    {
        public float Time;
        public int Lane;
    }
}
