using g3;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    public static class DiamondCrossSection
    {
        private static readonly Vector3d UnitBottom = new Vector3d(0, 0, -1);
        private static readonly Vector3d UnitLeft = new Vector3d(-0.5, 0, -0.5);
        private static readonly Vector3d UnitRight = new Vector3d(0.5, 0, -0.5);
        private static readonly Vector3d UnitTop = new Vector3d(0, 0, 0);

        public static Vector3d Bottom(Vector2d dimensions)
        {
            return new Vector3d(
                UnitBottom.x * dimensions.x,
                UnitBottom.y,
                UnitBottom.z * dimensions.y);
        }

        public static Vector3d Left(Vector2d dimensions, double scaleX = 1, double shiftY = 0)
        {
            return new Vector3d(
                UnitLeft.x * dimensions.x * scaleX,
                UnitLeft.y + shiftY,
                UnitLeft.z * dimensions.y);
        }

        public static Vector3d Right(Vector2d dimensions, double scaleX = 1, double shiftY = 0)
        {
            return new Vector3d(
                UnitRight.x * dimensions.x * scaleX,
                UnitRight.y + shiftY,
                UnitRight.z * dimensions.y);
        }

        public static Vector3d Top(Vector2d dimensions)
        {
            return new Vector3d(
                UnitTop.x * dimensions.x,
                UnitTop.y,
                UnitTop.z * dimensions.y);
        }
    }
}