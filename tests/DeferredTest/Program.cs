using System;

using DeferredTest.Shaders;

namespace DeferredTest
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var result = ShaderSharp.Shader.Compile<StandardDeferredShader>();

			Console.WriteLine(result.VertexCode);
			Console.WriteLine(result.FragmentCode);
		}
	}
}
