using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace GMAssetCompiler
{
	internal class GML2JavaScript
	{
		private static GMLCode ms_code;

		private static GMAssets ms_assets;

		private static int ms_numErrors;

		private static bool ms_error;

		public static string ms_varPrefix = "gml";

		public static Dictionary<string, string> ms_globals = new Dictionary<string, string>();

		private static Dictionary<string, string> ms_locals = new Dictionary<string, string>();

		private static Dictionary<string, string> ms_arguments = new Dictionary<string, string>();

		private static Stack<string> ms_thisName = new Stack<string>();

		private static Stack<string> ms_otherName = new Stack<string>();

		private static Stack<string> ms_breakContext = new Stack<string>();

		private static int ms_unique = 0;

		private static int ms_statements = 0;

		private static Dictionary<string, int> ms_arrays = new Dictionary<string, int>();

		private static StringBuilder ms_sbVariableName = new StringBuilder();

		private static string GetUniqueName()
		{
			return string.Format("__yy__v{0}", ms_unique++);
		}

		public static void Error(string _errorMessage, GMLToken _token)
		{
			if (!Program.InhibitErrorOutput)
			{
				int num = 1;
				for (int i = 0; i < _token.Index; i++)
				{
					if (ms_code.Code[i] == '\n')
					{
						num++;
					}
				}
				Console.WriteLine("Error : {0}({1}) : {2}", ms_code.Name, num, _errorMessage);
			}
			ms_numErrors++;
			ms_error = true;
			Program.ExitCode = 1;
		}

		private static eType CompileUnary(GMLToken _tok, TextWriter _sw)
		{
			eType result = eType.eGMLT_Unknown;
			switch (_tok.Id)
			{
			case 203:
				_sw.Write("!(");
				if (CompileExpression(_tok.Children[0], _sw) != 0)
				{
					_sw.Write(" > 0.5)");
				}
				else
				{
					_sw.Write(")");
				}
				result = eType.eGMLT_Bool;
				break;
			case 210:
			case 211:
			case 220:
				_sw.Write(_tok.Text);
				result = CompileExpression(_tok.Children[0], _sw);
				break;
			}
			return result;
		}

		private static eType CompileBinary(GMLToken _tok, TextWriter _sw)
		{
			eType result = 0;
			bool flag = false;
			switch (_tok.Children[1].Token)
			{
			case eToken.eDiv:
				_sw.Write("(~~");
				break;
			case eToken.eAnd:
			case eToken.eOr:
				flag = true;
				break;
			}
			_sw.Write("(");
			if (flag)
			{
				_sw.Write("(");
			}
			eType eType = CompileExpression(_tok.Children[0], _sw);
			if (flag && eType != 0)
			{
				_sw.Write(" > 0.5");
			}
			if (flag)
			{
				_sw.Write(")");
			}
			for (int i = 1; i < _tok.Children.Count; i += 2)
			{
				switch (_tok.Children[i].Token)
				{
				case eToken.eNot:
					_sw.Write("!");
					break;
				case eToken.eBitXor:
					_sw.Write("^");
					break;
				case eToken.eDiv:
					_sw.Write("/");
					break;
				case eToken.eMod:
					_sw.Write("%");
					break;
				case eToken.eAnd:
					_sw.Write("&&");
					break;
				case eToken.eOr:
					_sw.Write("||");
					break;
				case eToken.eNotEqual:
					_sw.Write("!=");
					break;
				case eToken.eAssign:
				case eToken.eEqual:
					_sw.Write("==");
					break;
				default:
					_sw.Write(_tok.Children[i].Text);
					break;
				}
				if (flag)
				{
					_sw.Write("(");
				}
				eType eType2 = CompileExpression(_tok.Children[i + 1], _sw);
				if (flag && eType2 != 0)
				{
					_sw.Write(" > 0.5");
				}
				if (flag)
				{
					_sw.Write(")");
				}
				switch (_tok.Children[i].Token)
				{
				case eToken.eAssign:
				case eToken.eAnd:
				case eToken.eOr:
				case eToken.eLess:
				case eToken.eLessEqual:
				case eToken.eEqual:
				case eToken.eNotEqual:
				case eToken.eGreaterEqual:
				case eToken.eGreater:
					result = eType.eGMLT_Bool;
					break;
				}
				eType = eType2;
			}
			_sw.Write(")");
			eToken token = _tok.Children[1].Token;
			if (token == eToken.eDiv)
			{
				_sw.Write(")");
			}
			return result;
		}

		private static eType CompileConstant(GMLToken _tok, TextWriter _sw)
		{
			bool _setFunc = false;
			eType result = eType.eGMLT_Var;
			switch (_tok.Text)
			{
			case "global":
			case "other":
			case "self":
				CompileVariable(_tok, _sw, false, out _setFunc);
				break;
			default:
				switch (_tok.Value.Kind)
				{
				case eKind.eConstant:
					_sw.Write("g_gmlConst.{0}", _tok.Value.ValueS);
					break;
				case eKind.eNone:
					_sw.Write("null");
					break;
				case eKind.eNumber:
					if (_tok.Value.ValueI < 0.0)
					{
						_sw.Write("(");
					}
					_sw.Write("{0}", Convert.ToString(_tok.Value.ValueI, CultureInfo.InvariantCulture.NumberFormat));
					if (_tok.Value.ValueI < 0.0)
					{
						_sw.Write(")");
					}
					result = ((!(_tok.Text == "true") && !(_tok.Text == "false")) ? eType.eGMLT_Real : eType.eGMLT_Bool);
					break;
				case eKind.eString:
				{
					string text = _tok.Value.ValueS.Replace("\\", "\\\\");
					text = text.Replace("\"", "\\\"");
					text = text.Replace(Environment.NewLine, "#");
					text = text.Replace(new string('\n', 1), "#");
					_sw.Write("\"{0}\"", text);
					result = eType.eGMLT_String;
					break;
				}
				}
				break;
			}
			return result;
		}

		private static eType CompileExpression(GMLToken _tok, TextWriter _sw)
		{
			eType result = eType.eGMLT_Unknown;
			bool _setFunc = false;
			switch (_tok.Token)
			{
			case eToken.eConstant:
				result = CompileConstant(_tok, _sw);
				break;
			case eToken.eBinary:
				result = CompileBinary(_tok, _sw);
				break;
			case eToken.eUnary:
				result = CompileUnary(_tok, _sw);
				break;
			case eToken.eFunction:
				result = CompileFunction(_tok, _sw);
				break;
			case eToken.eVariable:
			case eToken.eDot:
				result = CompileVariable(_tok, _sw, false, out _setFunc);
				break;
			}
			return result;
		}

		private static void CompileSimpleVariable(GMLToken _tok, StringWriter _sw, bool _lvalue, out bool _setFunc, bool _inhibitExpansion)
		{
			string text = null;
			string text2 = _tok.Text;
			GMLVariable value = null;
			if (!GMLCompile.ms_builtins.TryGetValue(text2, out value))
			{
				GMLCompile.ms_builtinsLocal.TryGetValue(text2, out value);
			}
			_setFunc = false;
			if (value != null)
			{
				if (_lvalue && value.setFunction != null)
				{
					text2 = string.Format("{0}( ", value.setFunction);
					_setFunc = true;
				}
				else if (!_lvalue && value.getFunction != null)
				{
					text2 = string.Format("{0}()", value.getFunction);
				}
			}
			else
			{
				text2 = ms_varPrefix + text2;
			}
			text = text2;
			if (!_inhibitExpansion && _tok.Id < 100000 && GMLCompile.ms_builtins.TryGetValue(_tok.Text, out value) && !ms_arguments.TryGetValue(_tok.Text, out text))
			{
				string empty = string.Empty;
				empty = ((!(text2 == "argument_count")) ? string.Format("g_pBuiltIn.{0}", text2) : string.Format("({0}.arguments.length-2)", ms_code.Name));
				_sw.Write(empty);
			}
			else if (!_inhibitExpansion && ms_globals.TryGetValue(_tok.Text, out text))
			{
				string value2 = string.Format("global.{0}", text2);
				_sw.Write(value2);
			}
			else if (_inhibitExpansion || ms_locals.TryGetValue(text2, out text) || ms_arguments.TryGetValue(_tok.Text, out text) || string.IsNullOrEmpty(ms_thisName.Peek()))
			{
				_sw.Write(text);
			}
			else if (!_inhibitExpansion && GMLCompile.Find(ms_assets.Objects, _tok.Text) >= 0)
			{
				_sw.Write(GMLCompile.Find(ms_assets.Objects, _tok.Text).ToString());
			}
			else
			{
				string value3 = string.Format("{0}.{1}", ms_thisName.Peek(), text2);
				_sw.Write(value3);
			}
			bool flag = false;
			string value4 = "]";
			string text3 = string.Empty;
			if (_tok.Children.Count > 0)
			{
				string text4 = _sw.GetStringBuilder().ToString();
				if (text4 == "g_pBuiltIn.instance_id")
				{
					_sw.GetStringBuilder().Length = 0;
					_sw.Write("g_pBuiltIn.get_instance_id(");
					value4 = ")";
					flag = true;
				}
				else if (text4 == "g_pBuiltIn.argument")
				{
					_sw.GetStringBuilder().Length = 0;
					_sw.Write("{0}.arguments[", ms_code.Name);
					text3 = "2+";
					flag = true;
				}
				else
				{
					int num = text4.LastIndexOf('.');
					string arg = (num < 0) ? "_inst" : text4.Substring(0, num);
					string arg2 = (num < 0) ? text4 : text4.Substring(num + 1, text4.Length - (num + 1));
					_sw.GetStringBuilder().Length = 0;
					_setFunc = _lvalue;
					if (_lvalue)
					{
						string value5 = string.Format("array_set_{0}D( {1}, \"__{2}__\" ", _tok.Children.Count, arg, arg2);
						_sw.Write(value5);
					}
					else
					{
						string value6 = string.Format("array_get_{0}D( {1}, \"__{2}__\" ", _tok.Children.Count, arg, arg2);
						_sw.Write(value6);
					}
				}
			}
			ms_thisName.Pop();
			if (_tok.Token != eToken.eVariable)
			{
				return;
			}
			int num2 = 0;
			foreach (GMLToken child in _tok.Children)
			{
				if (!flag || num2 > 0)
				{
					_sw.Write(", ");
				}
				if (!string.IsNullOrEmpty(text3))
				{
					_sw.Write("{0} (", text3);
				}
				CompileExpression(child, _sw);
				if (!string.IsNullOrEmpty(text3))
				{
					_sw.Write(")");
				}
				num2++;
			}
			if (_tok.Children.Count > 0)
			{
				if (flag)
				{
					_sw.Write(value4);
				}
				else if (_lvalue)
				{
					_sw.Write(",  ");
				}
				else
				{
					_sw.Write(" ) ");
				}
			}
		}

		private static eType CompileVariable(GMLToken _tok, TextWriter _sw, bool _lvalue, out bool _setFunc)
		{
			eType result = eType.eGMLT_Var;
			_setFunc = false;
			int count = ms_thisName.Count;
			ms_sbVariableName.Length = 0;
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(stringBuilder);
			switch (_tok.Token)
			{
			case eToken.eDot:
				switch (_tok.Children.Count)
				{
				case 0:
				case 1:
					Error("Expecting expression after the '.' ", _tok);
					break;
				default:
				{
					string text = null;
					switch (_tok.Children[0].Text)
					{
					case "global":
						text = _tok.Children[0].Text;
						stringWriter.Write(text);
						flag = true;
						break;
					case "self":
						stringWriter.Write(ms_thisName.Peek());
						flag = true;
						break;
					case "other":
						stringWriter.Write(ms_otherName.Peek());
						flag = true;
						break;
					default:
						switch (_tok.Children[0].Token)
						{
						case eToken.eConstant:
							CompileConstant(_tok.Children[0], stringWriter);
							break;
						case eToken.eVariable:
							ms_thisName.Push(ms_thisName.Peek());
							CompileSimpleVariable(_tok.Children[0], stringWriter, false, out _setFunc, false);
							break;
						case eToken.eFunction:
							CompileFunction(_tok.Children[0], stringWriter);
							break;
						case eToken.eDot:
							CompileVariable(_tok.Children[0], stringWriter, _lvalue, out _setFunc);
							break;
						default:
							Console.WriteLine("This is a problem now!");
							break;
						}
						break;
					}
					for (int i = 1; i < _tok.Children.Count; i++)
					{
						if (!flag)
						{
							string arg = stringBuilder.ToString();
							stringBuilder.Length = 0;
							stringWriter.Write("yyInst({0})", arg);
						}
						flag = false;
						stringWriter.Write(".");
						ms_thisName.Push("");
						CompileSimpleVariable(_tok.Children[i], stringWriter, i == _tok.Children.Count - 1 && _lvalue, out _setFunc, true);
					}
					break;
				}
				}
				break;
			case eToken.eVariable:
				ms_thisName.Push(ms_thisName.Peek());
				CompileSimpleVariable(_tok, stringWriter, _lvalue, out _setFunc, false);
				break;
			case eToken.eConstant:
				switch (_tok.Text)
				{
				default:
					Error("constant is invalid here", _tok);
					break;
				case "self":
					stringWriter.Write("{0}.id", ms_thisName.Peek());
					break;
				case "other":
					stringWriter.Write("{0}.id", ms_otherName.Peek());
					break;
				}
				break;
			}
			stringWriter.Close();
			_sw.Write(stringBuilder.ToString());
			if (count != ms_thisName.Count)
			{
				Error("Houston we have a problem!", _tok);
			}
			return result;
		}

		private static void CompileVar(GMLToken _tok, TextWriter _sw)
		{
			if (_tok.Children.Count > 0)
			{
				_sw.Write("var ");
				int num = 0;
				foreach (GMLToken child in _tok.Children)
				{
					if (num != 0)
					{
						_sw.Write(",");
					}
					string text = ms_varPrefix + child.Text;
					_sw.Write("{0}", text);
					string value;
					if (!ms_locals.TryGetValue(text, out value))
					{
						ms_locals.Add(text, text);
					}
					num++;
				}
			}
		}

		private static void CompileGlobalVar(GMLToken _tok, TextWriter _sw)
		{
			foreach (GMLToken child in _tok.Children)
			{
				ms_globals[child.Text] = child.Text;
			}
		}

		private static void CompileRepeat(GMLToken _tok, TextWriter _sw)
		{
			if (_tok.Children.Count != 2)
			{
				Error("malformed repeat statement", _tok);
			}
			string uniqueName = GetUniqueName();
			string uniqueName2 = GetUniqueName();
			_sw.Write("for( var {0}=0, {1}=", uniqueName, uniqueName2);
			CompileExpression(_tok.Children[0], _sw);
			_sw.Write("; {0}<{1}; {0}++) ", uniqueName, uniqueName2);
			if (_tok.Children[1].Token == eToken.eBegin)
			{
				ms_statements++;
			}
			ms_breakContext.Push("CompileRepeat");
			CompileStatement(_tok.Children[1], _sw);
			ms_breakContext.Pop();
		}

		private static void CompileIf(GMLToken _tok, TextWriter _sw)
		{
			if (_tok.Children.Count < 2)
			{
				Error("malformed if statement", _tok);
			}
			_sw.Write("if (");
			if (CompileExpression(_tok.Children[0], _sw) != 0)
			{
				_sw.Write(" > 0.5");
			}
			_sw.Write(") {");
			if (_tok.Children.Count > 1)
			{
				if (_tok.Children[1].Token == eToken.eBegin)
				{
					ms_statements++;
				}
				CompileStatement(_tok.Children[1], _sw);
				_sw.WriteLine(";}");
				if (_tok.Children.Count > 2)
				{
					_sw.Write(" else {");
					if (_tok.Children[2].Token == eToken.eBegin)
					{
						ms_statements++;
					}
					CompileStatement(_tok.Children[2], _sw);
					_sw.Write(";}");
				}
			}
			else
			{
				Error("if requires a then statement", _tok);
			}
		}

		private static void CompileWhile(GMLToken _tok, TextWriter _sw)
		{
			if (_tok.Children.Count != 2)
			{
				Error("malformed while statement", _tok);
			}
			_sw.Write("while (");
			CompileExpression(_tok.Children[0], _sw);
			_sw.Write(") ");
			ms_breakContext.Push("CompileWhile");
			if (_tok.Children[1].Token == eToken.eBegin)
			{
				ms_statements++;
			}
			CompileStatement(_tok.Children[1], _sw);
			ms_breakContext.Pop();
		}

		private static void CompileDo(GMLToken _tok, TextWriter _sw)
		{
			if (_tok.Children.Count != 2)
			{
				Error("malformed do statement", _tok);
			}
			_sw.WriteLine("do {");
			ms_breakContext.Push("CompileDo");
			CompileStatement(_tok.Children[0], _sw);
			ms_breakContext.Pop();
			_sw.Write("} while( !(");
			CompileExpression(_tok.Children[1], _sw);
			_sw.WriteLine("))");
		}

		private static void CompileFor(GMLToken _tok, TextWriter _sw)
		{
			if (_tok.Children.Count != 4)
			{
				Error("malformed for statement", _tok);
			}
			_sw.Write("for (");
			CompileStatement(_tok.Children[0], _sw);
			_sw.Write(" ; ");
			CompileExpression(_tok.Children[1], _sw);
			_sw.Write(" ; ");
			CompileStatement(_tok.Children[2], _sw);
			_sw.Write(") ");
			if (_tok.Children[3].Token == eToken.eBegin)
			{
				ms_statements++;
			}
			ms_breakContext.Push("CompileFor");
			CompileStatement(_tok.Children[3], _sw);
			ms_breakContext.Pop();
		}

		private static void CompileWith(GMLToken _tok, TextWriter _sw)
		{
			if (_tok.Children.Count != 2)
			{
				Error("malformed with statement", _tok);
			}
			if (_tok.Children[0].Token == eToken.eConstant && (_tok.Children[0].Text == "other" || _tok.Children[0].Text == "self"))
			{
				string item = string.Empty;
				switch (_tok.Children[0].Text)
				{
				case "other":
					item = ms_otherName.Peek();
					break;
				case "self":
					item = ms_thisName.Peek();
					break;
				}
				ms_otherName.Push(ms_thisName.Peek());
				ms_thisName.Push(item);
				CompileStatement(_tok.Children[1], _sw);
				if (_tok.Children[1].Token != eToken.eBegin)
				{
					_sw.Write(";");
				}
				ms_thisName.Pop();
				ms_otherName.Pop();
				return;
			}
			_sw.WriteLine("{");
			string uniqueName = GetUniqueName();
			string uniqueName2 = GetUniqueName();
			string uniqueName3 = GetUniqueName();
			_sw.Write("var {0} = GetWithArray(", uniqueName);
			CompileExpression(_tok.Children[0], _sw);
			_sw.WriteLine(" );");
			_sw.WriteLine("for( var {0} in {1} ) {{", uniqueName2, uniqueName);
			_sw.WriteLine(" var {0} = {1}[{2}];", uniqueName3, uniqueName, uniqueName2);
			ms_otherName.Push(ms_thisName.Peek());
			ms_thisName.Push(uniqueName3);
			ms_statements++;
			ms_breakContext.Push("CompileWith");
			CompileStatement(_tok.Children[1], _sw);
			ms_breakContext.Pop();
			if (_tok.Children[1].Token != eToken.eBegin)
			{
				_sw.Write(";");
			}
			_sw.WriteLine("}");
			ms_thisName.Pop();
			ms_otherName.Pop();
			_sw.WriteLine("}");
		}

		private static void CompileSwitch(GMLToken _tok, TextWriter _sw)
		{
			if (_tok.Children.Count == 0)
			{
				Error("malformed switch statement", _tok);
			}
			_sw.Write("switch(");
			CompileExpression(_tok.Children[0], _sw);
			_sw.WriteLine(") {");
			ms_breakContext.Push("CompileSwitch");
			bool flag = false;
			for (int i = 1; i < _tok.Children.Count; i++)
			{
				switch (_tok.Children[i].Token)
				{
				case eToken.eCase:
					_sw.Write("case ");
					CompileExpression(_tok.Children[i].Children[0], _sw);
					_sw.WriteLine(":");
					flag = true;
					break;
				case eToken.eDefault:
					_sw.WriteLine("default:");
					flag = true;
					break;
				case eToken.eBreak:
					if (!flag)
					{
						Error("statement in a switch MUST appear after case or default", _tok.Children[i]);
						break;
					}
					CompileStatement(_tok.Children[i], _sw);
					_sw.WriteLine(";");
					flag = false;
					break;
				default:
					if (!flag)
					{
						Error("statement in a switch MUST appear after case or default", _tok.Children[i]);
						break;
					}
					CompileStatement(_tok.Children[i], _sw);
					_sw.WriteLine(";");
					break;
				}
			}
			_sw.Write("}");
			ms_breakContext.Pop();
		}

		private static eType CompileFunction(GMLToken _tok, TextWriter _sw)
		{
			eType result = eType.eGMLT_Var;
			string text = _tok.Text;
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (_tok.Id < 100000)
			{
				if (_tok.Text == "script_execute")
				{
					_sw.Write("JSON_game.Scripts[ ");
					CompileExpression(_tok.Children[0], _sw);
					_sw.Write("]( ");
					flag = true;
					flag2 = true;
					flag3 = true;
				}
				else
				{
					int num2 = _tok.Id - GMLCompile.ms_id;
					int index = (num2 >> 8) & 0xF;
					int index2 = num2 >> 12;
					if (_tok.Id > GMLCompile.ms_id && ms_assets.Extensions[index].Includes[index2].Kind == 2)
					{
						_sw.Write("gml_Script_{0}( ", _tok.Text);
						flag = true;
						flag2 = true;
					}
					else
					{
						GMLFunction value = null;
						GMLCompile.ms_funcs.TryGetValue(text, out value);
						_sw.Write("{0}( ", text);
						flag = (value != null && value.InstanceFirstParam);
						flag2 = (value != null && value.OtherSecondParam);
					}
				}
			}
			else
			{
				_sw.Write("gml_Script_{0}( ", _tok.Text);
				flag = true;
				flag2 = true;
			}
			if (flag)
			{
				if (!string.IsNullOrEmpty(ms_thisName.Peek()))
				{
					_sw.Write("{0} ", ms_thisName.Peek());
					num++;
				}
				else
				{
					Error("calling a function that needs an instance and no instance is available", _tok);
				}
				if (flag2)
				{
					if (!string.IsNullOrEmpty(ms_otherName.Peek()))
					{
						_sw.Write(", {0} ", ms_otherName.Peek());
						num++;
					}
					else
					{
						Error("calling a function that needs an \"other\" and no \"other\" is available", _tok);
					}
				}
			}
			foreach (GMLToken child in _tok.Children)
			{
				if ((flag3 && num < 2) || !flag3)
				{
					if (num != 0)
					{
						_sw.Write(", ");
					}
					CompileExpression(child, _sw);
				}
				num++;
				flag3 = false;
			}
			_sw.Write(" )");
			return result;
		}

		private static void CompileAssign(GMLToken _tok, TextWriter _sw)
		{
			if (_tok.Children.Count != 3)
			{
				Error("malformed assignment statement", _tok);
			}
			bool _setFunc = false;
			CompileVariable(_tok.Children[0], _sw, true, out _setFunc);
			if (!_setFunc)
			{
				switch (_tok.Children[1].Token)
				{
				case eToken.eAssignPlus:
				case eToken.eAssignMinus:
				case eToken.eAssignTimes:
				case eToken.eAssignDivide:
				case eToken.eAssignOr:
				case eToken.eAssignAnd:
				case eToken.eAssignXor:
					_sw.Write(_tok.Children[1].Text);
					CompileExpression(_tok.Children[2], _sw);
					break;
				case eToken.eAssign:
					_sw.Write("=");
					CompileExpression(_tok.Children[2], _sw);
					break;
				}
				return;
			}
			switch (_tok.Children[1].Token)
			{
			case eToken.eAssignPlus:
			case eToken.eAssignMinus:
			case eToken.eAssignTimes:
			case eToken.eAssignDivide:
			case eToken.eAssignOr:
			case eToken.eAssignAnd:
			case eToken.eAssignXor:
				CompileVariable(_tok.Children[0], _sw, false, out _setFunc);
				_sw.Write(" {0} ", _tok.Children[1].Text[0]);
				CompileExpression(_tok.Children[2], _sw);
				break;
			case eToken.eAssign:
				CompileExpression(_tok.Children[2], _sw);
				break;
			}
			_sw.Write(" )");
		}

		private static void CompileReturn(GMLToken _tok, TextWriter _sw)
		{
			_sw.Write("return ");
			if (_tok.Children.Count > 0)
			{
				CompileExpression(_tok.Children[0], _sw);
			}
		}

		private static void CompileBreak(GMLToken _tok, TextWriter _sw)
		{
			if (ms_breakContext.Count > 0)
			{
				_sw.Write("break");
			}
			else
			{
				_sw.Write("return");
			}
		}

		private static void CompileExit(GMLToken _tok, TextWriter _sw)
		{
			_sw.Write("return");
		}

		private static void CompileContinue(GMLToken _tok, TextWriter _sw)
		{
			if (ms_breakContext.Count > 0)
			{
				_sw.Write("continue");
			}
			else
			{
				_sw.Write("return");
			}
		}

		private static void CompileStatement(GMLToken _tok, TextWriter _sw)
		{
			switch (_tok.Token)
			{
			case eToken.eVar:
				CompileVar(_tok, _sw);
				break;
			case eToken.eGlobalVar:
				CompileGlobalVar(_tok, _sw);
				break;
			case eToken.eBegin:
			case eToken.eBlock:
				CompileBlock(_tok, _sw);
				break;
			case eToken.eRepeat:
				CompileRepeat(_tok, _sw);
				break;
			case eToken.eIf:
				CompileIf(_tok, _sw);
				break;
			case eToken.eWhile:
				CompileWhile(_tok, _sw);
				break;
			case eToken.eDo:
				CompileDo(_tok, _sw);
				break;
			case eToken.eFor:
				CompileFor(_tok, _sw);
				break;
			case eToken.eWith:
				CompileWith(_tok, _sw);
				break;
			case eToken.eSwitch:
				CompileSwitch(_tok, _sw);
				break;
			case eToken.eFunction:
				CompileFunction(_tok, _sw);
				break;
			case eToken.eAssign:
				CompileAssign(_tok, _sw);
				break;
			case eToken.eReturn:
				CompileReturn(_tok, _sw);
				break;
			case eToken.eBreak:
				CompileBreak(_tok, _sw);
				break;
			case eToken.eExit:
				CompileExit(_tok, _sw);
				break;
			case eToken.eContinue:
				CompileContinue(_tok, _sw);
				break;
			}
		}

		private static void CheckEmitArrayCheck(TextWriter _sw)
		{
			IEnumerable<KeyValuePair<string, int>> enumerable = ms_arrays.Where((KeyValuePair<string, int> a) => a.Value == ms_statements);
			foreach (KeyValuePair<string, int> item in enumerable)
			{
				_sw.WriteLine("if (!{0}) {0} = [];", item.Key);
			}
		}

		private static void CompileStatement2(GMLToken _tok, TextWriter _sw)
		{
			CheckEmitArrayCheck(_sw);
			CompileStatement(_tok, _sw);
			ms_statements++;
		}

		private static void CompileBlock(GMLToken _tok, TextWriter _sw)
		{
			_sw.WriteLine("{");
			foreach (GMLToken child in _tok.Children)
			{
				CompileStatement2(child, _sw);
				_sw.WriteLine(";");
			}
			_sw.WriteLine("}");
		}

		private static void CompileProgram(GMLToken _tok, TextWriter _sw)
		{
			switch (_tok.Token)
			{
			case eToken.eEOF:
				break;
			case eToken.eBlock:
				CompileBlock(_tok, _sw);
				break;
			default:
				Error("No program to compile", _tok);
				break;
			}
		}

		private static void GatherArgs(GMLToken _tok)
		{
			eToken token = _tok.Token;
			if (token == eToken.eVariable)
			{
				switch (_tok.Text)
				{
				case "argument0":
				case "argument1":
				case "argument2":
				case "argument3":
				case "argument4":
				case "argument5":
				case "argument6":
				case "argument7":
				case "argument8":
				case "argument9":
				case "argument10":
				case "argument11":
				case "argument12":
				case "argument13":
				case "argument14":
				case "argument15":
				{
					string value;
					if (!ms_arguments.TryGetValue(_tok.Text, out value))
					{
						ms_arguments.Add(_tok.Text, _tok.Text);
					}
					break;
				}
				}
			}
			foreach (GMLToken child in _tok.Children)
			{
				GatherArgs(child);
			}
		}

		public static void Compile(GMAssets _assets, GMLCode _code, TextWriter _sw)
		{
			ms_assets = _assets;
			ms_code = _code;
			ms_numErrors = 0;
			ms_error = false;
			Console.WriteLine("Compiling - {0}", _code.Name);
			_sw.WriteLine("\n// #####################################################################################################");
			string[] array = _code.Code.Split('\n');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Replace('\r', ' ');
				text2 = text2.Replace('\n', ' ');
				_sw.WriteLine("// {0}", text2);
			}
			ms_arguments.Clear();
			switch (_code.Type)
			{
			case eGMLCodeType.eRoomCreate:
				_sw.WriteLine("function {0}(_inst)", _code.Name);
				ms_thisName.Push("_inst");
				ms_otherName.Push("_inst");
				break;
			case eGMLCodeType.eScript:
			{
				GatherArgs(_code.Token);
				_sw.Write("function {0}( _inst, _other ", _code.Name);
				for (int j = 0; j < ms_arguments.Count; j++)
				{
					string text3 = string.Format("argument{0}", j);
					string value = string.Empty;
					if (!ms_arguments.TryGetValue(text3, out value))
					{
						Error(string.Format("argument naming error, {0} arguments but no reference found to {1}", ms_arguments.Count, text3), _code.Token);
					}
					_sw.Write(", {0}", text3);
				}
				_sw.Write(")");
				ms_thisName.Push("_inst");
				ms_otherName.Push("_other");
				break;
			}
			case eGMLCodeType.eRoomInstanceCreate:
				_sw.WriteLine("function {0}( _inst )", _code.Name);
				ms_thisName.Push("_inst");
				ms_otherName.Push("_inst");
				break;
			case eGMLCodeType.eEvent:
			case eGMLCodeType.eTrigger:
				_sw.WriteLine("function {0}( _inst, _other )", _code.Name);
				ms_thisName.Push("_inst");
				ms_otherName.Push("_other");
				break;
			case eGMLCodeType.eConstant:
				_sw.WriteLine("function {0}()", _code.Name);
				ms_thisName.Push("");
				ms_otherName.Push("");
				break;
			}
			DummyStream stream = new DummyStream();
			StreamWriter sw = new StreamWriter(stream);
			int num = ms_unique;
			ms_locals.Clear();
			ms_arrays.Clear();
			ms_breakContext.Clear();
			ms_statements = 0;
			CompileProgram(_code.Token, sw);
			if (!ms_error)
			{
				ms_unique = num;
				ms_locals.Clear();
				ms_breakContext.Clear();
				ms_statements = 0;
				CompileProgram(_code.Token, _sw);
			}
			ms_thisName.Pop();
			ms_otherName.Pop();
		}
	}
}
