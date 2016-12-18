using OpenTK;

namespace InfiniTK.Engine
{
    public interface IMove : IPosition
    {
        void ApplyCamera();
        void MoveForward(double amount);
        void MoveSideways(double amount);
        void Move(Vector3d amount);
        void Spin(float degrees);
        void Tilt(float degrees);
    }
}
