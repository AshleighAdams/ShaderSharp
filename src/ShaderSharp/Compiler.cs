using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;

using ShaderSharp.Shaders;

using Syntax = ICSharpCode.Decompiler.CSharp.Syntax;

namespace ShaderSharp
{
	public record CompiledShader
	{
		public CompiledShader(string vertexCode, string fragmentCode)
		{
			VertexCode = vertexCode;
			FragmentCode = fragmentCode;
		}

		public string VertexCode { get; init; }
		public string FragmentCode { get; init; }
	}

	public static class Shader
	{
		public class DynamicSemanticAttribute : Shaders.SemanticAttributeAttribute
		{
			public string Name { get; set; }
			public DynamicSemanticAttribute(Shaders.Semantics sem, int index, string name) :
				base(sem, index)
			{
				Name = name;
			}
		}

		private static readonly Dictionary<string, DynamicSemanticAttribute> AutoAttributes = new Dictionary<string, DynamicSemanticAttribute>();
		public static bool TryGetSemantic(MemberInfo prop, out DynamicSemanticAttribute attr)
		{
			return AutoAttributes.TryGetValue($"{prop.DeclaringType.FullName}.{prop.Name}", out attr);
		}
		public static bool TryGetSemantic(string fulltypename, out DynamicSemanticAttribute attr)
		{
			return AutoAttributes.TryGetValue(fulltypename, out attr);
		}
		public static void SetSemantic(MemberInfo prop, DynamicSemanticAttribute attr)
		{
			AutoAttributes[$"{prop.DeclaringType.FullName}.{prop.Name}"] = attr;
		}

		private static string GenerateLayoutLocation(PropertyInfo prop, ref int auto_location)
		{
			string ret = null;
			if (prop.GetCustomAttributes<Shaders.LayoutAttribute>(true).Count() > 0)
			{
				var layout = prop.GetCustomAttribute<Shaders.LayoutAttribute>(true);
				ret = layout.Kind switch
				{
					Layout.DepthAny => "depth_any",
					Layout.DepthLess => "depth_less",
					Layout.DepthGreater => "depth_greater",
					Layout.Explicit => $"location = {layout.Location}",
					Layout.Automatic => null,
					_ => null,
				};
			}

			return ret ?? $"location = {auto_location++}";
		}

		public static CompiledShader Compile<T>()
		{
			var basetype = typeof(T);

			// generate the code for it
			var vert_in = basetype.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
				.Where(prop => Attribute.IsDefined(prop, typeof(Shaders.VertexInAttribute)));
			var frag_in = basetype.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
				.Where(prop => Attribute.IsDefined(prop, typeof(Shaders.FragmentInAttribute)));
			var @out = basetype.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
				.Where(prop => Attribute.IsDefined(prop, typeof(Shaders.OutAttribute)));
			var uniforms = basetype.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(prop => Attribute.IsDefined(prop, typeof(Shaders.UniformAttribute)));
			var constants = basetype.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(prop => Attribute.IsDefined(prop, typeof(Shaders.ConstantAttribute)));
			var extensions = basetype.GetCustomAttributes<ExtensionAttribute>(true);

			// automatic semantics
			int pos_n = 0;
			foreach (var v in frag_in)
			{
				if (TryGetSemantic(v, out var _))
					break; // already done it
				if (v.GetCustomAttribute<Shaders.FragmentInAttribute>().Semantics != Shaders.Semantics.Automatic)
					continue;

				// first vec 4 with pos in the name, assume it's gl_Position
				if (v.Name.ToLowerInvariant().Contains("pos") && v.PropertyType == typeof(Shaders.Vec4) && pos_n++ == 0)
					SetSemantic(v, new DynamicSemanticAttribute(Shaders.Semantics.Position, 0, "gl_Position"));
			}

			var extensions_sb = new StringBuilder();
			foreach (var extension in extensions)
			{
				string as_str = extension.Kind switch
				{
					ExtensionKind.ConservativeDepth => "#extension GL_ARB_conservative_depth: enable",
					_ => null,
				};

				if (as_str == null)
				{
					System.Console.WriteLine($"Unknown extension {extension.Kind}, ignoring...");
					continue;
				}

				extensions_sb.AppendLine(as_str);
			}

			foreach (var v in @out)
			{
				if (TryGetSemantic(v, out var _))
					break; // already done it
				if (v.GetCustomAttribute<Shaders.OutAttribute>().Semantics != Shaders.Semantics.Automatic)
					continue;

				if (v.Name.ToLowerInvariant().Contains("depth")
						&& (v.PropertyType == typeof(float) || v.PropertyType == typeof(double)))
					SetSemantic(v, new DynamicSemanticAttribute(Shaders.Semantics.Depth, 0, "gl_FragDepth"));
			}


			var uniformsstr = new StringBuilder();
			{
				foreach (var uni in uniforms)
				{
					string typestr = Shaders.OpenTKShaderCompiler.GlslTypeConverter[uni.PropertyType].TypeString;
					uniformsstr.AppendLine($"uniform {typestr} {uni.Name};");
				}
				uniformsstr.AppendLine();
			}

			var constantsstr = new StringBuilder();
			{
				foreach (var @const in constants)
				{
					var type_converter = Shaders.OpenTKShaderCompiler.GlslTypeConverter[@const.PropertyType];
					constantsstr.AppendLine($"{type_converter.TypeString} {@const.Name} = {type_converter.ValueGenerator(@const.GetValue(null))};");
				}
				constantsstr.AppendLine();
			}

			static Syntax.SyntaxTree get_expression(MethodInfo method)
			{
				var decopts = new ICSharpCode.Decompiler.DecompilerSettings(languageVersion: ICSharpCode.Decompiler.CSharp.LanguageVersion.Latest);
				var decompiler = new ICSharpCode.Decompiler.CSharp.CSharpDecompiler(new Uri(method.DeclaringType.Assembly.CodeBase).LocalPath, decopts);
				var name = new ICSharpCode.Decompiler.TypeSystem.FullTypeName(method.DeclaringType.FullName);

				var definition = decompiler.TypeSystem.FindType(name).GetDefinition();
				var member = definition.Methods.Single(p => p.Name.Equals(method.Name));

				return decompiler.Decompile(member.MetadataToken);
			};

			string vert_main_func;
			var vert_header = new StringBuilder();
			var vert_funcs = new StringBuilder();
			{
				vert_header.AppendLine("#version 330 core");
				vert_header.Append(extensions_sb.ToString());
				vert_header.AppendLine();

				int auto_layout = 0;
				foreach (var input in vert_in)
				{
					string typestr = Shaders.OpenTKShaderCompiler.GlslTypeConverter[input.PropertyType].TypeString;
					string loc = GenerateLayoutLocation(input, ref auto_layout);

					vert_header.AppendLine($"layout({loc}) in {typestr} {input.Name};");
				}

				vert_header.AppendLine();
				foreach (var output in frag_in)
				{
					if (TryGetSemantic(output, out var attr) && attr.Semantics == Shaders.Semantics.Position)
						continue;

					string typestr = Shaders.OpenTKShaderCompiler.GlslTypeConverter[output.PropertyType].TypeString;
					vert_header.AppendLine($"out {typestr} {output.Name};");
				}

				vert_header.AppendLine();

				var fb = new Shaders.OpenTKShaderCompiler.FuncBuilder(vert_header, vert_funcs, typeof(T))
				{
					GetExpression = get_expression
				};

				MethodInfo vert_func = basetype.GetMethod("Vertex", BindingFlags.Instance | BindingFlags.NonPublic);

				vert_main_func = Shaders.OpenTKShaderCompiler.CompileGlslFunctionCall(vert_func, true, fb);
			}

			string vert =
				vert_header.ToString() +
				uniformsstr.ToString() +
				constantsstr.ToString() +
				vert_funcs.ToString() +
				$"void main() {{ {vert_main_func}(); }}";

			string frag_main_func;
			var frag_header = new StringBuilder();
			var frag_funcs = new StringBuilder();
			{
				List<string> functions_order = new List<string>();

				frag_header.AppendLine("#version 330 core");
				frag_header.Append(extensions_sb.ToString());
				frag_header.AppendLine();

				foreach (var input in frag_in)
				{
					string typestr = Shaders.OpenTKShaderCompiler.GlslTypeConverter[input.PropertyType].TypeString;

					if (TryGetSemantic(input, out var attr) && attr.Semantics == Shaders.Semantics.Position)
						continue;
					frag_header.AppendLine($"in {typestr} {input.Name};");
				}

				frag_header.AppendLine();
				int auto_layout = 0;
				foreach (var output in @out)
				{
					string output_name = output.Name;
					if (TryGetSemantic(output, out var attr))
						output_name = attr.Name;

					string typestr = Shaders.OpenTKShaderCompiler.GlslTypeConverter[output.PropertyType].TypeString;
					string loc = GenerateLayoutLocation(output, ref auto_layout);
					frag_header.AppendLine($"layout({loc}) out {typestr} {output_name};");
				}

				frag_header.AppendLine();

				var fb = new Shaders.OpenTKShaderCompiler.FuncBuilder(frag_header, frag_funcs, typeof(T))
				{
					GetExpression = get_expression
				};

				MethodInfo frag_func = basetype.GetMethod("Fragment", BindingFlags.Instance | BindingFlags.NonPublic);
				frag_main_func = Shaders.OpenTKShaderCompiler.CompileGlslFunctionCall(frag_func, true, fb);
			}

			string frag =
				frag_header.ToString() +
				uniformsstr.ToString() +
				constantsstr.ToString() +
				frag_funcs.ToString() +
				$"void main() {{ {frag_main_func}(); }}";

			return new CompiledShader(vert, frag);
		}
	}

	namespace Shaders
	{
		using ICSharpCode.Decompiler.CSharp.Syntax;
		public static class OpenTKShaderCompiler
		{
			public static Type GetUnderlyingType(this MemberInfo member)
			{
				return member.MemberType switch
				{
					MemberTypes.Event => ((EventInfo)member).EventHandlerType,
					MemberTypes.Field => ((FieldInfo)member).FieldType,
					MemberTypes.Method => ((MethodInfo)member).ReturnType,
					MemberTypes.Property => ((PropertyInfo)member).PropertyType,
					_ => throw new ArgumentException
						(
						 "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
						),
				};
			}

			public struct TypeConverter
			{
				public string TypeString { get; set; }
				public string DefaultConstruction { get; set; }
				public Func<PrimitiveExpression, string> ConstantGenerator { get; set; }
				public Func<object, string> ValueGenerator { get; set; }
				public TypeConverter(string a, string default_ctor, Func<PrimitiveExpression, string> const_gen, Func<object, string> value_gen)
				{
					TypeString = a;
					DefaultConstruction = default_ctor;
					ConstantGenerator = const_gen;
					ValueGenerator = value_gen;
				}
			}
			public static Dictionary<Type, TypeConverter> GlslTypeConverter { get; set; } = new()
			{
				{
					typeof(void),
					new TypeConverter(
						"void", "void()",
						(p) => throw new NotImplementedException(),
						(p) => throw new NotImplementedException())
				},
				{
					typeof(float),
					new TypeConverter(
						"float", "0.0",
						(p) => $"{(float)p.Value:0.0#####################}",
						(p) => $"{(float)p:0.0#####################}")
				},
				{
					typeof(double),
					new TypeConverter(
						 "double", "0.0d",
						 (p) => $"{(float)p.Value:0.0###########################################}d",
						 (p) => $"{(float)p:0.0###########################################}d")
				},
				{
					typeof(int),
					new TypeConverter(
						"int", "0",
						(p) => $"{(int)p.Value}",
						(p) => $"{(int)p}")
				},
				{
					typeof(uint),
					new TypeConverter(
						"uint", "0u",
						(p) => $"{(uint)p.Value}u",
						(p) => $"{(uint)p}u")
				},
				{
					typeof(bool),
					new TypeConverter(
						"bool", "false",
						(p) => p.ToString(),
						(p) => p.ToString())
				},
				{
					typeof(Mat3),
					new TypeConverter(
						"mat3", "mat3()",
						(p) => p.ToString(),
						(p) => throw new NotImplementedException())
				},
				{
					typeof(Mat4),
					new TypeConverter(
						"mat4", "mat4()",
						(p) => p.ToString(),
						(p) => throw new NotImplementedException())
				},
				{
					typeof(Vec2),
					new TypeConverter(
						"vec2", "vec2(0.0)",
						(p) => p.ToString(),
						(p) =>
						{
							var val = (Vec2)p;
							return $"vec2({val.X:0.0#####################}, {val.Y:0.0#####################})";
						})
				},
				{
					typeof(Vec3),
					new TypeConverter(
						"vec3", "vec3(0.0)",
						(p) => p.ToString(),
						(p) =>
						{
							var val = (Vec3)p;
							return $"vec3({val.X:0.0#####################}, {val.Y:0.0#####################}, {val.Z:0.0#####################})";
						})
				},
				{
					typeof(Vec4),
					new TypeConverter(
						"vec4", "vec4(0.0)",
						(p) => p.ToString(),
						(p) =>
						{
							var val = (Vec4)p;
							return $"vec4({val.X:0.0#####################}, {val.Y:0.0#####################}, {val.Z:0.0#####################}, {val.W:0.0#####################})";
						})
				},
				{
					typeof(Sampler1D),
					new TypeConverter(
						"sampler1D", "sampler1D()",
						(p) => p.ToString(),
						(p) => throw new NotImplementedException())
				},
				{
					typeof(Sampler2D),
					new TypeConverter(
						"sampler2D", "sampler2D()",
						(p) => p.ToString(),
						(p) => throw new NotImplementedException())
				},
				{
					typeof(Sampler3D),
					new TypeConverter(
						"sampler3D", "sampler3D()",
						(p) => p.ToString(),
						(p) => throw new NotImplementedException())
				},
				{
					typeof(SamplerCube),
					new TypeConverter(
						"samplerCube", "samplerCube()",
						(p) => p.ToString(),
						(p) => throw new NotImplementedException())
				},
			};

			public class FuncBuilder
			{
				public StringBuilder Prototypes { get; set; }
				public StringBuilder Implementations { get; set; }
				public HashSet<MethodInfo> Done { get; set; } = new HashSet<MethodInfo>();
				public Func<MethodInfo, Syntax.SyntaxTree> GetExpression { get; set; }
				public Type ShaderType { get; set; }
				public FuncBuilder(StringBuilder protos, StringBuilder imps, Type shader_type)
				{
					Prototypes = protos;
					Implementations = imps;
					ShaderType = shader_type;
				}
			}

			private static Func<InvocationExpression, FuncBuilder, string> GlslSimpleTranslator(string name)
			{
				return delegate (InvocationExpression call, FuncBuilder fb)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(name);
					sb.Append('(');

					string pre = "";
					foreach (var arg in call.Arguments)
					{
						sb.Append(pre);
						pre = ", ";
						sb.Append(CompileGlslExpression(arg, fb));
					}

					sb.Append(')');

					return sb.ToString();
				};

			}

			private static Func<string, Expression, FuncBuilder, string> GlslSimpleMemberTranslator()
			{
				return delegate (string member, Expression obj, FuncBuilder fb)
				{
					member = member.ToLowerInvariant();
					if (member.StartsWith("get_"))
						member = member.Substring("get_".Length);
					else if (member.StartsWith("set_"))
						member = member.Substring("set_".Length);

					return $"{CompileGlslExpression(obj, fb)}.{member}";
				};
			}

			private static readonly Dictionary<System.Type, Func<string, Expression, FuncBuilder, string>> GlslMemberTranslator = new Dictionary<System.Type, Func<string, Expression, FuncBuilder, string>>()
			{
				{ typeof(Vec2), GlslSimpleMemberTranslator() },
				{ typeof(Vec3), GlslSimpleMemberTranslator() },
				{ typeof(Vec4), GlslSimpleMemberTranslator() },
			};
			private static readonly Dictionary<string, Func<InvocationExpression, FuncBuilder, string>> GlslFuncTranslator = new Dictionary<string, Func<InvocationExpression, FuncBuilder, string>>()
			{
				{ "Transpose",  GlslSimpleTranslator("transpose") },
				{ "Inverse",    GlslSimpleTranslator("inverse") },
				{ "Normalize",  GlslSimpleTranslator("normalize") },
				{ "Length",     GlslSimpleTranslator("length") },
				{ "Texture",    GlslSimpleTranslator("texture") },
				{ "Ceil",       GlslSimpleTranslator("ceil") },
				{ "Floor",      GlslSimpleTranslator("floor") },
				{ "Round",      GlslSimpleTranslator("round") },
				{ "Log",        GlslSimpleTranslator("log") },
				{ "Log10",      GlslSimpleTranslator("log10") },
				{ "Pow",        GlslSimpleTranslator("pow") },
				{ "Exp",        GlslSimpleTranslator("exp") },
				{ "Sin",        GlslSimpleTranslator("sin") },
				{ "Cos",        GlslSimpleTranslator("cos") },
				{ "Tan",        GlslSimpleTranslator("tan") },
				{ "Mix",        GlslSimpleTranslator("mix") },
				{ "Min",        GlslSimpleTranslator("min") },
				{ "Max",        GlslSimpleTranslator("max") },
				{ "Clamp",      GlslSimpleTranslator("clamp") },
				{ "Abs",        GlslSimpleTranslator("abs") },
				{ "Sqrt",       GlslSimpleTranslator("sqrt") },
				{ "SmoothStep", GlslSimpleTranslator("smoothstep") },
				{ "Dot",        GlslSimpleTranslator("dot") },
				{ "Cross",      GlslSimpleTranslator("cross") },
			};

			public static string Mangle(string @namespace, string name, Type[] types)
			{
				var sb = new StringBuilder();
				sb.Append("_Z");

				if (@namespace.Length > 0)
				{
					sb.Append("N");
					foreach (string s in @namespace.Split('.', '+'))
					{
						sb.Append(s.Length);
						sb.Append(s);
					}

					sb.Append("E");
				}

				sb.Append(name.Length);
				sb.Append(name);

				if (types != null && types.Length != 0)
				{
					sb.Append("E");
					foreach (Type t in types)
					{
						sb.Append("P");
						string mangled_type = Mangle(t.Namespace, name, null);

						sb.Append(mangled_type.Length);
						sb.Append(mangled_type);
					}
				}

				return sb.ToString();
			}

			public static string CompileGlslFunctionCall(MethodInfo method, bool virtcall, FuncBuilder fb)
			{
				Type[] params_types = method.GetParameters()
					.Select(x => x.ParameterType)
					.ToArray();

				MethodInfo concrete = !virtcall
					? method
					: fb.ShaderType.GetMethod(method.Name, BindingFlags.NonPublic | BindingFlags.Instance, null, params_types, null);

				string name = Mangle(concrete.DeclaringType.FullName, concrete.Name, params_types);

				if (!fb.Done.Contains(method))
				{
					if (method.IsSpecialName || method.IsConstructor)
						throw new NotSupportedException();
					if (!method.DeclaringType.IsSubclassOf(typeof(Shaders.Shader)))
						throw new NotSupportedException();

					fb.Done.Add(method);

					var typeinf = GlslTypeConverter[method.ReturnType];

					string prototype = $"{typeinf.TypeString} {name}(";
					string pre = "";
					foreach (var arg in concrete.GetParameters())
					{
						typeinf = GlslTypeConverter[arg.ParameterType];
						prototype += $"{pre}{typeinf.TypeString} {arg.Name}";
						pre = ", ";
					}
					prototype += $");";

					string code = CompileGlslExpression(fb.GetExpression(concrete), fb, method: name);

					fb.Implementations.AppendLine(code);
					fb.Prototypes.AppendLine(prototype);
				}

				return name;
			}

			public static Type FindType(string reflection_name)
			{
				return AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(a => a.GetTypes())
					.First(t => t.FullName == reflection_name);
			}

			public static string CompileGlslFunctionCall(InvocationExpression call, FuncBuilder fb)
			{
				var invocinfo = call.Annotations
					.Where(t => t is ICSharpCode.Decompiler.Semantics.InvocationResolveResult)
					.Select(x => x as ICSharpCode.Decompiler.Semantics.InvocationResolveResult)
					.First();

				Type[] argtypes = invocinfo.Arguments
					.Select(x => FindType(x.Type.ReflectionName))
					.ToArray();

				var type = FindType(invocinfo.Member.DeclaringType.ReflectionName);
				var methodinfo = type.GetMethod(invocinfo.Member.Name, BindingFlags.Instance | BindingFlags.NonPublic, null, argtypes, null);

				string mangled_name = CompileGlslFunctionCall(methodinfo, invocinfo.IsVirtualCall, fb);

				var sb = new StringBuilder();

				sb.Append(mangled_name);
				sb.Append('(');
				{
					string pre = "";
					foreach (var param in call.Arguments)
					{
						sb.Append(pre);
						pre = ", ";
						sb.Append(CompileGlslExpression(param, fb));
					}
				}
				sb.Append(')');

				return sb.ToString();
			}

			private static readonly Dictionary<UnaryOperatorType, (string pre, string post)> UnaryOperatorMap = new Dictionary<UnaryOperatorType, (string pre, string post)>()
			{
				{ UnaryOperatorType.AddressOf,                 ("&", "") },
				{ UnaryOperatorType.Await,                     ("await ", "") },
				{ UnaryOperatorType.BitNot,                    ("~", "") },
				{ UnaryOperatorType.Decrement,                 ("--", "") },
				{ UnaryOperatorType.Dereference,               ("*", "") },
				{ UnaryOperatorType.Increment,                 ("++", "") },
				{ UnaryOperatorType.IsTrue,                    ("!!", "") },
				{ UnaryOperatorType.Minus,                     ("-", "") },
				{ UnaryOperatorType.Not,                       ("!", "") },
				{ UnaryOperatorType.NullConditional,           ("?", "") },
				{ UnaryOperatorType.NullConditionalRewrap,     ("(", ")") },
				{ UnaryOperatorType.Plus,                      ("+", "") },
				{ UnaryOperatorType.PostDecrement,             ("", "--") },
				{ UnaryOperatorType.PostIncrement,             ("", "++") },
				//{ UnaryOperatorType.SuppressNullableWarning, ("", "!") },
			};
			private static readonly Dictionary<BinaryOperatorType, string> BinaryOperatorMap = new Dictionary<BinaryOperatorType, string>()
			{
				{ BinaryOperatorType.Add,                "+" },
				{ BinaryOperatorType.BitwiseAnd,         "&" },
				{ BinaryOperatorType.BitwiseOr,          "|" },
				{ BinaryOperatorType.ConditionalAnd,     "&&" },
				{ BinaryOperatorType.ConditionalOr,      "||" },
				{ BinaryOperatorType.Divide,             "/" },
				{ BinaryOperatorType.Equality,           "==" },
				{ BinaryOperatorType.ExclusiveOr,        "^" },
				{ BinaryOperatorType.GreaterThan,        ">" },
				{ BinaryOperatorType.GreaterThanOrEqual, ">=" },
				{ BinaryOperatorType.InEquality,         "!=" },
				{ BinaryOperatorType.LessThan,           "<" },
				{ BinaryOperatorType.LessThanOrEqual,    "<=" },
				{ BinaryOperatorType.Modulus,            "%" },
				{ BinaryOperatorType.Multiply,           "*" },
				//{ BinaryOperatorType.NullCoalescing,   "??" },
				{ BinaryOperatorType.ShiftLeft,          "<<" },
				{ BinaryOperatorType.ShiftRight,         ">>" },
				{ BinaryOperatorType.Subtract,           "-" },
			};
			private static readonly Dictionary<AssignmentOperatorType, string> AssignmentOperatorMap = new Dictionary<AssignmentOperatorType, string>()
			{
				{ AssignmentOperatorType.Add,                "+=" },
				{ AssignmentOperatorType.Assign,             "=" },
				{ AssignmentOperatorType.BitwiseAnd,         "&=" },
				{ AssignmentOperatorType.BitwiseOr,          "|=" },
				{ AssignmentOperatorType.Divide,             "/=" },
				{ AssignmentOperatorType.ExclusiveOr,        "^=" },
				{ AssignmentOperatorType.Modulus,            "%=" },
				{ AssignmentOperatorType.Multiply,           "*=" },
				{ AssignmentOperatorType.ShiftLeft,          "<<=" },
				{ AssignmentOperatorType.ShiftRight,         ">>=" },
				{ AssignmentOperatorType.Subtract,           "-=" },
			};

			public static string CompileGlslExpression(AstNode node, FuncBuilder fb, string depth = "", string method = null)
			{
				if (node is SyntaxTree tree)
				{
					return CompileGlslExpression(tree.FirstChild, fb, depth, method);
				}
				else if (node is PrimitiveExpression primative)
				{
					if (GlslTypeConverter.TryGetValue(primative.Value.GetType(), out var converter))
						return converter.ConstantGenerator(primative);
					throw new NotImplementedException();
				}
				else if (node is PrimitiveType primative_type)
				{
					return primative_type.Keyword;
				}
				else if (node is Identifier identfier)
				{
					return $"_{identfier.Name}";
				}
				else if (node is IdentifierExpression id_expr)
				{
					var member = id_expr.Annotations
						.Where(t => t is ICSharpCode.Decompiler.Semantics.MemberResolveResult)
						.Select(x => x as ICSharpCode.Decompiler.Semantics.MemberResolveResult)
						.ToList();

					if (member.Count == 1)
					{
						if (global::ShaderSharp.Shader.TryGetSemantic(member.First().Member.ReflectionName, out var attr))
						{
							return attr.Name;
						}

						return id_expr.IdentifierToken.Name;
					}

					return CompileGlslExpression(id_expr.IdentifierToken, fb, depth, method);
				}
				else if (node is SimpleType simple_type)
				{
					var typeis = (simple_type.Annotations
						.Where(t => t is ICSharpCode.Decompiler.Semantics.TypeResolveResult)
						.First() as ICSharpCode.Decompiler.Semantics.TypeResolveResult)
						.Type;

					if (GlslTypeConverter.TryGetValue(FindType(typeis.FullName), out var translator))
						return translator.TypeString;

					var generics = new StringBuilder();
					string pre = "";

					// TODO: Translate intrinsics to glsl!
					foreach (var type in simple_type.TypeArguments)
					{
						generics.Append($"{pre}{CompileGlslExpression(type, fb, depth, method)}");
						pre = ", ";
					}

					return $"{CompileGlslExpression(simple_type.IdentifierToken, fb, depth, method)}<{generics}>";
				}
				else if (node is TypeReferenceExpression type_ref)
				{
					return $"{CompileGlslExpression(type_ref.Type, fb, depth, method)}";
				}
				else if (node is MemberReferenceExpression member_ref)
				{
					var typeis = (member_ref.Annotations
						.Where(x => x is ICSharpCode.Decompiler.Semantics.MemberResolveResult)
						.First() as ICSharpCode.Decompiler.Semantics.MemberResolveResult)
						.TargetResult.Type;

					var resolved_type = FindType(typeis.FullName);

					if (GlslMemberTranslator.TryGetValue(resolved_type, out var translator))
						return translator(member_ref.MemberName, member_ref.Target, fb);

					var generics = new StringBuilder();
					string pre = "";
					foreach (var type in member_ref.TypeArguments)
					{
						generics.Append($"{pre}{CompileGlslExpression(type, fb, depth, method)}");
						pre = ", ";
					}

					return $"{CompileGlslExpression(member_ref.Target, fb, depth, method)}.{CompileGlslExpression(member_ref.MemberNameToken, fb, depth, method)}<{generics}>";
				}
				else if (node is UnaryOperatorExpression unop)
				{
					if (UnaryOperatorMap.TryGetValue(unop.Operator, out var op))
						return $"({op.pre}{CompileGlslExpression(unop.Expression, fb, depth, method)}{op.post})";
					throw new NotSupportedException();
				}
				else if (node is BinaryOperatorExpression binop)
				{
					if (BinaryOperatorMap.TryGetValue(binop.Operator, out var op))
						return $"({CompileGlslExpression(binop.Left, fb, depth, method)} {op} {CompileGlslExpression(binop.Right, fb, depth, method)})";
					throw new NotSupportedException();
				}
				else if (node is AssignmentExpression assignment)
				{
					if (!AssignmentOperatorMap.TryGetValue(assignment.Operator, out var oper))
						throw new NotSupportedException("assignment expr");
					return $"{CompileGlslExpression(assignment.Left, fb, depth, method)} {oper} {CompileGlslExpression(assignment.Right, fb, depth, method)}";
				}
				else if (node is ExpressionStatement statement)
				{
					return $"{CompileGlslExpression(statement.Expression, fb, depth, method)};";
				}
				else if (node is ParenthesizedExpression paren)
				{
					return $"({CompileGlslExpression(paren.Expression, fb, depth, method)})";
				}
				else if (node is InvocationExpression call)
				{
					if (call.Target is MemberReferenceExpression call_member_ref)
					{
						if (call_member_ref.Target is TypeReferenceExpression memberof)
						{
							var typeis = memberof.Annotations
								.Where(t => t is ICSharpCode.Decompiler.Semantics.TypeResolveResult)
								.Select(x => x as ICSharpCode.Decompiler.Semantics.TypeResolveResult)
								.First()
								.Type;

							if (typeof(Shader).FullName == typeis.FullName)
							{
								if (GlslFuncTranslator.TryGetValue(call_member_ref.MemberName, out var translator))
									return translator(call, fb);
							}
						}
					}

					return CompileGlslFunctionCall(call, fb);
				}
				else if (node is ObjectCreateExpression new_expr)
				{
					var typeis = new_expr.Type.Annotations
						.Where(t => t is ICSharpCode.Decompiler.Semantics.ResolveResult)
						.Select(x => x as ICSharpCode.Decompiler.Semantics.ResolveResult)
						.First()
						.Type;

					string type_str = typeis.FullName;
					if (GlslTypeConverter.TryGetValue(FindType(type_str), out var translator))
					{
						type_str = translator.TypeString;
					}

					var sb = new StringBuilder();

					sb.Append(type_str);
					sb.Append('(');
					{
						string pre = "";
						foreach (var param in new_expr.Arguments)
						{
							sb.Append(pre);
							pre = ", ";
							sb.Append(CompileGlslExpression(param, fb, depth, method));
						}
					}
					sb.Append(')');

					return sb.ToString();
				}
				else if (node is DefaultValueExpression default_expr)
				{
					var typeis = default_expr.Type.Annotations
						.Where(t => t is ICSharpCode.Decompiler.Semantics.ResolveResult)
						.Select(x => x as ICSharpCode.Decompiler.Semantics.ResolveResult)
						.First()
						.Type;

					string type_str = typeis.FullName;
					if (GlslTypeConverter.TryGetValue(FindType(type_str), out var translator))
						return translator.DefaultConstruction;
					throw new NotSupportedException();
				}
				else if (node is VariableDeclarationStatement var_decl)
				{
					var sb = new StringBuilder();
					string type = CompileGlslExpression(var_decl.Type, fb, depth, method);

					foreach (var variable in var_decl.Variables)
					{
						sb.Append($"{type} {CompileGlslExpression(variable.NameToken, fb, depth, method)}");

						if (variable.Initializer != null)
						{
							sb.Append($" = {CompileGlslExpression(variable.Initializer, fb, depth, method)}");
						}
						sb.Append(';');
					}

					return sb.ToString();
				}
				else if (node is MethodDeclaration func_decl)
				{
					var sb = new StringBuilder();

					// Mangle

					var resolve = func_decl.Annotations
								.Where(t => t is ICSharpCode.Decompiler.Semantics.MemberResolveResult)
								.Select(x => x as ICSharpCode.Decompiler.Semantics.MemberResolveResult)
								.First();

					Type[] arg_types = func_decl.Parameters
						.Select(x => FindType(x.Type.Annotations
							.Where(t => t is ICSharpCode.Decompiler.Semantics.ResolveResult)
							.Select(y => y as ICSharpCode.Decompiler.Semantics.ResolveResult)
							.First().Type.ReflectionName))
						.ToArray();

					string mangled = Mangle(resolve.Member.DeclaringType.ReflectionName, resolve.Member.Name, arg_types);

					sb.Append(CompileGlslExpression(func_decl.ReturnType, fb, depth, method));
					sb.Append(' ');
					sb.Append(mangled);
					sb.Append('(');
					{
						string pre = "";
						foreach (var param in func_decl.Parameters)
						{
							sb.Append(pre);
							pre = ", ";

							sb.Append(CompileGlslExpression(param.Type, fb, depth, method));
							sb.Append(' ');
							sb.Append(CompileGlslExpression(param.NameToken, fb, depth, method));
						}
					}
					sb.Append(')');

					sb.AppendLine(CompileGlslExpression(func_decl.Body, fb, depth, method));

					return sb.ToString();
				}
				else if (node is BlockStatement block)
				{
					var sb = new StringBuilder();

					sb.AppendLine();

					string newdepth = $"{depth}\t";

					sb.AppendLine($"{depth}{{");

					foreach (var sub_node in block.Statements)
					{
						sb.AppendLine($"{newdepth}{CompileGlslExpression(sub_node, fb, newdepth, method)}");
					}

					sb.AppendLine($"{depth}}}");

					return sb.ToString();
				}
				else if (node is ReturnStatement ret)
				{
					return ret.Expression == Expression.Null ?
						$"return;" :
						$"return {CompileGlslExpression(ret.Expression, fb, depth, method)};";
				}
				else if (node is Comment comment)
				{
					return $"{depth}/*{comment.Content}*/";
				}
				else if (node is UsingDeclaration usingdecl)
				{
					return CompileGlslExpression(usingdecl.NextSibling, fb, depth, method);
				}
				else if (node is IfElseStatement ifelse)
				{
					string ifstr =
						$"if ({CompileGlslExpression(ifelse.Condition, fb, depth, method)})\n" +
							$"{depth + "\t"}{CompileGlslExpression(ifelse.TrueStatement, fb, depth + "\t", method)}";

					if (ifelse.FalseStatement != null && ifelse.FalseStatement != Statement.Null)
						ifstr +=
							$"\n{depth}else\n" +
								$"{depth + "\t"}{CompileGlslExpression(ifelse.FalseStatement, fb, depth + "\t", method)}\n";

					return ifstr;
				}
				else
				{
					throw new Exception($"/* Unsupported node: {node.GetType().Name}: {node} */");
				}
			}
		}
	}
}
