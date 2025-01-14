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

        [SerializeField]
        private Sprite _victorySprite1, _victorySprite2;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Decide which CG to show
        /// </summary>
        /// <param name="allowShowVictory">Show victory CG instead of highest level</param>
        public void UpdateSprite(float line1, float line2, bool allowShowVictory)
        {
            if (line1 > line2)
            {
                var index = Mathf.FloorToInt((line1 - line2) * (_line2Sprites.Length + 2)) - 1;
                if (index == -1) _cgDisplay.sprite = _neutralSprite;
                else
                {
                    var target = Mathf.Clamp(index, 0, _line2Sprites.Length - 1);
                    if (allowShowVictory && target == _line2Sprites.Length - 1) _cgDisplay.sprite = _victorySprite1;
                    else _cgDisplay.sprite = _line2Sprites[Mathf.Clamp(index, 0, _line2Sprites.Length - 1)];
                }
            }
            else
            {
                var index = Mathf.FloorToInt((line2 - line1) * (_line1Sprites.Length + 2)) - 1;
                if (index == -1) _cgDisplay.sprite = _neutralSprite;
                else
                {
                    var target = Mathf.Clamp(index, 0, _line1Sprites.Length - 1);
                    if (allowShowVictory && target == _line1Sprites.Length - 1) _cgDisplay.sprite = _victorySprite1;
                    else _cgDisplay.sprite = _line1Sprites[Mathf.Clamp(index, 0, _line1Sprites.Length - 1)];
                }
            }
        }
    }
}
