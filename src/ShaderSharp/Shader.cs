using System;

namespace ShaderSharp.Shaders
{
	public class ShaderException : System.Exception
	{
		public ShaderException(string msg) : base(msg)
		{ }
	}

	public class ShaderCompileException : ShaderException
	{
		public ShaderCompileException(string log) : base("Shader failed to compile: " + log)
		{ }
	}

	public class ShaderLinkException : ShaderException
	{
		public ShaderLinkException(string log) : base("Shader failed to link: " + log)
		{ }
	}

	public enum Layout
	{
		Automatic,
		Explicit,
		DepthLess,
		DepthGreater,
		DepthAny,
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class LayoutAttribute : Attribute
	{
		public Layout Kind;
		public int Location;
		public LayoutAttribute(Layout kind)
		{
			this.Kind = kind;
			this.Location = -1;
		}
		public LayoutAttribute(int location)
		{
			this.Kind = Layout.Explicit;
			this.Location = location;
		}
	}

	public enum ExtensionKind
	{
		ConservativeDepth, // #extension GL_ARB_conservative_depth: enable
	}
	public class ExtensionAttribute : Attribute
	{
		public ExtensionKind Kind;
		public ExtensionAttribute(ExtensionKind kind)
		{
			this.Kind = kind;
		}
	}

	public enum Semantics
	{
		Automatic = 0,
		None, // explicitly not auto
		Position, // gl_Position
		Color,
		Depth, // gl_Depth
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class SemanticAttributeAttribute : Attribute
	{
		public Semantics Semantics;
		public int Index; // COLOR0, COLOR1, etc..
		public SemanticAttributeAttribute(Semantics sem, int index)
		{
			this.Semantics = sem;
			this.Index = index;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class VertexInAttribute : SemanticAttributeAttribute
	{
		public VertexInAttribute(Semantics semantic = Semantics.Automatic, int semantic_index = -1) :
			base(semantic, semantic_index)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class FragmentInAttribute : SemanticAttributeAttribute
	{
		public FragmentInAttribute(Semantics semantic = Semantics.Automatic, int semantic_index = -1) :
			base(semantic, semantic_index)
		{ }
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class OutAttribute : SemanticAttributeAttribute
	{
		public OutAttribute(int layout_location = -1, Semantics semantic = Semantics.Automatic, int semantic_index = -1) :
			base(semantic, semantic_index)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class UniformAttribute : SemanticAttributeAttribute
	{
		public UniformAttribute(Semantics semantic = Semantics.Automatic, int semantic_index = -1) :
			   base(semantic, semantic_index)
		{ }
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class ConstantAttribute : Attribute
	{
		public ConstantAttribute()
		{ }
	}

	public abstract partial class Shader
	{
		public abstract void Use();
		public abstract void Unuse();

		protected abstract void Vertex();
		protected abstract void Fragment();
	}
}