using System.Collections.Generic;
using System.IO;

namespace GMAssetCompiler
{
	public class GMAction
	{
		private static int countScript;

		public int LibID
		{
			get;
			private set;
		}

		public int ID
		{
			get;
			private set;
		}

		public eAction Kind
		{
			get;
			private set;
		}

		public bool UseRelative
		{
			get;
			private set;
		}

		public bool IsQuestion
		{
			get;
			private set;
		}

		public bool UseApplyTo
		{
			get;
			private set;
		}

		public eExecuteTypes ExeType
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public string Code
		{
			get;
			private set;
		}

		public int ArgumentCount
		{
			get;
			private set;
		}

		public IList<eArgTypes> ArgTypes
		{
			get;
			private set;
		}

		public int Who
		{
			get;
			private set;
		}

		public bool Relative
		{
			get;
			private set;
		}

		public IList<string> Args
		{
			get;
			private set;
		}

		public bool IsNot
		{
			get;
			private set;
		}

		public GMAction(GMAssets _a, Stream _stream)
		{
			_stream.ReadInteger();
			LibID = _stream.ReadInteger();
			ID = _stream.ReadInteger();
			Kind = (eAction)_stream.ReadInteger();
			UseRelative = _stream.ReadBoolean();
			IsQuestion = _stream.ReadBoolean();
			UseApplyTo = _stream.ReadBoolean();
			ExeType = (eExecuteTypes)_stream.ReadInteger();
			Name = _stream.ReadString();
			Code = _stream.ReadString();
			ArgumentCount = _stream.ReadInteger();
			int num = _stream.ReadInteger();
			ArgTypes = new List<eArgTypes>(num);
			for (int i = 0; i < num; i++)
			{
				ArgTypes.Add((eArgTypes)_stream.ReadInteger());
			}
			Who = _stream.ReadInteger();
			Relative = _stream.ReadBoolean();
			num = _stream.ReadInteger();
			Args = new List<string>(num);
			for (int j = 0; j < num; j++)
			{
				Args.Add(_stream.ReadString());
			}
			IsNot = _stream.ReadBoolean();
		}

		public GMAction(int _id, string _code)
		{
			LibID = 1;
			ID = _id;
			Kind = eAction.ACT_CODE;
			UseRelative = false;
			IsQuestion = false;
			UseApplyTo = true;
			ExeType = eExecuteTypes.EXE_CODE;
			Name = string.Empty;
			Code = string.Empty;
			ArgumentCount = 1;
			ArgTypes = new List<eArgTypes>(1);
			ArgTypes.Add(eArgTypes.ARG_STRING);
			Who = -1;
			Relative = false;
			Args = new List<string>(1);
			Args.Add(_code);
			IsNot = false;
		}

		public void Compile(GMAssets _assets)
		{
			switch (Kind)
			{
			case eAction.ACT_VARIABLE:
				if (Relative)
				{
					Code = Code + Args[0] + " += " + Args[1];
				}
				else
				{
					Code = Code + Args[0] + " = " + Args[1];
				}
				Kind = eAction.ACT_NORMAL;
				Args.Clear();
				break;
			case eAction.ACT_CODE:
				Code = Args[0];
				Kind = eAction.ACT_NORMAL;
				Args.Clear();
				if (Program.RemoveDND)
				{
					List<GMLError> _errors = null;
					bool inhibitErrorOutput = Program.InhibitErrorOutput;
					Program.InhibitErrorOutput = true;
					GMLCompile.Compile(_assets, "test_compile", Code, out _errors);
					Program.InhibitErrorOutput = inhibitErrorOutput;
					if (_errors.Count > 0)
					{
						foreach (GMLError item2 in _errors)
						{
							eErrorKind kind = item2.Kind;
							if (kind == eErrorKind.Warning_Unclosed_Comment)
							{
								Code += "*/";
							}
						}
					}
				}
				break;
			}
			if (IsQuestion && ExeType == eExecuteTypes.EXE_CODE)
			{
				Name = string.Format("__script{0}__", countScript);
				GMScript value = new GMScript(Code);
				KeyValuePair<string, GMScript> item = new KeyValuePair<string, GMScript>(Name, value);
				_assets.Scripts.Add(item);
				countScript++;
				ExeType = eExecuteTypes.EXE_FUNCTION;
			}
		}
	}
}
