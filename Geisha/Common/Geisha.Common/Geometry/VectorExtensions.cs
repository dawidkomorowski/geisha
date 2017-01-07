namespace Geisha.Common.Geometry
{
    public static class VectorExtensions
    {
        public static Vector2 AsVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.X, vector3.Y);
        }

        public static Vector2 AsVector2(this Vector4 vector4)
        {
            return new Vector2(vector4.X, vector4.Y);
        }

        public static Vector3 AsVector3(this Vector4 vector4)
        {
            return new Vector3(vector4.X, vector4.Y, vector4.Z);
        }
    }
}