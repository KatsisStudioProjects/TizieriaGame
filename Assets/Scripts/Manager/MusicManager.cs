using System.Collections.Generic;
using System.Linq;
using Tizieria.Game;
using Tizieria.SO;
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

        [SerializeField]
        private AudioSource _source;

        private Queue<PreloadedNotedata> _unspawnedNotes;
        private readonly List<NoteData> _spawnedNotes = new();

        private float _currFallDuration = 1f;

        private float _currentTime;

        private void Awake()
        {
            Instance = this;

            _unspawnedNotes
                = new Queue<PreloadedNotedata>(_info.Notes
                    .OrderBy(note => note.Index)
                    .Select(note => new PreloadedNotedata() { Lane = note.Line, Time = (60f / _info.BPM) * note.Index })
                );
        }

        private void Update()
        {
            _currentTime += Time.deltaTime;

            TrySpawningNotes(_currentTime);

            foreach (var note in _spawnedNotes)
            {
                note.Time += Time.deltaTime * 1f / note.FallDuration;

                if (note.GameObject != null)
                {
                    note.Transform.position = Vector2.LerpUnclamped(note.Lane.SpawnPos, note.Lane.Container.position, note.Time);
                }

                if (note.Time > 1f)
                {
                    if (note.GameObject != null)
                    {
                        Destroy(note.GameObject);
                        note.GameObject = null;
                    }

                    /*if (!note.PendingRemoval && note.HitArea.IsAIController && note.CurrentTime > note.AIHitTiming)
                    {
                        note.HitArea.OnKeyDownSpring(note.Line);
                        HitNote(note.Line, note.HitArea.GetInstanceID());
                    }*/

                    if (note.Time > 1f + .1f)
                    {
                        /*note.HitArea.ShowHitInfo(_info.MissInfo);
                        note.PendingRemoval = true;*/
                    }
                }
            }
        }

        private void SpawnNote(int lineId)
        {
            var line = ResourceManager.Instance.Lines[lineId];

            var note = Instantiate(_notePrefab, line.Container);
            note.transform.position = line.SpawnPos;

            _spawnedNotes.Add(new()
            {
                GameObject = note,
                Transform = note.transform,
                Time = 0f,
                Lane = line,

                FallDuration = 1f
            });
        }

        private void TrySpawningNotes(float currentTime)
        {
            if (_unspawnedNotes.Count == 0) return;

            var closestUnspawnedNote = _unspawnedNotes.Peek();
                
            Debug.Log(closestUnspawnedNote.Time);
            if (currentTime > closestUnspawnedNote.Time - _currFallDuration)
            {
                _unspawnedNotes.Dequeue();
                SpawnNote(closestUnspawnedNote.Lane);//, currentTime - (closestUnspawnedNote.Time - _currFallDuration));
                TrySpawningNotes(currentTime);
            }
        }
    }

    public class NoteData
    {
        public GameObject GameObject;
        public Transform Transform;
        public Line Lane;
        public float Time;

        public float FallDuration;
    }

    public class PreloadedNotedata
    {
        public float Time;
        public int Lane;
    }
}
