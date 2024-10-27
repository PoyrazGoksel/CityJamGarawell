#if UNITY_EDITOR

namespace Components.Buildings
{
    public partial class Building
    {
        public void SetID(int newID)
        {
            _id = newID;
        }
    }
}
#endif