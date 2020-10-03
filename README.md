# ShaderSharp

Prototype that compiles C# classes like this:

```cs
public abstract class AgnosticShader : Shader
{
	[Uniform] public abstract float Time { get; set; }
	[Uniform] public abstract Mat4 ModelViewProjection { get; set; }
	[Uniform] public abstract Mat4 Model { get; set; }
	[Uniform] public abstract Vec3 ViewDirection { get; set; }
	[Uniform] public abstract Vec3 RenderColor { get; set; }

	protected AgnosticShader()
	{
		Time = 0;
		ModelViewProjection = new Mat4();
		Model = new Mat4();
		ViewDirection = new Vec3();
		RenderColor = new Vec3(1.0f, 1.0f, 1.0f);
	}
}

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
```

Into this:

```glsl
#version 330 core

layout(location = 0) in vec4 VertXyzw;
layout(location = 1) in vec3 VertUvw;
layout(location = 2) in vec3 VertNrm;
layout(location = 3) in vec3 VertTangent;

out vec3 FragUvw;
out mat3 FragTbn;

void _ZN12DeferredTest7Shaders14DeferredShaderE6Vertex();
uniform sampler2D DiffuseTexture;
uniform sampler2D NormalTexture;
uniform float Time;
uniform mat4 ModelViewProjection;
uniform mat4 Model;
uniform vec3 ViewDirection;
uniform vec3 RenderColor;


void _ZN12DeferredTest7Shaders14DeferredShaderE6Vertex()
{
	gl_Position = (ModelViewProjection * vec4(VertXyzw.xyz, 1.0));
	FragUvw = VertUvw;
	mat3 _left = transpose(inverse(mat3(Model)));
	vec3 _vec = normalize((_left * VertTangent));
	vec3 _vec2 = normalize((_left * VertNrm));
	_vec = normalize((_vec - (dot(_vec, _vec2) * _vec2)));
	vec3 _b = cross(_vec2, _vec);
	FragTbn = mat3(_vec, _b, _vec2);
}


void main() { _ZN12DeferredTest7Shaders14DeferredShaderE6Vertex(); }
```

```glsl
#version 330 core

in vec3 FragUvw;
in mat3 FragTbn;

layout(location = 0) out vec4 Diffuse;
layout(location = 1) out vec4 Normal;
layout(location = 2) out vec4 Unused;

vec4 _ZN12DeferredTest7Shaders22StandardDeferredShaderE10GetDiffuseEP37_ZN11ShaderSharp7ShadersE10GetDiffuse(vec2 uv);
vec4 _ZN12DeferredTest7Shaders22StandardDeferredShaderE9GetNormalEP35_ZN11ShaderSharp7ShadersE9GetNormal(vec2 uv);
void _ZN12DeferredTest7Shaders14DeferredShaderE8Fragment();
uniform sampler2D DiffuseTexture;
uniform sampler2D NormalTexture;
uniform float Time;
uniform mat4 ModelViewProjection;
uniform mat4 Model;
uniform vec3 ViewDirection;
uniform vec3 RenderColor;


vec4 _ZN12DeferredTest7Shaders22StandardDeferredShaderE10GetDiffuseEP37_ZN11ShaderSharp7ShadersE10GetDiffuse(vec2 _uv)
{
	return texture(DiffuseTexture, FragUvw.xy).rgba;
}


vec4 _ZN12DeferredTest7Shaders22StandardDeferredShaderE9GetNormalEP35_ZN11ShaderSharp7ShadersE9GetNormal(vec2 _uv)
{
	vec4 _rgba = texture(NormalTexture, FragUvw.xy).rgba;
	return vec4(normalize(((_rgba.xyz * 2.0) - 1.0)), _rgba.a);
}


void _ZN12DeferredTest7Shaders14DeferredShaderE8Fragment()
{
	vec4 _diffuse = _ZN12DeferredTest7Shaders22StandardDeferredShaderE10GetDiffuseEP37_ZN11ShaderSharp7ShadersE10GetDiffuse(FragUvw.xy);
	_diffuse.rgb *= RenderColor;
	Diffuse = _diffuse;
	vec4 _normal = _ZN12DeferredTest7Shaders22StandardDeferredShaderE9GetNormalEP35_ZN11ShaderSharp7ShadersE9GetNormal(FragUvw.xy);
	Normal = vec4((FragTbn * _normal.xyz), _normal.a);
	Unused = vec4(0.0);
}


void main() { _ZN12DeferredTest7Shaders14DeferredShaderE8Fragment(); }
```

## Should I use this?

**No.**

Do not use this project, this is only here for educational purposes because:

 - The code quality is abysmal.
 - ShaderGen exists, and is tried and tested in many more projects.

## Why did you write this then?

This was a concept I thought up independantly, and wanted to test it out.
I didn't discover ShaderGen until much later, and even then it was still in its infancy.

The implementation side to this also ties heavily into my experimental engine, implementing uniform getters and setters at runtime:

```cs
RenderContext rc = ...;
ITexture texture = rc.LoadTexture(pixelData, texOptions);
IMesh mesh = rc.LoadMesh(interleavedData);

DeferredShader shader = rc.LoadShader<DeferredShader>();
shader.Time = 0.5f;
shader.DiffuseTexture = rc.LoadTexture(...).AsSampler2D();
shader.Use();

rc.Start3D(cam.Position, cam.QAngles, fov);
mesh.Draw();
rc.End3D();
```

## Future Plans

I plan to clean this up, and use .NET 5's code generators to generate the shaders at build time, perhaps compiling them to SPIR-V shaders.

I'd also like to provide a shader implementation, like my engine did, but again, at compile time.
