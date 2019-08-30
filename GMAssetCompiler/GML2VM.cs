using System;
using System.Collections.Generic;
using System.IO;

namespace GMAssetCompiler
{
	internal class GML2VM
	{
		private const int VARIABLE_ARRAY_MAX_DIMENSION = 32000;

		public static int ms_numErrors;

		public VMBuffer VMB
		{
			get;
			set;
		}

		public GMLCode Code
		{
			get;
			set;
		}

		public bool ErrorFlag
		{
			get;
			set;
		}

		public Stack<eVM_Type> TypeStack
		{
			get;
			set;
		}

		public Stack<VMLabel> LoopEnv
		{
			get;
			set;
		}

		public Stack<VMLabel> LoopEndEnv
		{
			get;
			set;
		}

		public static List<string> Strings
		{
			get;
			set;
		}

		public static List<long> StringPatches
		{
			get;
			set;
		}

		public static List<int> VarStringEntry
		{
			get;
			set;
		}

		public static List<long> VarPatches
		{
			get;
			set;
		}

		public static List<int> ObjectStringEntry
		{
			get;
			set;
		}

		public static List<long> ObjectPatches
		{
			get;
			set;
		}

		static GML2VM()
		{
			Strings = new List<string>();
			StringPatches = new List<long>();
			VarStringEntry = new List<int>();
			VarPatches = new List<long>();
			ObjectStringEntry = new List<int>();
			ObjectPatches = new List<long>();
		}

		public GML2VM()
		{
			VMB = new VMBuffer();
			TypeStack = new Stack<eVM_Type>();
			LoopEnv = new Stack<VMLabel>();
			LoopEndEnv = new Stack<VMLabel>();
		}

		private int AddString(string _string)
		{
			int num = Strings.IndexOf(_string);
			if (num == -1)
			{
				num = Strings.Count;
				Strings.Add(_string);
			}
			return num;
		}

		private int AddStringAndPatch(string _string)
		{
			int result = AddString(_string);
			StringPatches.Add(VMB.Buffer.Position);
			return result;
		}

		private int AddVar(string _string)
		{
			int item = AddString(_string);
			int num = VarStringEntry.IndexOf(item);
			if (num == -1)
			{
				num = VarStringEntry.Count;
				VarStringEntry.Add(item);
			}
			return num;
		}

		private int AddVarAndPatch(string _string)
		{
			int result = AddVar(_string);
			VarPatches.Add(VMB.Buffer.Position);
			return result;
		}

		private int AddObjectAndPatch(string _string)
		{
			int item = AddString(_string);
			int num = ObjectStringEntry.IndexOf(item);
			if (num == -1)
			{
				num = ObjectStringEntry.Count;
				ObjectStringEntry.Add(item);
			}
			ObjectPatches.Add(VMB.Buffer.Position);
			return num;
		}

		private void EmitBreak(ushort _v)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg(255, 15) | _v);
		}

		private void Emit(eVM_Instruction _inst, eVM_Type _type1)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, (int)_type1));
		}

		private void Emit(eVM_Instruction _inst, eVM_Type _type1, eVM_Type _type2)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, VMBuffer.EncodeArgDouble((int)_type1, (int)_type2)));
		}

		private void EmitI(eVM_Instruction _inst, double _val)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, 0));
			VMB.Buffer.WriteDouble(_val);
		}

		private void EmitI(eVM_Instruction _inst, float _val)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, 1));
			VMB.Buffer.WriteSingle(_val);
		}

		private void EmitI(eVM_Instruction _inst, long _val)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, 3));
			VMB.Buffer.WriteLong(_val);
		}

		private void EmitI(eVM_Instruction _inst, int _val)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, 2));
			VMB.Buffer.WriteInteger(_val);
		}

		private void EmitI(eVM_Instruction _inst, bool _val)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, 4));
			VMB.Buffer.WriteBoolean(_val);
		}

		private void EmitI(eVM_Instruction _inst, string _val)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, 6));
			int count = Strings.Count;
			Strings.Add(_val);
			StringPatches.Add(VMB.Buffer.Position);
			VMB.Buffer.WriteInteger(count);
		}

		private void EmitIVar(eVM_Instruction _inst, int _var)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, 5));
			VMB.Buffer.WriteInteger(_var);
		}

		private void EmitIVar(eVM_Instruction _inst, int _var, eVM_Type _target)
		{
			VMB.Add(VMBuffer.EncodeInstructionArg((int)_inst, VMBuffer.EncodeArgDouble(5, (int)_target)));
			VMB.Buffer.WriteInteger(_var);
		}

		private void Emit(eVM_Instruction _inst, VMLabel _label)
		{
			if (!_label.Marked)
			{
				_label.Patches.Add((int)VMB.Buffer.Position);
				VMB.Add(VMBuffer.EncodeInstructionBranch((int)_inst, 0));
			}
			else
			{
				long num = _label.Address - VMB.Buffer.Position;
				VMB.Add(VMBuffer.EncodeInstructionBranch((int)_inst, (int)num));
			}
		}

		private void EmitDebugInfo(GMLToken _tok)
		{
		}

		public void Error(string _errorMessage, GMLToken _token)
		{
			if (!Program.InhibitErrorOutput)
			{
				int num = 1;
				for (int i = 0; i < _token.Index; i++)
				{
					if (Code.Code[i] == '\n')
					{
						num++;
					}
				}
				Console.WriteLine("Error : {0}({1}) : {2}", Code.Name, num, _errorMessage);
			}
			ms_numErrors++;
			ErrorFlag = true;
			Program.ExitCode = 1;
		}

		private void CompileConstant(GMLToken _tok)
		{
			switch (_tok.Value.Kind)
			{
			case eKind.eConstant:
				Error("constant token", _tok);
				break;
			case eKind.eNone:
				Error("None constant token", _tok);
				break;
			case eKind.eNumber:
			{
				double num = (long)_tok.Value.ValueI;
				if (num == _tok.Value.ValueI)
				{
					long num2 = (long)_tok.Value.ValueI;
					if (num2 > int.MaxValue || num2 < int.MinValue)
					{
						EmitI(eVM_Instruction.eVMI_PUSH, num2);
						TypeStack.Push(eVM_Type.eVMT_Long);
					}
					else if (num2 > 32767 || num2 < -32768)
					{
						EmitI(eVM_Instruction.eVMI_PUSH, (int)num2);
						TypeStack.Push(eVM_Type.eVMT_Int);
					}
					else
					{
						VMB.Add(VMBuffer.EncodeInstructionArg(192, 15) | (int)(num2 & 0xFFFF));
						TypeStack.Push(eVM_Type.eVMT_Int);
					}
				}
				else
				{
					EmitI(eVM_Instruction.eVMI_PUSH, _tok.Value.ValueI);
					TypeStack.Push(eVM_Type.eVMT_Double);
				}
				break;
			}
			case eKind.eString:
				EmitI(eVM_Instruction.eVMI_PUSH, _tok.Value.ValueS);
				TypeStack.Push(eVM_Type.eVMT_String);
				break;
			}
		}

		private void BinaryTypeCoercion(GMLToken _tok, int _parmNum)
		{
			eVM_Type eVM_Type = TypeStack.Peek();
			switch (_tok.Children[1].Token)
			{
			case eToken.eNot:
			case eToken.eLess:
			case eToken.eLessEqual:
			case eToken.eEqual:
			case eToken.eNotEqual:
			case eToken.eGreaterEqual:
			case eToken.eGreater:
			case eToken.eBitNegate:
				break;
			case eToken.ePlus:
			case eToken.eMinus:
			case eToken.eTime:
			case eToken.eDivide:
			case eToken.eDiv:
			case eToken.eMod:
				if (eVM_Type == eVM_Type.eVMT_Bool)
				{
					TypeStack.Pop();
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type, eVM_Type.eVMT_Int);
					TypeStack.Push(eVM_Type.eVMT_Int);
				}
				break;
			case eToken.eAnd:
			case eToken.eOr:
			case eToken.eXor:
				if (eVM_Type != eVM_Type.eVMT_Bool)
				{
					TypeStack.Pop();
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type, eVM_Type.eVMT_Bool);
					TypeStack.Push(eVM_Type.eVMT_Bool);
				}
				break;
			case eToken.eBitOr:
			case eToken.eBitAnd:
			case eToken.eBitXor:
			case eToken.eBitShiftLeft:
			case eToken.eBitShiftRight:
				if (eVM_Type == eVM_Type.eVMT_Int)
				{
					TypeStack.Pop();
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type, eVM_Type.eVMT_Int);
					TypeStack.Push(eVM_Type.eVMT_Int);
				}
				break;
			}
		}

		private int TypeSize(eVM_Type _t)
		{
			int result = 0;
			switch (_t)
			{
			case eVM_Type.eVMT_Double:
				result = 8;
				break;
			case eVM_Type.eVMT_Long:
				result = 8;
				break;
			case eVM_Type.eVMT_String:
				result = 4;
				break;
			case eVM_Type.eVMT_Variable:
				result = 12;
				break;
			case eVM_Type.eVMT_Float:
				result = 4;
				break;
			case eVM_Type.eVMT_Bool:
				result = 4;
				break;
			case eVM_Type.eVMT_Int:
				result = 4;
				break;
			}
			return result;
		}

		private void CompileBinary(GMLToken _tok)
		{
			CompileExpression(_tok.Children[0]);
			BinaryTypeCoercion(_tok, 1);
			CompileExpression(_tok.Children[2]);
			BinaryTypeCoercion(_tok, 2);
			eVM_Type eVM_Type = TypeStack.Pop();
			eVM_Type eVM_Type2 = TypeStack.Pop();
			int num = TypeSize(eVM_Type);
			int num2 = TypeSize(eVM_Type2);
			eVM_Type item = (num > num2) ? eVM_Type : eVM_Type2;
			switch (_tok.Children[1].Token)
			{
			case eToken.eTime:
				Emit(eVM_Instruction.eVMI_MUL, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.eDivide:
				Emit(eVM_Instruction.eVMI_DIV, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.eDiv:
				Emit(eVM_Instruction.eVMI_REM, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.eMod:
				Emit(eVM_Instruction.eVMI_MOD, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.ePlus:
				Emit(eVM_Instruction.eVMI_ADD, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.eMinus:
				Emit(eVM_Instruction.eVMI_SUB, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.eLess:
				Emit(eVM_Instruction.eVMI_SET_LT, eVM_Type, eVM_Type2);
				TypeStack.Push(eVM_Type.eVMT_Bool);
				break;
			case eToken.eLessEqual:
				Emit(eVM_Instruction.eVMI_SET_LE, eVM_Type, eVM_Type2);
				TypeStack.Push(eVM_Type.eVMT_Bool);
				break;
			case eToken.eAssign:
			case eToken.eEqual:
				Emit(eVM_Instruction.eVMI_SET_EQ, eVM_Type, eVM_Type2);
				TypeStack.Push(eVM_Type.eVMT_Bool);
				break;
			case eToken.eNotEqual:
				Emit(eVM_Instruction.eVMI_SET_NE, eVM_Type, eVM_Type2);
				TypeStack.Push(eVM_Type.eVMT_Bool);
				break;
			case eToken.eGreaterEqual:
				Emit(eVM_Instruction.eVMI_SET_GE, eVM_Type, eVM_Type2);
				TypeStack.Push(eVM_Type.eVMT_Bool);
				break;
			case eToken.eGreater:
				Emit(eVM_Instruction.eVMI_SET_GT, eVM_Type, eVM_Type2);
				TypeStack.Push(eVM_Type.eVMT_Bool);
				break;
			case eToken.eAnd:
			case eToken.eBitAnd:
				Emit(eVM_Instruction.eVMI_AND, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.eOr:
			case eToken.eBitOr:
				Emit(eVM_Instruction.eVMI_OR, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.eXor:
			case eToken.eBitXor:
				Emit(eVM_Instruction.eVMI_XOR, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.eBitShiftLeft:
				Emit(eVM_Instruction.eVMI_SHL, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			case eToken.eBitShiftRight:
				Emit(eVM_Instruction.eVMI_SHR, eVM_Type, eVM_Type2);
				TypeStack.Push(item);
				break;
			}
		}

		private void CompileUnary(GMLToken _tok)
		{
			CompileExpression(_tok.Children[0]);
			eVM_Type eVM_Type = TypeStack.Peek();
			switch (_tok.Id)
			{
			case 203:
				switch (eVM_Type)
				{
				case eVM_Type.eVMT_String:
				case eVM_Type.eVMT_Error:
					Error("Unable to Not a string", _tok);
					break;
				case eVM_Type.eVMT_Double:
				case eVM_Type.eVMT_Float:
				case eVM_Type.eVMT_Int:
				case eVM_Type.eVMT_Long:
				case eVM_Type.eVMT_Variable:
					TypeStack.Pop();
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type, eVM_Type.eVMT_Bool);
					TypeStack.Push(eVM_Type.eVMT_Bool);
					eVM_Type = eVM_Type.eVMT_Bool;
					break;
				}
				Emit(eVM_Instruction.eVMI_NOT, eVM_Type);
				break;
			case 211:
				Emit(eVM_Instruction.eVMI_NEG, eVM_Type);
				break;
			case 220:
				switch (eVM_Type)
				{
				case eVM_Type.eVMT_String:
				case eVM_Type.eVMT_Error:
					Error("Unable to Negate a string", _tok);
					break;
				case eVM_Type.eVMT_Double:
				case eVM_Type.eVMT_Float:
				case eVM_Type.eVMT_Variable:
					TypeStack.Pop();
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type, eVM_Type.eVMT_Int);
					TypeStack.Push(eVM_Type.eVMT_Int);
					eVM_Type = eVM_Type.eVMT_Int;
					break;
				}
				Emit(eVM_Instruction.eVMI_NOT, eVM_Type);
				break;
			}
		}

		private void CompileVariable(GMLToken _tok)
		{
			switch (_tok.Token)
			{
			case eToken.eVariable:
			case eToken.eDot:
				if (_tok.Children.Count >= 2)
				{
					int num = 0;
					CompileExpression(_tok.Children[0]);
					if (TypeStack.Peek() != eVM_Type.eVMT_Int)
					{
						Emit(eVM_Instruction.eVMI_CONV, TypeStack.Pop(), eVM_Type.eVMT_Int);
						TypeStack.Push(eVM_Type.eVMT_Int);
					}
					if (_tok.Children[1].Children.Count > 0)
					{
						CompileExpression(_tok.Children[1].Children[0]);
						if (TypeStack.Peek() != eVM_Type.eVMT_Int)
						{
							Emit(eVM_Instruction.eVMI_CONV, TypeStack.Pop(), eVM_Type.eVMT_Int);
							TypeStack.Push(eVM_Type.eVMT_Int);
						}
						if (_tok.Children[1].Children.Count > 1)
						{
							EmitI(eVM_Instruction.eVMI_PUSH, 32000);
							Emit(eVM_Instruction.eVMI_MUL, eVM_Type.eVMT_Int, eVM_Type.eVMT_Int);
							CompileExpression(_tok.Children[1].Children[1]);
							if (TypeStack.Peek() != eVM_Type.eVMT_Int)
							{
								Emit(eVM_Instruction.eVMI_CONV, TypeStack.Pop(), eVM_Type.eVMT_Int);
								TypeStack.Push(eVM_Type.eVMT_Int);
							}
							Emit(eVM_Instruction.eVMI_ADD, eVM_Type.eVMT_Int, eVM_Type.eVMT_Int);
							TypeStack.Pop();
						}
						TypeStack.Pop();
					}
					else
					{
						num |= int.MinValue;
					}
					TypeStack.Pop();
					EmitIVar(eVM_Instruction.eVMI_PUSH, _tok.Children[1].Id | num);
					TypeStack.Push(eVM_Type.eVMT_Variable);
				}
				else
				{
					Error("Malformed variable reference", _tok);
				}
				break;
			case eToken.eConstant:
				Error("Unsure where these come from", _tok);
				break;
			}
		}

		private void CompileExpression(GMLToken _tok)
		{
			EmitDebugInfo(_tok);
			switch (_tok.Token)
			{
			case eToken.eConstant:
				CompileConstant(_tok);
				break;
			case eToken.eBinary:
				CompileBinary(_tok);
				break;
			case eToken.eUnary:
				CompileUnary(_tok);
				break;
			case eToken.eFunction:
				CompileFunction(_tok);
				break;
			case eToken.eVariable:
			case eToken.eDot:
				CompileVariable(_tok);
				break;
			}
		}

		private void CompileGlobalVar(GMLToken _tok)
		{
		}

		private void CompileRepeat(GMLToken _tok)
		{
			VMLabel vMLabel = new VMLabel("End", VMB);
			VMLabel vMLabel2 = new VMLabel("Repeat", VMB);
			CompileExpression(_tok.Children[0]);
			eVM_Type eVM_Type = TypeStack.Pop();
			if (eVM_Type != eVM_Type.eVMT_Int)
			{
				Emit(eVM_Instruction.eVMI_CONV, eVM_Type, eVM_Type.eVMT_Int);
			}
			Emit(eVM_Instruction.eVMI_DUP, eVM_Type.eVMT_Int);
			EmitI(eVM_Instruction.eVMI_PUSH, 0);
			Emit(eVM_Instruction.eVMI_SET_LE, eVM_Type.eVMT_Int, eVM_Type.eVMT_Int);
			Emit(eVM_Instruction.eVMI_BTRUE, vMLabel);
			LoopEnv.Push(vMLabel2);
			LoopEndEnv.Push(vMLabel);
			vMLabel2.Mark(VMB.Buffer.Position);
			CompileStatement(_tok.Children[1]);
			EmitI(eVM_Instruction.eVMI_PUSH, 1);
			Emit(eVM_Instruction.eVMI_SUB, eVM_Type.eVMT_Int, eVM_Type.eVMT_Int);
			Emit(eVM_Instruction.eVMI_DUP, eVM_Type.eVMT_Int);
			Emit(eVM_Instruction.eVMI_CONV, eVM_Type.eVMT_Int, eVM_Type.eVMT_Bool);
			Emit(eVM_Instruction.eVMI_BTRUE, vMLabel2);
			vMLabel.Mark(VMB.Buffer.Position);
			Emit(eVM_Instruction.eVMI_POPNULL, eVM_Type.eVMT_Int);
			LoopEnv.Pop();
			LoopEndEnv.Pop();
		}

		private void CompileIf(GMLToken _tok)
		{
		}

		private void CompileWhile(GMLToken _tok)
		{
		}

		private void CompileDo(GMLToken _tok)
		{
		}

		private void CompileFor(GMLToken _tok)
		{
		}

		private void CompileWith(GMLToken _tok)
		{
		}

		private void CompileSwitch(GMLToken _tok)
		{
		}

		private void CompileFunction(GMLToken _tok)
		{
			foreach (GMLToken child in _tok.Children)
			{
				CompileExpression(child);
				eVM_Type eVM_Type = TypeStack.Pop();
				if (eVM_Type != eVM_Type.eVMT_Variable)
				{
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type, eVM_Type.eVMT_Variable);
				}
			}
			EmitI(eVM_Instruction.eVMI_CALL, _tok.Id);
			TypeStack.Push(GMAssetCompiler.eVM_Type.eVMT_Variable);
		}

		private void CompilePop(GMLToken _tok, eVM_Type _type)
		{
			switch (_tok.Token)
			{
			case eToken.eVariable:
			case eToken.eDot:
				if (_tok.Children.Count >= 2)
				{
					int num = 0;
					CompileExpression(_tok.Children[0]);
					if (TypeStack.Peek() != eVM_Type.eVMT_Int)
					{
						Emit(eVM_Instruction.eVMI_CONV, TypeStack.Pop(), eVM_Type.eVMT_Int);
						TypeStack.Push(eVM_Type.eVMT_Int);
					}
					if (_tok.Children[1].Children.Count > 0)
					{
						CompileExpression(_tok.Children[1].Children[0]);
						if (TypeStack.Peek() != eVM_Type.eVMT_Int)
						{
							Emit(eVM_Instruction.eVMI_CONV, TypeStack.Pop(), eVM_Type.eVMT_Int);
							TypeStack.Push(eVM_Type.eVMT_Int);
						}
						if (_tok.Children[1].Children.Count > 1)
						{
							EmitI(eVM_Instruction.eVMI_PUSH, 32000);
							Emit(eVM_Instruction.eVMI_MUL, eVM_Type.eVMT_Int, eVM_Type.eVMT_Int);
							CompileExpression(_tok.Children[1].Children[1]);
							if (TypeStack.Peek() != eVM_Type.eVMT_Int)
							{
								Emit(eVM_Instruction.eVMI_CONV, TypeStack.Pop(), eVM_Type.eVMT_Int);
								TypeStack.Push(eVM_Type.eVMT_Int);
							}
							Emit(eVM_Instruction.eVMI_ADD, eVM_Type.eVMT_Int, eVM_Type.eVMT_Int);
							TypeStack.Pop();
						}
						TypeStack.Pop();
					}
					else
					{
						num |= int.MinValue;
					}
					TypeStack.Pop();
					EmitIVar(eVM_Instruction.eVMI_POP, _tok.Children[1].Id | num, _type);
					TypeStack.Push(eVM_Type.eVMT_Variable);
				}
				else
				{
					Error("Malformed variable reference", _tok);
				}
				break;
			case eToken.eConstant:
				Error("Unsure where these come from", _tok);
				break;
			}
		}

		private void CompileAssign(GMLToken _tok)
		{
			switch (_tok.Children[1].Token)
			{
			case eToken.eAssign:
				CompileExpression(_tok.Children[2]);
				CompilePop(_tok.Children[0], TypeStack.Pop());
				break;
			case eToken.eAssignPlus:
			{
				CompileVariable(_tok.Children[0]);
				TypeStack.Pop();
				CompileExpression(_tok.Children[2]);
				eVM_Type eVM_Type3 = TypeStack.Pop();
				if (eVM_Type3 == eVM_Type.eVMT_Bool)
				{
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type3, eVM_Type.eVMT_Int);
					eVM_Type3 = eVM_Type.eVMT_Int;
				}
				Emit(eVM_Instruction.eVMI_ADD, eVM_Type3, eVM_Type.eVMT_Variable);
				CompilePop(_tok.Children[0], eVM_Type.eVMT_Variable);
				break;
			}
			case eToken.eAssignMinus:
			{
				CompileVariable(_tok.Children[0]);
				TypeStack.Pop();
				CompileExpression(_tok.Children[2]);
				eVM_Type eVM_Type7 = TypeStack.Pop();
				if (eVM_Type7 == eVM_Type.eVMT_Bool)
				{
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type7, eVM_Type.eVMT_Int);
					eVM_Type7 = eVM_Type.eVMT_Int;
				}
				Emit(eVM_Instruction.eVMI_SUB, eVM_Type7, eVM_Type.eVMT_Variable);
				CompilePop(_tok.Children[0], eVM_Type.eVMT_Variable);
				break;
			}
			case eToken.eAssignTimes:
			{
				CompileVariable(_tok.Children[0]);
				TypeStack.Pop();
				CompileExpression(_tok.Children[2]);
				eVM_Type eVM_Type2 = TypeStack.Pop();
				if (eVM_Type2 == eVM_Type.eVMT_Bool)
				{
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type2, eVM_Type.eVMT_Int);
					eVM_Type2 = eVM_Type.eVMT_Int;
				}
				Emit(eVM_Instruction.eVMI_MUL, eVM_Type2, eVM_Type.eVMT_Variable);
				CompilePop(_tok.Children[0], eVM_Type.eVMT_Variable);
				break;
			}
			case eToken.eAssignDivide:
			{
				CompileVariable(_tok.Children[0]);
				TypeStack.Pop();
				CompileExpression(_tok.Children[2]);
				eVM_Type eVM_Type6 = TypeStack.Pop();
				if (eVM_Type6 == eVM_Type.eVMT_Bool)
				{
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type6, eVM_Type.eVMT_Int);
					eVM_Type6 = eVM_Type.eVMT_Int;
				}
				Emit(eVM_Instruction.eVMI_DIV, eVM_Type6, eVM_Type.eVMT_Variable);
				CompilePop(_tok.Children[0], eVM_Type.eVMT_Variable);
				break;
			}
			case eToken.eAssignOr:
			{
				CompileVariable(_tok.Children[0]);
				TypeStack.Pop();
				CompileExpression(_tok.Children[2]);
				eVM_Type eVM_Type4 = TypeStack.Pop();
				if (eVM_Type4 != eVM_Type.eVMT_Int && eVM_Type4 != eVM_Type.eVMT_Long)
				{
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type4, eVM_Type.eVMT_Int);
					eVM_Type4 = eVM_Type.eVMT_Int;
				}
				Emit(eVM_Instruction.eVMI_OR, eVM_Type4, eVM_Type.eVMT_Variable);
				CompilePop(_tok.Children[0], eVM_Type.eVMT_Variable);
				break;
			}
			case eToken.eAssignAnd:
			{
				CompileVariable(_tok.Children[0]);
				TypeStack.Pop();
				CompileExpression(_tok.Children[2]);
				eVM_Type eVM_Type5 = TypeStack.Pop();
				if (eVM_Type5 != eVM_Type.eVMT_Int && eVM_Type5 != eVM_Type.eVMT_Long)
				{
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type5, eVM_Type.eVMT_Int);
					eVM_Type5 = eVM_Type.eVMT_Int;
				}
				Emit(eVM_Instruction.eVMI_AND, eVM_Type5, eVM_Type.eVMT_Variable);
				CompilePop(_tok.Children[0], eVM_Type.eVMT_Variable);
				break;
			}
			case eToken.eAssignXor:
			{
				CompileVariable(_tok.Children[0]);
				TypeStack.Pop();
				CompileExpression(_tok.Children[2]);
				eVM_Type eVM_Type = TypeStack.Pop();
				if (eVM_Type != eVM_Type.eVMT_Int && eVM_Type != eVM_Type.eVMT_Long)
				{
					Emit(eVM_Instruction.eVMI_CONV, eVM_Type, eVM_Type.eVMT_Int);
					eVM_Type = eVM_Type.eVMT_Int;
				}
				Emit(eVM_Instruction.eVMI_XOR, eVM_Type, eVM_Type.eVMT_Variable);
				CompilePop(_tok.Children[0], eVM_Type.eVMT_Variable);
				break;
			}
			}
		}

		private void CompileReturn(GMLToken _tok)
		{
		}

		private void CompileBreak(GMLToken _tok)
		{
		}

		private void CompileExit(GMLToken _tok)
		{
		}

		private void CompileContinue(GMLToken _tok)
		{
		}

		private void CompileStatement(GMLToken _tok)
		{
			EmitDebugInfo(_tok);
			switch (_tok.Token)
			{
			case eToken.eVar:
				break;
			case eToken.eGlobalVar:
				CompileGlobalVar(_tok);
				break;
			case eToken.eBegin:
			case eToken.eBlock:
				CompileBlock(_tok);
				break;
			case eToken.eRepeat:
				CompileRepeat(_tok);
				break;
			case eToken.eIf:
				CompileIf(_tok);
				break;
			case eToken.eWhile:
				CompileWhile(_tok);
				break;
			case eToken.eDo:
				CompileDo(_tok);
				break;
			case eToken.eFor:
				CompileFor(_tok);
				break;
			case eToken.eWith:
				CompileWith(_tok);
				break;
			case eToken.eSwitch:
				CompileSwitch(_tok);
				break;
			case eToken.eFunction:
				CompileFunction(_tok);
				break;
			case eToken.eAssign:
				CompileAssign(_tok);
				break;
			case eToken.eReturn:
				CompileReturn(_tok);
				break;
			case eToken.eBreak:
				CompileBreak(_tok);
				break;
			case eToken.eExit:
				CompileExit(_tok);
				break;
			case eToken.eContinue:
				CompileContinue(_tok);
				break;
			default:
				Error("Token is undefined at CompileStatement", _tok);
				break;
			}
		}

		private void CompileBlock(GMLToken _tok)
		{
			foreach (GMLToken child in _tok.Children)
			{
				CompileStatement(child);
			}
		}

		private void CompileProgram(GMLToken _tok)
		{
			switch (_tok.Token)
			{
			case eToken.eEOF:
				break;
			case eToken.eBlock:
				CompileBlock(_tok);
				break;
			default:
				Error("No program to compile", _tok);
				break;
			}
		}

		private GMLToken RewriteVariable(GMLToken _tok)
		{
			GMLToken gMLToken = new GMLToken(_tok);
			gMLToken.Token = eToken.eDot;
			gMLToken.Children = new List<GMLToken>(2);
			gMLToken.Children.Add(new GMLToken(eToken.eConstant, _tok, -6, new GMLValue(-6.0)));
			gMLToken.Children.Add(_tok);
			return gMLToken;
		}

		private GMLToken RewriteDot(GMLToken _tok)
		{
			GMLToken gMLToken = null;
			if (_tok.Children.Count > 2)
			{
				gMLToken = new GMLToken(_tok);
				GMLToken gMLToken2 = gMLToken;
				gMLToken2.Children = new List<GMLToken>(2);
				gMLToken2.Children.Add(null);
				gMLToken2.Children.Add(null);
				gMLToken2.Children[1] = _tok.Children[_tok.Children.Count - 1];
				for (int num = _tok.Children.Count - 2; num > 2; num--)
				{
					gMLToken2.Children[0] = new GMLToken(_tok);
					gMLToken2.Children[0].Children = new List<GMLToken>(2);
					gMLToken2.Children[0].Children.Add(null);
					gMLToken2.Children[0].Children.Add(null);
					gMLToken2.Children[0].Children[1] = _tok.Children[num];
					gMLToken2 = gMLToken2.Children[0];
				}
				gMLToken2.Children[0] = new GMLToken(_tok);
				gMLToken2.Children[0].Children = new List<GMLToken>(2);
				gMLToken2.Children[0].Children.Add(null);
				gMLToken2.Children[0].Children.Add(null);
				gMLToken2.Children[0].Children[1] = _tok.Children[1];
				gMLToken2.Children[0].Children[0] = _tok.Children[0];
			}
			return gMLToken;
		}

		private GMLToken RewriteTree(GMLToken _tok)
		{
			GMLToken result = null;
			bool flag = true;
			switch (_tok.Token)
			{
			case eToken.eDot:
				result = RewriteDot(_tok);
				flag = false;
				break;
			case eToken.eVariable:
				result = RewriteVariable(_tok);
				flag = false;
				break;
			}
			if (flag)
			{
				for (int i = 0; i < _tok.Children.Count; i++)
				{
					GMLToken gMLToken = RewriteTree(_tok.Children[i]);
					if (gMLToken != null)
					{
						_tok.Children[i] = gMLToken;
					}
				}
			}
			return result;
		}

		private int ParamSize(int _arg)
		{
			int result = 0;
			switch (_arg & 0xF)
			{
			case 0:
				result = 8;
				break;
			case 3:
				result = 8;
				break;
			case 1:
			case 2:
			case 4:
			case 5:
			case 6:
				result = 4;
				break;
			}
			return result;
		}

		private string Instruction2String(int _ins)
		{
			string result = "unknown";
			switch (_ins)
			{
			case 192:
				result = "push";
				break;
			case 65:
				result = "pop";
				break;
			case 130:
				result = "dup";
				break;
			case 4:
				result = "mul";
				break;
			case 5:
				result = "div";
				break;
			case 6:
				result = "rem";
				break;
			case 7:
				result = "mod";
				break;
			case 8:
				result = "add";
				break;
			case 9:
				result = "sub";
				break;
			case 3:
				result = "conv";
				break;
			case 10:
				result = "and";
				break;
			case 11:
				result = "or";
				break;
			case 12:
				result = "xor";
				break;
			case 13:
				result = "neg";
				break;
			case 14:
				result = "not";
				break;
			case 15:
				result = "shl";
				break;
			case 16:
				result = "shr";
				break;
			case 17:
				result = "slt";
				break;
			case 18:
				result = "sle";
				break;
			case 19:
				result = "seq";
				break;
			case 20:
				result = "sne";
				break;
			case 21:
				result = "sge";
				break;
			case 22:
				result = "sgt";
				break;
			case 183:
				result = "b";
				break;
			case 184:
				result = "bt";
				break;
			case 185:
				result = "bf";
				break;
			case 218:
				result = "call";
				break;
			case 187:
				result = "pushenv";
				break;
			case 188:
				result = "popenv";
				break;
			case 157:
				result = "ret";
				break;
			case 158:
				result = "exit";
				break;
			case 159:
				result = "popz";
				break;
			case 255:
				result = "break";
				break;
			}
			return result;
		}

		private string Arg2String(int _arg)
		{
			string result = "";
			switch (_arg & 0xF)
			{
			case 3:
				result = ".l";
				break;
			case 0:
				result = ".d";
				break;
			case 2:
				result = ".i";
				break;
			case 5:
				result = ".v";
				break;
			case 6:
				result = ".s";
				break;
			case 1:
				result = ".f";
				break;
			case 4:
				result = ".b";
				break;
			case 15:
				result = ".e";
				break;
			}
			return result;
		}

		public int DisasmOne(VMBuffer _vmb, int _offs, TextWriter _sw)
		{
			int num = _offs;
			int @int = _vmb.GetInt(_offs);
			_offs += 4;
			int instruction = VMBuffer.GetInstruction(@int);
			int arg = VMBuffer.GetArg(@int);
			int num2 = _offs;
			if ((instruction & 0x40) != 0)
			{
				_offs += ParamSize(arg);
			}
			int i = 11;
			_sw.Write("{0:x8} : ", num);
			int num3 = num;
			while (num3 < _offs)
			{
				_sw.Write("{0:x2}", _vmb.Buffer.GetBuffer()[num3]);
				num3++;
				i += 2;
			}
			for (; i < 36; i++)
			{
				_sw.Write(" ");
			}
			string text = Instruction2String(instruction);
			_sw.Write(text);
			i += text.Length;
			if ((instruction & 0xA0) == 128)
			{
				_sw.Write(Arg2String(arg));
				i += 2;
			}
			else if ((instruction & 0xA0) == 0)
			{
				_sw.Write(Arg2String(arg & 0xF));
				_sw.Write(Arg2String(arg >> 4));
				i += 4;
			}
			for (; i < 46; i++)
			{
				_sw.Write(" ");
			}
			if ((instruction & 0x40) != 0)
			{
				long position = _vmb.Buffer.Position;
				_vmb.Buffer.Position = num2;
				switch (arg & 0xF)
				{
				case 3:
					_sw.Write("{0}", _vmb.Buffer.ReadLong());
					break;
				case 0:
					_sw.Write("{0}", _vmb.Buffer.ReadDouble());
					break;
				case 2:
					_sw.Write("{0}", _vmb.Buffer.ReadInteger());
					break;
				case 5:
					_sw.Write("{0}", _vmb.Buffer.ReadInteger() & 0x1FFFFFFF);
					break;
				case 6:
					_sw.Write("{0}", Strings[_vmb.Buffer.ReadInteger()]);
					break;
				case 1:
					_sw.Write("{0}", _vmb.Buffer.ReadSingle());
					break;
				case 4:
					_sw.Write("{0}", _vmb.Buffer.ReadInteger() != 0);
					break;
				case 15:
					_sw.Write("{0}", @int << 16 >> 16);
					break;
				}
				_vmb.Buffer.Position = position;
			}
			else if ((instruction & 0x20) != 0)
			{
				int num4 = num + VMBuffer.GetBranch(@int);
				_sw.WriteLine("0x{0:x8}", num4);
			}
			_sw.WriteLine("");
			return _offs;
		}

		public void Disasm(TextWriter _sw)
		{
			for (int num = 0; num < VMB.Buffer.Position; num = DisasmOne(VMB, num, _sw))
			{
			}
		}

		public void Compile(GMAssets _assets, GMLCode _code)
		{
			Code = _code;
			if (Program.CompileVerbose)
			{
				RewriteTree(_code.Token);
				CompileProgram(_code.Token);
				Disasm(Program.Out);
			}
		}
	}
}
