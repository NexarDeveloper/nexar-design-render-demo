using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Nexar.Renderer
{
    public class ObjectPicker
    {
        private static Tuple<Vector3, Vector3> GetNearFarPlanes(
            Point mouseLocation, 
            Matrix4 modelView, 
            Matrix4 projection)
        {
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            Vector3 near = UnProject(new Vector3(mouseLocation.X, mouseLocation.Y, 0), modelView, projection);
            Vector3 far = UnProject(new Vector3(mouseLocation.X, mouseLocation.Y, 1), modelView, projection);

            return new Tuple<Vector3, Vector3>(near, far);
        }

        public static float DistanceFromPoint(
            Point mouseLocation, 
            Vector3 testPoint, 
            Matrix4 modelView, 
            Matrix4 projection)
        {
            var nearFarPlanes = GetNearFarPlanes(mouseLocation, modelView, projection);
            Vector3 pt = ClosestPoint(nearFarPlanes.Item1, nearFarPlanes.Item2, testPoint);

            return Vector3.Distance(pt, testPoint);
        }

        public static Tuple<float, float> GetXYOnZeroZPlane(
            Point mouseLocation,
            Matrix4 modelView,
            Matrix4 projection)
        {
            var nearFarPlanes = GetNearFarPlanes(mouseLocation, modelView, projection);

            var near = nearFarPlanes.Item1;
            var far = nearFarPlanes.Item2;

            float deltaX = near.X - far.X;
            float deltaY = near.Y - far.Y;
            float deltaZ = Math.Max(near.Z, far.Z) - Math.Min(near.Z, far.Z);

            //float rayLength = (float)Math.Sqrt(Math.Pow(deltaX, 2.0F) + Math.Pow(deltaZ, 2.0F));

            float zeroZSegment = Math.Max(near.Z, 0.0F) - Math.Min(near.Z, 0.0F);

            float hypotenuseFraction = zeroZSegment / deltaZ;

            float zeroXSegment = hypotenuseFraction * deltaX;
            float zeroYSegment = hypotenuseFraction * deltaY;
            float zeroXPosition = near.X - zeroXSegment;
            float zeroYPosition = near.Y - zeroYSegment;

            return new Tuple<float, float>(zeroXPosition, zeroYPosition);
        }

        private static Vector3 ClosestPoint(Vector3 near, Vector3 far, Vector3 testPoint)
        {
            Vector3 totalDelta = far - near;
            float totalDeltaSquare = Vector3.Dot(totalDelta, totalDelta);
            Vector3 testPointToNearDelta = testPoint - near;
            float ap_dot_ab = Vector3.Dot(testPointToNearDelta, totalDelta);
            // t is a projection param when we project vector AP onto AB 
            float t = ap_dot_ab / totalDeltaSquare;
            // calculate the closest point 
            Vector3 Q = near + Vector3.Multiply(totalDelta, t);
            return Q;
        }

        private static Vector3 UnProject(Vector3 screen, Matrix4 modelView, Matrix4 projection)
        {
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            Vector4 pos = new Vector4();

            // Map x and y from window coordinates, map to range -1 to 1 
            pos.X = (screen.X - (float)viewport[0]) / (float)viewport[2] * 2.0f - 1.0f;
            pos.Y = 1 - (screen.Y - (float)viewport[1]) / (float)viewport[3] * 2.0f;
            pos.Z = screen.Z * 2.0f - 1.0f;
            pos.W = 1.0f;

            Vector4 pos2 = Vector4.TransformRow(pos, Matrix4.Invert(projection) * Matrix4.Invert(modelView));
            Vector3 pos_out = new Vector3(pos2.X, pos2.Y, pos2.Z);

            return pos_out / pos2.W;
        }
    }
}
