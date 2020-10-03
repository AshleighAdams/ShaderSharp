
using System;
using System.Runtime.InteropServices;

namespace ShaderSharp.Shaders
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct Mat4 : FloatType
	{
		public Mat4(Mat4 x)
		{
			this.M11 = x.M11; this.M12 = x.M12; this.M13 = x.M13; this.M14 = x.M14;
			this.M21 = x.M21; this.M22 = x.M22; this.M23 = x.M23; this.M24 = x.M24;
			this.M31 = x.M31; this.M32 = x.M32; this.M33 = x.M33; this.M34 = x.M34;
			this.M41 = x.M41; this.M42 = x.M42; this.M43 = x.M43; this.M44 = x.M44;
		}

		public float
			M11, M12, M13, M14,
			M21, M22, M23, M24,
			M31, M32, M33, M34,
			M41, M42, M43, M44;

		public static Mat4 operator +(Mat4 left, Mat4 right) { throw new NotSupportedException(); }
		public static Mat4 operator -(Mat4 left, Mat4 right) { throw new NotSupportedException(); }
		public static Mat4 operator *(Mat4 left, Mat4 right) { throw new NotSupportedException(); }
		public static Mat4 operator /(Mat4 left, Mat4 right) { throw new NotSupportedException(); }
		public static Mat4 operator +(Mat4 left, Mat3 right) { throw new NotSupportedException(); }
		public static Mat4 operator -(Mat4 left, Mat3 right) { throw new NotSupportedException(); }
		public static Mat4 operator *(Mat4 left, Mat3 right) { throw new NotSupportedException(); }
		public static Mat4 operator /(Mat4 left, Mat3 right) { throw new NotSupportedException(); }
		public static Vec4 operator +(Mat4 left, Vec4 right) { throw new NotSupportedException(); }
		public static Vec4 operator -(Mat4 left, Vec4 right) { throw new NotSupportedException(); }
		public static Vec4 operator *(Mat4 left, Vec4 right) { throw new NotSupportedException(); }
		public static Vec4 operator /(Mat4 left, Vec4 right) { throw new NotSupportedException(); }
		public static Vec3 operator +(Mat4 left, Vec3 right) { throw new NotSupportedException(); }
		public static Vec3 operator -(Mat4 left, Vec3 right) { throw new NotSupportedException(); }
		public static Vec3 operator *(Mat4 left, Vec3 right) { throw new NotSupportedException(); }
		public static Vec3 operator /(Mat4 left, Vec3 right) { throw new NotSupportedException(); }
		public ref Vec4 this[int x] { get { throw new NotSupportedException(); } }
		public ref float this[int x, int y] { get { throw new NotSupportedException(); } }
	};
 
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct Mat3 : FloatType
	{
		public Mat3(Mat3 x)
		{
			this.M11 = x.M11;
			this.M12 = x.M12;
			this.M13 = x.M13;
			this.M21 = x.M21;
			this.M22 = x.M22;
			this.M23 = x.M23;
			this.M31 = x.M31;
			this.M32 = x.M32;
			this.M33 = x.M33;
		}
		public Mat3(Mat4 x) { throw new NotSupportedException(); }
		public Mat3(Vec3 a, Vec3 b, Vec3 c) { throw new NotSupportedException(); }

		public float
			M11, M12, M13,
			M21, M22, M23,
			M31, M32, M33;

		public static Mat3 operator +(Mat3 left, Mat3 right) { throw new NotSupportedException(); }
		public static Mat3 operator -(Mat3 left, Mat3 right) { throw new NotSupportedException(); }
		public static Mat3 operator *(Mat3 left, Mat3 right) { throw new NotSupportedException(); }
		public static Mat3 operator /(Mat3 left, Mat3 right) { throw new NotSupportedException(); }
		public static Vec3 operator +(Mat3 left, Vec3 right) { throw new NotSupportedException(); }
		public static Vec3 operator -(Mat3 left, Vec3 right) { throw new NotSupportedException(); }
		public static Vec3 operator *(Mat3 left, Vec3 right) { throw new NotSupportedException(); }
		public static Vec3 operator /(Mat3 left, Vec3 right) { throw new NotSupportedException(); }
		public ref Vec3 this[int x] { get { throw new NotSupportedException(); } }
		public ref float this[int x, int y] { get { throw new NotSupportedException(); } }
	};

	public partial class Shader
	{
		protected static Mat3 Transpose(Mat3 x) { throw new NotSupportedException(); }
		protected static Mat4 Transpose(Mat4 x) { throw new NotSupportedException(); }
		protected static Mat3 Inverse(Mat3 x) { throw new NotSupportedException(); }
		protected static Mat4 Inverse(Mat4 x) { throw new NotSupportedException(); }
		protected static Mat3 Normalize(Mat3 x) { throw new NotSupportedException(); }
		protected static Mat4 Normalize(Mat4 x) { throw new NotSupportedException(); }
		protected static float Length(Mat3 x) { throw new NotSupportedException(); }
		protected static float Length(Mat4 x) { throw new NotSupportedException(); }
	}
}