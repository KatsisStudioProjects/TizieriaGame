using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Tizieria.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started)
            {
                SceneManager.LoadScene("Main");
            }
        }
    }
}