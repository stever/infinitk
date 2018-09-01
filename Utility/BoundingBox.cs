#region License
/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Authors:
Olivier Dufour (Duff)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.Collections.Generic;
using OpenTK;

namespace MoonPad.Utility
{
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public readonly Vector3d Min;
        public readonly Vector3d Max;
        public const int CornerCount = 8;

        public BoundingBox(Vector3d min, Vector3d max)
        {
            Min = min;
            Max = max;
        }

        public enum ContainmentType
        {
            Disjoint,
            Contains,
            Intersects
        }

        public ContainmentType Contains(BoundingBox box)
        {
            // Test if all corner is in the same side of a face by just
            // checking min and max.
            if (box.Max.X < Min.X
                || box.Min.X > Max.X
                || box.Max.Y < Min.Y
                || box.Min.Y > Max.Y
                || box.Max.Z < Min.Z
                || box.Min.Z > Max.Z)
                return ContainmentType.Disjoint;

            if (box.Min.X >= Min.X
                && box.Max.X <= Max.X
                && box.Min.Y >= Min.Y
                && box.Max.Y <= Max.Y
                && box.Min.Z >= Min.Z
                && box.Max.Z <= Max.Z)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }

        public ContainmentType Contains(Vector3d center, float radius)
        {
            if (center.X - Min.X > radius
                && center.Y - Min.Y > radius
                && center.Z - Min.Z > radius
                && Max.X - center.X > radius
                && Max.Y - center.Y > radius
                && Max.Z - center.Z > radius)
                return ContainmentType.Contains;

            double dmin = 0;

            if (center.X - Min.X <= radius) dmin += (center.X - Min.X) * (center.X - Min.X);
            else if (Max.X - center.X <= radius) dmin += (center.X - Max.X) * (center.X - Max.X);

            if (center.Y - Min.Y <= radius) dmin += (center.Y - Min.Y) * (center.Y - Min.Y);
            else if (Max.Y - center.Y <= radius) dmin += (center.Y - Max.Y) * (center.Y - Max.Y);

            if (center.Z - Min.Z <= radius) dmin += (center.Z - Min.Z) * (center.Z - Min.Z);
            else if (Max.Z - center.Z <= radius) dmin += (center.Z - Max.Z) * (center.Z - Max.Z);

            if (dmin <= radius * radius) return ContainmentType.Intersects;

            return ContainmentType.Disjoint;
        }

        public ContainmentType Contains(Vector3d point)
        {
            ContainmentType result;
            Contains(ref point, out result);
            return result;
        }

        public void Contains(ref Vector3d point, out ContainmentType result)
        {
            // First we get if point is out of box.
            if (point.X < Min.X
                || point.X > Max.X
                || point.Y < Min.Y
                || point.Y > Max.Y
                || point.Z < Min.Z
                || point.Z > Max.Z)
                result = ContainmentType.Disjoint;

            // or if point is on box because coordonate of point is lesser or equal.
            else if (Math.Abs(point.X - Min.X) < double.Epsilon
                || Math.Abs(point.X - Max.X) < double.Epsilon
                || Math.Abs(point.Y - Min.Y) < double.Epsilon
                || Math.Abs(point.Y - Max.Y) < double.Epsilon
                || Math.Abs(point.Z - Min.Z) < double.Epsilon
                || Math.Abs(point.Z - Max.Z) < double.Epsilon)
                result = ContainmentType.Intersects;

            else
                result = ContainmentType.Contains;
        }

        public static BoundingBox CreateFromPoints(IEnumerable<Vector3d> points)
        {
            if (points == null)
                throw new ArgumentNullException();

            var empty = true;
            var vector2 = new Vector3d(float.MaxValue);
            var vector1 = new Vector3d(float.MinValue);
            foreach (var vector3D in points)
            {
                vector2 = Vector3d.Min(vector2, vector3D);
                vector1 = Vector3d.Max(vector1, vector3D);
                empty = false;
            }

            if (empty)
                throw new ArgumentException();

            return new BoundingBox(vector2, vector1);
        }

        public static BoundingBox CreateFromSphere(Vector3d center, float radius)
        {
            var vector1 = new Vector3d(radius, radius, radius);
            return new BoundingBox(center - vector1, center + vector1);
        }

        public static BoundingBox CreateMerged(BoundingBox original, BoundingBox additional)
        {
            return new BoundingBox(Vector3d.Min(original.Min, additional.Min), Vector3d.Max(original.Max, additional.Max));
        }

        public bool Equals(BoundingBox other)
        {
            return Min == other.Min && Max == other.Max;
        }

        public override bool Equals(object obj)
        {
            return obj is BoundingBox && Equals((BoundingBox) obj);
        }

        public Vector3d[] GetCorners()
        {
            return new[] {
                new Vector3d(Min.X, Max.Y, Max.Z),
                new Vector3d(Max.X, Max.Y, Max.Z),
                new Vector3d(Max.X, Min.Y, Max.Z),
                new Vector3d(Min.X, Min.Y, Max.Z),
                new Vector3d(Min.X, Max.Y, Min.Z),
                new Vector3d(Max.X, Max.Y, Min.Z),
                new Vector3d(Max.X, Min.Y, Min.Z),
                new Vector3d(Min.X, Min.Y, Min.Z)
            };
        }

        public void GetCorners(Vector3d[] corners)
        {
            if (corners == null) throw new ArgumentNullException(nameof(corners));
            if (corners.Length < 8) throw new ArgumentOutOfRangeException(nameof(corners), "Not Enought Corners");

            corners[0].X = Min.X;
            corners[0].Y = Max.Y;
            corners[0].Z = Max.Z;
            corners[1].X = Max.X;
            corners[1].Y = Max.Y;
            corners[1].Z = Max.Z;
            corners[2].X = Max.X;
            corners[2].Y = Min.Y;
            corners[2].Z = Max.Z;
            corners[3].X = Min.X;
            corners[3].Y = Min.Y;
            corners[3].Z = Max.Z;
            corners[4].X = Min.X;
            corners[4].Y = Max.Y;
            corners[4].Z = Min.Z;
            corners[5].X = Max.X;
            corners[5].Y = Max.Y;
            corners[5].Z = Min.Z;
            corners[6].X = Max.X;
            corners[6].Y = Min.Y;
            corners[6].Z = Min.Z;
            corners[7].X = Min.X;
            corners[7].Y = Min.Y;
            corners[7].Z = Min.Z;
        }

        public override int GetHashCode()
        {
            return Min.GetHashCode() + Max.GetHashCode();
        }

        public bool Intersects(BoundingBox box)
        {
            if (Max.X < box.Min.X || Min.X > box.Max.X) return false;
            if (Max.Y < box.Min.Y || Min.Y > box.Max.Y) return false;
            return Max.Z >= box.Min.Z && Min.Z <= box.Max.Z;
        }

        public bool Intersects(Vector3d center, float radius)
        {
            if (center.X - Min.X > radius
                && center.Y - Min.Y > radius
                && center.Z - Min.Z > radius
                && Max.X - center.X > radius
                && Max.Y - center.Y > radius
                && Max.Z - center.Z > radius)
                return true;

            double dmin = 0;

            if (center.X - Min.X <= radius) dmin += (center.X - Min.X) * (center.X - Min.X);
            else if (Max.X - center.X <= radius) dmin += (center.X - Max.X) * (center.X - Max.X);

            if (center.Y - Min.Y <= radius) dmin += (center.Y - Min.Y) * (center.Y - Min.Y);
            else if (Max.Y - center.Y <= radius) dmin += (center.Y - Max.Y) * (center.Y - Max.Y);

            if (center.Z - Min.Z <= radius) dmin += (center.Z - Min.Z) * (center.Z - Min.Z);
            else if (Max.Z - center.Z <= radius) dmin += (center.Z - Max.Z) * (center.Z - Max.Z);

            return dmin <= radius * radius;
        }

        public static bool operator == (BoundingBox a, BoundingBox b)
        {
            return a.Equals(b);
        }

        public static bool operator != (BoundingBox a, BoundingBox b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return $"{{Min:{Min} Max:{Max}}}";
        }
    }
}
