using ShaderSharp.Shaders;

namespace DeferredTest.Shaders
{
	public abstract class DeferredShader : AgnosticShader
	{
		[VertexIn] protected abstract Vec4 VertXyzw { get; set; }
		[VertexIn] protected abstract Vec3 VertUvw { get; set; }
		[VertexIn] protected abstract Vec3 VertNrm { get; set; }
		[VertexIn] protected abstract Vec3 VertTangent { get; set; }

		[FragmentIn] protected abstract Vec4 FragPosition { get; set; }
		[FragmentIn] protected abstract Vec3 FragUvw { get; set; }
		[FragmentIn] protected abstract Mat3 FragTbn { get; set; }

		[Out] protected abstract Vec4 Diffuse { get; set; }
		[Out] protected abstract Vec4 Normal { get; set; }
		[Out] protected abstract Vec4 Unused { get; set; }

		protected abstract Vec4 GetDiffuse(Vec2 uv);
		protected abstract Vec4 GetNormal(Vec2 uv);

		protected override void Vertex()
		{
			FragPosition = ModelViewProjection * new Vec4(VertXyzw.Xyz, 1.0f);
			FragUvw = VertUvw;

			Mat3 normalMatrix = Transpose(Inverse(new Mat3(Model)));
			Vec3 t = Normalize(normalMatrix * VertTangent);
			Vec3 n = Normalize(normalMatrix * VertNrm);
			t = Normalize(t - Dot(t, n) * n);
			Vec3 b = Cross(n, t);

			FragTbn = new Mat3(t, b, n);
		}
		protected override void Fragment()
		{
			Vec4 diffuse = GetDiffuse(FragUvw.Xy);
			diffuse.Rgb *= RenderColor;
			Diffuse = diffuse;

			Vec4 normal_height = GetNormal(FragUvw.Xy);
			Normal = new Vec4(FragTbn * normal_height.Xyz, normal_height.A);

			Unused = new Vec4();
		}
	}

	public abstract class StandardDeferredShader : DeferredShader
	{
		[Uniform] public abstract Sampler2D DiffuseTexture { get; set; }
		[Uniform] public abstract Sampler2D NormalTexture { get; set; }

		protected override Vec4 GetDiffuse(Vec2 uv)
		{
			return Texture(DiffuseTexture, FragUvw.Xy).Rgba;
		}

		protected override Vec4 GetNormal(Vec2 uv)
		{
			var norm = Texture(NormalTexture, FragUvw.Xy).Rgba;
			return new Vec4(Normalize(norm.Xyz * 2.0f - 1.0f), norm.A);
		}
	}
}
