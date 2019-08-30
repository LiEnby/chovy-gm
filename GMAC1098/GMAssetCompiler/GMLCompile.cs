using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace GMAssetCompiler
{
	internal class GMLCompile
	{
		public const int OBJECT_SELF = -1;

		public const int OBJECT_OTHER = -2;

		public const int OBJECT_ALL = -3;

		public const int OBJECT_NOONE = -4;

		public const int OBJECT_GLOBAL = -5;

		public const int OBJECT_LOCAL = -7;

		public const int OBJECT_NOTSPECIFIED = -6;

		private const int clBlack = 0;

		private const int clMaroon = 128;

		private const int clGreen = 32768;

		private const int clOlive = 32896;

		private const int clNavy = 8388608;

		private const int clPurple = 8388736;

		private const int clTeal = 8421376;

		private const int clGray = 8421504;

		private const int clSilver = 12632256;

		private const int clRed = 255;

		private const int clLime = 65280;

		private const int clYellow = 65535;

		private const int clBlue = 16711680;

		private const int clFuchsia = 16711935;

		private const int clAqua = 16776960;

		private const int clLtGray = 12632256;

		private const int clDkGray = 8421504;

		private const int clWhite = 16777215;

		private const int clMoneyGreen = 12639424;

		private const int clSkyBlue = 15780518;

		private const int clCream = 15793151;

		private const int clMedGray = 10789024;

		private const int EVENT_CREATE = 0;

		private const int EVENT_DESTROY = 1;

		private const int EVENT_ALARM = 2;

		private const int EVENT_STEP = 3;

		private const int EVENT_COLLISION = 4;

		private const int EVENT_KEYBOARD = 5;

		private const int EVENT_MOUSE = 6;

		private const int EVENT_OTHER = 7;

		private const int EVENT_DRAW = 8;

		private const int EVENT_KEYPRESS = 9;

		private const int EVENT_KEYRELEASE = 10;

		private const int EVENT_TRIGGER = 11;

		private const int VK_RETURN = 13;

		private const int VK_SHIFT = 16;

		private const int VK_CONTROL = 17;

		private const int VK_MENU = 18;

		private const int VK_ESCAPE = 27;

		private const int VK_SPACE = 32;

		private const int VK_BACK = 8;

		private const int VK_TAB = 9;

		private const int VK_PAUSE = 19;

		private const int VK_SNAPSHOT = 44;

		private const int VK_LEFT = 37;

		private const int VK_RIGHT = 39;

		private const int VK_UP = 38;

		private const int VK_DOWN = 40;

		private const int VK_HOME = 36;

		private const int VK_END = 35;

		private const int VK_DELETE = 46;

		private const int VK_INSERT = 45;

		private const int VK_PRIOR = 33;

		private const int VK_NEXT = 34;

		private const int VK_F1 = 112;

		private const int VK_F2 = 113;

		private const int VK_F3 = 114;

		private const int VK_F4 = 115;

		private const int VK_F5 = 116;

		private const int VK_F6 = 117;

		private const int VK_F7 = 118;

		private const int VK_F8 = 119;

		private const int VK_F9 = 120;

		private const int VK_F10 = 121;

		private const int VK_F11 = 128;

		private const int VK_F12 = 129;

		private const int VK_NUMPAD0 = 96;

		private const int VK_NUMPAD1 = 97;

		private const int VK_NUMPAD2 = 98;

		private const int VK_NUMPAD3 = 99;

		private const int VK_NUMPAD4 = 100;

		private const int VK_NUMPAD5 = 101;

		private const int VK_NUMPAD6 = 102;

		private const int VK_NUMPAD7 = 103;

		private const int VK_NUMPAD8 = 104;

		private const int VK_NUMPAD9 = 105;

		private const int VK_DIVIDE = 111;

		private const int VK_MULTIPLY = 106;

		private const int VK_SUBTRACT = 109;

		private const int VK_ADD = 107;

		private const int VK_DECIMAL = 110;

		private const int VK_LSHIFT = 160;

		private const int VK_LCONTROL = 162;

		private const int VK_LMENU = 164;

		private const int VK_RSHIFT = 161;

		private const int VK_RCONTROL = 163;

		private const int VK_RMENU = 165;

		public static Dictionary<string, double> ms_constants = new Dictionary<string, double>();

		public static Dictionary<string, int> ms_ConstantCount = new Dictionary<string, int>();

		public static Dictionary<string, GMLFunction> ms_funcs = new Dictionary<string, GMLFunction>();

		public static Dictionary<string, GMLVariable> ms_builtins = new Dictionary<string, GMLVariable>();

		public static Dictionary<string, GMLVariable> ms_builtinsArray = new Dictionary<string, GMLVariable>();

		public static Dictionary<string, GMLVariable> ms_builtinsLocal = new Dictionary<string, GMLVariable>();

		public static Dictionary<string, GMLVariable> ms_builtinsLocalArray = new Dictionary<string, GMLVariable>();

		private static Dictionary<string, GMLVariable> ms_vars = new Dictionary<string, GMLVariable>();

		public static int ms_id = 0;

		private static GMAssets ms_assets = null;

		private static string ms_script = string.Empty;

		private static List<GMLError> ms_errors = null;

		private static bool ms_error = false;

		private static int ms_numErrors = 0;

		private static string ms_scriptName = "";

		private static void SkipWhitespace(string _script, ref int _index)
		{
			bool flag = false;
			while (!flag)
			{
				while (_index < _script.Length && char.IsWhiteSpace(_script[_index]))
				{
					_index++;
				}
				if (_index < _script.Length && _script[_index] == '/')
				{
					if (_index + 1 < _script.Length)
					{
						switch (_script[_index + 1])
						{
						case '*':
							_index += 2;
							while (_index < _script.Length && (_script[_index] != '*' || _script[_index + 1] != '/'))
							{
								_index++;
							}
							if (_index >= _script.Length)
							{
								ms_errors.Add(new GMLError(eErrorKind.Warning_Unclosed_Comment, "unclosed comment (/*) at tend of script", null, _index));
							}
							else
							{
								_index += 2;
							}
							break;
						case '/':
							_index += 2;
							while (_index < _script.Length && _script[_index] != '\n' && _script[_index] != '\r')
							{
								_index++;
							}
							_index++;
							break;
						default:
							flag = true;
							break;
						}
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
			}
		}

		private static GMLToken NextName(string _script, ref int _index)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int index = _index;
			stringBuilder.Append(_script[_index]);
			_index++;
			while (_index < _script.Length && (char.IsLetterOrDigit(_script[_index]) || _script[_index] == '_'))
			{
				stringBuilder.Append(_script[_index]);
				_index++;
			}
			string text = stringBuilder.ToString();
			switch (text)
			{
			case "var":
				return new GMLToken(eToken.eVar, index, text);
			case "if":
				return new GMLToken(eToken.eIf, index, text);
			case "end":
				return new GMLToken(eToken.eEnd, index, text);
			case "else":
				return new GMLToken(eToken.eElse, index, text);
			case "while":
				return new GMLToken(eToken.eWhile, index, text);
			case "do":
				return new GMLToken(eToken.eDo, index, text);
			case "for":
				return new GMLToken(eToken.eFor, index, text);
			case "begin":
				return new GMLToken(eToken.eBegin, index, text);
			case "then":
				return new GMLToken(eToken.eThen, index, text);
			case "with":
				return new GMLToken(eToken.eWith, index, text);
			case "until":
				return new GMLToken(eToken.eUntil, index, text);
			case "repeat":
				return new GMLToken(eToken.eRepeat, index, text);
			case "exit":
				return new GMLToken(eToken.eExit, index, text);
			case "return":
				return new GMLToken(eToken.eReturn, index, text);
			case "break":
				return new GMLToken(eToken.eBreak, index, text);
			case "continue":
				return new GMLToken(eToken.eContinue, index, text);
			case "switch":
				return new GMLToken(eToken.eSwitch, index, text);
			case "case":
				return new GMLToken(eToken.eCase, index, text);
			case "default":
				return new GMLToken(eToken.eDefault, index, text);
			case "and":
				return new GMLToken(eToken.eAnd, index, text);
			case "or":
				return new GMLToken(eToken.eOr, index, text);
			case "not":
				return new GMLToken(eToken.eNot, index, text);
			case "div":
				return new GMLToken(eToken.eDiv, index, text);
			case "mod":
				return new GMLToken(eToken.eMod, index, text);
			case "xor":
				return new GMLToken(eToken.eBitXor, index, text);
			case "globalvar":
				return new GMLToken(eToken.eGlobalVar, index, text);
			default:
				return new GMLToken(eToken.eName, index, text);
			}
		}

		private static GMLToken NextValue(string _script, ref int _index)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int index = _index;
			stringBuilder.Append(_script[_index]);
			_index++;
			while (_index < _script.Length && (char.IsDigit(_script[_index]) || _script[_index] == '.'))
			{
				stringBuilder.Append(_script[_index]);
				_index++;
			}
			return new GMLToken(eToken.eNumber, index, stringBuilder.ToString());
		}

		private static GMLToken NextHex(string _script, ref int _index)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int index = _index;
			stringBuilder.Append(_script[_index]);
			_index++;
			while (_index < _script.Length && (char.IsDigit(_script[_index]) || (char.ToLower(_script[_index]) >= 'a' && char.ToLower(_script[_index]) <= 'f')))
			{
				stringBuilder.Append(_script[_index]);
				_index++;
			}
			return new GMLToken(eToken.eNumber, index, stringBuilder.ToString());
		}

		private static GMLToken NextString(string _script, ref int _index)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int index = _index;
			char c = _script[_index];
			_index++;
			while (_index < _script.Length && c != _script[_index])
			{
				stringBuilder.Append(_script[_index]);
				_index++;
			}
			if (_index < _script.Length)
			{
				_index++;
			}
			return new GMLToken(eToken.eString, index, stringBuilder.ToString());
		}

		private static GMLToken NextToken(string _script, ref int _index)
		{
			SkipWhitespace(_script, ref _index);
			if (_index >= _script.Length)
			{
				return new GMLToken(eToken.eEOF, _index, string.Empty);
			}
			char c = _script[_index];
			if (char.IsLetter(c) || c == '_')
			{
				return NextName(_script, ref _index);
			}
			if (char.IsDigit(c))
			{
				return NextValue(_script, ref _index);
			}
			switch (c)
			{
			case '$':
				return NextHex(_script, ref _index);
			case '"':
			case '\'':
				return NextString(_script, ref _index);
			default:
				if (_index + 1 < _script.Length && _script[_index] == '.' && char.IsDigit(_script[_index + 1]))
				{
					return NextValue(_script, ref _index);
				}
				switch (c)
				{
				case '{':
					return new GMLToken(eToken.eBegin, _index++, "{");
				case '}':
					return new GMLToken(eToken.eEnd, _index++, "}");
				case '(':
					return new GMLToken(eToken.eOpen, _index++, "(");
				case ')':
					return new GMLToken(eToken.eClose, _index++, ")");
				case '[':
					return new GMLToken(eToken.eArrayOpen, _index++, "[");
				case ']':
					return new GMLToken(eToken.eArrayClose, _index++, "]");
				case ';':
					return new GMLToken(eToken.eSepStatement, _index++, ";");
				case ',':
					return new GMLToken(eToken.eSepArgument, _index++, ",");
				case '.':
					return new GMLToken(eToken.eDot, _index++, ".");
				case '~':
					return new GMLToken(eToken.eBitNegate, _index++, "~");
				case '!':
					_index++;
					if (_index >= _script.Length || _script[_index] != '=')
					{
						return new GMLToken(eToken.eNot, _index - 1, "!");
					}
					return new GMLToken(eToken.eNotEqual, _index++ - 2, "!=");
				case '=':
					_index++;
					if (_index >= _script.Length || _script[_index] != '=')
					{
						return new GMLToken(eToken.eAssign, _index - 1, "=");
					}
					return new GMLToken(eToken.eEqual, _index++ - 2, "==");
				case ':':
					_index++;
					if (_index >= _script.Length || _script[_index] != '=')
					{
						return new GMLToken(eToken.eLabel, _index - 1, ":");
					}
					return new GMLToken(eToken.eAssign, _index++ - 2, ":=");
				case '+':
					_index++;
					if (_index >= _script.Length || _script[_index] != '=')
					{
						return new GMLToken(eToken.ePlus, _index - 1, "+");
					}
					return new GMLToken(eToken.eAssignPlus, _index++ - 2, "+=");
				case '-':
					_index++;
					if (_index >= _script.Length || _script[_index] != '=')
					{
						return new GMLToken(eToken.eMinus, _index - 1, "-");
					}
					return new GMLToken(eToken.eAssignMinus, _index++ - 2, "-=");
				case '*':
					_index++;
					if (_index >= _script.Length || _script[_index] != '=')
					{
						return new GMLToken(eToken.eTime, _index - 1, "*");
					}
					return new GMLToken(eToken.eAssignTimes, _index++ - 2, "*=");
				case '/':
					_index++;
					if (_index >= _script.Length || _script[_index] != '=')
					{
						return new GMLToken(eToken.eDivide, _index - 1, "/");
					}
					return new GMLToken(eToken.eAssignDivide, _index++ - 2, "/=");
				case '<':
					if (_index + 1 < _script.Length)
					{
						switch (_script[_index + 1])
						{
						case '>':
							_index += 2;
							return new GMLToken(eToken.eNotEqual, _index, "<>");
						case '<':
							_index += 2;
							return new GMLToken(eToken.eBitShiftLeft, _index, "<<");
						case '=':
							_index += 2;
							return new GMLToken(eToken.eLessEqual, _index, "<=");
						default:
							return new GMLToken(eToken.eLess, _index++, "<");
						}
					}
					return new GMLToken(eToken.eLess, _index++, "<");
				case '>':
					if (_index + 1 < _script.Length)
					{
						switch (_script[_index + 1])
						{
						case '>':
							_index += 2;
							return new GMLToken(eToken.eBitShiftRight, _index, ">>");
						case '=':
							_index += 2;
							return new GMLToken(eToken.eGreaterEqual, _index, ">=");
						default:
							return new GMLToken(eToken.eGreater, _index++, ">");
						}
					}
					return new GMLToken(eToken.eGreater, _index++, ">");
				case '|':
					if (_index + 1 < _script.Length)
					{
						switch (_script[_index + 1])
						{
						case '|':
							_index += 2;
							return new GMLToken(eToken.eOr, _index, "||");
						case '=':
							_index += 2;
							return new GMLToken(eToken.eAssignOr, _index, "|=");
						default:
							return new GMLToken(eToken.eBitOr, _index++, "|");
						}
					}
					return new GMLToken(eToken.eBitOr, _index++, "|");
				case '&':
					if (_index + 1 < _script.Length)
					{
						switch (_script[_index + 1])
						{
						case '&':
							_index += 2;
							return new GMLToken(eToken.eAnd, _index, "&&");
						case '=':
							_index += 2;
							return new GMLToken(eToken.eAssignAnd, _index, "&=");
						default:
							return new GMLToken(eToken.eBitAnd, _index++, "&");
						}
					}
					return new GMLToken(eToken.eBitAnd, _index++, "&");
				case '^':
					if (_index + 1 < _script.Length)
					{
						switch (_script[_index + 1])
						{
						case '^':
							_index += 2;
							return new GMLToken(eToken.eXor, _index, "^^");
						case '=':
							_index += 2;
							return new GMLToken(eToken.eAssignXor, _index, "^=");
						default:
							return new GMLToken(eToken.eBitXor, _index++, "^");
						}
					}
					return new GMLToken(eToken.eBitXor, _index++, "^");
				default:
					return new GMLToken(eToken.eError, _index, string.Empty);
				}
			}
		}

		public static void Check(string _check, eToken _token)
		{
			int _index = 0;
			GMLToken gMLToken = NextToken(_check, ref _index);
			if (gMLToken.Token != _token && _index == _check.Length)
			{
				Console.WriteLine("Failed Token Check : got {0:G} should be {1:G} for {2}", gMLToken.Token, _token, _check);
			}
		}

		public static void Test()
		{
			Check("012345", eToken.eNumber);
			Check("var", eToken.eVar);
			Check("if", eToken.eIf);
			Check("end", eToken.eEnd);
			Check("else", eToken.eElse);
			Check("while", eToken.eWhile);
			Check("do", eToken.eDo);
			Check("for", eToken.eFor);
			Check("begin", eToken.eBegin);
			Check("then", eToken.eThen);
			Check("with", eToken.eWith);
			Check("until", eToken.eUntil);
			Check("repeat", eToken.eRepeat);
			Check("exit", eToken.eExit);
			Check("return", eToken.eReturn);
			Check("break", eToken.eBreak);
			Check("continue", eToken.eContinue);
			Check("switch", eToken.eSwitch);
			Check("case", eToken.eCase);
			Check("default", eToken.eDefault);
			Check("and", eToken.eAnd);
			Check("or", eToken.eOr);
			Check("div", eToken.eDiv);
			Check("mod", eToken.eMod);
			Check("xor", eToken.eBitXor);
			Check("globalvar", eToken.eGlobalVar);
			Check("$0123456", eToken.eNumber);
			Check(".0123456", eToken.eNumber);
			Check("0123.456", eToken.eNumber);
			Check("\"0123.456\"", eToken.eString);
			Check("'0123.456'", eToken.eString);
			Check("blahblah", eToken.eName);
			Check("{", eToken.eBegin);
			Check("}", eToken.eEnd);
			Check("(", eToken.eOpen);
			Check(")", eToken.eClose);
			Check("[", eToken.eArrayOpen);
			Check("]", eToken.eArrayClose);
			Check(";", eToken.eSepStatement);
			Check(",", eToken.eSepArgument);
			Check(".", eToken.eDot);
			Check("~", eToken.eBitNegate);
			Check("!=", eToken.eNotEqual);
			Check("!", eToken.eNot);
			Check("==", eToken.eEqual);
			Check("=", eToken.eAssign);
			Check(":=", eToken.eAssign);
			Check(":", eToken.eLabel);
			Check("+=", eToken.eAssignPlus);
			Check("+", eToken.ePlus);
			Check("-=", eToken.eAssignMinus);
			Check("-", eToken.eMinus);
			Check("*=", eToken.eAssignTimes);
			Check("*", eToken.eTime);
			Check("/=", eToken.eAssignDivide);
			Check("/", eToken.eDivide);
			Check("<<", eToken.eBitShiftLeft);
			Check("<=", eToken.eLessEqual);
			Check("<", eToken.eLess);
			Check(">>", eToken.eBitShiftRight);
			Check(">=", eToken.eGreaterEqual);
			Check(">", eToken.eGreater);
			Check("||", eToken.eOr);
			Check("|=", eToken.eAssignOr);
			Check("&&", eToken.eAnd);
			Check("&=", eToken.eAssignAnd);
			Check("&", eToken.eBitAnd);
			Check("^^", eToken.eXor);
			Check("^=", eToken.eAssignXor);
			Check("^", eToken.eBitXor);
		}

		public static void Error(string _errorMessage, string _script, GMLToken _token)
		{
			if (!Program.InhibitErrorOutput)
			{
				int num = 1;
				for (int i = 0; i < _token.Index; i++)
				{
					if (_script[i] == '\n')
					{
						num++;
					}
				}
				Console.WriteLine("Error : {0}({1}) : {2}", ms_scriptName, num, _errorMessage);
			}
			ms_numErrors++;
			ms_error = true;
			Program.ExitCode = 1;
		}

		private static void Function_Add(string _string, int _numArgs7, int _numArgs8, bool _Pro)
		{
			GMLFunction gMLFunction = new GMLFunction();
			gMLFunction.Name = _string;
			gMLFunction.Id = ms_id++;
			gMLFunction.NumArgs7 = _numArgs7;
			gMLFunction.NumArgs8 = _numArgs8;
			gMLFunction.Pro = _Pro;
			gMLFunction.InstanceFirstParam = false;
			gMLFunction.OtherSecondParam = false;
			try
			{
				ms_funcs.Add(_string, gMLFunction);
			}
			catch (Exception)
			{
				MessageBox.Show(_string);
			}
		}

		private static void Function_Add(string _string, int _numArgs7, int _numArgs8, bool _Pro, bool _InstanceFirstParam)
		{
			GMLFunction gMLFunction = new GMLFunction();
			gMLFunction.Name = _string;
			gMLFunction.Id = ms_id++;
			gMLFunction.NumArgs7 = _numArgs7;
			gMLFunction.NumArgs8 = _numArgs8;
			gMLFunction.Pro = _Pro;
			gMLFunction.InstanceFirstParam = _InstanceFirstParam;
			gMLFunction.OtherSecondParam = false;
			ms_funcs.Add(_string, gMLFunction);
		}

		private static void Function_Add(string _string, int _numArgs7, int _numArgs8, bool _Pro, bool _InstanceFirstParam, bool _OtherSecondParam)
		{
			GMLFunction gMLFunction = new GMLFunction();
			gMLFunction.Name = _string;
			gMLFunction.Id = ms_id++;
			gMLFunction.NumArgs7 = _numArgs7;
			gMLFunction.NumArgs8 = _numArgs8;
			gMLFunction.Pro = _Pro;
			gMLFunction.InstanceFirstParam = _InstanceFirstParam;
			gMLFunction.OtherSecondParam = _OtherSecondParam;
			ms_funcs.Add(_string, gMLFunction);
		}

		private static void AddRealConstant(string _name, double _value)
		{
			ms_constants.Add(_name, _value);
		}

		private static void Variable_BuiltIn_Add(string _name, bool _get, bool _set, bool _pro, string _setFunc, string _getFunc)
		{
			GMLVariable gMLVariable = new GMLVariable();
			gMLVariable.Name = _name;
			gMLVariable.Id = ms_id++;
			gMLVariable.Get = _get;
			gMLVariable.Set = _set;
			gMLVariable.Pro = _pro;
			gMLVariable.setFunction = _setFunc;
			gMLVariable.getFunction = _getFunc;
			ms_builtins.Add(_name, gMLVariable);
		}

		private static void Variable_BuiltIn_Array_Add(string _name, bool _get, bool _set, bool _pro)
		{
			GMLVariable gMLVariable = new GMLVariable();
			gMLVariable.Name = _name;
			gMLVariable.Id = ms_id++;
			gMLVariable.Get = _get;
			gMLVariable.Set = _set;
			gMLVariable.Pro = _pro;
			gMLVariable.setFunction = null;
			gMLVariable.getFunction = null;
			ms_builtins.Add(_name, gMLVariable);
			ms_builtinsArray.Add(_name, gMLVariable);
		}

		private static void Variable_BuiltIn_Local_Add(string _name, bool _get, bool _set, bool _pro, string _setFunc, string _getFunc)
		{
			GMLVariable gMLVariable = new GMLVariable();
			gMLVariable.Name = _name;
			gMLVariable.Id = ms_id++;
			gMLVariable.Get = _get;
			gMLVariable.Set = _set;
			gMLVariable.Pro = _pro;
			gMLVariable.setFunction = _setFunc;
			gMLVariable.getFunction = _getFunc;
			ms_builtinsLocal.Add(_name, gMLVariable);
		}

		private static void Variable_BuiltIn_Local_Array_Add(string _name, bool _get, bool _set, bool _pro)
		{
			GMLVariable gMLVariable = new GMLVariable();
			gMLVariable.Name = _name;
			gMLVariable.Id = ms_id++;
			gMLVariable.Get = _get;
			gMLVariable.Set = _set;
			gMLVariable.Pro = _pro;
			gMLVariable.setFunction = null;
			gMLVariable.getFunction = null;
			ms_builtinsLocal.Add(_name, gMLVariable);
			ms_builtinsLocalArray.Add(_name, gMLVariable);
		}

		public static void Code_Init()
		{
			Function_Add("d3d_start", 0, 0, true);
			Function_Add("d3d_end", 0, 0, true);
			Function_Add("d3d_set_perspective", 1, 1, true);
			Function_Add("d3d_set_hidden", 1, 1, true);
			Function_Add("d3d_set_depth", 1, 1, true);
			Function_Add("d3d_set_lighting", 1, 1, true);
			Function_Add("d3d_set_shading", 1, 1, true);
			Function_Add("d3d_set_fog", 4, 4, true);
			Function_Add("d3d_set_culling", 1, 1, true);
			Function_Add("d3d_primitive_begin", 1, 1, true);
			Function_Add("d3d_primitive_begin_texture", 2, 2, true);
			Function_Add("d3d_primitive_end", 0, 0, true);
			Function_Add("d3d_vertex", 3, 3, true);
			Function_Add("d3d_vertex_color", 5, 5, true);
			Function_Add("d3d_vertex_texture", 5, 5, true);
			Function_Add("d3d_vertex_texture_color", 7, 7, true);
			Function_Add("d3d_vertex_normal", 6, 6, true);
			Function_Add("d3d_vertex_normal_color", 8, 8, true);
			Function_Add("d3d_vertex_normal_texture", 8, 8, true);
			Function_Add("d3d_vertex_normal_texture_color", 10, 10, true);
			Function_Add("d3d_draw_block", 9, 9, true);
			Function_Add("d3d_draw_cylinder", 11, 11, true);
			Function_Add("d3d_draw_cone", 11, 11, true);
			Function_Add("d3d_draw_ellipsoid", 10, 10, true);
			Function_Add("d3d_draw_wall", 9, 9, true);
			Function_Add("d3d_draw_floor", 9, 9, true);
			Function_Add("d3d_set_projection", 9, 9, true);
			Function_Add("d3d_set_projection_ext", 13, 13, true);
			Function_Add("d3d_set_projection_ortho", 5, 5, true);
			Function_Add("d3d_set_projection_perspective", 5, 5, true);
			Function_Add("d3d_transform_set_identity", 0, 0, true);
			Function_Add("d3d_transform_set_translation", 3, 3, true);
			Function_Add("d3d_transform_set_scaling", 3, 3, true);
			Function_Add("d3d_transform_set_rotation_x", 1, 1, true);
			Function_Add("d3d_transform_set_rotation_y", 1, 1, true);
			Function_Add("d3d_transform_set_rotation_z", 1, 1, true);
			Function_Add("d3d_transform_set_rotation_axis", 4, 4, true);
			Function_Add("d3d_transform_add_translation", 3, 3, true);
			Function_Add("d3d_transform_add_scaling", 3, 3, true);
			Function_Add("d3d_transform_add_rotation_x", 1, 1, true);
			Function_Add("d3d_transform_add_rotation_y", 1, 1, true);
			Function_Add("d3d_transform_add_rotation_z", 1, 1, true);
			Function_Add("d3d_transform_add_rotation_axis", 4, 4, true);
			Function_Add("d3d_transform_stack_clear", 0, 0, true);
			Function_Add("d3d_transform_stack_empty", 0, 0, true);
			Function_Add("d3d_transform_stack_push", 0, 0, true);
			Function_Add("d3d_transform_stack_pop", 0, 0, true);
			Function_Add("d3d_transform_stack_top", 0, 0, true);
			Function_Add("d3d_transform_stack_discard", 0, 0, true);
			Function_Add("d3d_light_define_direction", 5, 5, true);
			Function_Add("d3d_light_define_point", 6, 6, true);
			Function_Add("d3d_light_enable", 2, 2, true);
			Function_Add("d3d_model_create", 0, 0, true);
			Function_Add("d3d_model_destroy", 1, 1, true);
			Function_Add("d3d_model_clear", 1, 1, true);
			Function_Add("d3d_model_load", 2, 2, true);
			Function_Add("d3d_model_save", 2, 2, true);
			Function_Add("d3d_model_draw", 5, 5, true);
			Function_Add("d3d_model_primitive_begin", 2, 2, true);
			Function_Add("d3d_model_primitive_end", 1, 1, true);
			Function_Add("d3d_model_vertex", 4, 4, true);
			Function_Add("d3d_model_vertex_color", 6, 6, true);
			Function_Add("d3d_model_vertex_texture", 6, 6, true);
			Function_Add("d3d_model_vertex_texture_color", 8, 8, true);
			Function_Add("d3d_model_vertex_normal", 7, 7, true);
			Function_Add("d3d_model_vertex_normal_color", 9, 9, true);
			Function_Add("d3d_model_vertex_normal_texture", 9, 9, true);
			Function_Add("d3d_model_vertex_normal_texture_color", 11, 11, true);
			Function_Add("d3d_model_block", 9, 9, true);
			Function_Add("d3d_model_cylinder", 11, 11, true);
			Function_Add("d3d_model_cone", 11, 11, true);
			Function_Add("d3d_model_ellipsoid", 10, 10, true);
			Function_Add("d3d_model_wall", 9, 9, true);
			Function_Add("d3d_model_floor", 9, 9, true);
			Function_Add("action_path_old", 3, 3, false);
			Function_Add("action_set_sprite", 2, 2, false, true);
			Function_Add("action_draw_font", 1, 1, false);
			Function_Add("action_draw_font_old", 6, 6, false);
			Function_Add("action_fill_color", 1, 1, false);
			Function_Add("action_line_color", 1, 1, false);
			Function_Add("action_highscore", 0, 0, false);
			Function_Add("action_set_relative", 1, 1, false);
			Function_Add("action_move", 2, 2, false, true);
			Function_Add("action_set_motion", 2, 2, false, true);
			Function_Add("action_set_hspeed", 1, 1, false, true);
			Function_Add("action_set_vspeed", 1, 1, false, true);
			Function_Add("action_set_gravity", 2, 2, false, true);
			Function_Add("action_set_friction", 1, 1, false, true);
			Function_Add("action_move_point", 3, 3, false, true);
			Function_Add("action_move_to", 2, 2, false, true);
			Function_Add("action_move_start", 0, 0, false, true);
			Function_Add("action_move_random", 2, 2, false, true);
			Function_Add("action_snap", 2, 2, false, true);
			Function_Add("action_wrap", 1, 1, false, true);
			Function_Add("action_reverse_xdir", 0, 0, false, true);
			Function_Add("action_reverse_ydir", 0, 0, false, true);
			Function_Add("action_move_contact", 3, 3, false, true);
			Function_Add("action_bounce", 2, 2, false, true);
			Function_Add("action_path", 4, 4, false, true);
			Function_Add("action_path_end", 0, 0, false, true);
			Function_Add("action_path_position", 1, 1, false, true);
			Function_Add("action_path_speed", 1, 1, false, true);
			Function_Add("action_linear_step", 4, 4, false, true);
			Function_Add("action_potential_step", 4, 4, false, true);
			Function_Add("action_kill_object", 0, 0, false, true);
			Function_Add("action_create_object", 3, 3, false, true);
			Function_Add("action_create_object_motion", 5, 5, false, true);
			Function_Add("action_create_object_random", 6, 6, false, true);
			Function_Add("action_change_object", 2, 2, false, true);
			Function_Add("action_kill_position", 2, 2, false, true);
			Function_Add("action_sprite_set", 3, 3, false, true);
			Function_Add("action_sprite_transform", 4, 4, true, true);
			Function_Add("action_sprite_color", 2, 2, true, true);
			Function_Add("action_sound", 2, 2, false);
			Function_Add("action_end_sound", 1, 1, false);
			Function_Add("action_if_sound", 1, 1, false);
			Function_Add("action_another_room", 2, 2, false);
			Function_Add("action_current_room", 1, 1, false);
			Function_Add("action_previous_room", 1, 1, false);
			Function_Add("action_next_room", 1, 1, false);
			Function_Add("action_if_previous_room", 0, 0, false);
			Function_Add("action_if_next_room", 0, 0, false);
			Function_Add("action_set_alarm", 2, 2, false, true);
			Function_Add("action_sleep", 2, 2, false);
			Function_Add("action_set_timeline", 2, 2, false, true);
			Function_Add("action_timeline_set", 2, 2, false, true);
			Function_Add("action_timeline_start", 0, 0, false, true);
			Function_Add("action_timeline_stop", 0, 0, false, true);
			Function_Add("action_timeline_pause", 0, 0, false, true);
			Function_Add("action_set_timeline_position", 1, 1, false, true);
			Function_Add("action_set_timeline_speed", 1, 1, false, true);
			Function_Add("action_message", 1, 1, false);
			Function_Add("action_show_info", 0, 0, false);
			Function_Add("action_show_video", 3, 3, true);
			Function_Add("action_end_game", 0, 0, false);
			Function_Add("action_restart_game", 0, 0, false);
			Function_Add("action_save_game", 1, 1, false);
			Function_Add("action_load_game", 1, 1, false);
			Function_Add("action_replace_sprite", 3, 3, true);
			Function_Add("action_replace_sound", 2, 2, true);
			Function_Add("action_replace_background", 2, 2, true);
			Function_Add("action_if_empty", 3, 3, false, true);
			Function_Add("action_if_collision", 3, 3, false, true);
			Function_Add("action_if", 1, 1, false);
			Function_Add("action_if_number", 3, 3, false);
			Function_Add("action_if_object", 3, 3, false, true);
			Function_Add("action_if_question", 1, 1, false);
			Function_Add("action_if_dice", 1, 1, false);
			Function_Add("action_if_mouse", 1, 1, false);
			Function_Add("action_if_aligned", 2, 2, false, true);
			Function_Add("action_execute_script", 6, 6, false, true);
			Function_Add("action_inherited", 0, 0, false, true, true);
			Function_Add("action_if_variable", 3, 3, false);
			Function_Add("action_draw_variable", 3, 3, false, true);
			Function_Add("action_set_score", 1, 1, false);
			Function_Add("action_if_score", 2, 2, false);
			Function_Add("action_draw_score", 3, 3, false, true);
			Function_Add("action_highscore_show", 5, 5, false);
			Function_Add("action_highscore_clear", 0, 0, false);
			Function_Add("action_set_life", 1, 1, false);
			Function_Add("action_if_life", 2, 2, false);
			Function_Add("action_draw_life", 3, 3, false, true);
			Function_Add("action_draw_life_images", 3, 3, false, true);
			Function_Add("action_set_health", 1, 1, false, true);
			Function_Add("action_if_health", 2, 2, false, true);
			Function_Add("action_draw_health", 6, 6, false, true);
			Function_Add("action_set_caption", 6, 6, false);
			Function_Add("action_partsyst_create", 1, 1, true);
			Function_Add("action_partsyst_destroy", 0, 0, true);
			Function_Add("action_partsyst_clear", 0, 0, true);
			Function_Add("action_parttype_create_old", 6, 6, true);
			Function_Add("action_parttype_create", 6, 6, true);
			Function_Add("action_parttype_color", 6, 6, true);
			Function_Add("action_parttype_life", 3, 3, true);
			Function_Add("action_parttype_speed", 6, 6, true);
			Function_Add("action_parttype_gravity", 3, 3, true);
			Function_Add("action_parttype_secondary", 5, 5, true);
			Function_Add("action_partemit_create", 6, 6, true);
			Function_Add("action_partemit_destroy", 1, 1, true);
			Function_Add("action_partemit_burst", 3, 3, true);
			Function_Add("action_partemit_stream", 3, 3, true);
			Function_Add("action_cd_play", 2, 2, true);
			Function_Add("action_cd_stop", 0, 0, true);
			Function_Add("action_cd_pause", 0, 0, true);
			Function_Add("action_cd_resume", 0, 0, true);
			Function_Add("action_cd_present", 0, 0, true);
			Function_Add("action_cd_playing", 0, 0, true);
			Function_Add("action_set_cursor", 2, 2, true);
			Function_Add("action_webpage", 1, 1, true);
			Function_Add("action_splash_web", 1, 1, true);
			Function_Add("action_draw_sprite", 4, 4, false, true);
			Function_Add("action_draw_background", 4, 4, false, true);
			Function_Add("action_draw_text", 3, 3, false, true);
			Function_Add("action_draw_text_transformed", 6, 6, true, true);
			Function_Add("action_draw_rectangle", 5, 5, false, true);
			Function_Add("action_draw_gradient_hor", 6, 6, true, true);
			Function_Add("action_draw_gradient_vert", 6, 6, true, true);
			Function_Add("action_draw_ellipse", 5, 5, false, true);
			Function_Add("action_draw_ellipse_gradient", 6, 6, true, true);
			Function_Add("action_draw_line", 4, 4, false, true);
			Function_Add("action_draw_arrow", 5, 5, false, true);
			Function_Add("action_color", 1, 1, false);
			Function_Add("action_font", 2, 2, false);
			Function_Add("action_fullscreen", 1, 1, false);
			Function_Add("action_snapshot", 1, 1, true);
			Function_Add("action_effect", 6, 6, true, true);
			Function_Add("ds_set_precision", 1, 1, true);
			Function_Add("ds_stack_create", 0, 0, true);
			Function_Add("ds_stack_destroy", 1, 1, true);
			Function_Add("ds_stack_clear", 1, 1, true);
			Function_Add("ds_stack_copy", 2, 2, true);
			Function_Add("ds_stack_size", 1, 1, true);
			Function_Add("ds_stack_empty", 1, 1, true);
			Function_Add("ds_stack_push", 2, 2, true);
			Function_Add("ds_stack_pop", 1, 1, true);
			Function_Add("ds_stack_top", 1, 1, true);
			Function_Add("ds_stack_write", 1, 1, true);
			Function_Add("ds_stack_read", 2, 2, true);
			Function_Add("ds_queue_create", 0, 0, true);
			Function_Add("ds_queue_destroy", 1, 1, true);
			Function_Add("ds_queue_clear", 1, 1, true);
			Function_Add("ds_queue_copy", 2, 2, true);
			Function_Add("ds_queue_size", 1, 1, true);
			Function_Add("ds_queue_empty", 1, 1, true);
			Function_Add("ds_queue_enqueue", 2, 2, true);
			Function_Add("ds_queue_dequeue", 1, 1, true);
			Function_Add("ds_queue_head", 1, 1, true);
			Function_Add("ds_queue_tail", 1, 1, true);
			Function_Add("ds_queue_write", 1, 1, true);
			Function_Add("ds_queue_read", 2, 2, true);
			Function_Add("ds_list_create", 0, 0, true);
			Function_Add("ds_list_destroy", 1, 1, true);
			Function_Add("ds_list_clear", 1, 1, true);
			Function_Add("ds_list_copy", 2, 2, true);
			Function_Add("ds_list_size", 1, 1, true);
			Function_Add("ds_list_empty", 1, 1, true);
			Function_Add("ds_list_add", 2, 2, true);
			Function_Add("ds_list_insert", 3, 3, true);
			Function_Add("ds_list_replace", 3, 3, true);
			Function_Add("ds_list_delete", 2, 2, true);
			Function_Add("ds_list_find_index", 2, 2, true);
			Function_Add("ds_list_find_value", 2, 2, true);
			Function_Add("ds_list_sort", 2, 2, true);
			Function_Add("ds_list_shuffle", 1, 1, true);
			Function_Add("ds_list_write", 1, 1, true);
			Function_Add("ds_list_read", 2, 2, true);
			Function_Add("ds_map_create", 0, 0, true);
			Function_Add("ds_map_destroy", 1, 1, true);
			Function_Add("ds_map_clear", 1, 1, true);
			Function_Add("ds_map_copy", 2, 2, true);
			Function_Add("ds_map_size", 1, 1, true);
			Function_Add("ds_map_empty", 1, 1, true);
			Function_Add("ds_map_add", 3, 3, true);
			Function_Add("ds_map_replace", 3, 3, true);
			Function_Add("ds_map_delete", 2, 2, true);
			Function_Add("ds_map_exists", 2, 2, true);
			Function_Add("ds_map_find_value", 2, 2, true);
			Function_Add("ds_map_find_previous", 2, 2, true);
			Function_Add("ds_map_find_next", 2, 2, true);
			Function_Add("ds_map_find_first", 1, 1, true);
			Function_Add("ds_map_find_last", 1, 1, true);
			Function_Add("ds_map_write", 1, 1, true);
			Function_Add("ds_map_read", 1, 2, true);
			Function_Add("ds_priority_create", 0, 0, true);
			Function_Add("ds_priority_destroy", 1, 1, true);
			Function_Add("ds_priority_clear", 1, 1, true);
			Function_Add("ds_priority_copy", 1, 2, true);
			Function_Add("ds_priority_size", 1, 1, true);
			Function_Add("ds_priority_empty", 1, 1, true);
			Function_Add("ds_priority_add", 3, 3, true);
			Function_Add("ds_priority_change_priority", 3, 3, true);
			Function_Add("ds_priority_find_priority", 2, 2, true);
			Function_Add("ds_priority_delete_value", 2, 2, true);
			Function_Add("ds_priority_delete_min", 1, 1, true);
			Function_Add("ds_priority_find_min", 1, 1, true);
			Function_Add("ds_priority_delete_max", 1, 1, true);
			Function_Add("ds_priority_find_max", 1, 1, true);
			Function_Add("ds_priority_write", 1, 1, true);
			Function_Add("ds_priority_read", 2, 2, true);
			Function_Add("ds_grid_create", 2, 2, true);
			Function_Add("ds_grid_destroy", 1, 1, true);
			Function_Add("ds_grid_copy", 2, 2, true);
			Function_Add("ds_grid_resize", 3, 3, true);
			Function_Add("ds_grid_width", 1, 1, true);
			Function_Add("ds_grid_height", 1, 1, true);
			Function_Add("ds_grid_clear", 2, 2, true);
			Function_Add("ds_grid_set", 4, 4, true);
			Function_Add("ds_grid_add", 4, 4, true);
			Function_Add("ds_grid_multiply", 4, 4, true);
			Function_Add("ds_grid_set_region", 6, 6, true);
			Function_Add("ds_grid_add_region", 6, 6, true);
			Function_Add("ds_grid_multiply_region", 6, 6, true);
			Function_Add("ds_grid_set_disk", 5, 5, true);
			Function_Add("ds_grid_add_disk", 5, 5, true);
			Function_Add("ds_grid_multiply_disk", 5, 5, true);
			Function_Add("ds_grid_set_grid_region", 8, 8, true);
			Function_Add("ds_grid_add_grid_region", 8, 8, true);
			Function_Add("ds_grid_multiply_grid_region", 8, 8, true);
			Function_Add("ds_grid_get", 3, 3, true);
			Function_Add("ds_grid_get_sum", 5, 5, true);
			Function_Add("ds_grid_get_max", 5, 5, true);
			Function_Add("ds_grid_get_min", 5, 5, true);
			Function_Add("ds_grid_get_mean", 5, 5, true);
			Function_Add("ds_grid_get_disk_sum", 4, 4, true);
			Function_Add("ds_grid_get_disk_max", 4, 4, true);
			Function_Add("ds_grid_get_disk_min", 4, 4, true);
			Function_Add("ds_grid_get_disk_mean", 4, 4, true);
			Function_Add("ds_grid_value_exists", 6, 6, true);
			Function_Add("ds_grid_value_x", 6, 6, true);
			Function_Add("ds_grid_value_y", 6, 6, true);
			Function_Add("ds_grid_value_disk_exists", 5, 5, true);
			Function_Add("ds_grid_value_disk_x", 5, 5, true);
			Function_Add("ds_grid_value_disk_y", 5, 5, true);
			Function_Add("ds_grid_shuffle", 1, 1, true);
			Function_Add("ds_grid_write", 1, 1, true);
			Function_Add("ds_grid_read", 2, 2, true);
			Function_Add("mplay_init_ipx", 0, 0, true);
			Function_Add("mplay_init_tcpip", 1, 1, true);
			Function_Add("mplay_init_modem", 2, 2, true);
			Function_Add("mplay_init_serial", 5, 5, true);
			Function_Add("mplay_connect_status", 0, 0, true);
			Function_Add("mplay_end", 0, 0, true);
			Function_Add("mplay_session_mode", 1, 1, true);
			Function_Add("mplay_session_create", 3, 3, true);
			Function_Add("mplay_session_find", 0, 0, true);
			Function_Add("mplay_session_name", 1, 1, true);
			Function_Add("mplay_session_join", 2, 2, true);
			Function_Add("mplay_session_status", 0, 0, true);
			Function_Add("mplay_session_end", 0, 0, true);
			Function_Add("mplay_player_find", 0, 0, true);
			Function_Add("mplay_player_name", 1, 1, true);
			Function_Add("mplay_player_id", 1, 1, true);
			Function_Add("mplay_data_write", 2, 2, true);
			Function_Add("mplay_data_read", 1, 1, true);
			Function_Add("mplay_data_mode", 1, 1, true);
			Function_Add("mplay_message_send", 3, 3, true);
			Function_Add("mplay_message_send_guaranteed", 3, 3, true);
			Function_Add("mplay_message_receive", 1, 1, true);
			Function_Add("mplay_message_id", 0, 0, true);
			Function_Add("mplay_message_value", 0, 0, true);
			Function_Add("mplay_message_player", 0, 0, true);
			Function_Add("mplay_message_name", 0, 0, true);
			Function_Add("mplay_message_count", 1, 1, true);
			Function_Add("mplay_message_clear", 1, 1, true);
			Function_Add("mplay_ipaddress", 0, 0, true);
			Function_Add("file_bin_open", 2, 2, false);
			Function_Add("file_bin_rewrite", 1, 1, false);
			Function_Add("file_bin_close", 1, 1, false);
			Function_Add("file_bin_position", 1, 1, false);
			Function_Add("file_bin_size", 1, 1, false);
			Function_Add("file_bin_seek", 2, 2, false);
			Function_Add("file_bin_read_byte", 1, 1, false);
			Function_Add("file_bin_write_byte", 2, 2, false);
			Function_Add("file_text_open_read", 1, 1, false);
			Function_Add("file_text_open_write", 1, 1, false);
			Function_Add("file_text_open_append", 1, 1, false);
			Function_Add("file_text_close", 1, 1, false);
			Function_Add("file_text_read_string", 1, 1, false);
			Function_Add("file_text_read_real", 1, 1, false);
			Function_Add("file_text_readln", 1, 1, false);
			Function_Add("file_text_eof", 1, 1, false);
			Function_Add("file_text_write_string", 2, 2, false);
			Function_Add("file_text_write_real", 2, 2, false);
			Function_Add("file_text_writeln", 1, 1, false);
			Function_Add("file_open_read", 1, 1, false);
			Function_Add("file_open_write", 1, 1, false);
			Function_Add("file_open_append", 1, 1, false);
			Function_Add("file_close", 0, 0, false);
			Function_Add("file_read_string", 0, 0, false);
			Function_Add("file_read_real", 0, 0, false);
			Function_Add("file_readln", 0, 0, false);
			Function_Add("file_eof", 0, 0, false);
			Function_Add("file_write_string", 1, 1, false);
			Function_Add("file_write_real", 1, 1, false);
			Function_Add("file_writeln", 0, 0, false);
			Function_Add("file_exists", 1, 1, false);
			Function_Add("file_delete", 1, 1, false);
			Function_Add("file_rename", 2, 2, false);
			Function_Add("file_copy", 2, 2, false);
			Function_Add("directory_exists", 1, 1, false);
			Function_Add("directory_create", 1, 1, false);
			Function_Add("file_find_first", 2, 2, false);
			Function_Add("file_find_next", 0, 0, false);
			Function_Add("file_find_close", 0, 0, false);
			Function_Add("file_attributes", 2, 2, false);
			Function_Add("filename_name", 1, 1, false);
			Function_Add("filename_path", 1, 1, false);
			Function_Add("filename_dir", 1, 1, false);
			Function_Add("filename_drive", 1, 1, false);
			Function_Add("filename_ext", 1, 1, false);
			Function_Add("filename_change_ext", 2, 2, false);
			Function_Add("export_include_file", 1, 1, false);
			Function_Add("export_include_file_location", 2, 2, false);
			Function_Add("discard_include_file", 1, 1, false);
			Function_Add("execute_program", 3, 3, false);
			Function_Add("execute_shell", 2, 2, false);
			Function_Add("parameter_count", 0, 0, false);
			Function_Add("parameter_string", 1, 1, false);
			Function_Add("environment_get_variable", 1, 1, false);
			Function_Add("registry_write_string", 2, 2, false);
			Function_Add("registry_write_real", 2, 2, false);
			Function_Add("registry_read_string", 1, 1, false);
			Function_Add("registry_read_real", 1, 1, false);
			Function_Add("registry_exists", 1, 1, false);
			Function_Add("registry_write_string_ext", 3, 3, false);
			Function_Add("registry_write_real_ext", 3, 3, false);
			Function_Add("registry_read_string_ext", 2, 2, false);
			Function_Add("registry_read_real_ext", 2, 2, false);
			Function_Add("registry_exists_ext", 2, 2, false);
			Function_Add("registry_set_root", 1, 1, false);
			Function_Add("ini_open", 1, 1, false);
			Function_Add("ini_close", 0, 0, false);
			Function_Add("ini_read_string", 3, 3, false);
			Function_Add("ini_read_real", 3, 3, false);
			Function_Add("ini_write_string", 3, 3, false);
			Function_Add("ini_write_real", 3, 3, false);
			Function_Add("ini_key_exists", 2, 2, false);
			Function_Add("ini_section_exists", 1, 1, false);
			Function_Add("ini_key_delete", 2, 2, false);
			Function_Add("ini_section_delete", 1, 1, false);
			Function_Add("http_post_string", 2, 2, false);
			Function_Add("http_get", 1, 1, false);
			Function_Add("move_random", 2, 2, false, true);
			Function_Add("place_free", 2, 2, false, true);
			Function_Add("place_empty", 2, 2, false, true);
			Function_Add("place_meeting", 3, 3, false, true);
			Function_Add("place_snapped", 2, 2, false, true);
			Function_Add("move_snap", 2, 2, false, true);
			Function_Add("move_towards_point", 3, 3, false, true);
			Function_Add("move_contact", 1, 1, false, true);
			Function_Add("move_contact_solid", 2, 2, false, true);
			Function_Add("move_contact_all", 2, 2, false, true);
			Function_Add("move_outside_solid", 2, 2, false, true);
			Function_Add("move_outside_all", 2, 2, false, true);
			Function_Add("move_bounce", 1, 1, false, true);
			Function_Add("move_bounce_solid", 1, 1, false, true);
			Function_Add("move_bounce_all", 1, 1, false, true);
			Function_Add("move_wrap", 3, 3, false, true);
			Function_Add("motion_set", 2, 2, false, true);
			Function_Add("motion_add", 2, 2, false, true);
			Function_Add("distance_to_point", 2, 2, false, true);
			Function_Add("distance_to_object", 1, 1, false, true);
			Function_Add("path_start", 4, 4, false, true);
			Function_Add("path_end", 0, 0, false, true);
			Function_Add("mp_linear_step", 4, 4, false, true);
			Function_Add("mp_linear_path", 5, 5, true, true);
			Function_Add("mp_linear_step_object", 4, 4, false, true);
			Function_Add("mp_linear_path_object", 5, 5, true, true);
			Function_Add("mp_potential_settings", 4, 4, false, true);
			Function_Add("mp_potential_step", 4, 4, false, true);
			Function_Add("mp_potential_path", 6, 6, true, true);
			Function_Add("mp_potential_step_object", 4, 4, false, true);
			Function_Add("mp_potential_path_object", 6, 6, true, true);
			Function_Add("mp_grid_create", 6, 6, true);
			Function_Add("mp_grid_destroy", 1, 1, true);
			Function_Add("mp_grid_clear_all", 1, 1, true);
			Function_Add("mp_grid_clear_cell", 3, 3, true);
			Function_Add("mp_grid_clear_rectangle", 5, 5, true);
			Function_Add("mp_grid_add_cell", 3, 3, true);
			Function_Add("mp_grid_add_rectangle", 5, 5, true);
			Function_Add("mp_grid_add_instances", 3, 3, true, true);
			Function_Add("mp_grid_path", 7, 7, true, true);
			Function_Add("mp_grid_draw", 1, 1, true);
			Function_Add("collision_point", 5, 5, false, true);
			Function_Add("collision_rectangle", 7, 7, false, true);
			Function_Add("collision_circle", 6, 6, false, true);
			Function_Add("collision_ellipse", 7, 7, false, true);
			Function_Add("collision_line", 7, 7, false, true);
			Function_Add("instance_find", 2, 2, false);
			Function_Add("instance_exists", 1, 1, false);
			Function_Add("instance_number", 1, 1, false);
			Function_Add("instance_position", 3, 3, false);
			Function_Add("instance_nearest", 3, 3, false, true);
			Function_Add("instance_furthest", 3, 3, false, true);
			Function_Add("instance_place", 3, 3, false, true);
			Function_Add("instance_create", 3, 3, false);
			Function_Add("instance_copy", 1, 1, false, true);
			Function_Add("instance_change", 2, 2, false, true);
			Function_Add("instance_destroy", 0, 0, false, true);
			Function_Add("instance_sprite", 1, 1, false, true);
			Function_Add("position_empty", 2, 2, false, true);
			Function_Add("position_meeting", 3, 3, false, true);
			Function_Add("position_destroy", 2, 2, false, true);
			Function_Add("position_change", 4, 4, false, true);
			Function_Add("instance_deactivate_all", 1, 1, false, true);
			Function_Add("instance_deactivate_object", 1, 1, false, true);
			Function_Add("instance_deactivate_region", 6, 6, false, true);
			Function_Add("instance_activate_all", 0, 0, false, true);
			Function_Add("instance_activate_object", 1, 1, false, true);
			Function_Add("instance_activate_region", 5, 5, false, true);
			Function_Add("room_goto", 1, 1, false);
			Function_Add("room_goto_previous", 0, 0, false);
			Function_Add("room_goto_next", 0, 0, false);
			Function_Add("room_previous", 1, 1, false);
			Function_Add("room_next", 1, 1, false);
			Function_Add("room_restart", 0, 0, false);
			Function_Add("game_end", 0, 0, false);
			Function_Add("game_restart", 0, 0, false);
			Function_Add("game_load", 1, 1, false);
			Function_Add("game_save", 1, 1, false);
			Function_Add("transition_define", 2, 2, true);
			Function_Add("transition_exists", 1, 1, true);
			Function_Add("sleep", 1, 1, false);
			Function_Add("display_get_width", 0, 0, false);
			Function_Add("display_get_height", 0, 0, false);
			Function_Add("display_get_colordepth", 0, 0, false);
			Function_Add("display_get_frequency", 0, 0, false);
			Function_Add("display_get_orientation", 0, 0, false);
			Function_Add("display_set_size", 2, 2, true);
			Function_Add("display_set_colordepth", 1, 1, true);
			Function_Add("display_set_frequency", 1, 1, true);
			Function_Add("display_set_all", 4, 4, true);
			Function_Add("display_test_all", 4, 4, true);
			Function_Add("display_reset", 0, 0, true);
			Function_Add("display_mouse_get_x", 0, 0, false);
			Function_Add("display_mouse_get_y", 0, 0, false);
			Function_Add("display_mouse_set", 2, 2, false);
			Function_Add("window_set_visible", 1, 1, false);
			Function_Add("window_get_visible", 0, 0, false);
			Function_Add("window_set_fullscreen", 1, 1, false);
			Function_Add("window_get_fullscreen", 0, 0, false);
			Function_Add("window_set_showborder", 1, 1, false);
			Function_Add("window_get_showborder", 0, 0, false);
			Function_Add("window_set_showicons", 1, 1, false);
			Function_Add("window_get_showicons", 0, 0, false);
			Function_Add("window_set_stayontop", 1, 1, false);
			Function_Add("window_get_stayontop", 0, 0, false);
			Function_Add("window_set_sizeable", 1, 1, false);
			Function_Add("window_get_sizeable", 0, 0, false);
			Function_Add("window_set_caption", 1, 1, false);
			Function_Add("window_get_caption", 0, 0, false);
			Function_Add("window_set_cursor", 1, 1, false);
			Function_Add("window_get_cursor", 0, 0, false);
			Function_Add("window_set_color", 1, 1, false);
			Function_Add("window_get_color", 0, 0, false);
			Function_Add("window_set_position", 2, 2, false);
			Function_Add("window_set_size", 2, 2, false);
			Function_Add("window_set_rectangle", 4, 4, false);
			Function_Add("window_center", 0, 0, false);
			Function_Add("window_default", 0, 0, false);
			Function_Add("window_get_x", 0, 0, false);
			Function_Add("window_get_y", 0, 0, false);
			Function_Add("window_get_width", 0, 0, false);
			Function_Add("window_get_height", 0, 0, false);
			Function_Add("window_set_region_size", 3, 3, false);
			Function_Add("window_get_region_width", 0, 0, false);
			Function_Add("window_get_region_height", 0, 0, false);
			Function_Add("window_set_region_scale", 2, 2, false);
			Function_Add("window_get_region_scale", 0, 0, false);
			Function_Add("window_mouse_get_x", 0, 0, false);
			Function_Add("window_mouse_get_y", 0, 0, false);
			Function_Add("window_mouse_set", 2, 2, false);
			Function_Add("window_view_mouse_get_x", 1, 1, false);
			Function_Add("window_view_mouse_get_y", 1, 1, false);
			Function_Add("window_view_mouse_set", 3, 3, false);
			Function_Add("window_views_mouse_get_x", 0, 0, false);
			Function_Add("window_views_mouse_get_y", 0, 0, false);
			Function_Add("window_views_mouse_set", 2, 2, false);
			Function_Add("set_synchronization", 1, 1, false);
			Function_Add("set_automatic_draw", 1, 1, false);
			Function_Add("screen_redraw", 0, 0, false);
			Function_Add("screen_refresh", 0, 0, false);
			Function_Add("screen_wait_vsync", 0, 0, false);
			Function_Add("screen_save", 1, 1, false);
			Function_Add("screen_save_part", 5, 5, false);
			Function_Add("draw_getpixel", 2, 2, false);
			Function_Add("draw_set_color", 1, 1, false);
			Function_Add("draw_set_alpha", 1, 1, false);
			Function_Add("draw_get_color", 0, 0, false);
			Function_Add("draw_get_alpha", 0, 0, false);
			Function_Add("make_color", 3, 3, false);
			Function_Add("make_color_rgb", 3, 3, false);
			Function_Add("make_color_hsv", 3, 3, false);
			Function_Add("color_get_red", 1, 1, false);
			Function_Add("color_get_green", 1, 1, false);
			Function_Add("color_get_blue", 1, 1, false);
			Function_Add("color_get_hue", 1, 1, false);
			Function_Add("color_get_saturation", 1, 1, false);
			Function_Add("color_get_value", 1, 1, false);
			Function_Add("merge_color", 3, 3, false);
			Function_Add("draw_set_blend_mode", 1, 1, true);
			Function_Add("draw_set_blend_mode_ext", 2, 2, true);
			Function_Add("draw_clear", 1, 1, false);
			Function_Add("draw_clear_alpha", 2, 2, false);
			Function_Add("draw_point", 2, 2, false);
			Function_Add("draw_line", 4, 4, false);
			Function_Add("draw_line_width", 5, 5, false);
			Function_Add("draw_rectangle", 5, 5, false);
			Function_Add("draw_roundrect", 5, 5, false);
			Function_Add("draw_triangle", 7, 7, false);
			Function_Add("draw_circle", 4, 4, false);
			Function_Add("draw_ellipse", 5, 5, false);
			Function_Add("draw_arrow", 5, 5, false);
			Function_Add("draw_button", 5, 5, false);
			Function_Add("draw_healthbar", 11, 11, false);
			Function_Add("draw_path", 4, 4, false);
			Function_Add("draw_point_color", 3, 3, true);
			Function_Add("draw_line_color", 6, 6, true);
			Function_Add("draw_line_width_color", 7, 7, true);
			Function_Add("draw_rectangle_color", 9, 9, true);
			Function_Add("draw_roundrect_color", 7, 7, true);
			Function_Add("draw_triangle_color", 10, 10, true);
			Function_Add("draw_circle_color", 6, 6, true);
			Function_Add("draw_ellipse_color", 7, 7, true);
			Function_Add("draw_set_circle_precision", 1, 1, true);
			Function_Add("draw_primitive_begin", 1, 1, true);
			Function_Add("draw_primitive_begin_texture", 2, 2, true);
			Function_Add("draw_primitive_end", 0, 0, true);
			Function_Add("draw_vertex", 2, 2, true);
			Function_Add("draw_vertex_color", 4, 4, true);
			Function_Add("draw_vertex_texture", 4, 4, true);
			Function_Add("draw_vertex_texture_color", 6, 6, true);
			Function_Add("sprite_get_texture", 2, 2, true);
			Function_Add("background_get_texture", 1, 1, true);
			Function_Add("texture_exists", 1, 1, true);
			Function_Add("texture_set_interpolation", 1, 1, true);
			Function_Add("texture_set_blending", 1, 1, true);
			Function_Add("texture_set_repeat", 1, 1, true);
			Function_Add("texture_get_width", 1, 1, true);
			Function_Add("texture_get_height", 1, 1, true);
			Function_Add("texture_preload", 1, 1, true);
			Function_Add("texture_set_priority", 2, 2, true);
			Function_Add("draw_set_font", 1, 1, false);
			Function_Add("draw_set_halign", 1, 1, false);
			Function_Add("draw_set_valign", 1, 1, false);
			Function_Add("string_width", 1, 1, false);
			Function_Add("string_height", 1, 1, false);
			Function_Add("string_width_ext", 3, 3, false);
			Function_Add("string_height_ext", 3, 3, false);
			Function_Add("draw_text", 3, 3, false);
			Function_Add("draw_text_ext", 5, 5, false);
			Function_Add("draw_text_transformed", 6, 6, true);
			Function_Add("draw_text_ext_transformed", 8, 8, true);
			Function_Add("draw_text_color", 8, 8, true);
			Function_Add("draw_text_transformed_color", 11, 11, true);
			Function_Add("draw_text_ext_color", 10, 10, true);
			Function_Add("draw_text_ext_transformed_color", 13, 13, true);
			Function_Add("draw_self", 0, 0, false, true);
			Function_Add("draw_sprite", 4, 4, false, true);
			Function_Add("draw_sprite_pos", 11, 11, false, true);
			Function_Add("draw_sprite_ext", 9, 9, true, true);
			Function_Add("draw_sprite_stretched", 6, 6, false, true);
			Function_Add("draw_sprite_stretched_ext", 8, 8, true, true);
			Function_Add("draw_sprite_part", 8, 8, false, true);
			Function_Add("draw_sprite_part_ext", 12, 12, true, true);
			Function_Add("draw_sprite_general", 16, 16, true, true);
			Function_Add("draw_sprite_tiled", 4, 4, false, true);
			Function_Add("draw_sprite_tiled_ext", 8, 8, true, true);
			Function_Add("draw_background", 3, 3, false);
			Function_Add("draw_background_ext", 8, 8, true);
			Function_Add("draw_background_stretched", 5, 5, false);
			Function_Add("draw_background_stretched_ext", 7, 7, true);
			Function_Add("draw_background_part", 7, 7, false);
			Function_Add("draw_background_part_ext", 11, 11, true);
			Function_Add("draw_background_general", 15, 15, true);
			Function_Add("draw_background_tiled", 3, 3, false);
			Function_Add("draw_background_tiled_ext", 7, 7, true);
			Function_Add("tile_get_x", 1, 1, false);
			Function_Add("tile_get_y", 1, 1, false);
			Function_Add("tile_get_left", 1, 1, false);
			Function_Add("tile_get_top", 1, 1, false);
			Function_Add("tile_get_width", 1, 1, false);
			Function_Add("tile_get_height", 1, 1, false);
			Function_Add("tile_get_depth", 1, 1, false);
			Function_Add("tile_get_visible", 1, 1, false);
			Function_Add("tile_get_xscale", 1, 1, false);
			Function_Add("tile_get_yscale", 1, 1, false);
			Function_Add("tile_get_blend", 1, 1, false);
			Function_Add("tile_get_alpha", 1, 1, false);
			Function_Add("tile_get_background", 1, 1, false);
			Function_Add("tile_set_visible", 2, 2, false);
			Function_Add("tile_set_background", 2, 2, false);
			Function_Add("tile_set_region", 5, 5, false);
			Function_Add("tile_set_position", 3, 3, false);
			Function_Add("tile_set_depth", 2, 2, false);
			Function_Add("tile_set_scale", 3, 3, false);
			Function_Add("tile_set_blend", 2, 2, true);
			Function_Add("tile_set_alpha", 2, 2, false);
			Function_Add("tile_add", 8, 8, false);
			Function_Add("tile_find", 3, 3, false);
			Function_Add("tile_exists", 1, 1, false);
			Function_Add("tile_delete", 1, 1, false);
			Function_Add("tile_delete_at", 3, 3, false);
			Function_Add("tile_layer_hide", 1, 1, false);
			Function_Add("tile_layer_show", 1, 1, false);
			Function_Add("tile_layer_delete", 1, 1, false);
			Function_Add("tile_layer_shift", 3, 3, false);
			Function_Add("tile_layer_find", 3, 3, false);
			Function_Add("tile_layer_delete_at", 3, 3, false);
			Function_Add("tile_layer_depth", 2, 2, false);
			Function_Add("surface_create", 2, 2, true);
			Function_Add("surface_create_ext", 3, 3, true);
			Function_Add("surface_free", 1, 1, true);
			Function_Add("surface_exists", 1, 1, true);
			Function_Add("surface_get_width", 1, 1, true);
			Function_Add("surface_get_height", 1, 1, true);
			Function_Add("surface_get_texture", 1, 1, true);
			Function_Add("surface_set_target", 1, 1, true);
			Function_Add("surface_reset_target", 0, 0, true);
			Function_Add("draw_surface", 3, 3, true);
			Function_Add("draw_surface_ext", 8, 8, true);
			Function_Add("draw_surface_stretched", 5, 5, true);
			Function_Add("draw_surface_stretched_ext", 7, 7, true);
			Function_Add("draw_surface_part", 7, 7, true);
			Function_Add("draw_surface_part_ext", 11, 11, true);
			Function_Add("draw_surface_general", 15, 15, true);
			Function_Add("draw_surface_tiled", 3, 3, true);
			Function_Add("draw_surface_tiled_ext", 7, 7, true);
			Function_Add("surface_save", 2, 2, true);
			Function_Add("surface_save_part", 6, 6, true);
			Function_Add("surface_getpixel", 3, 3, true);
			Function_Add("surface_copy", 4, 4, true);
			Function_Add("surface_copy_part", 8, 8, true);
			Function_Add("splash_show_video", 2, 2, true);
			Function_Add("splash_show_text", 2, 2, true);
			Function_Add("splash_show_image", 2, 2, true);
			Function_Add("splash_show_web", 2, 2, true);
			Function_Add("splash_set_caption", 1, 1, true);
			Function_Add("splash_set_fullscreen", 1, 1, true);
			Function_Add("splash_set_border", 1, 1, true);
			Function_Add("splash_set_size", 2, 2, true);
			Function_Add("splash_set_adapt", 1, 1, true);
			Function_Add("splash_set_top", 1, 1, true);
			Function_Add("splash_set_color", 1, 1, true);
			Function_Add("splash_set_main", 1, 1, true);
			Function_Add("splash_set_scale", 1, 1, true);
			Function_Add("splash_set_cursor", 1, 1, true);
			Function_Add("splash_set_interrupt", 1, 1, true);
			Function_Add("splash_set_stop_key", 1, 1, true);
			Function_Add("splash_set_stop_mouse", 1, 1, true);
			Function_Add("splash_set_close_button", 1, 1, true);
			Function_Add("splash_set_position", 1, 1, true);
			Function_Add("show_image", 3, 3, true);
			Function_Add("show_video", 3, 3, true);
			Function_Add("show_text", 4, 4, true);
			Function_Add("show_message", 1, 1, false);
			Function_Add("show_question", 1, 1, false);
			Function_Add("show_error", 2, 2, false);
			Function_Add("show_info", 0, 0, false);
			Function_Add("load_info", 1, 1, false);
			Function_Add("highscore_show", 1, 1, false);
			Function_Add("highscore_set_background", 1, 1, false);
			Function_Add("highscore_set_border", 1, 1, false);
			Function_Add("highscore_set_font", 3, 3, false);
			Function_Add("highscore_set_strings", 3, 3, false);
			Function_Add("highscore_set_colors", 3, 3, false);
			Function_Add("highscore_show_ext", 7, 7, false);
			Function_Add("highscore_clear", 0, 0, false);
			Function_Add("highscore_add", 2, 2, false);
			Function_Add("highscore_add_current", 0, 0, false);
			Function_Add("highscore_value", 1, 1, false);
			Function_Add("highscore_name", 1, 1, false);
			Function_Add("draw_highscore", 4, 4, false);
			Function_Add("show_message_ext", 4, 4, false);
			Function_Add("message_background", 1, 1, false);
			Function_Add("message_button", 1, 1, false);
			Function_Add("message_alpha", 1, 1, false);
			Function_Add("message_text_font", 4, 4, false);
			Function_Add("message_button_font", 4, 4, false);
			Function_Add("message_input_font", 4, 4, false);
			Function_Add("message_mouse_color", 1, 1, false);
			Function_Add("message_input_color", 1, 1, false);
			Function_Add("message_position", 2, 2, false);
			Function_Add("message_size", 2, 2, false);
			Function_Add("message_caption", 2, 2, false);
			Function_Add("show_menu", 2, 2, false);
			Function_Add("show_menu_pos", 4, 4, false);
			Function_Add("get_integer", 2, 2, false);
			Function_Add("get_string", 2, 2, false);
			Function_Add("get_color", 1, 1, false);
			Function_Add("get_open_filename", 2, 2, false);
			Function_Add("get_save_filename", 2, 2, false);
			Function_Add("get_directory", 1, 1, false);
			Function_Add("get_directory_alt", 2, 2, false);
			Function_Add("keyboard_get_numlock", 0, 0, false);
			Function_Add("keyboard_set_numlock", 1, 1, false);
			Function_Add("keyboard_key_press", 1, 1, false);
			Function_Add("keyboard_key_release", 1, 1, false);
			Function_Add("keyboard_set_map", 2, 2, false);
			Function_Add("keyboard_get_map", 1, 1, false);
			Function_Add("keyboard_unset_map", 0, 0, false);
			Function_Add("keyboard_check", 1, 1, false);
			Function_Add("keyboard_check_pressed", 1, 1, false);
			Function_Add("keyboard_check_released", 1, 1, false);
			Function_Add("keyboard_check_direct", 1, 1, false);
			Function_Add("mouse_check_button", 1, 1, false);
			Function_Add("mouse_check_button_pressed", 1, 1, false);
			Function_Add("mouse_check_button_released", 1, 1, false);
			Function_Add("joystick_exists", 1, 1, false);
			Function_Add("joystick_direction", 1, 1, false);
			Function_Add("joystick_name", 1, 1, false);
			Function_Add("joystick_axes", 1, 1, false);
			Function_Add("joystick_buttons", 1, 1, false);
			Function_Add("joystick_has_pov", 1, 1, false);
			Function_Add("joystick_check_button", 2, 2, false);
			Function_Add("joystick_xpos", 1, 1, false);
			Function_Add("joystick_ypos", 1, 1, false);
			Function_Add("joystick_zpos", 1, 1, false);
			Function_Add("joystick_rpos", 1, 1, false);
			Function_Add("joystick_upos", 1, 1, false);
			Function_Add("joystick_vpos", 1, 1, false);
			Function_Add("joystick_pov", 1, 1, false);
			Function_Add("keyboard_clear", 1, 1, false);
			Function_Add("mouse_clear", 1, 1, false);
			Function_Add("io_clear", 0, 0, false);
			Function_Add("io_handle", 0, 0, false);
			Function_Add("keyboard_wait", 0, 0, false);
			Function_Add("mouse_wait", 0, 0, false);
			Function_Add("is_real", 1, 1, false);
			Function_Add("is_string", 1, 1, false);
			Function_Add("random", 1, 1, false);
			Function_Add("random_range", 2, 2, false);
			Function_Add("irandom", 1, 1, false);
			Function_Add("irandom_range", 2, 2, false);
			Function_Add("random_set_seed", 1, 1, false);
			Function_Add("random_get_seed", 0, 0, false);
			Function_Add("randomize", 0, 0, false);
			Function_Add("abs", 1, 1, false);
			Function_Add("round", 1, 1, false);
			Function_Add("floor", 1, 1, false);
			Function_Add("ceil", 1, 1, false);
			Function_Add("sign", 1, 1, false);
			Function_Add("frac", 1, 1, false);
			Function_Add("sqrt", 1, 1, false);
			Function_Add("sqr", 1, 1, false);
			Function_Add("exp", 1, 1, false);
			Function_Add("ln", 1, 1, false);
			Function_Add("log2", 1, 1, false);
			Function_Add("log10", 1, 1, false);
			Function_Add("sin", 1, 1, false);
			Function_Add("cos", 1, 1, false);
			Function_Add("tan", 1, 1, false);
			Function_Add("arcsin", 1, 1, false);
			Function_Add("arccos", 1, 1, false);
			Function_Add("arctan", 1, 1, false);
			Function_Add("arctan2", 2, 2, false);
			Function_Add("degtorad", 1, 1, false);
			Function_Add("radtodeg", 1, 1, false);
			Function_Add("power", 2, 2, false);
			Function_Add("logn", 2, 2, false);
			Function_Add("min", -1, -1, false);
			Function_Add("max", -1, -1, false);
			Function_Add("min3", 3, 3, false);
			Function_Add("max3", 3, 3, false);
			Function_Add("mean", -1, -1, false);
			Function_Add("median", -1, -1, false);
			Function_Add("choose", -1, -1, false);
			Function_Add("clamp", 3, 3, true);
			Function_Add("lerp", 3, 3, true);
			Function_Add("real", 1, 1, false);
			Function_Add("string", 1, 1, false);
			Function_Add("string_format", 3, 3, false);
			Function_Add("chr", 1, 1, false);
			Function_Add("ord", 1, 1, false);
			Function_Add("string_length", 1, 1, false);
			Function_Add("string_pos", 2, 2, false);
			Function_Add("string_copy", 3, 3, false);
			Function_Add("string_char_at", 2, 2, false);
			Function_Add("string_delete", 3, 3, false);
			Function_Add("string_insert", 3, 3, false);
			Function_Add("string_lower", 1, 1, false);
			Function_Add("string_upper", 1, 1, false);
			Function_Add("string_repeat", 2, 2, false);
			Function_Add("string_letters", 1, 1, false);
			Function_Add("string_digits", 1, 1, false);
			Function_Add("string_lettersdigits", 1, 1, false);
			Function_Add("string_replace", 3, 3, false);
			Function_Add("string_replace_all", 3, 3, false);
			Function_Add("string_count", 2, 2, false);
			Function_Add("point_distance", 4, 4, false);
			Function_Add("point_direction", 4, 4, false);
			Function_Add("lengthdir_x", 2, 2, false);
			Function_Add("lengthdir_y", 2, 2, false);
			Function_Add("event_inherited", 0, 0, false, true, true);
			Function_Add("event_perform", 2, 2, false, true, true);
			Function_Add("event_user", 1, 1, false, true, true);
			Function_Add("event_perform_object", 3, 3, false, true, true);
			Function_Add("external_define", -1, -1, true);
			Function_Add("external_call", -1, -1, true);
			Function_Add("external_free", 1, 1, true);
			Function_Add("external_define0", 3, 3, true);
			Function_Add("external_call0", 1, 1, true);
			Function_Add("external_define1", 4, 4, true);
			Function_Add("external_call1", 2, 2, true);
			Function_Add("external_define2", 5, 5, true);
			Function_Add("external_call2", 3, 3, true);
			Function_Add("external_define3", 6, 6, true);
			Function_Add("external_call3", 4, 4, true);
			Function_Add("external_define4", 7, 7, true);
			Function_Add("external_call4", 5, 5, true);
			Function_Add("external_define5", 3, 3, true);
			Function_Add("external_call5", 6, 6, true);
			Function_Add("external_define6", 3, 3, true);
			Function_Add("external_call6", 7, 7, true);
			Function_Add("external_define7", 3, 3, true);
			Function_Add("external_call7", 8, 8, true);
			Function_Add("external_define8", 3, 3, true);
			Function_Add("external_call8", 9, 9, true);
			Function_Add("execute_string", -1, -1, false);
			Function_Add("execute_file", -1, -1, false);
			Function_Add("window_handle", 0, 0, false);
			Function_Add("show_debug_message", 1, 1, false);
			Function_Add("set_program_priority", 1, 1, false);
			Function_Add("set_application_title", 1, 1, false);
			Function_Add("variable_global_exists", 1, 1, false);
			Function_Add("variable_global_get", 1, 1, false);
			Function_Add("variable_global_array_get", 2, 2, false);
			Function_Add("variable_global_array2_get", 3, 3, false);
			Function_Add("variable_global_set", 2, 2, false);
			Function_Add("variable_global_array_set", 3, 3, false);
			Function_Add("variable_global_array2_set", 4, 4, false);
			Function_Add("variable_local_exists", 1, 1, false, true);
			Function_Add("variable_local_get", 1, 1, false, true);
			Function_Add("variable_local_array_get", 2, 2, false, true);
			Function_Add("variable_local_array2_get", 3, 3, false, true);
			Function_Add("variable_local_set", 2, 2, false, true);
			Function_Add("variable_local_array_set", 3, 3, false, true);
			Function_Add("variable_local_array2_set", 4, 4, false, true);
			Function_Add("clipboard_has_text", 0, 0, false);
			Function_Add("clipboard_set_text", 1, 1, false);
			Function_Add("clipboard_get_text", 0, 0, false);
			Function_Add("date_current_datetime", 0, 0, false);
			Function_Add("date_current_date", 0, 0, false);
			Function_Add("date_current_time", 0, 0, false);
			Function_Add("date_create_datetime", 6, 6, false);
			Function_Add("date_create_date", 3, 3, false);
			Function_Add("date_create_time", 3, 3, false);
			Function_Add("date_valid_datetime", 6, 6, false);
			Function_Add("date_valid_date", 3, 3, false);
			Function_Add("date_valid_time", 3, 3, false);
			Function_Add("date_inc_year", 2, 2, false);
			Function_Add("date_inc_month", 2, 2, false);
			Function_Add("date_inc_week", 2, 2, false);
			Function_Add("date_inc_day", 2, 2, false);
			Function_Add("date_inc_hour", 2, 2, false);
			Function_Add("date_inc_minute", 2, 2, false);
			Function_Add("date_inc_second", 2, 2, false);
			Function_Add("date_get_year", 1, 1, false);
			Function_Add("date_get_month", 1, 1, false);
			Function_Add("date_get_week", 1, 1, false);
			Function_Add("date_get_day", 1, 1, false);
			Function_Add("date_get_hour", 1, 1, false);
			Function_Add("date_get_minute", 1, 1, false);
			Function_Add("date_get_second", 1, 1, false);
			Function_Add("date_get_weekday", 1, 1, false);
			Function_Add("date_get_day_of_year", 1, 1, false);
			Function_Add("date_get_hour_of_year", 1, 1, false);
			Function_Add("date_get_minute_of_year", 1, 1, false);
			Function_Add("date_get_second_of_year", 1, 1, false);
			Function_Add("date_year_span", 2, 2, false);
			Function_Add("date_month_span", 2, 2, false);
			Function_Add("date_week_span", 2, 2, false);
			Function_Add("date_day_span", 2, 2, false);
			Function_Add("date_hour_span", 2, 2, false);
			Function_Add("date_minute_span", 2, 2, false);
			Function_Add("date_second_span", 2, 2, false);
			Function_Add("date_compare_datetime", 2, 2, false);
			Function_Add("date_compare_date", 2, 2, false);
			Function_Add("date_compare_time", 2, 2, false);
			Function_Add("date_date_of", 1, 1, false);
			Function_Add("date_time_of", 1, 1, false);
			Function_Add("date_datetime_string", 1, 1, false);
			Function_Add("date_date_string", 1, 1, false);
			Function_Add("date_time_string", 1, 1, false);
			Function_Add("date_days_in_month", 1, 1, false);
			Function_Add("date_days_in_year", 1, 1, false);
			Function_Add("date_leap_year", 1, 1, false);
			Function_Add("date_is_today", 1, 1, false);
			Function_Add("part_type_create", 0, 0, true);
			Function_Add("part_type_destroy", 1, 1, true);
			Function_Add("part_type_exists", 1, 1, true);
			Function_Add("part_type_clear", 1, 1, true);
			Function_Add("part_type_shape", 2, 2, true);
			Function_Add("part_type_sprite", 5, 5, true);
			Function_Add("part_type_size", 5, 5, true);
			Function_Add("part_type_scale", 3, 3, true);
			Function_Add("part_type_life", 3, 3, true);
			Function_Add("part_type_step", 3, 3, true);
			Function_Add("part_type_death", 3, 3, true);
			Function_Add("part_type_speed", 5, 5, true);
			Function_Add("part_type_direction", 5, 5, true);
			Function_Add("part_type_orientation", 6, 6, true);
			Function_Add("part_type_gravity", 3, 3, true);
			Function_Add("part_type_color_mix", 3, 3, true);
			Function_Add("part_type_color_rgb", 7, 7, true);
			Function_Add("part_type_color_hsv", 7, 7, true);
			Function_Add("part_type_color1", 2, 2, true);
			Function_Add("part_type_color2", 3, 3, true);
			Function_Add("part_type_color3", 4, 4, true);
			Function_Add("part_type_color", 4, 4, true);
			Function_Add("part_type_alpha1", 2, 2, true);
			Function_Add("part_type_alpha2", 3, 3, true);
			Function_Add("part_type_alpha3", 4, 4, true);
			Function_Add("part_type_alpha", 4, 4, true);
			Function_Add("part_type_blend", 2, 2, true);
			Function_Add("part_system_create", 0, 0, true);
			Function_Add("part_system_destroy", 1, 1, true);
			Function_Add("part_system_exists", 1, 1, true);
			Function_Add("part_system_clear", 1, 1, true);
			Function_Add("part_system_draw_order", 2, 2, true);
			Function_Add("part_system_depth", 2, 2, true);
			Function_Add("part_system_position", 3, 3, true);
			Function_Add("part_system_automatic_update", 2, 2, true);
			Function_Add("part_system_automatic_draw", 2, 2, true);
			Function_Add("part_system_update", 1, 1, true);
			Function_Add("part_system_drawit", 1, 1, true);
			Function_Add("part_particles_create", 5, 5, true);
			Function_Add("part_particles_create_color", 6, 6, true);
			Function_Add("part_particles_clear", 1, 1, true);
			Function_Add("part_particles_count", 1, 1, true);
			Function_Add("part_emitter_create", 1, 1, true);
			Function_Add("part_emitter_destroy", 2, 2, true);
			Function_Add("part_emitter_destroy_all", 1, 1, true);
			Function_Add("part_emitter_exists", 2, 2, true);
			Function_Add("part_emitter_clear", 2, 2, true);
			Function_Add("part_emitter_region", 8, 8, true);
			Function_Add("part_emitter_burst", 4, 4, true);
			Function_Add("part_emitter_stream", 4, 4, true);
			Function_Add("part_attractor_create", 1, 1, true);
			Function_Add("part_attractor_destroy", 2, 2, true);
			Function_Add("part_attractor_destroy_all", 1, 1, true);
			Function_Add("part_attractor_exists", 2, 2, true);
			Function_Add("part_attractor_clear", 2, 2, true);
			Function_Add("part_attractor_position", 4, 4, true);
			Function_Add("part_attractor_force", 6, 6, true);
			Function_Add("part_destroyer_create", 1, 1, true);
			Function_Add("part_destroyer_destroy", 2, 2, true);
			Function_Add("part_destroyer_destroy_all", 1, 1, true);
			Function_Add("part_destroyer_exists", 2, 2, true);
			Function_Add("part_destroyer_clear", 2, 2, true);
			Function_Add("part_destroyer_region", 7, 7, true);
			Function_Add("part_deflector_create", 1, 1, true);
			Function_Add("part_deflector_destroy", 2, 2, true);
			Function_Add("part_deflector_destroy_all", 1, 1, true);
			Function_Add("part_deflector_exists", 2, 2, true);
			Function_Add("part_deflector_clear", 2, 2, true);
			Function_Add("part_deflector_region", 6, 6, true);
			Function_Add("part_deflector_kind", 3, 3, true);
			Function_Add("part_deflector_friction", 3, 3, true);
			Function_Add("part_changer_create", 1, 1, true);
			Function_Add("part_changer_destroy", 2, 2, true);
			Function_Add("part_changer_destroy_all", 1, 1, true);
			Function_Add("part_changer_exists", 2, 2, true);
			Function_Add("part_changer_clear", 2, 2, true);
			Function_Add("part_changer_region", 7, 7, true);
			Function_Add("part_changer_kind", 3, 3, true);
			Function_Add("part_changer_types", 4, 4, true);
			Function_Add("effect_create_below", 5, 5, true);
			Function_Add("effect_create_above", 5, 5, true);
			Function_Add("effect_clear", 0, 0, true);
			Function_Add("sprite_name", 1, 1, false);
			Function_Add("sprite_exists", 1, 1, false);
			Function_Add("sprite_get_name", 1, 1, false);
			Function_Add("sprite_get_number", 1, 1, false);
			Function_Add("sprite_get_width", 1, 1, false);
			Function_Add("sprite_get_height", 1, 1, false);
			Function_Add("sprite_get_transparent", 1, 1, false);
			Function_Add("sprite_get_smooth", 1, 1, false);
			Function_Add("sprite_get_preload", 1, 1, false);
			Function_Add("sprite_get_xoffset", 1, 1, false);
			Function_Add("sprite_get_yoffset", 1, 1, false);
			Function_Add("sprite_get_bbox_mode", 1, 1, false);
			Function_Add("sprite_get_bbox_left", 1, 1, false);
			Function_Add("sprite_get_bbox_right", 1, 1, false);
			Function_Add("sprite_get_bbox_top", 1, 1, false);
			Function_Add("sprite_get_bbox_bottom", 1, 1, false);
			Function_Add("sprite_get_precise", 1, 1, false);
			Function_Add("sprite_collision_mask", 9, 9, false);
			Function_Add("sprite_set_offset", 3, 3, true);
			Function_Add("sprite_set_bbox_mode", 2, 2, true);
			Function_Add("sprite_set_bbox", 5, 5, true);
			Function_Add("sprite_set_precise", 2, 2, true);
			Function_Add("sprite_set_alpha_from_sprite", 2, 2, true);
			Function_Add("sprite_create_from_screen", 10, 10, true);
			Function_Add("sprite_add_from_screen", 5, 5, true);
			Function_Add("sprite_create_from_surface", 11, 11, true);
			Function_Add("sprite_add_from_surface", 6, 6, true);
			Function_Add("sprite_add", 8, 8, true);
			Function_Add("sprite_replace", 9, 9, true);
			Function_Add("sprite_add_alpha", 6, 6, true);
			Function_Add("sprite_replace_alpha", 7, 7, true);
			Function_Add("sprite_delete", 1, 1, true);
			Function_Add("sprite_duplicate", 1, 1, true);
			Function_Add("sprite_assign", 2, 2, true);
			Function_Add("sprite_merge", 2, 2, true);
			Function_Add("sprite_save", 3, 3, true);
			Function_Add("sprite_set_cache_size", 2, 2, true);
			Function_Add("sprite_set_cache_size_ext", 3, 3, true);
			Function_Add("background_name", 1, 1, false);
			Function_Add("background_exists", 1, 1, false);
			Function_Add("background_get_name", 1, 1, false);
			Function_Add("background_get_width", 1, 1, false);
			Function_Add("background_get_height", 1, 1, false);
			Function_Add("background_get_transparent", 1, 1, false);
			Function_Add("background_get_smooth", 1, 1, false);
			Function_Add("background_get_preload", 1, 1, false);
			Function_Add("background_set_alpha_from_background", 2, 2, true);
			Function_Add("background_create_from_screen", 7, 7, true);
			Function_Add("background_create_from_surface", 8, 8, true);
			Function_Add("background_create_color", 4, 3, true);
			Function_Add("background_create_gradient", 6, 6, true);
			Function_Add("background_add", 4, 4, true);
			Function_Add("background_replace", 5, 5, true);
			Function_Add("background_add_alpha", 2, 2, true);
			Function_Add("background_replace_alpha", 3, 3, true);
			Function_Add("background_delete", 1, 1, true);
			Function_Add("background_duplicate", 1, 1, true);
			Function_Add("background_assign", 2, 2, true);
			Function_Add("background_save", 2, 2, true);
			Function_Add("sound_name", 1, 1, false);
			Function_Add("sound_exists", 1, 1, false);
			Function_Add("sound_get_name", 1, 1, false);
			Function_Add("sound_get_kind", 1, 1, false);
			Function_Add("sound_get_preload", 1, 1, false);
			Function_Add("sound_discard", 1, 1, false);
			Function_Add("sound_restore", 1, 1, false);
			Function_Add("sound_add", 3, 3, true);
			Function_Add("sound_replace", 4, 4, true);
			Function_Add("sound_delete", 1, 1, true);
			Function_Add("font_name", 1, 1, false);
			Function_Add("font_exists", 1, 1, false);
			Function_Add("font_get_name", 1, 1, false);
			Function_Add("font_get_fontname", 1, 1, false);
			Function_Add("font_get_size", 1, 1, false);
			Function_Add("font_get_bold", 1, 1, false);
			Function_Add("font_get_italic", 1, 1, false);
			Function_Add("font_get_first", 1, 1, false);
			Function_Add("font_get_last", 1, 1, false);
			Function_Add("font_add", 6, 6, true);
			Function_Add("font_replace", 7, 7, true);
			Function_Add("font_add_sprite", 4, 4, true);
			Function_Add("font_replace_sprite", 5, 5, true);
			Function_Add("font_delete", 1, 1, true);
			Function_Add("script_name", 1, 1, false);
			Function_Add("script_exists", 1, 1, false);
			Function_Add("script_get_name", 1, 1, false);
			Function_Add("script_get_text", 1, 1, false);
			Function_Add("script_execute", -1, -1, false);
			Function_Add("path_name", 1, 1, false);
			Function_Add("path_exists", 1, 1, false);
			Function_Add("path_get_name", 1, 1, false);
			Function_Add("path_get_length", 1, 1, false);
			Function_Add("path_get_kind", 1, 1, false);
			Function_Add("path_get_closed", 1, 1, false);
			Function_Add("path_get_precision", 1, 1, false);
			Function_Add("path_get_number", 1, 1, false);
			Function_Add("path_get_point_x", 2, 2, false);
			Function_Add("path_get_point_y", 2, 2, false);
			Function_Add("path_get_point_speed", 2, 2, false);
			Function_Add("path_get_x", 2, 2, false);
			Function_Add("path_get_y", 2, 2, false);
			Function_Add("path_get_speed", 2, 2, false);
			Function_Add("path_set_kind", 2, 2, true);
			Function_Add("path_set_closed", 2, 2, true);
			Function_Add("path_set_precision", 2, 2, true);
			Function_Add("path_add", 0, 0, true);
			Function_Add("path_duplicate", 1, 1, true);
			Function_Add("path_assign", 2, 2, true);
			Function_Add("path_append", 2, 2, true);
			Function_Add("path_delete", 1, 1, true);
			Function_Add("path_add_point", 4, 4, true);
			Function_Add("path_insert_point", 5, 5, true);
			Function_Add("path_change_point", 5, 5, true);
			Function_Add("path_delete_point", 2, 2, true);
			Function_Add("path_clear_points", 1, 1, true);
			Function_Add("path_reverse", 1, 1, true);
			Function_Add("path_mirror", 1, 1, true);
			Function_Add("path_flip", 1, 1, true);
			Function_Add("path_rotate", 2, 2, true);
			Function_Add("path_scale", 3, 3, true);
			Function_Add("path_shift", 3, 3, true);
			Function_Add("timeline_name", 1, 1, false);
			Function_Add("timeline_exists", 1, 1, false);
			Function_Add("timeline_get_name", 1, 1, false);
			Function_Add("timeline_add", 0, 0, true);
			Function_Add("timeline_delete", 1, 1, true);
			Function_Add("timeline_moment_clear", 2, 2, true);
			Function_Add("timeline_moment_add", 3, 3, true);
			Function_Add("object_name", 1, 1, false);
			Function_Add("object_exists", 1, 1, false);
			Function_Add("object_get_name", 1, 1, false);
			Function_Add("object_get_sprite", 1, 1, false);
			Function_Add("object_get_solid", 1, 1, false);
			Function_Add("object_get_visible", 1, 1, false);
			Function_Add("object_get_depth", 1, 1, false);
			Function_Add("object_get_persistent", 1, 1, false);
			Function_Add("object_get_mask", 1, 1, false);
			Function_Add("object_get_parent", 1, 1, false);
			Function_Add("object_is_ancestor", 2, 2, false);
			Function_Add("object_set_sprite", 2, 2, true);
			Function_Add("object_set_solid", 2, 2, true);
			Function_Add("object_set_visible", 2, 2, true);
			Function_Add("object_set_depth", 2, 2, true);
			Function_Add("object_set_persistent", 2, 2, true);
			Function_Add("object_set_mask", 2, 2, true);
			Function_Add("object_set_parent", 2, 2, true);
			Function_Add("object_add", 0, 0, true);
			Function_Add("object_delete", 1, 1, true);
			Function_Add("object_event_clear", 3, 3, true);
			Function_Add("object_event_add", 4, 4, true);
			Function_Add("room_name", 1, 1, false);
			Function_Add("room_exists", 1, 1, false);
			Function_Add("room_get_name", 1, 1, false);
			Function_Add("room_set_width", 2, 2, true);
			Function_Add("room_set_height", 2, 2, true);
			Function_Add("room_set_caption", 2, 2, true);
			Function_Add("room_set_persistent", 2, 2, true);
			Function_Add("room_set_code", 2, 2, true);
			Function_Add("room_set_background_color", 3, 3, true);
			Function_Add("room_set_background", 12, 12, true);
			Function_Add("room_set_view", 16, 16, true);
			Function_Add("room_set_view_enabled", 2, 2, true);
			Function_Add("room_add", 0, 0, true);
			Function_Add("room_duplicate", 1, 1, true);
			Function_Add("room_assign", 2, 2, true);
			Function_Add("room_instance_add", 4, 4, true);
			Function_Add("room_instance_clear", 1, 1, true);
			Function_Add("room_tile_add", 9, 9, true);
			Function_Add("room_tile_add_ext", 12, 12, true);
			Function_Add("room_tile_clear", 1, 1, true);
			Function_Add("sound_play", 1, 1, false);
			Function_Add("sound_loop", 1, 1, false);
			Function_Add("sound_stop", 1, 1, false);
			Function_Add("sound_stop_all", 0, 0, false);
			Function_Add("sound_isplaying", 1, 1, false);
			Function_Add("sound_volume", 2, 2, false);
			Function_Add("sound_fade", 3, 3, false);
			Function_Add("sound_pan", 2, 2, false);
			Function_Add("sound_background_tempo", 1, 1, false);
			Function_Add("sound_global_volume", 1, 1, false);
			Function_Add("sound_set_search_directory", 1, 1, false);
			Function_Add("sound_effect_set", 2, 2, true);
			Function_Add("sound_effect_chorus", 8, 8, true);
			Function_Add("sound_effect_compressor", 7, 7, true);
			Function_Add("sound_effect_echo", 6, 6, true);
			Function_Add("sound_effect_flanger", 8, 8, true);
			Function_Add("sound_effect_gargle", 3, 3, true);
			Function_Add("sound_effect_equalizer", 4, 4, true);
			Function_Add("sound_effect_reverb", 5, 5, true);
			Function_Add("sound_3d_set_sound_position", 4, 4, true);
			Function_Add("sound_3d_set_sound_velocity", 4, 4, true);
			Function_Add("sound_3d_set_sound_distance", 3, 3, true);
			Function_Add("sound_3d_set_sound_cone", 7, 7, true);
			Function_Add("cd_init", 0, 0, true);
			Function_Add("cd_present", 0, 0, true);
			Function_Add("cd_number", 0, 0, true);
			Function_Add("cd_playing", 0, 0, true);
			Function_Add("cd_paused", 0, 0, true);
			Function_Add("cd_track", 0, 0, true);
			Function_Add("cd_length", 0, 0, true);
			Function_Add("cd_track_length", 1, 1, true);
			Function_Add("cd_position", 0, 0, true);
			Function_Add("cd_track_position", 0, 0, true);
			Function_Add("cd_play", 2, 2, true);
			Function_Add("cd_stop", 0, 0, true);
			Function_Add("cd_pause", 0, 0, true);
			Function_Add("cd_resume", 0, 0, true);
			Function_Add("cd_set_position", 1, 1, true);
			Function_Add("cd_set_track_position", 1, 1, true);
			Function_Add("cd_open_door", 0, 0, true);
			Function_Add("cd_close_door", 0, 0, true);
			Function_Add("MCI_command", 1, 1, true);
			Function_Add("YoYo_AddVirtualKey", 5, 5, false);
			Function_Add("YoYo_DeleteVirtualKey", 1, 1, false);
			Function_Add("YoYo_ShowVirtualKey", 1, 1, false);
			Function_Add("YoYo_HideVirtualKey", 1, 1, false);
			Function_Add("YoYo_LoginAchievements", 0, 0, false);
			Function_Add("YoYo_LogoutAchievements", 0, 0, false);
			Function_Add("YoYo_PostAchievement", 2, 2, false);
			Function_Add("YoYo_PostScore", 2, 2, false);
			Function_Add("YoYo_AchievementsAvailable", 0, 0, false);
			Function_Add("YoYo_GetDomain", 0, 0, false);
			Function_Add("YoYo_OpenURL", 1, 1, false);
			Function_Add("YoYo_OpenURL_ext", 2, 2, false);
			Function_Add("YoYo_OpenURL_full", 3, 3, false);
			Function_Add("YoYo_EnableAds", 5, 5, false);
			Function_Add("YoYo_DisableAds", 0, 0, false);
			Function_Add("YoYo_LeaveRating", 4, 4, false);
			Function_Add("YoYo_GetTimer", 0, 0, false);
			Function_Add("YoYo_EnableAlphaBlend", 1, 1, false);
			Function_Add("YoYo_GetPlatform", 0, 0, false);
			Function_Add("YoYo_GetDevice", 0, 0, false);
			Function_Add("YoYo_GetConfig", 0, 0, false);
			Function_Add("YoYo_GetTiltX", 0, 0, false);
			Function_Add("YoYo_GetTiltY", 0, 0, false);
			Function_Add("YoYo_GetTiltZ", 0, 0, false);
			Function_Add("YoYo_SelectPicture", 0, 0, false);
			Function_Add("YoYo_GetPictureSprite", 0, 0, false);
			Function_Add("YoYo_IsKeypadOpen", 0, 0, false);
			Function_Add("YoYo_OF_StartDashboard", 0, 0, false);
			Function_Add("YoYo_OF_AddAchievement", 2, 2, false);
			Function_Add("YoYo_OF_AddLeaderboard", 3, 3, false);
			Function_Add("YoYo_OF_SendChallenge", 3, 3, false);
			Function_Add("YoYo_OF_SendInvite", 1, 1, false);
			Function_Add("YoYo_OF_SendSocial", 3, 3, false);
			Function_Add("YoYo_OF_SetURL", 1, 1, false);
			Function_Add("YoYo_OF_AcceptChallenge", 0, 0, false);
			Function_Add("YoYo_OF_IsOnline", 0, 0, false);
			Function_Add("YoYo_OF_SendChallengeResult", 2, 2, false);
			Function_Add("YoYo_MouseCheckButton", 2, 2, false);
			Function_Add("YoYo_MouseCheckButtonPressed", 2, 2, false);
			Function_Add("YoYo_MouseCheckButtonReleased", 2, 2, false);
			Function_Add("YoYo_MouseX", 1, 1, false);
			Function_Add("YoYo_MouseY", 1, 1, false);
			Function_Add("YoYo_EnableInAppPurchases", 0, 0, false);
			Function_Add("YoYo_RestoreInAppPurchases", 0, 0, false);
			Function_Add("YoYo_RetrieveInAppPurchases", 0, 0, false);
			Function_Add("YoYo_AcquireInAppPurchase", 1, 1, false);
			Function_Add("YoYo_GetPurchasedDetails", 0, 0, false);
			Function_Add("YoYo_ProductPurchased", 1, 1, false);
			Function_Add("YoYo_FacebookInit", 1, 1, false);
			Function_Add("YoYo_FacebookLogin", 1, 1, false);
			Function_Add("YoYo_FacebookLoginStatus", 0, 0, false);
			Function_Add("YoYo_FacebookGraphRequest", 4, 4, false);
			Function_Add("YoYo_FacebookDialog", 3, 3, false);
			Function_Add("YoYo_FacebookLogout", 0, 0, false);
			Function_Add("YoYo_OSPauseEvent", 0, 0, false);
			AddRealConstant("self", -1.0);
			AddRealConstant("other", -2.0);
			AddRealConstant("all", -3.0);
			AddRealConstant("noone", -4.0);
			AddRealConstant("global", -5.0);
			AddRealConstant("local", -7.0);
			AddRealConstant("true", 1.0);
			AddRealConstant("false", 0.0);
			AddRealConstant("pi", Math.PI);
			AddRealConstant("pr_pointlist", 1.0);
			AddRealConstant("pr_linelist", 2.0);
			AddRealConstant("pr_linestrip", 3.0);
			AddRealConstant("pr_trianglelist", 4.0);
			AddRealConstant("pr_trianglestrip", 5.0);
			AddRealConstant("pr_trianglefan", 6.0);
			AddRealConstant("c_aqua", 16776960.0);
			AddRealConstant("c_black", 0.0);
			AddRealConstant("c_blue", 16711680.0);
			AddRealConstant("c_dkgray", 4210752.0);
			AddRealConstant("c_fuchsia", 16711935.0);
			AddRealConstant("c_gray", 8421504.0);
			AddRealConstant("c_green", 32768.0);
			AddRealConstant("c_lime", 65280.0);
			AddRealConstant("c_ltgray", 12632256.0);
			AddRealConstant("c_maroon", 128.0);
			AddRealConstant("c_navy", 8388608.0);
			AddRealConstant("c_olive", 32896.0);
			AddRealConstant("c_purple", 8388736.0);
			AddRealConstant("c_red", 255.0);
			AddRealConstant("c_silver", 12632256.0);
			AddRealConstant("c_teal", 8421376.0);
			AddRealConstant("c_white", 16777215.0);
			AddRealConstant("c_yellow", 65535.0);
			AddRealConstant("c_orange", 4235519.0);
			AddRealConstant("bm_normal", 0.0);
			AddRealConstant("bm_add", 1.0);
			AddRealConstant("bm_max", 2.0);
			AddRealConstant("bm_subtract", 3.0);
			AddRealConstant("bm_zero", 1.0);
			AddRealConstant("bm_one", 2.0);
			AddRealConstant("bm_src_color", 3.0);
			AddRealConstant("bm_inv_src_color", 4.0);
			AddRealConstant("bm_src_alpha", 5.0);
			AddRealConstant("bm_inv_src_alpha", 6.0);
			AddRealConstant("bm_dest_alpha", 7.0);
			AddRealConstant("bm_inv_dest_alpha", 8.0);
			AddRealConstant("bm_dest_color", 9.0);
			AddRealConstant("bm_inv_dest_color", 10.0);
			AddRealConstant("bm_src_alpha_sat", 11.0);
			AddRealConstant("se_none", 0.0);
			AddRealConstant("se_chorus", 1.0);
			AddRealConstant("se_echo", 2.0);
			AddRealConstant("se_flanger", 4.0);
			AddRealConstant("se_gargle", 8.0);
			AddRealConstant("se_reverb", 16.0);
			AddRealConstant("se_compressor", 32.0);
			AddRealConstant("se_equalizer", 64.0);
			AddRealConstant("fa_left", 0.0);
			AddRealConstant("fa_center", 1.0);
			AddRealConstant("fa_right", 2.0);
			AddRealConstant("fa_top", 0.0);
			AddRealConstant("fa_middle", 1.0);
			AddRealConstant("fa_bottom", 2.0);
			AddRealConstant("mb_any", -1.0);
			AddRealConstant("mb_none", 0.0);
			AddRealConstant("mb_left", 1.0);
			AddRealConstant("mb_right", 2.0);
			AddRealConstant("mb_middle", 3.0);
			AddRealConstant("vk_nokey", 0.0);
			AddRealConstant("vk_anykey", 1.0);
			AddRealConstant("vk_enter", 13.0);
			AddRealConstant("vk_return", 13.0);
			AddRealConstant("vk_shift", 16.0);
			AddRealConstant("vk_control", 17.0);
			AddRealConstant("vk_alt", 18.0);
			AddRealConstant("vk_escape", 27.0);
			AddRealConstant("vk_space", 32.0);
			AddRealConstant("vk_backspace", 8.0);
			AddRealConstant("vk_tab", 9.0);
			AddRealConstant("vk_pause", 19.0);
			AddRealConstant("vk_printscreen", 44.0);
			AddRealConstant("vk_left", 37.0);
			AddRealConstant("vk_right", 39.0);
			AddRealConstant("vk_up", 38.0);
			AddRealConstant("vk_down", 40.0);
			AddRealConstant("vk_home", 36.0);
			AddRealConstant("vk_end", 35.0);
			AddRealConstant("vk_delete", 46.0);
			AddRealConstant("vk_insert", 45.0);
			AddRealConstant("vk_pageup", 33.0);
			AddRealConstant("vk_pagedown", 34.0);
			AddRealConstant("vk_f1", 112.0);
			AddRealConstant("vk_f2", 113.0);
			AddRealConstant("vk_f3", 114.0);
			AddRealConstant("vk_f4", 115.0);
			AddRealConstant("vk_f5", 116.0);
			AddRealConstant("vk_f6", 117.0);
			AddRealConstant("vk_f7", 118.0);
			AddRealConstant("vk_f8", 119.0);
			AddRealConstant("vk_f9", 120.0);
			AddRealConstant("vk_f10", 121.0);
			AddRealConstant("vk_f11", 128.0);
			AddRealConstant("vk_f12", 129.0);
			AddRealConstant("vk_numpad0", 96.0);
			AddRealConstant("vk_numpad1", 97.0);
			AddRealConstant("vk_numpad2", 98.0);
			AddRealConstant("vk_numpad3", 99.0);
			AddRealConstant("vk_numpad4", 100.0);
			AddRealConstant("vk_numpad5", 101.0);
			AddRealConstant("vk_numpad6", 102.0);
			AddRealConstant("vk_numpad7", 103.0);
			AddRealConstant("vk_numpad8", 104.0);
			AddRealConstant("vk_numpad9", 105.0);
			AddRealConstant("vk_divide", 111.0);
			AddRealConstant("vk_multiply", 106.0);
			AddRealConstant("vk_subtract", 109.0);
			AddRealConstant("vk_add", 107.0);
			AddRealConstant("vk_decimal", 110.0);
			AddRealConstant("vk_lshift", 160.0);
			AddRealConstant("vk_lcontrol", 162.0);
			AddRealConstant("vk_lalt", 164.0);
			AddRealConstant("vk_rshift", 161.0);
			AddRealConstant("vk_rcontrol", 163.0);
			AddRealConstant("vk_ralt", 165.0);
			AddRealConstant("ev_create", 0.0);
			AddRealConstant("ev_destroy", 1.0);
			AddRealConstant("ev_step", 3.0);
			AddRealConstant("ev_alarm", 2.0);
			AddRealConstant("ev_keyboard", 5.0);
			AddRealConstant("ev_mouse", 6.0);
			AddRealConstant("ev_collision", 4.0);
			AddRealConstant("ev_other", 7.0);
			AddRealConstant("ev_draw", 8.0);
			AddRealConstant("ev_keypress", 9.0);
			AddRealConstant("ev_keyrelease", 10.0);
			AddRealConstant("ev_trigger", 11.0);
			AddRealConstant("ev_left_button", 0.0);
			AddRealConstant("ev_right_button", 1.0);
			AddRealConstant("ev_middle_button", 2.0);
			AddRealConstant("ev_no_button", 3.0);
			AddRealConstant("ev_left_press", 4.0);
			AddRealConstant("ev_right_press", 5.0);
			AddRealConstant("ev_middle_press", 6.0);
			AddRealConstant("ev_left_release", 7.0);
			AddRealConstant("ev_right_release", 8.0);
			AddRealConstant("ev_middle_release", 9.0);
			AddRealConstant("ev_mouse_enter", 10.0);
			AddRealConstant("ev_mouse_leave", 11.0);
			AddRealConstant("ev_global_press", 12.0);
			AddRealConstant("ev_global_release", 13.0);
			AddRealConstant("ev_joystick1_left", 16.0);
			AddRealConstant("ev_joystick1_right", 17.0);
			AddRealConstant("ev_joystick1_up", 18.0);
			AddRealConstant("ev_joystick1_down", 19.0);
			AddRealConstant("ev_joystick1_button1", 21.0);
			AddRealConstant("ev_joystick1_button2", 22.0);
			AddRealConstant("ev_joystick1_button3", 23.0);
			AddRealConstant("ev_joystick1_button4", 24.0);
			AddRealConstant("ev_joystick1_button5", 25.0);
			AddRealConstant("ev_joystick1_button6", 26.0);
			AddRealConstant("ev_joystick1_button7", 27.0);
			AddRealConstant("ev_joystick1_button8", 28.0);
			AddRealConstant("ev_joystick2_left", 31.0);
			AddRealConstant("ev_joystick2_right", 32.0);
			AddRealConstant("ev_joystick2_up", 33.0);
			AddRealConstant("ev_joystick2_down", 34.0);
			AddRealConstant("ev_joystick2_button1", 36.0);
			AddRealConstant("ev_joystick2_button2", 37.0);
			AddRealConstant("ev_joystick2_button3", 38.0);
			AddRealConstant("ev_joystick2_button4", 39.0);
			AddRealConstant("ev_joystick2_button5", 40.0);
			AddRealConstant("ev_joystick2_button6", 41.0);
			AddRealConstant("ev_joystick2_button7", 42.0);
			AddRealConstant("ev_joystick2_button8", 43.0);
			AddRealConstant("ev_global_left_button", 50.0);
			AddRealConstant("ev_global_right_button", 51.0);
			AddRealConstant("ev_global_middle_button", 52.0);
			AddRealConstant("ev_global_left_press", 53.0);
			AddRealConstant("ev_global_right_press", 54.0);
			AddRealConstant("ev_global_middle_press", 55.0);
			AddRealConstant("ev_global_left_release", 56.0);
			AddRealConstant("ev_global_right_release", 57.0);
			AddRealConstant("ev_global_middle_release", 58.0);
			AddRealConstant("ev_mouse_wheel_up", 60.0);
			AddRealConstant("ev_mouse_wheel_down", 61.0);
			AddRealConstant("ev_outside", 0.0);
			AddRealConstant("ev_boundary", 1.0);
			AddRealConstant("ev_game_start", 2.0);
			AddRealConstant("ev_game_end", 3.0);
			AddRealConstant("ev_room_start", 4.0);
			AddRealConstant("ev_room_end", 5.0);
			AddRealConstant("ev_no_more_lives", 6.0);
			AddRealConstant("ev_animation_end", 7.0);
			AddRealConstant("ev_end_of_path", 8.0);
			AddRealConstant("ev_no_more_health", 9.0);
			AddRealConstant("ev_user0", 10.0);
			AddRealConstant("ev_user1", 11.0);
			AddRealConstant("ev_user2", 12.0);
			AddRealConstant("ev_user3", 13.0);
			AddRealConstant("ev_user4", 14.0);
			AddRealConstant("ev_user5", 15.0);
			AddRealConstant("ev_user6", 16.0);
			AddRealConstant("ev_user7", 17.0);
			AddRealConstant("ev_user8", 18.0);
			AddRealConstant("ev_user9", 19.0);
			AddRealConstant("ev_user10", 20.0);
			AddRealConstant("ev_user11", 21.0);
			AddRealConstant("ev_user12", 22.0);
			AddRealConstant("ev_user13", 23.0);
			AddRealConstant("ev_user14", 24.0);
			AddRealConstant("ev_user15", 25.0);
			AddRealConstant("ev_close_button", 30.0);
			AddRealConstant("ev_step_normal", 0.0);
			AddRealConstant("ev_step_begin", 1.0);
			AddRealConstant("ev_step_end", 2.0);
			AddRealConstant("ty_real", 0.0);
			AddRealConstant("ty_string", 1.0);
			AddRealConstant("dll_cdecl", 0.0);
			AddRealConstant("dll_stdcall", 1.0);
			AddRealConstant("fa_readonly", 1.0);
			AddRealConstant("fa_hidden", 2.0);
			AddRealConstant("fa_sysfile", 4.0);
			AddRealConstant("fa_volumeid", 8.0);
			AddRealConstant("fa_directory", 16.0);
			AddRealConstant("fa_archive", 32.0);
			AddRealConstant("cr_default", 0.0);
			AddRealConstant("cr_none", -1.0);
			AddRealConstant("cr_arrow", -2.0);
			AddRealConstant("cr_arrrow", -3.0);
			AddRealConstant("cr_cross", -4.0);
			AddRealConstant("cr_beam", -5.0);
			AddRealConstant("cr_size_nesw", -6.0);
			AddRealConstant("cr_size_ns", -7.0);
			AddRealConstant("cr_size_nwse", -8.0);
			AddRealConstant("cr_size_we", -9.0);
			AddRealConstant("cr_uparrow", -10.0);
			AddRealConstant("cr_hourglass", -11.0);
			AddRealConstant("cr_drag", -12.0);
			AddRealConstant("cr_nodrop", -13.0);
			AddRealConstant("cr_hsplit", -14.0);
			AddRealConstant("cr_vsplit", -15.0);
			AddRealConstant("cr_multidrag", -16.0);
			AddRealConstant("cr_sqlwait", -17.0);
			AddRealConstant("cr_no", -18.0);
			AddRealConstant("cr_appstart", -19.0);
			AddRealConstant("cr_help", -20.0);
			AddRealConstant("cr_handpoint", -21.0);
			AddRealConstant("cr_size_all", -22.0);
			AddRealConstant("pt_shape_pixel", 0.0);
			AddRealConstant("pt_shape_disk", 1.0);
			AddRealConstant("pt_shape_square", 2.0);
			AddRealConstant("pt_shape_line", 3.0);
			AddRealConstant("pt_shape_star", 4.0);
			AddRealConstant("pt_shape_circle", 5.0);
			AddRealConstant("pt_shape_ring", 6.0);
			AddRealConstant("pt_shape_sphere", 7.0);
			AddRealConstant("pt_shape_flare", 8.0);
			AddRealConstant("pt_shape_spark", 9.0);
			AddRealConstant("pt_shape_explosion", 10.0);
			AddRealConstant("pt_shape_cloud", 11.0);
			AddRealConstant("pt_shape_smoke", 12.0);
			AddRealConstant("pt_shape_snow", 13.0);
			AddRealConstant("ps_distr_linear", 0.0);
			AddRealConstant("ps_distr_gaussian", 1.0);
			AddRealConstant("ps_distr_invgaussian", 2.0);
			AddRealConstant("ps_shape_rectangle", 0.0);
			AddRealConstant("ps_shape_ellipse", 1.0);
			AddRealConstant("ps_shape_diamond", 2.0);
			AddRealConstant("ps_shape_line", 3.0);
			AddRealConstant("ps_force_constant", 0.0);
			AddRealConstant("ps_force_linear", 1.0);
			AddRealConstant("ps_force_quadratic", 2.0);
			AddRealConstant("ps_deflect_vertical", 0.0);
			AddRealConstant("ps_deflect_horizontal", 1.0);
			AddRealConstant("ps_change_all", 0.0);
			AddRealConstant("ps_change_shape", 1.0);
			AddRealConstant("ps_change_motion", 2.0);
			AddRealConstant("ef_explosion", 0.0);
			AddRealConstant("ef_ring", 1.0);
			AddRealConstant("ef_ellipse", 2.0);
			AddRealConstant("ef_firework", 3.0);
			AddRealConstant("ef_smoke", 4.0);
			AddRealConstant("ef_smokeup", 5.0);
			AddRealConstant("ef_star", 6.0);
			AddRealConstant("ef_spark", 7.0);
			AddRealConstant("ef_flare", 8.0);
			AddRealConstant("ef_cloud", 9.0);
			AddRealConstant("ef_rain", 10.0);
			AddRealConstant("ef_snow", 11.0);
			AddRealConstant("display_landscape", 0.0);
			AddRealConstant("display_portrait", 1.0);
			AddRealConstant("os_win32", 0.0);
			AddRealConstant("os_windows", 0.0);
			AddRealConstant("os_maxos", 1.0);
			AddRealConstant("os_psp", 2.0);
			AddRealConstant("os_ios", 3.0);
			AddRealConstant("os_android", 4.0);
			AddRealConstant("os_symbian", 5.0);
			AddRealConstant("os_linux", 6.0);
			AddRealConstant("browser_not_a_browser", -1.0);
			AddRealConstant("browser_unknown", 0.0);
			AddRealConstant("browser_ie", 1.0);
			AddRealConstant("browser_firefox", 2.0);
			AddRealConstant("browser_chrome", 3.0);
			AddRealConstant("browser_safari", 4.0);
			AddRealConstant("browser_safari_mobile", 5.0);
			AddRealConstant("browser_opera", 6.0);
			AddRealConstant("device_ios_unknown", -1.0);
			AddRealConstant("device_ios_ipad", 0.0);
			AddRealConstant("device_ios_iphone", 1.0);
			AddRealConstant("device_ios_iphone_retina", 2.0);
			AddRealConstant("of_challenge_win", 0.0);
			AddRealConstant("of_challenge_lose", 1.0);
			AddRealConstant("of_challenge_tie", 2.0);
			AddRealConstant("leaderboard_type_number", 0.0);
			AddRealConstant("leaderboard_type_time_mins_secs", 1.0);
			AddRealConstant("phy_joint_anchor_1_x", 0.0);
			AddRealConstant("phy_joint_anchor_1_y", 1.0);
			AddRealConstant("phy_joint_anchor_2_x", 2.0);
			AddRealConstant("phy_joint_anchor_2_y", 3.0);
			AddRealConstant("phy_joint_reaction_force_x", 4.0);
			AddRealConstant("phy_joint_reaction_force_y", 5.0);
			AddRealConstant("phy_joint_reaction_torque", 6.0);
			AddRealConstant("phy_joint_motor_speed", 7.0);
			AddRealConstant("phy_joint_angle", 8.0);
			AddRealConstant("phy_joint_motor_torque", 9.0);
			AddRealConstant("phy_joint_max_motor_torque", 10.0);
			AddRealConstant("phy_joint_translation", 11.0);
			AddRealConstant("phy_joint_speed", 12.0);
			AddRealConstant("phy_joint_motor_force", 13.0);
			AddRealConstant("phy_joint_max_motor_force", 14.0);
			AddRealConstant("phy_joint_length_1", 15.0);
			AddRealConstant("phy_joint_length_2", 16.0);
			AddRealConstant("phy_debug_render_shapes", 1.0);
			AddRealConstant("phy_debug_render_joints", 2.0);
			AddRealConstant("phy_debug_render_coms", 4.0);
			AddRealConstant("phy_debug_render_aabb", 8.0);
			AddRealConstant("phy_debug_render_obb", 16.0);
			AddRealConstant("phy_debug_render_core_shapes", 32.0);
			AddRealConstant("phy_debug_render_collision_pairs", 64.0);
			Variable_BuiltIn_Add("argument_relative", true, false, false, null, null);
			Variable_BuiltIn_Add("argument_count", true, false, false, null, null);
			Variable_BuiltIn_Add("argument", true, true, true, null, null);
			Variable_BuiltIn_Add("argument0", true, true, true, null, null);
			Variable_BuiltIn_Add("argument1", true, true, true, null, null);
			Variable_BuiltIn_Add("argument2", true, true, true, null, null);
			Variable_BuiltIn_Add("argument3", true, true, true, null, null);
			Variable_BuiltIn_Add("argument4", true, true, true, null, null);
			Variable_BuiltIn_Add("argument5", true, true, true, null, null);
			Variable_BuiltIn_Add("argument6", true, true, true, null, null);
			Variable_BuiltIn_Add("argument7", true, true, true, null, null);
			Variable_BuiltIn_Add("argument8", true, true, true, null, null);
			Variable_BuiltIn_Add("argument9", true, true, true, null, null);
			Variable_BuiltIn_Add("argument10", true, true, true, null, null);
			Variable_BuiltIn_Add("argument11", true, true, true, null, null);
			Variable_BuiltIn_Add("argument12", true, true, true, null, null);
			Variable_BuiltIn_Add("argument13", true, true, true, null, null);
			Variable_BuiltIn_Add("argument14", true, true, true, null, null);
			Variable_BuiltIn_Add("argument15", true, true, true, null, null);
			Variable_BuiltIn_Add("debug_mode", true, true, true, null, null);
			Variable_BuiltIn_Add("room", true, true, true, "set_current_room", "get_current_room");
			Variable_BuiltIn_Add("room_first", true, false, false, null, null);
			Variable_BuiltIn_Add("room_last", true, false, false, null, null);
			Variable_BuiltIn_Add("transition_kind", true, true, true, null, null);
			Variable_BuiltIn_Add("transition_steps", true, true, true, null, null);
			Variable_BuiltIn_Add("score", true, true, true, null, null);
			Variable_BuiltIn_Add("lives", true, true, true, "set_lives_function", null);
			Variable_BuiltIn_Add("health", true, true, true, "set_health_function", null);
			Variable_BuiltIn_Add("game_id", true, false, false, null, null);
			Variable_BuiltIn_Add("working_directory", true, false, false, null, null);
			Variable_BuiltIn_Add("temp_directory", true, false, false, null, null);
			Variable_BuiltIn_Add("program_directory", true, false, false, null, null);
			Variable_BuiltIn_Add("instance_count", true, false, false, null, "get_instance_count");
			Variable_BuiltIn_Add("instance_id", true, false, false, null, null);
			Variable_BuiltIn_Add("room_width", true, false, false, "set_room_width", null);
			Variable_BuiltIn_Add("room_height", true, false, false, "set_room_height", null);
			Variable_BuiltIn_Add("room_caption", true, true, true, "set_room_caption", null);
			Variable_BuiltIn_Add("room_speed", true, true, true, "set_room_speed", null);
			Variable_BuiltIn_Add("room_persistent", true, true, true, "room_persistent", null);
			Variable_BuiltIn_Add("background_color", true, true, true, "setbackground_color", "getbackground_color");
			Variable_BuiltIn_Add("background_showcolor", true, true, true, "setbackground_showcolor", "getbackground_showcolor");
			Variable_BuiltIn_Array_Add("background_visible", true, true, true);
			Variable_BuiltIn_Array_Add("background_foreground", true, true, true);
			Variable_BuiltIn_Array_Add("background_index", true, true, true);
			Variable_BuiltIn_Array_Add("background_x", true, true, true);
			Variable_BuiltIn_Array_Add("background_y", true, true, true);
			Variable_BuiltIn_Array_Add("background_width", true, false, false);
			Variable_BuiltIn_Array_Add("background_height", true, false, false);
			Variable_BuiltIn_Array_Add("background_htiled", true, true, true);
			Variable_BuiltIn_Array_Add("background_vtiled", true, true, true);
			Variable_BuiltIn_Array_Add("background_xscale", true, true, true);
			Variable_BuiltIn_Array_Add("background_yscale", true, true, true);
			Variable_BuiltIn_Array_Add("background_hspeed", true, true, true);
			Variable_BuiltIn_Array_Add("background_vspeed", true, true, true);
			Variable_BuiltIn_Array_Add("background_blend", true, true, true);
			Variable_BuiltIn_Array_Add("background_alpha", true, true, true);
			Variable_BuiltIn_Add("view_enabled", true, true, true, "set_view_enable", "get_view_enable");
			Variable_BuiltIn_Add("view_current", true, false, false, null, null);
			Variable_BuiltIn_Add("view_visible", true, true, true, null, null);
			Variable_BuiltIn_Array_Add("view_xview", true, true, true);
			Variable_BuiltIn_Array_Add("view_yview", true, true, true);
			Variable_BuiltIn_Array_Add("view_wview", true, true, true);
			Variable_BuiltIn_Array_Add("view_hview", true, true, true);
			Variable_BuiltIn_Array_Add("view_xport", true, true, true);
			Variable_BuiltIn_Array_Add("view_yport", true, true, true);
			Variable_BuiltIn_Array_Add("view_wport", true, true, true);
			Variable_BuiltIn_Array_Add("view_hport", true, true, true);
			Variable_BuiltIn_Array_Add("view_angle", true, true, true);
			Variable_BuiltIn_Array_Add("view_hborder", true, true, true);
			Variable_BuiltIn_Array_Add("view_vborder", true, true, true);
			Variable_BuiltIn_Array_Add("view_hspeed", true, true, true);
			Variable_BuiltIn_Array_Add("view_vspeed", true, true, true);
			Variable_BuiltIn_Array_Add("view_object", true, true, true);
			Variable_BuiltIn_Array_Add("view_surface_id", true, true, true);
			Variable_BuiltIn_Add("mouse_x", true, false, false, null, null);
			Variable_BuiltIn_Add("mouse_y", true, false, false, null, null);
			Variable_BuiltIn_Add("mouse_button", true, true, true, null, null);
			Variable_BuiltIn_Add("mouse_lastbutton", true, true, true, null, null);
			Variable_BuiltIn_Add("keyboard_key", true, true, true, null, null);
			Variable_BuiltIn_Add("keyboard_lastkey", true, true, true, null, null);
			Variable_BuiltIn_Add("keyboard_lastchar", true, true, true, null, null);
			Variable_BuiltIn_Add("keyboard_string", true, true, true, null, null);
			Variable_BuiltIn_Add("cursor_sprite", true, true, true, null, null);
			Variable_BuiltIn_Add("show_score", true, true, true, null, null);
			Variable_BuiltIn_Add("show_lives", true, true, true, null, null);
			Variable_BuiltIn_Add("show_health", true, true, true, null, null);
			Variable_BuiltIn_Add("caption_score", true, true, true, null, null);
			Variable_BuiltIn_Add("caption_lives", true, true, true, null, null);
			Variable_BuiltIn_Add("caption_health", true, true, true, null, null);
			Variable_BuiltIn_Add("fps", true, false, false, null, null);
			Variable_BuiltIn_Add("current_time", true, false, false, null, null);
			Variable_BuiltIn_Add("current_year", true, false, false, null, null);
			Variable_BuiltIn_Add("current_month", true, false, false, null, null);
			Variable_BuiltIn_Add("current_day", true, false, false, null, null);
			Variable_BuiltIn_Add("current_weekday", true, false, false, null, null);
			Variable_BuiltIn_Add("current_hour", true, false, false, null, null);
			Variable_BuiltIn_Add("current_minute", true, false, false, null, null);
			Variable_BuiltIn_Add("current_second", true, false, false, null, null);
			Variable_BuiltIn_Add("event_type", true, false, false, null, null);
			Variable_BuiltIn_Add("event_number", true, false, false, null, null);
			Variable_BuiltIn_Add("event_object", true, false, false, null, null);
			Variable_BuiltIn_Add("event_action", true, false, false, null, null);
			Variable_BuiltIn_Add("secure_mode", true, false, false, null, null);
			Variable_BuiltIn_Add("error_occurred", true, true, true, null, null);
			Variable_BuiltIn_Add("error_last", true, true, true, null, null);
			Variable_BuiltIn_Add("gamemaker_registered", true, false, false, null, null);
			Variable_BuiltIn_Add("gamemaker_pro", true, false, false, null, null);
			Variable_BuiltIn_Add("os_type", true, false, false, null, "get_os_type");
			Variable_BuiltIn_Add("os_device", true, false, false, null, "get_os_device");
			Variable_BuiltIn_Add("os_browser", true, false, false, null, "get_os_browser");
			Variable_BuiltIn_Add("os_version", true, false, false, null, "get_os_version");
			Variable_BuiltIn_Add("browser_width", true, false, false, null, "get_browser_width");
			Variable_BuiltIn_Add("browser_height", true, false, false, null, "get_browser_height");
			Variable_BuiltIn_Add("async_load", true, false, false, null, "get_async_load");
			Variable_BuiltIn_Local_Add("x", true, true, true, "setx", null);
			Variable_BuiltIn_Local_Add("y", true, true, true, "sety", null);
			Variable_BuiltIn_Local_Add("xprevious", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("yprevious", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("xstart", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("ystart", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("hspeed", true, true, true, "sethspeed", null);
			Variable_BuiltIn_Local_Add("vspeed", true, true, true, "setvspeed", null);
			Variable_BuiltIn_Local_Add("direction", true, true, true, "setdirection", null);
			Variable_BuiltIn_Local_Add("speed", true, true, true, "setspeed", null);
			Variable_BuiltIn_Local_Add("friction", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("gravity", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("gravity_direction", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("object_index", true, false, false, null, null);
			Variable_BuiltIn_Local_Add("id", true, false, false, null, null);
			Variable_BuiltIn_Local_Add("alarm", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("solid", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("visible", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("persistent", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("depth", true, true, true, "setdepth", "getdepth");
			Variable_BuiltIn_Local_Add("bbox_left", true, false, false, null, "get_bbox_left");
			Variable_BuiltIn_Local_Add("bbox_right", true, false, false, null, "get_bbox_right");
			Variable_BuiltIn_Local_Add("bbox_top", true, false, false, null, "get_bbox_top");
			Variable_BuiltIn_Local_Add("bbox_bottom", true, false, false, null, "get_bbox_bottom");
			Variable_BuiltIn_Local_Add("sprite_index", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("image_index", true, true, true, "set_image_index", null);
			Variable_BuiltIn_Local_Add("image_single", true, true, true, "set_image_single", "get_image_single");
			Variable_BuiltIn_Local_Add("image_number", true, false, false, null, "get_image_number");
			Variable_BuiltIn_Local_Add("sprite_width", true, false, false, null, "get_sprite_width");
			Variable_BuiltIn_Local_Add("sprite_height", true, false, false, null, "get_sprite_height");
			Variable_BuiltIn_Local_Add("sprite_xoffset", true, false, false, null, "get_sprite_xoffset");
			Variable_BuiltIn_Local_Add("sprite_yoffset", true, false, false, null, "get_sprite_yoffset");
			Variable_BuiltIn_Local_Add("image_xscale", true, true, true, "setxscale", null);
			Variable_BuiltIn_Local_Add("image_yscale", true, true, true, "setyscale", null);
			Variable_BuiltIn_Local_Add("image_angle", true, true, true, "setangle", null);
			Variable_BuiltIn_Local_Add("image_alpha", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("image_blend", true, true, true, "set_imageblend", "get_imageblend");
			Variable_BuiltIn_Local_Add("image_speed", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("mask_index", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("path_index", true, false, false, null, null);
			Variable_BuiltIn_Local_Add("path_position", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("path_positionprevious", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("path_speed", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("path_scale", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("path_orientation", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("path_endaction", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("timeline_index", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("timeline_position", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("timeline_speed", true, true, true, null, null);
			Variable_BuiltIn_Local_Add("timeline_running", true, true, true, "set_timeline_running", "get_timeline_running");
			Variable_BuiltIn_Local_Add("timeline_loop", true, true, true, "set_timeline_loop", "get_timeline_loop");
			Variable_BuiltIn_Local_Add("phy_mass", true, false, true, null, null);
			Variable_BuiltIn_Local_Add("phy_inertia", true, false, true, null, null);
			Variable_BuiltIn_Local_Add("phy_com_x", true, false, true, null, null);
			Variable_BuiltIn_Local_Add("phy_com_y", true, false, true, null, null);
			Variable_BuiltIn_Local_Add("phy_dynamic", true, false, true, null, null);
			Variable_BuiltIn_Local_Add("phy_frozen", true, false, true, null, null);
			Variable_BuiltIn_Local_Add("phy_sleeping", true, false, true, null, null);
			Variable_BuiltIn_Local_Add("phy_collision_points", true, false, true, null, null);
			Variable_BuiltIn_Local_Array_Add("phy_collision_x", true, false, true);
			Variable_BuiltIn_Local_Array_Add("phy_collision_y", true, false, true);
			Variable_BuiltIn_Local_Array_Add("phy_col_normal_x", true, false, true);
			Variable_BuiltIn_Local_Array_Add("phy_col_normal_y", true, false, true);
		}

		public static int Find<T>(IList<KeyValuePair<string, T>> _list, string _name)
		{
			int i;
			for (i = 0; i < _list.Count && !(_list[i].Key == _name); i++)
			{
			}
			if (i >= _list.Count)
			{
				return -1;
			}
			return i;
		}

		public static int FindTriggerConstName(IList<GMTrigger> _list, string _name)
		{
			int result = -1;
			for (int i = 0; i < _list.Count; i++)
			{
				if (_list[i].ConstName == _name)
				{
					result = i + 1;
					break;
				}
			}
			return result;
		}

		private static int Code_Function_Find(string _name)
		{
			if (ms_assets != null)
			{
				int num = 0;
				foreach (GMExtension extension in ms_assets.Extensions)
				{
					int num2 = 0;
					foreach (GMExtensionInclude include in extension.Includes)
					{
						int num3 = 0;
						foreach (GMExtensionFunction function in include.Functions)
						{
							if (function.Name == _name)
							{
								return ms_id + num * 256 + num2 * 4096 + num3;
							}
							num3++;
						}
						num2++;
					}
					num++;
				}
				int num4 = Find(ms_assets.Scripts, _name);
				if (num4 >= 0)
				{
					return num4 + 100000;
				}
			}
			GMLFunction value = null;
			if (!ms_funcs.TryGetValue(_name, out value))
			{
				return -1;
			}
			return value.Id;
		}

		private static int FindResourceIndexFromName(string _name)
		{
			int result = -1;
			if (ms_assets != null)
			{
				int num = Find(ms_assets.Objects, _name);
				if (num >= 0)
				{
					return num;
				}
				num = Find(ms_assets.Sprites, _name);
				if (num >= 0)
				{
					return num;
				}
				num = Find(ms_assets.Sounds, _name);
				if (num >= 0)
				{
					return num;
				}
				num = Find(ms_assets.Backgrounds, _name);
				if (num >= 0)
				{
					return num;
				}
				num = Find(ms_assets.Paths, _name);
				if (num >= 0)
				{
					return num;
				}
				num = Find(ms_assets.Fonts, _name);
				if (num >= 0)
				{
					return num;
				}
				num = Find(ms_assets.TimeLines, _name);
				if (num >= 0)
				{
					return num;
				}
				num = Find(ms_assets.Scripts, _name);
				if (num >= 0)
				{
					return num;
				}
				num = Find(ms_assets.Rooms, _name);
				if (num >= 0)
				{
					return num;
				}
				num = FindTriggerConstName(ms_assets.Triggers, _name);
				if (num >= 0)
				{
					return num;
				}
			}
			return result;
		}

		private static bool Code_Constant_Find(string _name, out GMLValue _val)
		{
			_val = new GMLValue();
			_val.Kind = eKind.eNumber;
			int value = 0;
			ms_ConstantCount.TryGetValue(_name, out value);
			ms_ConstantCount[_name] = value + 1;
			int num = FindResourceIndexFromName(_name);
			if (num >= 0)
			{
				_val.ValueI = num;
				return true;
			}
			string value2;
			if (ms_assets.Options.Constants.TryGetValue(_name, out value2))
			{
				double result = 0.0;
				if (double.TryParse(value2, out result))
				{
					_val.ValueI = result;
					return true;
				}
				num = FindResourceIndexFromName(value2);
				if (num >= 0)
				{
					_val.ValueI = num;
					return true;
				}
				_val.Kind = eKind.eConstant;
				_val.ValueS = _name;
				return true;
			}
			double value3 = 0.0;
			if (!ms_constants.TryGetValue(_name, out value3))
			{
				return false;
			}
			_val.ValueI = value3;
			return true;
		}

		private static int Code_Variable_Find(string _name)
		{
			GMLVariable value = null;
			if (!ms_builtins.TryGetValue(_name, out value) && !ms_builtinsLocal.TryGetValue(_name, out value) && !ms_vars.TryGetValue(_name, out value))
			{
				value = new GMLVariable();
				value.Name = _name;
				value.Id = 100000 + ms_vars.Count;
				ms_vars.Add(_name, value);
			}
			return value.Id;
		}

		private static void CreateFunctionsToken(string _script, List<GMLToken> _pass1, List<GMLToken> _pass2, int _index)
		{
			int num = Code_Function_Find(_pass1[_index].Text);
			if (num < 0)
			{
				Error(string.Format("unknown function or script {0}", _pass1[_index].Text), _script, _pass1[_index]);
			}
			_pass2.Add(new GMLToken(eToken.eFunction, _pass1[_index], num));
		}

		private static void CreateNameToken(string _script, List<GMLToken> _pass1, List<GMLToken> _pass2, int _index)
		{
			GMLValue _val = null;
			if (!Code_Constant_Find(_pass1[_index].Text, out _val))
			{
				int id = Code_Variable_Find(_pass1[_index].Text);
				_pass2.Add(new GMLToken(eToken.eVariable, _pass1[_index], id));
			}
			else
			{
				_pass2.Add(new GMLToken(eToken.eConstant, _pass1[_index], 0, _val));
			}
		}

		private static void CreateValueToken(string _script, List<GMLToken> _pass1, List<GMLToken> _pass2, int _index)
		{
			string text = _pass1[_index].Text;
			GMLValue gMLValue = null;
			if (text[0] == '$')
			{
				long num = Convert.ToInt64(text.Substring(1), 16);
				gMLValue = new GMLValue(num);
			}
			else
			{
				double result = 0.0;
				if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
				{
					Error(string.Format("Number {0} in incorrect format", text), _script, _pass1[_index]);
				}
				gMLValue = new GMLValue(result);
			}
			_pass2.Add(new GMLToken(eToken.eConstant, _pass1[_index], 0, gMLValue));
		}

		private static void CreateStringToken(string _script, List<GMLToken> _pass1, List<GMLToken> _pass2, int _index)
		{
			_pass2.Add(new GMLToken(eToken.eConstant, _pass1[_index], 0, new GMLValue(_pass1[_index].Text)));
		}

		private static void CreateNormalToken(string _script, List<GMLToken> _pass1, List<GMLToken> _pass2, int _index)
		{
			_pass2.Add(new GMLToken(_pass1[_index].Token, _pass1[_index], 0));
		}

		private static int ParseStatement(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int i = _index;
			switch (_pass2[i].Token)
			{
			case eToken.eEOF:
				Error("unexpected EOF encountered", ms_script, _pass2[i]);
				break;
			case eToken.eVar:
				i = ParseVar(_pass3, _pass2, _index);
				break;
			case eToken.eGlobalVar:
				i = ParseGlobalVar(_pass3, _pass2, _index);
				break;
			case eToken.eBegin:
				i = ParseBlock(_pass3, _pass2, _index);
				break;
			case eToken.eRepeat:
				i = ParseRepeat(_pass3, _pass2, _index);
				break;
			case eToken.eIf:
				i = ParseIf(_pass3, _pass2, _index);
				break;
			case eToken.eWhile:
				i = ParseWhile(_pass3, _pass2, _index);
				break;
			case eToken.eFor:
				i = ParseFor(_pass3, _pass2, _index);
				break;
			case eToken.eDo:
				i = ParseDo(_pass3, _pass2, _index);
				break;
			case eToken.eWith:
				i = ParseWith(_pass3, _pass2, _index);
				break;
			case eToken.eSwitch:
				i = ParseSwitch(_pass3, _pass2, _index);
				break;
			case eToken.eCase:
				i = ParseCase(_pass3, _pass2, _index);
				break;
			case eToken.eDefault:
				i = ParseDefault(_pass3, _pass2, _index);
				break;
			case eToken.eReturn:
				i = ParseReturn(_pass3, _pass2, _index);
				break;
			case eToken.eFunction:
				i = ParseFunction(_pass3, _pass2, _index);
				break;
			case eToken.eExit:
			case eToken.eBreak:
			case eToken.eContinue:
				_pass3.Add(new GMLToken(_pass2[_index].Token, _pass2[_index], _pass2[_index].Id, _pass2[_index].Value));
				i++;
				break;
			default:
				i = ParseAssignment(_pass3, _pass2, _index);
				break;
			case eToken.eSepStatement:
				break;
			}
			for (; _pass2[i].Token == eToken.eSepStatement; i++)
			{
			}
			return i;
		}

		private static int ParseVar(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eVar, _pass2[_index], 0);
			int num = _index + 1;
			while (_pass2[num].Token == eToken.eVariable)
			{
				if (_pass2[num].Id < 100000)
				{
					Error("cannot redeclare a builtin varable", ms_script, _pass2[num]);
				}
				GMLToken gMLToken2 = new GMLToken(_pass2[num]);
				gMLToken2.Token = eToken.eConstant;
				gMLToken.Children.Add(gMLToken2);
				num++;
				if (_pass2[num].Token == eToken.eSepArgument)
				{
					num++;
				}
			}
			_pass3.Add(gMLToken);
			return num;
		}

		private static int ParseGlobalVar(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eGlobalVar, _pass2[_index], 0);
			int num = _index + 1;
			while (_pass2[num].Token == eToken.eVariable)
			{
				if (_pass2[num].Id < 100000)
				{
					Error("cannot redeclare a builtin varable", ms_script, _pass2[num]);
				}
				GMLToken gMLToken2 = new GMLToken(_pass2[num]);
				gMLToken2.Token = eToken.eConstant;
				gMLToken.Children.Add(gMLToken2);
				num++;
				GML2JavaScript.ms_globals[gMLToken2.Text] = gMLToken2.Text;
				if (_pass2[num].Token == eToken.eSepArgument)
				{
					num++;
				}
			}
			_pass3.Add(gMLToken);
			return num;
		}

		private static int ParseBlock(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int num = _index + 1;
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			while (!ms_error && _pass2[num].Token != eToken.eEOF && _pass2[num].Token != eToken.eEnd)
			{
				num = ParseStatement(gMLToken.Children, _pass2, num);
			}
			if (_pass2[num].Token != eToken.eEnd)
			{
				Error("symbol } expected", ms_script, _pass2[num]);
			}
			else
			{
				num++;
			}
			return num;
		}

		private static int ParseRepeat(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int index = _index + 1;
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			index = ParseExpression1(gMLToken.Children, _pass2, index);
			return ParseStatement(gMLToken.Children, _pass2, index);
		}

		private static int ParseIf(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			int num = ParseExpression1(gMLToken.Children, _pass2, _index + 1);
			if (_pass2[num].Token == eToken.eThen)
			{
				num++;
			}
			num = ParseStatement(gMLToken.Children, _pass2, num);
			if (_pass2[num].Token == eToken.eElse)
			{
				num = ParseStatement(gMLToken.Children, _pass2, num + 1);
			}
			return num;
		}

		private static int ParseWhile(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int index = _index + 1;
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			index = ParseExpression1(gMLToken.Children, _pass2, index);
			if (_pass2[index].Token == eToken.eDo)
			{
				index++;
			}
			return ParseStatement(gMLToken.Children, _pass2, index);
		}

		private static int ParseFor(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int num = _index + 1;
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			if (_pass2[num].Token != eToken.eOpen)
			{
				Error("symbol ( expected", ms_script, _pass2[num]);
			}
			num++;
			num = ParseStatement(gMLToken.Children, _pass2, num);
			if (_pass2[num].Token == eToken.eSepStatement)
			{
				num++;
			}
			num = ParseExpression1(gMLToken.Children, _pass2, num);
			if (_pass2[num].Token == eToken.eSepStatement)
			{
				num++;
			}
			num = ParseStatement(gMLToken.Children, _pass2, num);
			if (_pass2[num].Token != eToken.eClose)
			{
				Error("Symbol ) expected", ms_script, _pass2[num]);
			}
			num++;
			return ParseStatement(gMLToken.Children, _pass2, num);
		}

		private static int ParseDo(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int index = _index + 1;
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			index = ParseStatement(gMLToken.Children, _pass2, index);
			if (_pass2[index].Token != eToken.eUntil)
			{
				Error("keyword Until expected", ms_script, _pass2[index]);
			}
			return ParseExpression1(gMLToken.Children, _pass2, index + 1);
		}

		private static int ParseWith(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int index = _index + 1;
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			index = ParseExpression1(gMLToken.Children, _pass2, index);
			if (_pass2[index].Token == eToken.eDo)
			{
				index++;
			}
			return ParseStatement(gMLToken.Children, _pass2, index);
		}

		private static int ParseSwitch(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int index = _index + 1;
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			index = ParseExpression1(gMLToken.Children, _pass2, index);
			if (_pass2[index].Token != eToken.eBegin)
			{
				Error("Symbol { expected", ms_script, _pass2[index]);
			}
			index++;
			while (_pass2[index].Token != eToken.eEnd && _pass2[index].Token != eToken.eEOF)
			{
				index = ParseStatement(gMLToken.Children, _pass2, index);
			}
			if (_pass2[index].Token != eToken.eEnd)
			{
				Error("Symbol } expected", ms_script, _pass2[index]);
			}
			return index + 1;
		}

		private static int ParseCase(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			int num = ParseExpression1(gMLToken.Children, _pass2, _index + 1);
			if (_pass2[num].Token != eToken.eLabel)
			{
				Error("Symbol : expected", ms_script, _pass2[num]);
			}
			_pass3.Add(gMLToken);
			return num + 1;
		}

		private static int ParseDefault(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int num = _index + 1;
			GMLToken item = new GMLToken(_pass2[_index]);
			if (_pass2[num].Token != eToken.eLabel)
			{
				Error("Symbol : expected", ms_script, _pass2[num]);
			}
			_pass3.Add(item);
			return num + 1;
		}

		private static int ParseReturn(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eReturn, _pass2[_index], 0);
			int index = _index + 1;
			index = ParseExpression1(gMLToken.Children, _pass2, index);
			_pass3.Add(gMLToken);
			return index;
		}

		private static int ParseFunction(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			if (_pass2[_index].Token != eToken.eFunction)
			{
				Error("Function name expected", ms_script, _pass2[_index]);
			}
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			int num = _index + 1;
			if (_pass2[num].Token != eToken.eOpen)
			{
				Error("Symbol ( expected", ms_script, _pass2[num]);
			}
			num++;
			while (!ms_error && _pass2[num].Token != eToken.eEOF && _pass2[num].Token != eToken.eClose)
			{
				num = ParseExpression1(gMLToken.Children, _pass2, num);
				if (_pass2[num].Token == eToken.eSepArgument)
				{
					num++;
				}
				else if (_pass2[num].Token != eToken.eClose)
				{
					Error("Symbol , or ) expected", ms_script, _pass2[num]);
				}
			}
			if (_pass2[num].Token != eToken.eClose)
			{
				Error("Symbol ) expected", ms_script, _pass2[num]);
			}
			else
			{
				num++;
			}
			return num;
		}

		private static int ParseAssignment(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eAssign, _pass2[_index], 0);
			_pass3.Add(gMLToken);
			int num = ParseVariable2(gMLToken.Children, _pass2, _index);
			switch (_pass2[num].Token)
			{
			case eToken.eAssign:
			case eToken.eAssignPlus:
			case eToken.eAssignMinus:
			case eToken.eAssignTimes:
			case eToken.eAssignDivide:
			case eToken.eAssignOr:
			case eToken.eAssignAnd:
			case eToken.eAssignXor:
				gMLToken.Children.Add(_pass2[num]);
				num = ParseExpression1(gMLToken.Children, _pass2, num + 1);
				break;
			default:
				Error("Assignment operator expected", ms_script, _pass2[num]);
				break;
			}
			return num;
		}

		private static int ParseExpression1(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eBinary, _pass2[_index], _pass2[_index].Id, _pass2[_index].Value);
			int num = ParseExpression2(gMLToken.Children, _pass2, _index);
			if (!ms_error)
			{
				bool flag = true;
				while (_pass2[num].Token == eToken.eAnd || _pass2[num].Token == eToken.eOr || _pass2[num].Token == eToken.eXor)
				{
					flag = false;
					gMLToken.Children.Add(_pass2[num]);
					num = ParseExpression2(gMLToken.Children, _pass2, num + 1);
				}
				if (flag)
				{
					_pass3.AddRange(gMLToken.Children);
				}
				else
				{
					_pass3.Add(gMLToken);
				}
			}
			return num;
		}

		private static int ParseExpression2(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eBinary, _pass2[_index], _pass2[_index].Id, _pass2[_index].Value);
			int num = ParseExpression3(gMLToken.Children, _pass2, _index);
			if (!ms_error)
			{
				bool flag = true;
				while (_pass2[num].Token == eToken.eLess || _pass2[num].Token == eToken.eLessEqual || _pass2[num].Token == eToken.eEqual || _pass2[num].Token == eToken.eNotEqual || _pass2[num].Token == eToken.eAssign || _pass2[num].Token == eToken.eGreater || _pass2[num].Token == eToken.eGreaterEqual)
				{
					flag = false;
					gMLToken.Children.Add(_pass2[num]);
					num = ParseExpression3(gMLToken.Children, _pass2, num + 1);
				}
				if (flag)
				{
					_pass3.AddRange(gMLToken.Children);
				}
				else
				{
					_pass3.Add(gMLToken);
				}
			}
			return num;
		}

		private static int ParseExpression3(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eBinary, _pass2[_index], _pass2[_index].Id, _pass2[_index].Value);
			int num = ParseExpression4(gMLToken.Children, _pass2, _index);
			if (!ms_error)
			{
				bool flag = true;
				while (_pass2[num].Token == eToken.eBitOr || _pass2[num].Token == eToken.eBitAnd || _pass2[num].Token == eToken.eBitXor)
				{
					flag = false;
					gMLToken.Children.Add(_pass2[num]);
					num = ParseExpression4(gMLToken.Children, _pass2, num + 1);
				}
				if (flag)
				{
					_pass3.AddRange(gMLToken.Children);
				}
				else
				{
					_pass3.Add(gMLToken);
				}
			}
			return num;
		}

		private static int ParseExpression4(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eBinary, _pass2[_index], _pass2[_index].Id, _pass2[_index].Value);
			int num = ParseExpression5(gMLToken.Children, _pass2, _index);
			if (!ms_error)
			{
				bool flag = true;
				while (_pass2[num].Token == eToken.eBitShiftLeft || _pass2[num].Token == eToken.eBitShiftRight)
				{
					flag = false;
					gMLToken.Children.Add(_pass2[num]);
					num = ParseExpression5(gMLToken.Children, _pass2, num + 1);
				}
				if (flag)
				{
					_pass3.AddRange(gMLToken.Children);
				}
				else
				{
					_pass3.Add(gMLToken);
				}
			}
			return num;
		}

		private static int ParseExpression5(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eBinary, _pass2[_index], _pass2[_index].Id, _pass2[_index].Value);
			int num = ParseExpression6(gMLToken.Children, _pass2, _index);
			if (!ms_error)
			{
				bool flag = true;
				while (_pass2[num].Token == eToken.ePlus || _pass2[num].Token == eToken.eMinus)
				{
					flag = false;
					gMLToken.Children.Add(_pass2[num]);
					num = ParseExpression6(gMLToken.Children, _pass2, num + 1);
				}
				if (flag)
				{
					_pass3.AddRange(gMLToken.Children);
				}
				else
				{
					_pass3.Add(gMLToken);
				}
			}
			return num;
		}

		private static int ParseExpression6(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			GMLToken gMLToken = new GMLToken(eToken.eBinary, _pass2[_index], _pass2[_index].Id, _pass2[_index].Value);
			int num = ParseVariable2(gMLToken.Children, _pass2, _index);
			if (!ms_error)
			{
				bool flag = true;
				while (_pass2[num].Token == eToken.eTime || _pass2[num].Token == eToken.eDivide || _pass2[num].Token == eToken.eDiv || _pass2[num].Token == eToken.eMod)
				{
					flag = false;
					gMLToken.Children.Add(_pass2[num]);
					num = ParseVariable2(gMLToken.Children, _pass2, num + 1);
				}
				if (flag)
				{
					_pass3.AddRange(gMLToken.Children);
				}
				else
				{
					_pass3.Add(gMLToken);
				}
			}
			return num;
		}

		private static int ParseVariable2(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			List<GMLToken> list = new List<GMLToken>();
			int num = ParseTerm(list, _pass2, _index);
			if (!ms_error)
			{
				if (_pass2[num].Token == eToken.eDot)
				{
					GMLToken gMLToken = new GMLToken(eToken.eDot, _pass2[num], 0);
					gMLToken.Children.AddRange(list);
					_pass3.Add(gMLToken);
					while (_pass2[num].Token == eToken.eDot)
					{
						num = ParseVariable(gMLToken.Children, _pass2, num + 1);
					}
				}
				else
				{
					_pass3.AddRange(list);
				}
			}
			return num;
		}

		private static int ParseTerm(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			int num = _index;
			switch (_pass2[num].Token)
			{
			case eToken.eFunction:
				num = ParseFunction(_pass3, _pass2, num);
				break;
			case eToken.eConstant:
				_pass3.Add(_pass2[num]);
				num++;
				break;
			case eToken.eOpen:
				num = ParseExpression1(_pass3, _pass2, num + 1);
				if (_pass2[num].Token != eToken.eClose)
				{
					Error("Symbol ) expected", ms_script, _pass2[num]);
				}
				num++;
				break;
			case eToken.eVariable:
				num = ParseVariable(_pass3, _pass2, num);
				break;
			case eToken.eNot:
			case eToken.ePlus:
			case eToken.eMinus:
			case eToken.eBitNegate:
			{
				GMLToken gMLToken = new GMLToken(eToken.eUnary, _pass2[num], (int)_pass2[num].Token);
				num = ParseVariable2(gMLToken.Children, _pass2, num + 1);
				_pass3.Add(gMLToken);
				break;
			}
			default:
				Error("unexpected symbol in expression", ms_script, _pass2[num]);
				break;
			}
			return num;
		}

		private static int ParseVariable(List<GMLToken> _pass3, List<GMLToken> _pass2, int _index)
		{
			if (_pass2[_index].Token != eToken.eVariable)
			{
				Error("variable name expected", ms_script, _pass2[_index]);
			}
			GMLToken gMLToken = new GMLToken(_pass2[_index]);
			_pass3.Add(gMLToken);
			int num = _index + 1;
			GMLVariable value;
			if (_pass2[num].Token == eToken.eArrayOpen)
			{
				num++;
				while (_pass2[num].Token != eToken.eArrayClose && _pass2[num].Token != eToken.eEOF)
				{
					num = ParseExpression1(gMLToken.Children, _pass2, num);
					if (_pass2[num].Token == eToken.eSepArgument)
					{
						num++;
					}
					else if (_pass2[num].Token != eToken.eArrayClose)
					{
						Error("symbol , or ] expected", ms_script, _pass2[num]);
					}
				}
				if (_pass2[num].Token == eToken.eEOF)
				{
					Error("symbol ] expected", ms_script, _pass2[num]);
				}
				num++;
				if (gMLToken.Children.Count >= 3)
				{
					Error("only 1 or 2 dimensional arrays are supported", ms_script, _pass2[num]);
				}
			}
			else if (ms_builtinsArray.TryGetValue(gMLToken.Text, out value) || ms_builtinsLocalArray.TryGetValue(gMLToken.Text, out value))
			{
				GMLToken gMLToken2 = new GMLToken(eToken.eConstant, -1, "0");
				gMLToken2.Value = new GMLValue(0.0);
				gMLToken.Children.Add(gMLToken2);
			}
			return num;
		}

		public static GMLToken Compile(GMAssets _assets, string _name, string _script, out List<GMLError> _errors)
		{
			_errors = new List<GMLError>();
			ms_error = false;
			ms_numErrors = 0;
			ms_errors = _errors;
			ms_assets = _assets;
			ms_script = _script;
			ms_scriptName = _name;
			if (Program.CompileVerbose)
			{
				Console.WriteLine("Original GML script - \"{0}\"", _name);
				Console.WriteLine(_script);
			}
			List<GMLToken> list = new List<GMLToken>();
			int _index = 0;
			bool flag = false;
			while (!flag)
			{
				GMLToken gMLToken = NextToken(_script, ref _index);
				list.Add(gMLToken);
				flag = (gMLToken.Token == eToken.eEOF);
			}
			if (Program.CompileVerbose)
			{
				Console.WriteLine("-----\nPass 1\n-----");
				foreach (GMLToken item in list)
				{
					Console.WriteLine(item);
				}
			}
			List<GMLToken> list2 = new List<GMLToken>();
			if (!ms_error)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Token == eToken.eName && list[i + 1].Token == eToken.eOpen)
					{
						CreateFunctionsToken(_script, list, list2, i);
					}
					else if (list[i].Token == eToken.eName)
					{
						CreateNameToken(_script, list, list2, i);
					}
					else if (list[i].Token == eToken.eNumber)
					{
						CreateValueToken(_script, list, list2, i);
					}
					else if (list[i].Token == eToken.eString)
					{
						CreateStringToken(_script, list, list2, i);
					}
					else
					{
						CreateNormalToken(_script, list, list2, i);
					}
				}
				if (Program.CompileVerbose)
				{
					Console.WriteLine("-----\nPass 2\n-----");
					foreach (GMLToken item2 in list2)
					{
						Console.WriteLine(item2);
					}
				}
			}
			List<GMLToken> list3 = new List<GMLToken>();
			if (!ms_error)
			{
				int index = 0;
				while (!ms_error && list2[index].Token != eToken.eEOF)
				{
					index = ParseStatement(list3, list2, index);
				}
				if (Program.CompileVerbose)
				{
					Console.WriteLine("-----\nPass 3\n-----");
					foreach (GMLToken item3 in list3)
					{
						Console.WriteLine(item3);
					}
				}
			}
			GMLToken gMLToken2 = new GMLToken(eToken.eBlock, 0, "");
			gMLToken2.Children = list3;
			return gMLToken2;
		}
	}
}
