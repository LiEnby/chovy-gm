using System;
using System.Collections.Generic;
using System.IO;

namespace GMAssetCompiler
{
	public class YYObfuscate
	{
		public List<LexTree> m_TokenTree;

		public Dictionary<string, string> m_Encoder;

		public Dictionary<string, int> m_StringTable;

		public Dictionary<string, VFNode> m_FunctionList;

		public Dictionary<string, string> m_MaskConvert;

		private int m_RndCnt;

		private string m_LastName;

		private int m_StringIndex;

		private string m_StringTableName;

		private string m_DecodeFunction;

		private Random m_Rnd;

		private List<string> m_Files;

		private Lex m_pLex;

		private int m_tabcount;

		private int m_WriteTabs;

		private bool m_WriteSpace;

		private int m_LineWidth;

		private bool m_FunctionRemoved;

		public bool PrettyPrint
		{
			get;
			set;
		}

		public bool Obfuscate
		{
			get;
			set;
		}

		public bool EncodeStrings
		{
			get;
			set;
		}

		public bool RemovedUnused
		{
			get;
			set;
		}

		public int UglyWidth
		{
			get;
			set;
		}

		public bool Verbose
		{
			get;
			set;
		}

		public YYObfuscate()
		{
			UglyWidth = 500;
			m_TokenTree = new List<LexTree>(100000);
			m_Encoder = new Dictionary<string, string>(100000);
			m_StringTable = new Dictionary<string, int>();
			m_FunctionList = new Dictionary<string, VFNode>();
			m_MaskConvert = new Dictionary<string, string>(26);
			m_LastName = "";
			m_StringIndex = 0;
			m_StringTableName = "_STRING_TABLE_LOOKUP_";
			m_DecodeFunction = "for(var __iStr in " + m_StringTableName + "){var __ss=" + m_StringTableName + "[__iStr];var __st =\"\";for(var __i=0;__i<__ss.length;__i++){__st += String.fromCharCode(__ss.charCodeAt(__i)^0x15);}" + m_StringTableName + "[__iStr] = __st;}";
			m_Files = new List<string>();
			m_tabcount = 0;
			m_WriteTabs = 0;
			m_WriteSpace = false;
			m_LineWidth = 0;
			for (m_Rnd = new Random(); m_RndCnt < 26; m_RndCnt = (m_Rnd.Next() & 0xFF))
			{
			}
			m_pLex = new Lex(this);
			m_pLex.CaseSensitive = true;
		}

		private string NewName(bool _check)
		{
			while (true)
			{
				string text = "";
				for (int num = m_RndCnt; num > 0; num /= 26)
				{
					text += (char)(num % 26 + 65);
				}
				m_RndCnt++;
				m_LastName = text.ToLower();
				if (!_check)
				{
					break;
				}
				if (m_pLex.CheckToken(m_LastName) == eLex.None)
				{
					return m_LastName;
				}
			}
			return m_LastName;
		}

		private void DoObfuscation(List<LexTree> _list)
		{
			if (Obfuscate)
			{
				LexTree lexTree = null;
				LexTree lexTree2 = null;
				foreach (LexTree item in _list)
				{
					if (item.Token == eLex.ID)
					{
						string value;
						if (!m_Encoder.TryGetValue(item.Text, out value))
						{
							value = NewName(true);
							m_Encoder.Add(item.Text, value);
						}
						item.Text = value;
					}
					if (item.Children != null)
					{
						DoObfuscation(item.Children);
					}
					lexTree = lexTree2;
					lexTree2 = item;
				}
			}
		}

		private void DoEncodeStrings(List<LexTree> _lextree)
		{
			if (EncodeStrings)
			{
				foreach (LexTree item in _lextree)
				{
					if (item.Token == eLex.String)
					{
						int value = 0;
						if (!m_StringTable.TryGetValue(item.Text, out value))
						{
							value = m_StringIndex;
							m_StringTable.Add(item.Text, m_StringIndex++);
						}
						item.Text = m_StringTableName + "[" + value.ToString() + "]";
					}
					if (item.Children != null)
					{
						DoEncodeStrings(item.Children);
					}
				}
			}
		}

		private void WriteStringTable(StreamWriter _oFile)
		{
			string stringTableName = m_StringTableName;
			bool flag = true;
			_oFile.Write("var " + stringTableName + "=[");
			foreach (KeyValuePair<string, int> item in m_StringTable)
			{
				if (!flag)
				{
					_oFile.Write(",");
				}
				if (PrettyPrint)
				{
					_oFile.WriteLine();
				}
				string key = item.Key;
				string text = "\"";
				for (int i = 1; i < key.Length - 1; i++)
				{
					char c = (char)(key[i] ^ 0x15);
					if (c == '"' || c == '\'' || c == '\\')
					{
						text += "\\";
					}
					text += c;
				}
				_oFile.Write(text + "\"");
				flag = false;
			}
			_oFile.Write("];");
			if (PrettyPrint)
			{
				_oFile.WriteLine();
			}
		}

		private int ParseFunction(List<LexTree> _tokenlist, int _i)
		{
			bool flag = false;
			bool flag2 = true;
			LexTree lexTree = _tokenlist[_i];
			int num = _i;
			LexTree lexTree2 = _tokenlist[_i + 1];
			if (lexTree2.Text != "(" && lexTree2.Token == eLex.ID)
			{
				lexTree.Name = lexTree2.Text;
				flag = true;
				flag2 = false;
				lexTree.Used = new Dictionary<string, VFNode>(50);
				_i += 2;
				VFNode value;
				if (!m_FunctionList.TryGetValue(lexTree2.Text, out value))
				{
					value = new VFNode(lexTree2.Text);
					m_FunctionList.Add(lexTree2.Text, value);
				}
				value.pFunction.Add(lexTree);
				lexTree.Node = value;
			}
			int num2 = 0;
			bool flag3 = false;
			while (true)
			{
				LexTree lexTree3 = _tokenlist[_i];
				if (lexTree3.Token == eLex.RightBracket)
				{
					flag3 = true;
				}
				if (flag3)
				{
					if (lexTree3.Text == "{")
					{
						num2++;
					}
					if (lexTree3.Text == "}")
					{
						num2--;
						if (num2 == 0)
						{
							break;
						}
					}
				}
				if (lexTree3.Text == "function")
				{
					lexTree3.anon = true;
				}
				if (!flag2 && lexTree3.Token == eLex.ID)
				{
					VFNode value2;
					if (!lexTree.Used.TryGetValue(lexTree3.Text, out value2))
					{
						if (!m_FunctionList.TryGetValue(lexTree3.Text, out value2))
						{
							value2 = new VFNode(lexTree3.Text);
							m_FunctionList.Add(lexTree3.Text, value2);
						}
						lexTree.Used.Add(lexTree3.Text, value2);
					}
					value2.Count++;
				}
				else if (lexTree3.Text == "true")
				{
					lexTree3.Text = "!0";
				}
				else if (lexTree3.Text == "false")
				{
					lexTree3.Text = "!1";
				}
				_i++;
				if (_i >= _tokenlist.Count)
				{
					return _i;
				}
			}
			num2 = _i + 1 - num - 1;
			if (flag)
			{
				List<LexTree> range = _tokenlist.GetRange(num + 1, num2);
				_tokenlist.RemoveRange(num + 1, num2);
				lexTree.Children = new List<LexTree>(num2);
				lexTree.Children.AddRange(range);
				_i = num;
			}
			return _i;
		}

		private int ParseProtoTypeFunction(List<LexTree> _tokenlist, int _i)
		{
			bool flag = false;
			bool flag2 = true;
			_i -= 2;
			LexTree lexTree = _tokenlist[_i];
			if (_tokenlist[_i + 1].Token != eLex.Dot || _tokenlist[_i + 2].Text != "prototype" || _tokenlist[_i + 3].Token != eLex.Dot)
			{
				return _i + 3;
			}
			int num = _i;
			LexTree lexTree2 = _tokenlist[_i + 4];
			if (_tokenlist[_i + 5].Token != eLex.Equals || _tokenlist[_i + 6].Text != "function" || _tokenlist[_i + 7].Token != eLex.LeftBracket)
			{
				return _i + 3;
			}
			lexTree.Name = lexTree2.Text;
			flag = true;
			flag2 = false;
			lexTree.Used = new Dictionary<string, VFNode>(50);
			VFNode value;
			if (!m_FunctionList.TryGetValue(lexTree2.Text, out value))
			{
				value = new VFNode(lexTree2.Text);
				m_FunctionList.Add(lexTree2.Text, value);
			}
			value.pFunction.Add(lexTree);
			lexTree.Node = value;
			_i += 8;
			int num2 = 0;
			while (true)
			{
				LexTree lexTree3 = _tokenlist[_i];
				if (lexTree3.Text == "{")
				{
					num2++;
				}
				if (lexTree3.Text == "}")
				{
					num2--;
					if (num2 == 0)
					{
						break;
					}
				}
				if (!flag2 && lexTree3.Token == eLex.ID)
				{
					VFNode value2;
					if (!lexTree.Used.TryGetValue(lexTree3.Text, out value2))
					{
						if (!m_FunctionList.TryGetValue(lexTree3.Text, out value2))
						{
							value2 = new VFNode(lexTree3.Text);
							m_FunctionList.Add(lexTree3.Text, value2);
						}
						lexTree.Used.Add(lexTree3.Text, value2);
					}
					value2.Count++;
				}
				else if (lexTree3.Text == "true")
				{
					lexTree3.Text = "!0";
				}
				else if (lexTree3.Text == "false")
				{
					lexTree3.Text = "!1";
				}
				_i++;
				if (_i >= _tokenlist.Count)
				{
					return _i;
				}
			}
			num2 = _i + 1 - num;
			if (flag)
			{
				List<LexTree> range = _tokenlist.GetRange(num + 1, num2);
				_tokenlist.RemoveRange(num + 1, num2);
				lexTree.Children = new List<LexTree>(num2);
				lexTree.Children.AddRange(range);
				_i = num;
			}
			return _i;
		}

		public int RemoveFunctionReferences(List<LexTree> _tokenlist, int _i)
		{
			LexTree lexTree = _tokenlist[_i];
			if (lexTree.Children == null)
			{
				return _i;
			}
			if (lexTree.Node.Count == 0 && !lexTree.Node.AlreadyRemoved)
			{
				lexTree.Node.AlreadyRemoved = true;
				{
					foreach (KeyValuePair<string, VFNode> item in lexTree.Used)
					{
						item.Value.Count--;
						if (item.Value.Count <= 0 && item.Value.pFunction != null)
						{
							m_FunctionRemoved = true;
							if (Verbose)
							{
								foreach (LexTree item2 in item.Value.pFunction)
								{
									Program.Out.WriteLine("*Function: " + item2.Name);
								}
							}
						}
					}
					return _i;
				}
			}
			return _i;
		}

		public int RemoveFunctions(List<LexTree> _tokenlist, int _i, int _count)
		{
			LexTree lexTree = _tokenlist[_i];
			if (lexTree.Children == null)
			{
				return _i;
			}
			if (lexTree.Node.Count == 0)
			{
				_tokenlist.RemoveRange(_i, 1);
				_i--;
				if (Verbose)
				{
					Program.Out.WriteLine("Function: " + lexTree.Name);
				}
			}
			return _i;
		}

		public void AddCount(List<LexTree> _tokenlist, int _i)
		{
			LexTree lexTree = _tokenlist[_i];
			if (lexTree.Text != null)
			{
				VFNode value;
				if (!m_FunctionList.TryGetValue(lexTree.Text, out value))
				{
					value = new VFNode(lexTree.Text);
					m_FunctionList.Add(lexTree.Text, value);
				}
				value.Count++;
			}
		}

		public int CheckHex(List<LexTree> _tokenlist, int _i)
		{
			LexTree lexTree = _tokenlist[_i];
			if (lexTree.Text.Length < 3)
			{
				return _i;
			}
			string value;
			if (lexTree.Text[0] == '0' && (lexTree.Text[1] == 'x' || lexTree.Text[1] == 'X') && m_MaskConvert.TryGetValue(lexTree.Text, out value))
			{
				lexTree.Text = value;
			}
			return _i;
		}

		private void DoPassOne(List<LexTree> _tokenlist)
		{
			for (int i = 0; i < _tokenlist.Count; i++)
			{
				bool flag = true;
				LexTree lexTree = _tokenlist[i];
				if (lexTree.Text == "function")
				{
					i = ParseFunction(_tokenlist, i);
					flag = false;
				}
				else if (lexTree.Text == "prototype")
				{
					i = ParseProtoTypeFunction(_tokenlist, i);
					flag = false;
				}
				else if (lexTree.Token == eLex.ID)
				{
					AddCount(_tokenlist, i);
				}
				else if (lexTree.Text == "true")
				{
					lexTree.Text = "!0";
				}
				else if (lexTree.Text == "false")
				{
					lexTree.Text = "!1";
				}
				if (flag && lexTree.Children != null)
				{
					DoPassOne(lexTree.Children);
				}
			}
		}

		private void DoPassTwo(List<LexTree> _tokenlist, int _count)
		{
			for (int i = 0; i < _tokenlist.Count; i++)
			{
				LexTree lexTree = _tokenlist[i];
				if (_count == 0 && lexTree.Text == "function")
				{
					i = RemoveFunctionReferences(_tokenlist, i);
				}
				if (lexTree.Children != null)
				{
					DoPassTwo(lexTree.Children, _count + 1);
				}
			}
		}

		private void DoPassThree(List<LexTree> _tokenlist, int _count)
		{
			for (int i = 0; i < _tokenlist.Count; i++)
			{
				LexTree lexTree = _tokenlist[i];
				eLex eLex = eLex.None;
				if (i + 1 < _tokenlist.Count)
				{
					eLex = _tokenlist[i + 1].Token;
				}
				if (_count == 0 && lexTree.Node != null)
				{
					i = RemoveFunctions(_tokenlist, i, _count);
				}
				if (lexTree.Token == eLex.SemiColon && eLex == eLex.RCB)
				{
					_tokenlist.RemoveRange(i, 1);
					i--;
				}
				if (lexTree.Children != null)
				{
					DoPassThree(lexTree.Children, _count + 1);
				}
			}
		}

		private void ScanFile()
		{
			if (RemovedUnused)
			{
				DoPassOne(m_TokenTree);
				m_FunctionRemoved = true;
				while (m_FunctionRemoved)
				{
					m_FunctionRemoved = false;
					DoPassTwo(m_TokenTree, 0);
				}
				DoPassThree(m_TokenTree, 0);
			}
		}

		private unsafe void LoadFiles(string[] args)
		{
			if (EncodeStrings)
			{
				m_Files.Add(m_DecodeFunction);
			}
			string directoryName = Path.GetDirectoryName(args[0]);
			string fileName = Path.GetFileName(args[0]);
			string[] files = Directory.GetFiles(directoryName, fileName);
			Array.Sort(files);
			string[] array = files;
			foreach (string path in array)
			{
				byte[] array2 = File.ReadAllBytes(path);
				string item;
				try
				{
					fixed (byte* ptr = &array2[0])
					{
						int startIndex = 0;
						int num = array2.Length;
						if (*ptr == 239 && ptr[1] == 187 && ptr[2] == 191)
						{
							startIndex = 3;
							num -= 3;
						}
						item = new string((sbyte*)ptr, startIndex, num);
					}
				}
				finally
				{
				}
				m_Files.Add(item);
			}
		}

		public void WriteNodeList(List<LexTree> _nodelist, StreamWriter oFile)
		{
			int i = 0;
			LexTree lexTree = new LexTree(eLex.None, " ");
			for (; i < _nodelist.Count; i++)
			{
				LexTree lexTree2 = _nodelist[i];
				GMAssetCompiler.eLex eLexVar = lexTree2.Token;
				if (PrettyPrint)
				{
					if (eLexVar != eLex.NewLine && eLexVar != eLex.Comment)
					{
						if (m_WriteSpace && eLexVar != eLex.LeftBracket && eLexVar != eLex.SemiColon)
						{
							oFile.Write(" ");
						}
						m_WriteSpace = false;
						if (eLexVar == eLex.reserved3)
						{
							oFile.Write(" ");
							eLexVar = eLex.reserved2;
						}
						if (eLexVar == eLex.reserved2 && lexTree2.Text == "function" && lexTree.Text != "return")
						{
							oFile.WriteLine();
						}
						if (eLexVar == eLex.LCB)
						{
							oFile.WriteLine();
							for (int j = 0; j < m_tabcount; j++)
							{
								oFile.Write('\t');
							}
						}
						if (eLexVar == eLex.RCB)
						{
							m_tabcount--;
							m_WriteTabs = m_tabcount;
						}
						if (m_WriteTabs > 0)
						{
							for (int k = 0; k < m_WriteTabs; k++)
							{
								oFile.Write('\t');
							}
							m_WriteTabs = 0;
						}
						oFile.Write(lexTree2.Text);
						if (lexTree2.Text == "for" || lexTree2.Text == "while")
						{
							bool flag = false;
							int num = 0;
							while (!flag && i < _nodelist.Count)
							{
								i++;
								LexTree lexTree3 = _nodelist[i];
								eLex token = lexTree3.Token;
								if (token == eLex.LeftBracket)
								{
									num++;
								}
								if (token == eLex.RightBracket)
								{
									num--;
									if (num == 0)
									{
										flag = true;
									}
								}
								if (token == eLex.reserved3)
								{
									oFile.Write(" ");
								}
								oFile.Write(lexTree3.Text);
								if (token == eLex.reserved3 || token == eLex.reserved2)
								{
									oFile.Write(" ");
								}
							}
						}
						if (eLexVar == eLex.SemiColon || eLexVar == eLex.LCB || eLexVar == eLex.RCB)
						{
							if (eLexVar == eLex.LCB)
							{
								m_tabcount++;
							}
							oFile.WriteLine();
							m_WriteTabs = m_tabcount;
						}
					}
					if (eLexVar == eLex.reserved2)
					{
						m_WriteSpace = true;
					}
				}
				else
				{
					if (m_WriteSpace && eLexVar != eLex.LeftBracket && eLexVar != eLex.SemiColon)
					{
						oFile.Write(" ");
					}
					m_WriteSpace = false;
					if (eLexVar == eLex.reserved3)
					{
						oFile.Write(" ");
						eLexVar = eLex.reserved2;
					}
					if (eLexVar != eLex.NewLine && eLexVar != eLex.Comment && lexTree2.Text != null)
					{
						oFile.Write(lexTree2.Text);
						m_LineWidth += lexTree2.Text.Length;
					}
					if (eLexVar == eLex.reserved2)
					{
						m_WriteSpace = true;
					}
					if (m_LineWidth >= UglyWidth)
					{
						bool flag2 = true;
						if (eLexVar != eLex.SemiColon && eLexVar != eLex.Comma && eLexVar != eLex.RCB && eLexVar != eLex.LCB)
						{
							flag2 = false;
						}
						if (flag2)
						{
							m_LineWidth = 0;
							oFile.WriteLine();
							m_WriteSpace = false;
						}
					}
				}
				lexTree = _nodelist[i];
				if (lexTree2.Node != null && lexTree2.Children != null)
				{
					WriteNodeList(lexTree2.Children, oFile);
				}
			}
		}

		public void WriteOut_MaskLookup(StreamWriter _oFile)
		{
			_oFile.Write("var ");
			bool flag = true;
			foreach (KeyValuePair<string, string> item in m_MaskConvert)
			{
				if (!flag)
				{
					_oFile.Write(",");
				}
				_oFile.Write(item.Value + "=" + item.Key);
				flag = false;
			}
			_oFile.Write(";");
			if (PrettyPrint)
			{
				_oFile.WriteLine();
			}
		}

		public void DoObfuscate(IEnumerable<string> _files, string _outputfilename, IEnumerable<string> _obfuscateWhiteList)
		{
			List<string> list = new List<string>();
			if (EncodeStrings)
			{
				list.Add(m_DecodeFunction);
			}
			list.AddRange(_files);
			foreach (string _obfuscateWhite in _obfuscateWhiteList)
			{
				m_pLex.AddCommand(_obfuscateWhite, eLex.reserved);
			}
			using (StreamWriter streamWriter = new StreamWriter(_outputfilename))
			{
				foreach (string item2 in list)
				{
					m_pLex.Text = item2;
					eLex eLex;
					do
					{
						eLex = m_pLex.yylex();
						LexTree item = new LexTree(eLex, m_pLex.yyText);
						m_TokenTree.Add(item);
					}
					while (eLex != eLex.EOF);
				}
				ScanFile();
				DoObfuscation(m_TokenTree);
				if (Obfuscate)
				{
					string value = "";
					if (m_Encoder.TryGetValue(m_StringTableName, out value))
					{
						m_StringTableName = value;
					}
				}
				DoEncodeStrings(m_TokenTree);
				if (EncodeStrings)
				{
					WriteStringTable(streamWriter);
				}
				WriteNodeList(m_TokenTree, streamWriter);
				streamWriter.Close();
			}
		}
	}
}
