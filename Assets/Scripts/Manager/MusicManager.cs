using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tizieria.SO;
using UnityEngine;
using UnityEngine.UI;

namespace Tizieria.Manager
{
    /// <summary>
    /// Handle things related to music
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { private set; get; }

        [SerializeField]
        private MusicInfo _info;

        [SerializeField]
        private GameObject _notePrefab;

        [SerializeField]
        private GameObject _goodMarker;

        [SerializeField]
        private Animator _disappearAnim;

        /// <summary>
        /// Notes that are left to be spawned
        /// </summary>
        private Queue<PreloadedNotedata> _unspawnedNotes;

        /// <summary>
        /// Notes that were spawned and still on the board
        /// </summary>
        private readonly List<NoteData> _spawnedNotes = new();

        private Progress[] _progress;

        private float _goodMarkerTimer = -1f;

        /// <summary>
        /// Did we finish playing
        /// </summary>
        private bool _isDone = false;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // Create all notes that will need to be spawned
            _unspawnedNotes = new Queue<PreloadedNotedata>(
                Enumerable.Range(0, _info.NoteCount)
                .SelectMany(x =>
                {
                    var laneId = Random.Range(0, ResourceManager.Instance.Lines.Length);
                    var id = Random.Range(0, 2);

                    // Note that the user will need to click
                    List<PreloadedNotedata> dataList = new()
                    {
                        new PreloadedNotedata()
                        {
                            Lane = laneId,
                            Time = (60f / _info.BPM) * x,
                            ColorId = id,
                            ReferenceValue = null
                        }
                    };

                    // For all the other lines, we will spawn a "dummy" note
                    for (int i = 0; i < ResourceManager.Instance.Lines.Length; i++)
                    {
                        if (i == laneId) continue;

                        dataList.Add(new PreloadedNotedata()
                        {
                            Lane = i,
                            Time = (60f / _info.BPM) * x,
                            ColorId = -1,
                            ReferenceValue = id
                        });
                    }

                    return dataList;
                })
            );

            // The progress for the 2 lewd paths
            // We calculate the max value based on how many notes we spawned
            _progress = new Progress[2];
            _progress[0] = new Progress()
            {
                Value = 0,
                Max = _unspawnedNotes.Count(x => x.ColorId == 0)
            };
            _progress[1] = new Progress()
            {
                Value = 0,
                Max = _unspawnedNotes.Count(x => x.ColorId == 1)
            };
        }

        private void Update()
        {
            if (_goodMarkerTimer > 0f)
            {
                _goodMarkerTimer -= Time.deltaTime;
                if (_goodMarkerTimer <= 0f)
                {
                    _goodMarker.SetActive(false);
                }
            }
        }

        private void LateUpdate() // We do things on LateUpdate to be sure TimeManager update time stuffs beforehand
        {
            if (_isDone) return;

            // We see if we can spawn a new note
            TrySpawningNotes(TimeManager.Instance.Time);

            // We make all notes go soawn
            for (int i = _spawnedNotes.Count - 1; i >= 0; i--)
            {
                var note = _spawnedNotes[i];

                var time = (TimeManager.Instance.Time - note.RefTime) / note.FallDuration; // Calculate note fall depending on time and fall speed

                // Set position
                var lane = ResourceManager.Instance.Lines[note.LaneId];
                note.Transform.position = Vector2.LerpUnclamped(lane.SpawnPos, lane.Container.position, time);
                note.Image.color = new Color(note.Image.color.r, note.Image.color.g, note.Image.color.b, Mathf.Lerp(1f, note.TargetAlpha, time));

                if (time > 1f + .1f) // Note if out of screen...
                {
                    // ... we delete it
                    Destroy(note.GameObject);
                    _spawnedNotes.RemoveAt(i);
                }
            }

            if (!_unspawnedNotes.Any() && !_spawnedNotes.Any())
            {
                _disappearAnim.SetTrigger("Toggle"); // Trigger end of game anim
                _isDone = true;
                StartCoroutine(ShowGameOver());
            }
        }

        private IEnumerator ShowGameOver()
        {
            yield return new WaitForSeconds(2f);
            CGManager.Instance.UpdateSprite(_progress[0].Value01, _progress[1].Value01, true);
            MenuManager.Instance.ToggleGameover(_progress[0].Value01, _progress[1].Value01);
        }

        private void OnGUI()
        {
            // Lewd path debug
            GUI.TextArea(new Rect(20, 20, 50, 40), string.Join("\n", _progress.Select(x => $"{x.Value} / {x.Max}")));
        }

        /// <summary>
        /// Called when a line is clicked, to check if we hit a note
        /// </summary>
        public void TryClickLine(int laneId)
        {
            // Get the closest note from the bottom of the screen on the corresponding line
            var note = _spawnedNotes.FirstOrDefault(x => x.LaneId == laneId);

            if (note == null)
            {
                return; // There is nothing on this line, so there is nothing to do
            }

            var lane = ResourceManager.Instance.Lines[laneId];
            var dist = Mathf.Abs(lane.Container.position.y - note.Transform.position.y); // Distance between note and hit marker

            var diff = lane.SpawnPos.y - lane.Container.position.y; // Game area height
            if (note.ColorId == -1)
            {
                // Sample note, we just destroy it
            }
            else if (dist < diff / 10f) // If we are under a certain distance, the note is succesfully hit
            {
                Debug.Log($"GOOD => {dist} < {diff / 10f} ({diff})");
                _goodMarkerTimer = .5f;
                _goodMarker.SetActive(true);

                _progress[note.ColorId].Value++;

                CGManager.Instance.UpdateSprite(_progress[0].Value01, _progress[1].Value01, false); // Update lewd CG
            }
            else if (dist < diff / 5f)
            {
                Debug.Log($"NOPE => {dist} < {diff / 5f} ({diff})");
                _goodMarkerTimer = -1f;
                // Note is too far to be hit but still close
                // We do that to prevent the player spamming
            }
            else
            {
                return; // Note is too far from the hit marker so we don't do anything with it
            }

            // Destroy the note
            Destroy(note.GameObject);
            _spawnedNotes.RemoveAll(x => x.LaneId == note.LaneId && x.RefTime == note.RefTime);
        }

        /// <summary>
        /// Instantiate a new note on the given line and set its data
        /// </summary>
        /// <param name="data">Class containing data of a note that wasn't spawned yet</param>
        /// <param name="noteSpawnTime">At what time the note should be spawned</param>
        /// <param name="fallDuration">Time it'll take for the note to fall to the hit marker</param>
        /// <param name="progress01">Progress on the current lewd path between 0 and 1</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void SpawnNote(PreloadedNotedata data, float noteSpawnTime, float fallDuration, float progress01)
        {
            var lane = ResourceManager.Instance.Lines[data.Lane];

            // Spawn note
            var note = Instantiate(_notePrefab, lane.Container);
            note.transform.position = lane.SpawnPos;
            note.GetComponent<Image>().color = data.ColorId switch // Set color depending of lewd path
            {
                -1 => Color.gray,
                0 => Color.red,
                1 => Color.blue,

                _ => throw new System.NotImplementedException()
            };

            _spawnedNotes.Add(new()
            {
                GameObject = note,
                Transform = note.transform,
                Image = note.GetComponent<Image>(),
                RefTime = noteSpawnTime - fallDuration,
                LaneId = data.Lane,

                FallDuration = fallDuration,

                ColorId = data.ColorId,

                TargetAlpha = 1f - progress01 - 0.5f
            });
        }

        private void TrySpawningNotes(float currentTime)
        {
            if (_unspawnedNotes.Count == 0) // Nothing left to spawn
            {
                return;
            }

            var nextSpawn = _unspawnedNotes.Peek();

            var val01 = _progress[nextSpawn.ReferenceValue ?? nextSpawn.ColorId].Value01 * .75f;
            var fallDuration = 1.25f - val01;
            if (currentTime > nextSpawn.Time - fallDuration) // Is it time to spawn the note (take in consideration fall duration)
            {
                _unspawnedNotes.Dequeue();
                SpawnNote(nextSpawn, nextSpawn.Time, fallDuration, val01);
                TrySpawningNotes(currentTime); // If we spawn a note, we check if there is another one to spawn
            }
        }
    }

    public class Progress
    {
        public int Value;
        public int Max;

        public float Value01 => Value / (float)Max;
    }

    public class NoteData
    {
        // Actual gameobject of the note
        public GameObject GameObject;

        // Transform related to the gameobject
        public Transform Transform;

        // Image component associated to the gameobject
        public Image Image;

        // Which lane the note is on
        public int LaneId;

        // The time the note was spawned at, to calculate its future position
        public float RefTime;

        // How much time does the note need to reach the hit marker
        public float FallDuration;

        // Which lewd path will this note lead us to
        public int ColorId;

        // Control alpha to make notes more transparent
        public float TargetAlpha;
    }

    public class PreloadedNotedata
    {
        public float Time;
        public int Lane;
        public int ColorId;
        public int? ReferenceValue;
    }
}
