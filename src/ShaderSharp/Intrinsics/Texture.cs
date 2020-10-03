
using System;

namespace ShaderSharp.Shaders
{
	public class Sampler1D { protected Sampler1D() { } };
	public class Sampler2D { protected Sampler2D() { } };
	public class Sampler3D { protected Sampler3D() { } };
	public class SamplerCube { protected SamplerCube() { } };

	public partial class Shader
	{
		protected static Vec4 Texture(Sampler1D s, float x) { throw new NotSupportedException(); }
		protected static Vec4 Texture(Sampler2D s, Vec2 xy) { throw new NotSupportedException(); }
		protected static Vec4 Texture(Sampler3D s, Vec3 xyz) { throw new NotSupportedException(); }
	}
}