using UnityEngine;
using UnityEngine.UI;

namespace Tizieria.Manager
{
    /// <summary>
    /// Manage the image displayed on the background of the game
    /// </summary>
    public class CGManager : MonoBehaviour
    {
        public static CGManager Instance { private set; get; }

        [SerializeField]
        private Image _cgDisplay;

        [SerializeField]
        private Sprite _neutralSprite;

        [SerializeField]
        private Sprite[] _line1Sprites, _line2Sprites;

        private void Awake()
        {
            Instance = this;
        }

        public void UpdateSprite(float line1, float line2)
        {
            if (line1 > line2)
            {
                var index = Mathf.FloorToInt((line1 - line2) * (_line2Sprites.Length + 2)) - 1;
                if (index == -1) _cgDisplay.sprite = _neutralSprite;
                else _cgDisplay.sprite = _line2Sprites[Mathf.Clamp(index, 0, _line2Sprites.Length - 1)];
            }
            else
            {
                var index = Mathf.FloorToInt((line2 - line1) * (_line1Sprites.Length + 2)) - 1;
                if (index == -1) _cgDisplay.sprite = _neutralSprite;
                _cgDisplay.sprite = _line1Sprites[Mathf.Clamp(index, 0, _line1Sprites.Length - 1)];
            }
        }
    }
}
