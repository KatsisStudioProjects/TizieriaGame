using System.Collections.Generic;
using System.Linq;
using Tizieria.Game;
using Tizieria.SO;
using UnityEngine;
using UnityEngine.UI;

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

        private Progress[] _progress;

        private void Awake()
        {
            Instance = this;

            /*_unspawnedNotes
                = new Queue<PreloadedNotedata>(_info.Notes
                    .OrderBy(note => note.Index)
                    .Select(note => new PreloadedNotedata() { Lane = note.Line, Time = (60f / _info.BPM) * note.Index })
                );*/
        }

        private void Start()
        {
            _unspawnedNotes = new Queue<PreloadedNotedata>(
                Enumerable.Range(5, Mathf.FloorToInt(_source.clip.length - 1f))
                .Select(x => new PreloadedNotedata()
                {
                    Lane = Random.Range(0, ResourceManager.Instance.Lines.Length),
                    Time = (60f / _info.BPM) * x,
                    Id = Random.Range(0, 2)
                })
            );

            _progress = new Progress[2];
            _progress[0] = new Progress()
            {
                Value = 0,
                Max = _unspawnedNotes.Count(x => x.Id == 0)
            };
            _progress[1] = new Progress()
            {
                Value = 0,
                Max = _unspawnedNotes.Count(x => x.Id == 1)
            };
        }

        private void Update()
        {
            TrySpawningNotes(_source.time);

            for (int i = _spawnedNotes.Count - 1; i >= 0; i--)
            {
                var note = _spawnedNotes[i];

                var time = _source.time - note.RefTime;

                if (note.GameObject != null)
                {
                    var lane = ResourceManager.Instance.Lines[note.LaneId];
                    note.Transform.position = Vector2.LerpUnclamped(lane.SpawnPos, lane.Container.position, time);
                }

                if (time > 1f + .1f)
                {
                    Destroy(note.GameObject);
                    _spawnedNotes.RemoveAt(i);

                    /*if (!note.PendingRemoval && note.HitArea.IsAIController && note.CurrentTime > note.AIHitTiming)
                    {
                        note.HitArea.OnKeyDownSpring(note.Line);
                        HitNote(note.Line, note.HitArea.GetInstanceID());
                    }*/

                    if (time > 1f + .1f)
                    {
                        /*note.HitArea.ShowHitInfo(_info.MissInfo);
                        note.PendingRemoval = true;*/
                    }
                }
            }
        }

        private void OnGUI()
        {
            GUI.TextArea(new Rect(20, 20, 40, 40), string.Join("\n", _progress.Select(x => $"{x.Value} / {x.Max}")));
        }

        public void TryClickLine(int laneId)
        {
            var note = _spawnedNotes.FirstOrDefault(x => x.LaneId == laneId);

            if (note == null)
            {
                return;
            }

            var lane = ResourceManager.Instance.Lines[laneId];
            var dist = Mathf.Abs(lane.Container.position.y - note.Transform.position.y);

            var diff = lane.SpawnPos.y - lane.Container.position.y;
            if (dist < 30f)
            {
                _progress[note.NoteRoad].Value++;
            }
            else if (dist < diff / 10f)
            {
                // Note will just be destroyed
            }
            else
            {
                return;
            }


            Destroy(note.GameObject);
            _spawnedNotes.RemoveAll(x => x.LaneId == note.LaneId && x.RefTime == note.RefTime);
        }

        private void SpawnNote(PreloadedNotedata data, float currTime)
        {
            var line = ResourceManager.Instance.Lines[data.Lane];

            var note = Instantiate(_notePrefab, line.Container);
            note.transform.position = line.SpawnPos;
            note.GetComponent<Image>().color = data.Id == 0 ? Color.red : Color.blue;

            _spawnedNotes.Add(new()
            {
                GameObject = note,
                Transform = note.transform,
                RefTime = currTime,
                LaneId = data.Lane,

                FallDuration = 1f,

                NoteRoad = data.Id
            });
        }

        private void TrySpawningNotes(float currentTime)
        {
            if (_unspawnedNotes.Count == 0) return;

            var closestUnspawnedNote = _unspawnedNotes.Peek();

            if (currentTime > closestUnspawnedNote.Time - _currFallDuration)
            {
                _unspawnedNotes.Dequeue();
                SpawnNote(closestUnspawnedNote, closestUnspawnedNote.Time - _currFallDuration);
                TrySpawningNotes(currentTime);
            }
        }
    }

    public class Progress
    {
        public int Value;
        public int Max;
    }

    public class NoteData
    {
        public GameObject GameObject;
        public Transform Transform;
        public int LaneId;
        public float RefTime;

        public float FallDuration;

        public int NoteRoad;
    }

    public class PreloadedNotedata
    {
        public float Time;
        public int Lane;
        public int Id;
    }
}
