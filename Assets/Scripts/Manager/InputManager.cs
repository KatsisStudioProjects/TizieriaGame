using UnityEngine;
using UnityEngine.InputSystem;

namespace Tizieria.Manager
{
    public class InputManager : MonoBehaviour
    {
        public void OnLine1Click(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started)
            {
                ResourceManager.Instance.Lines[0].Click();
                MusicManager.Instance.TryClickLine(0);
            }
            else if (value.phase == InputActionPhase.Canceled) ResourceManager.Instance.Lines[0].Release();
        }

        public void OnLine2Click(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started)
            {
                ResourceManager.Instance.Lines[1].Click();
                MusicManager.Instance.TryClickLine(1);
            }
            else if (value.phase == InputActionPhase.Canceled) ResourceManager.Instance.Lines[1].Release();
        }
    }
}
