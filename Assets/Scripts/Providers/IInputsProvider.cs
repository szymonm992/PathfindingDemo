using UnityEngine;

namespace PathfindingDemo.Providers
{
    public interface IInputsProvider 
    {
        public float Horizontal { get; }
        public float Vertical { get; }
        public Vector2 MoveInputSigned { get; }
    }
}
