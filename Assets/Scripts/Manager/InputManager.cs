using UnityEngine;
using UnityEngine.InputSystem;

namespace Tizieria.Manager
{
    public class InputManager : MonoBehaviour
    {
        public void OnLine1Click(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started) ResourceManager.Instance.Lines[0].Click();
            else if (value.phase == InputActionPhase.Canceled) ResourceManager.Instance.Lines[0].Release();
        }

        public void OnLine2Click(InputAction.CallbackContext value)
        {
            if (value.phase == InputActionPhase.Started) ResourceManager.Instance.Lines[1].Click();
            else if (value.phase == InputActionPhase.Canceled) ResourceManager.Instance.Lines[1].Release();
        }
    }
}
