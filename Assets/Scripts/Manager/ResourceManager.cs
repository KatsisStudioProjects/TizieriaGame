using Tizieria.Game;
using UnityEngine;

namespace Tizieria.Manager
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { private set; get; }

        [SerializeField]
        private Line[] _lines;

        public Line[] Lines => _lines;

        private void Awake()
        {
            Instance = this;
        }
    }
}
