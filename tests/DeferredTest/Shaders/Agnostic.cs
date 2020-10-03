using ShaderSharp.Shaders;

namespace DeferredTest.Shaders
{
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
}
