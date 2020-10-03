#pragma warning disable IDE0060 // Remove unused parameter

using System;

namespace ShaderSharp.Shaders
{
	public interface GenericType { }
	public interface FloatType : GenericType { }
	public interface DoubleType : GenericType { }

	public interface Vec : GenericType { }
	public interface FloatVec : Vec, FloatType { }
	public interface DoubleVec : Vec, DoubleType { }

	public interface Mat : GenericType { }
	public interface FloatMat : GenericType, FloatType { }
	public interface DoubleMat : GenericType, DoubleType { }

	public partial class Shader
	{
		protected static float Ceil(float f) { throw new NotSupportedException(); }
		protected static float Floor(float f) { throw new NotSupportedException(); }
		protected static float Round(float f) { throw new NotSupportedException(); }
		protected static float Log(float f) { throw new NotSupportedException(); }
		protected static float Log10(float f) { throw new NotSupportedException(); }

		protected static T Pow<T>(T a, T b) where T : GenericType { throw new NotSupportedException(); }
		protected static float Pow(float a, float b) { throw new NotSupportedException(); }
		protected static double Pow(double a, double b) { throw new NotSupportedException(); }

		protected static T Exp<T>(T a) where T : GenericType { throw new NotSupportedException(); }
		protected static float Exp(float a) { throw new NotSupportedException(); }
		protected static double Exp(double a) { throw new NotSupportedException(); }

		protected static float Sin(float f) { throw new NotSupportedException(); }
		protected static double Sin(double f) { throw new NotSupportedException(); }
		protected static float Cos(float f) { throw new NotSupportedException(); }
		protected static double Cos(double f) { throw new NotSupportedException(); }
		protected static float Tan(float f) { throw new NotSupportedException(); }
		protected static double Tan(double f) { throw new NotSupportedException(); }

		protected static float Min(float x, float y) { throw new NotSupportedException(); }
		protected static double Min(double x, double y) { throw new NotSupportedException(); }
		protected static T Min<T>(T x, T y) where T : GenericType { throw new NotSupportedException(); }

		protected static float Max(float x, float y) { throw new NotSupportedException(); }
		protected static double Max(double x, double y) { throw new NotSupportedException(); }
		protected static T Max<T>(T x, T y) where T : GenericType { throw new NotSupportedException(); }


		protected static float Clamp(float val, float min, float max) { throw new NotSupportedException(); }
		protected static float Clamp(double val, double min, double max) { throw new NotSupportedException(); }
		protected static T Clamp<T>(T val, T min, T max) where T : GenericType { throw new NotSupportedException(); }

		protected static float Abs(float x) { throw new NotSupportedException(); }
		protected static double Abs(double x) { throw new NotSupportedException(); }
		protected static T Abs<T>(T x) where T : GenericType { throw new NotSupportedException(); }

		protected static float Sqrt(float x) { throw new NotSupportedException(); }
		protected static double Sqrt(double x) { throw new NotSupportedException(); }
		protected static T Sqrt<T>(T x) where T : GenericType { throw new NotSupportedException(); }

		protected static float Mix(float lo, float hi, float value) { throw new NotSupportedException(); }
		protected static double Mix(double lo, double hi, double value) { throw new NotSupportedException(); }
		protected static T Mix<T>(T lo, T hi, float value) where T : FloatType { throw new NotSupportedException(); }
		protected static T Mix<T>(T lo, T hi, double value) where T : DoubleType { throw new NotSupportedException(); }

		protected static float Step(float edge, float x) { throw new NotSupportedException(); }
		protected static double Step(double edge, double x) { throw new NotSupportedException(); }
		protected static T Step<T>(T edge, T x) where T : GenericType { throw new NotSupportedException(); }
		protected static T Step<T>(float edge, T x) where T : FloatType { throw new NotSupportedException(); }
		protected static T Step<T>(double edge, T x) where T : DoubleType { throw new NotSupportedException(); }

		protected static float SmoothStep(float edge0, float edge1, float x) { throw new NotSupportedException(); }
		protected static double SmoothStep(double edge0, double edge1, double x) { throw new NotSupportedException(); }
		protected static T SmoothStep<T>(T edge0, T edge1, T x) where T : GenericType { throw new NotSupportedException(); }
		protected static T SmoothStep<T>(float edge0, float edge1, T x) where T : FloatType { throw new NotSupportedException(); }
		protected static T SmoothStep<T>(double edge0, double edge1, T x) where T : DoubleType { throw new NotSupportedException(); }
	}
}