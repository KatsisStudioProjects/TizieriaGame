using UnityEngine;
using UnityEngine.UI;

namespace Tizieria.Manager
{
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
            if (line1 == line2) _cgDisplay.sprite = _neutralSprite;
            else if (line1 > line2)
            {
                _cgDisplay.sprite = _line2Sprites[Mathf.FloorToInt((line1 - line2) * (_line2Sprites.Length))];
            }
            else
            {
                _cgDisplay.sprite = _line1Sprites[Mathf.FloorToInt((line2 - line1) * (_line1Sprites.Length - 1))];
            }
        }
    }
}
