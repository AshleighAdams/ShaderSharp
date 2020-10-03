#pragma warning disable IDE0060 // Remove unused parameter

using System;
using System.Runtime.InteropServices;

namespace ShaderSharp.Shaders
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Vec2 : FloatVec
	{
		[FieldOffset(sizeof(float) * 0)] public float X;
		[FieldOffset(sizeof(float) * 1)] public float Y;

		[FieldOffset(sizeof(float) * 0)] public float R;
		[FieldOffset(sizeof(float) * 1)] public float G;

		public Vec2(Vec2 xy)
		{
			R = X = xy.X;
			G = Y = xy.Y;
		}
		public Vec2(float x, float y)
		{
			R = X = x;
			G = Y = y;
		}

		public Vec2 Xy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Yx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public Vec2 Rg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Gr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public static Vec2 operator +(Vec2 left, Vec2 right) => throw new NotSupportedException();
		public static Vec2 operator -(Vec2 left, Vec2 right) => throw new NotSupportedException();
		public static Vec2 operator *(Vec2 left, Vec2 right) => throw new NotSupportedException();
		public static Vec2 operator /(Vec2 left, Vec2 right) => throw new NotSupportedException();

		public static Vec2 operator +(Vec2 left, float right) => throw new NotSupportedException();
		public static Vec2 operator -(Vec2 left, float right) => throw new NotSupportedException();
		public static Vec2 operator *(Vec2 left, float right) => throw new NotSupportedException();
		public static Vec2 operator /(Vec2 left, float right) => throw new NotSupportedException();
	};

	[StructLayout(LayoutKind.Explicit)]
	public struct Vec3 : FloatVec
	{
		[FieldOffset(sizeof(float) * 0)] public float X;
		[FieldOffset(sizeof(float) * 1)] public float Y;
		[FieldOffset(sizeof(float) * 2)] public float Z;

		[FieldOffset(sizeof(float) * 0)] public float R;
		[FieldOffset(sizeof(float) * 1)] public float G;
		[FieldOffset(sizeof(float) * 2)] public float B;

		public Vec3(float x, float y, float z)
		{
			R = X = x;
			G = Y = y;
			B = Z = z;
		}
		public Vec3(float x, Vec2 yz)
		{
			R = X = x;
			G = Y = yz.X;
			B = Z = yz.Y;
		}
		public Vec3(Vec2 xy, float z)
		{
			R = X = xy.X;
			G = Y = xy.Y;
			B = Z = z;
		}

		public Vec2 Xy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Xz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Yx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Yz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Zx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Zy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public Vec3 Xyz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Xzy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Yxz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Yzx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Zxy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Zyx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public Vec2 Rg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Rb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Gr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Gb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Br { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Bg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public Vec3 Rgb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Rzb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Grb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Gbr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Brg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Bgr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public static Vec3 operator +(Vec3 left, Vec3 right) => throw new NotSupportedException();
		public static Vec3 operator -(Vec3 left, Vec3 right) => throw new NotSupportedException();
		public static Vec3 operator *(Vec3 left, Vec3 right) => throw new NotSupportedException();
		public static Vec3 operator /(Vec3 left, Vec3 right) => throw new NotSupportedException();

		public static Vec3 operator +(Vec3 left, float right) => throw new NotSupportedException();
		public static Vec3 operator -(Vec3 left, float right) => throw new NotSupportedException();
		public static Vec3 operator *(Vec3 left, float right) => throw new NotSupportedException();
		public static Vec3 operator /(Vec3 left, float right) => throw new NotSupportedException();

		public static Vec3 operator +(float left, Vec3 right) => throw new NotSupportedException();
		public static Vec3 operator -(float left, Vec3 right) => throw new NotSupportedException();
		public static Vec3 operator *(float left, Vec3 right) => throw new NotSupportedException();
		public static Vec3 operator /(float left, Vec3 right) => throw new NotSupportedException();
	};

	[StructLayout(LayoutKind.Explicit)]
	public struct Vec4 : FloatVec
	{
		[FieldOffset(sizeof(float) * 0)] public float X;
		[FieldOffset(sizeof(float) * 1)] public float Y;
		[FieldOffset(sizeof(float) * 2)] public float Z;
		[FieldOffset(sizeof(float) * 3)] public float W;

		[FieldOffset(sizeof(float) * 0)] public float R;
		[FieldOffset(sizeof(float) * 1)] public float G;
		[FieldOffset(sizeof(float) * 2)] public float B;
		[FieldOffset(sizeof(float) * 3)] public float A;

		public Vec4(float x, Vec3 yzw)
		{
			R = X = x;
			G = Y = yzw.X;
			B = Z = yzw.Y;
			A = W = yzw.Z;
		}
		public Vec4(Vec2 xy, Vec2 zw)
		{
			R = X = xy.X;
			G = Y = xy.Y;
			B = Z = zw.X;
			A = W = zw.Y;
		}
		public Vec4(Vec3 xyz, float w)
		{
			R = X = xyz.X;
			G = Y = xyz.Y;
			B = Z = xyz.Z;
			A = W = w;
		}
		public Vec4(float x, float y, float z, float w)
		{
			R = X = x;
			G = Y = y;
			B = Z = z;
			A = W = w;
		}

		public Vec2 Xy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Xz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Xw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Yx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Yz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Yw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Zx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Zy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Zw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Wx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Wy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Wz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public Vec3 Xyz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Xyw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Xzy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Xzw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Xwy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Xwz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Yxz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Yxw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Yzx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Yzw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Ywx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Ywz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Zxy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Zxw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Zyx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Zyw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Zwx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Zwy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Wxy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Wxz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Wyx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Wyz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Wzx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Wzy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public Vec4 Xyzw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Xywz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Xzyw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Xzwy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Xwyz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Xwzy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Yxzw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Yxwz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Yzxw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Yzwx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Ywxz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Ywzx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Zxyw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Zxwy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Zyxw { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Zywx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Zwxy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Zwyx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Wxyz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Wxzy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Wyxz { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Wyzx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Wzxy { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Wzyx { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }


		public Vec2 Rg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Rb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Ra { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Gr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Gb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Ga { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Br { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Bg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Ba { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Ar { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Ag { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec2 Ab { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public Vec3 Rgb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Rga { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Rbg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Rba { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Rag { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Rab { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Grb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Gra { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Gbr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Gba { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Gar { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Gab { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Brg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Bra { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Bgr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Bga { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Bar { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Bag { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Arg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Arb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Agr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Agb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Abr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec3 Abg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public Vec4 Rgba { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Rgab { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Rbga { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Rbag { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Ragb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Rabg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Grba { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Grab { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Gbra { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Gbar { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Garb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Gabr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Brga { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Brag { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Bgra { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Bgar { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Barg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Bagr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Argb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Arbg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Agrb { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Agbr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Abrg { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
		public Vec4 Abgr { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public static Vec4 operator +(Vec4 left, Vec4 right) => throw new NotSupportedException();
		public static Vec4 operator -(Vec4 left, Vec4 right) => throw new NotSupportedException();
		public static Vec4 operator *(Vec4 left, Vec4 right) => throw new NotSupportedException();
		public static Vec4 operator /(Vec4 left, Vec4 right) => throw new NotSupportedException();

		public static Vec4 operator +(Vec4 left, float right) => throw new NotSupportedException();
		public static Vec4 operator -(Vec4 left, float right) => throw new NotSupportedException();
		public static Vec4 operator *(Vec4 left, float right) => throw new NotSupportedException();
		public static Vec4 operator /(Vec4 left, float right) => throw new NotSupportedException();

		public static Vec4 operator +(float left, Vec4 right) => throw new NotSupportedException();
		public static Vec4 operator -(float left, Vec4 right) => throw new NotSupportedException();
		public static Vec4 operator *(float left, Vec4 right) => throw new NotSupportedException();
		public static Vec4 operator /(float left, Vec4 right) => throw new NotSupportedException();
	};

	public partial class Shader
	{
		protected static Vec3 Normalize(Vec3 x) { throw new NotSupportedException(); }
		protected static Vec4 Normalize(Vec4 x) { throw new NotSupportedException(); }
		protected static float Length(Vec x) { throw new NotSupportedException(); }
		protected static Vec2 Cross(Vec2 x, Vec2 y) { throw new NotSupportedException(); }
		protected static Vec3 Cross(Vec3 x, Vec3 y) { throw new NotSupportedException(); }
		protected static Vec4 Cross(Vec4 x, Vec4 y) { throw new NotSupportedException(); }
		protected static float Dot(Vec3 x, Vec3 y) { throw new NotSupportedException(); }
	}
}