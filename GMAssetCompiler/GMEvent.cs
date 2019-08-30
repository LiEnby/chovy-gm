using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GMAssetCompiler
{
	public class GMEvent
	{
		private static Stack<bool> RelativeStack = new Stack<bool>();

		private static bool conditionVar = false;

		public List<GMAction> Actions
		{
			get;
			set;
		}

		public GMEvent(GMAssets _a, Stream _stream)
		{
			_stream.ReadInteger();
			int num = _stream.ReadInteger();
			Actions = new List<GMAction>(num);
			for (int i = 0; i < num; i++)
			{
				Actions.Add(new GMAction(_a, _stream));
			}
		}

		private int CompressExit(int n, StringBuilder _sb)
		{
			if (RelativeStack.Count > 0)
			{
				_sb.AppendLine("action_set_relative( 0 );");
			}
			_sb.AppendLine("exit;");
			return n + 1;
		}

		private int CompressBlock(int n, StringBuilder _sb)
		{
			_sb.AppendLine("{");
			for (n++; n < Actions.Count; n = CompressAction(n, _sb))
			{
				GMAction gMAction = Actions[n];
				if (gMAction.Kind == eAction.ACT_END)
				{
					n++;
					break;
				}
			}
			_sb.AppendLine("}");
			return n;
		}

		private int CompressRepeat(int n, StringBuilder _sb)
		{
			GMAction gMAction = Actions[n];
			_sb.AppendFormat("repeat( {0} )", gMAction.Args[0]);
			return CompressAction(n + 1, _sb);
		}

		private int CompressCondition(int n, StringBuilder _sb)
		{
			GMAction gMAction = Actions[n];
			bool flag = false;
			bool flag2 = false;
			if (gMAction.UseApplyTo)
			{
				switch (gMAction.Who)
				{
				case -2:
					_sb.AppendLine("with( other ) ");
					flag = true;
					break;
				default:
					_sb.AppendLine(string.Format("with( {0} ) ", gMAction.Who));
					flag = true;
					flag2 = true;
					break;
				case -1:
					break;
				}
			}
			flag = (flag || (gMAction.UseRelative && RelativeStack.Count > 0 && RelativeStack.Peek() != gMAction.Relative));
			if (flag)
			{
				_sb.AppendLine("{");
			}
			if (!conditionVar)
			{
				_sb.AppendLine("var __b__;");
				conditionVar = true;
			}
			bool flag3 = PushRelativeness(_sb, gMAction);
			switch (gMAction.ExeType)
			{
			case eExecuteTypes.EXE_FUNCTION:
				if (gMAction.Name == "action_execute_script")
				{
					_sb.AppendFormat("__b__ = ");
					CompressExecuteScriptAction(n, gMAction, _sb);
				}
				else
				{
					_sb.AppendFormat("__b__ = {0}( ", gMAction.Name);
					CompressFunctionArguments(_sb, gMAction);
					_sb.AppendLine(" );");
				}
				break;
			case eExecuteTypes.EXE_CODE:
				_sb.AppendFormat("__b__ = {0};", gMAction.Code);
				_sb.AppendLine("");
				break;
			}
			if (flag2)
			{
				_sb.AppendFormat("if {0}__b__ break;", gMAction.IsNot ? "!" : "");
				_sb.AppendLine("");
			}
			if (flag3)
			{
				PopRelativeness(_sb);
			}
			if (flag)
			{
				_sb.AppendLine("}");
			}
			if (gMAction.IsNot)
			{
				_sb.AppendLine("if !__b__");
			}
			else
			{
				_sb.AppendLine("if __b__");
			}
			_sb.AppendLine("{");
			n = CompressAction(n + 1, _sb);
			_sb.AppendLine("}");
			if (n < Actions.Count && Actions[n].Kind == eAction.ACT_ELSE)
			{
				_sb.AppendLine("else");
				_sb.AppendLine("{");
				n = CompressAction(n + 1, _sb);
				_sb.AppendLine("}");
			}
			return n;
		}

		private static void PopRelativeness(StringBuilder _sb)
		{
			bool flag = RelativeStack.Pop();
			if (RelativeStack.Peek() != flag)
			{
				_sb.AppendLine(string.Format("action_set_relative( {0} );", RelativeStack.Peek() ? 1 : 0));
			}
		}

		private static bool PushRelativeness(StringBuilder _sb, GMAction action)
		{
			bool result = false;
			if (action.UseRelative && RelativeStack.Count > 0 && RelativeStack.Peek() != action.Relative)
			{
				_sb.AppendLine(string.Format("action_set_relative( {0} );", action.Relative ? 1 : 0));
				RelativeStack.Push(action.Relative);
				result = true;
			}
			return result;
		}

		private int CompressNormalAction(int n, StringBuilder _sb)
		{
			GMAction gMAction = Actions[n];
			if (gMAction.ExeType != 0)
			{
				bool flag = false;
				if (gMAction.UseApplyTo)
				{
					switch (gMAction.Who)
					{
					case -2:
						_sb.AppendLine("with( other ) ");
						flag = true;
						break;
					default:
						_sb.AppendLine(string.Format("with( {0} ) ", gMAction.Who));
						flag = true;
						break;
					case -1:
						break;
					}
				}
				flag = (flag || (gMAction.UseRelative && RelativeStack.Count > 0 && RelativeStack.Peek() != gMAction.Relative));
				if (flag)
				{
					_sb.AppendLine("{");
				}
				bool flag2 = PushRelativeness(_sb, gMAction);
				switch (gMAction.ExeType)
				{
				case eExecuteTypes.EXE_FUNCTION:
					if (gMAction.Name == "action_execute_script")
					{
						CompressExecuteScriptAction(n, gMAction, _sb);
						break;
					}
					_sb.AppendFormat("{0}( ", gMAction.Name);
					CompressFunctionArguments(_sb, gMAction);
					_sb.AppendLine(" );");
					break;
				case eExecuteTypes.EXE_CODE:
					_sb.AppendLine(gMAction.Code);
					break;
				}
				if (flag2)
				{
					PopRelativeness(_sb);
				}
				if (flag)
				{
					_sb.AppendLine("}");
				}
			}
			return n + 1;
		}

		private static void CompressFunctionArguments(StringBuilder _sb, GMAction action)
		{
			for (int i = 0; i < action.ArgumentCount; i++)
			{
				if (i > 0)
				{
					_sb.Append(", ");
				}
				switch (action.ArgTypes[i])
				{
				case eArgTypes.ARG_STRING:
				case eArgTypes.ARG_STRINGEXP:
					if (action.Args[i].StartsWith("'") || action.Args[i].StartsWith("\""))
					{
						_sb.AppendFormat("{0}", action.Args[i]);
					}
					else
					{
						_sb.AppendFormat("\"{0}\"", action.Args[i]);
					}
					break;
				case eArgTypes.ARG_CONSTANT:
				case eArgTypes.ARG_EXPRESSION:
				case eArgTypes.ARG_BOOLEAN:
				case eArgTypes.ARG_MENU:
				case eArgTypes.ARG_SPRITE:
				case eArgTypes.ARG_SOUND:
				case eArgTypes.ARG_BACKGROUND:
				case eArgTypes.ARG_PATH:
				case eArgTypes.ARG_SCRIPT:
				case eArgTypes.ARG_OBJECT:
				case eArgTypes.ARG_ROOM:
				case eArgTypes.ARG_FONTR:
				case eArgTypes.ARG_COLOR:
				case eArgTypes.ARG_TIMELINE:
				case eArgTypes.ARG_FONT:
					_sb.Append(action.Args[i]);
					break;
				}
			}
		}

		private int CompressAction(int n, StringBuilder _sb)
		{
			if (n < Actions.Count)
			{
				GMAction gMAction = Actions[n];
				switch (gMAction.Kind)
				{
				case eAction.ACT_EXIT:
					return CompressExit(n, _sb);
				case eAction.ACT_BEGIN:
					return CompressBlock(n, _sb);
				case eAction.ACT_REPEAT:
					return CompressRepeat(n, _sb);
				default:
					if (gMAction.IsQuestion)
					{
						return CompressCondition(n, _sb);
					}
					return CompressNormalAction(n, _sb);
				}
			}
			return n;
		}

		private int CompressExecuteScriptAction(int n, GMAction _action, StringBuilder _sb)
		{
			if (_action.Args.Count == 8)
			{
				int num = int.Parse(_action.Args[0]);
				if (num >= 0)
				{
					string key = Program.Assets.Scripts[num].Key;
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("{0}(", key);
					for (int i = 1; i < 6; i++)
					{
						if (!string.IsNullOrEmpty(_action.Args[i]))
						{
							if (i >= 2)
							{
								stringBuilder.Append(",");
							}
							stringBuilder.AppendFormat("{0}", _action.Args[i]);
						}
					}
					stringBuilder.Append(");");
					_sb.AppendLine(stringBuilder.ToString());
				}
				else
				{
					Console.WriteLine("Execute script action is not bound to a script");
				}
			}
			else
			{
				Console.WriteLine("Failed to parse action_execute_script() - incorrect number of arguments received\n");
			}
			return n + 1;
		}

		public string CompressEvent(IList<KeyValuePair<string, GMScript>> _scripts)
		{
			RelativeStack.Clear();
			conditionVar = false;
			int num = 0;
			bool flag = false;
			for (int i = 0; i < Actions.Count; i++)
			{
				if (Actions[i].UseRelative)
				{
					if (flag != Actions[i].Relative)
					{
						num++;
					}
					flag = Actions[i].Relative;
				}
			}
			if (num > 0)
			{
				for (int j = 0; j < Actions.Count; j++)
				{
					if (Actions[j].UseRelative)
					{
						RelativeStack.Push(Actions[j].Relative);
						break;
					}
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("{");
			if (RelativeStack.Count > 0)
			{
				stringBuilder.AppendLine(string.Format("action_set_relative( {0} );", RelativeStack.Peek() ? 1 : 0));
			}
			for (int num2 = 0; num2 < Actions.Count; num2 = ((!(Actions[num2].Name == "action_execute_script")) ? CompressAction(num2, stringBuilder) : CompressExecuteScriptAction(num2, Actions[num2], stringBuilder)))
			{
			}
			if (RelativeStack.Count > 0)
			{
				stringBuilder.AppendLine("action_set_relative( 0 );");
				RelativeStack.Pop();
			}
			if (RelativeStack.Count != 0)
			{
				Console.WriteLine("Unbalanced RelativeStack... need to rebalance now\n");
			}
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}
	}
}
