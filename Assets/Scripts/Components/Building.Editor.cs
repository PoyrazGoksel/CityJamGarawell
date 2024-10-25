#if UNITY_EDITOR

namespace Components
{
    public abstract partial class Building
    {
        private void SetID(int newID)
        {
            _id = newID;
        }
    }
}
#endif