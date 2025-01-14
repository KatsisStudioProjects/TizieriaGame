using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tizieria.Manager
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { private set; get; }

        [SerializeField]
        private GameObject _menu;

        [SerializeField]
        private TMP_Text _scoreText;

        private void Awake()
        {
            Instance = this;
        }

        public void ToggleGameover(float line1, float line2)
        {
            var success = Mathf.CeilToInt(Mathf.Abs(line1 - line2) * 100f);
            _scoreText.text = $"Hit rate: {success}%";
            _menu.SetActive(true);
        }

        public void Reload()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
