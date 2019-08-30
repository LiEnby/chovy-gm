using GMAssetCompiler.Output;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GMAssetCompiler
{
	internal class HTML5Saver
	{
		private delegate void WriteDelegateKVP<T>(KeyValuePair<string, T> _kvp, TextWriter _s, int _n);

		private delegate void WriteDelegate<T>(T _t, TextWriter _s, int _n);

		private enum Codepage
		{
			ANSI_CHARSET = 0,
			DEFAULT_CHARSET = 1,
			EASTEUROPE_CHARSET = 238,
			RUSSIAN_CHARSET = 204,
			SYMBOL_CHARSET = 2,
			SHIFTJIS_CHARSET = 0x80,
			HANGEUL_CHARSET = 129,
			GB2312_CHARSET = 134,
			CHINESEBIG5_CHARSET = 136,
			JOHAB_CHARSET = 130,
			HEBREW_CHARSET = 177,
			ARABIC_CHARSET = 178,
			GREEK_CHARSET = 161,
			TURKISH_CHARSET = 162,
			VIETNAMESE_CHARSET = 163,
			THAI_CHARSET = 222,
			MAC_CHARSET = 77,
			BALTIC_CHARSET = 186,
			OEM_CHARSET = 0xFF
		}

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

		private const int EVENT_STEP_NORMAL = 0;

		private const int EVENT_STEP_BEGIN = 1;

		private const int EVENT_STEP_END = 2;

		private const int EVENT_OTHER_OUTSIDE = 0;

		private const int EVENT_OTHER_BOUNDARY = 1;

		private const int EVENT_OTHER_STARTGAME = 2;

		private const int EVENT_OTHER_ENDGAME = 3;

		private const int EVENT_OTHER_STARTROOM = 4;

		private const int EVENT_OTHER_ENDROOM = 5;

		private const int EVENT_OTHER_NOLIVES = 6;

		private const int EVENT_OTHER_ANIMATIONEND = 7;

		private const int EVENT_OTHER_ENDOFPATH = 8;

		private const int EVENT_OTHER_NOHEALTH = 9;

		private const int EVENT_OTHER_CLOSEBUTTON = 30;

		private const int EVENT_OTHER_OUTSIDE_VIEW0 = 40;

		private const int EVENT_OTHER_BOUNDARY_VIEW0 = 50;

		private const int EVENT_OTHER_USER0 = 10;

		private const int EVENT_OTHER_USER1 = 11;

		private const int EVENT_OTHER_USER2 = 12;

		private const int EVENT_OTHER_USER3 = 13;

		private const int EVENT_OTHER_USER4 = 14;

		private const int EVENT_OTHER_USER5 = 15;

		private const int EVENT_OTHER_USER6 = 16;

		private const int EVENT_OTHER_USER7 = 17;

		private const int EVENT_OTHER_USER8 = 18;

		private const int EVENT_OTHER_USER9 = 19;

		private const int EVENT_OTHER_USER10 = 20;

		private const int EVENT_OTHER_USER11 = 21;

		private const int EVENT_OTHER_USER12 = 22;

		private const int EVENT_OTHER_USER13 = 23;

		private const int EVENT_OTHER_USER14 = 24;

		private const int EVENT_OTHER_USER15 = 25;

		private const int EVENT_OTHER_WEB_IMAGE_LOADED = 60;

		private const int EVENT_OTHER_WEB_SOUND_LOADED = 61;

		private const int EVENT_OTHER_WEB_ASYNC = 62;

		private const int MOUSE_LeftButton = 0;

		private const int MOUSE_RightButton = 1;

		private const int MOUSE_MiddleButton = 2;

		private const int MOUSE_NoButton = 3;

		private const int MOUSE_LeftPressed = 4;

		private const int MOUSE_RightPressed = 5;

		private const int MOUSE_MiddlePressed = 6;

		private const int MOUSE_LeftReleased = 7;

		private const int MOUSE_RightReleased = 8;

		private const int MOUSE_MiddleReleased = 9;

		private const int MOUSE_MouseEnter = 10;

		private const int MOUSE_MouseLeave = 11;

		private const int MOUSE_Joystick1Left = 16;

		private const int MOUSE_Joystick1Right = 17;

		private const int MOUSE_Joystick1Up = 18;

		private const int MOUSE_Joystick1Down = 19;

		private const int MOUSE_Joystick1Button1 = 21;

		private const int MOUSE_Joystick1Button2 = 22;

		private const int MOUSE_Joystick1Button3 = 23;

		private const int MOUSE_Joystick1Button4 = 24;

		private const int MOUSE_Joystick1Button5 = 25;

		private const int MOUSE_Joystick1Button6 = 26;

		private const int MOUSE_Joystick1Button7 = 27;

		private const int MOUSE_Joystick1Button8 = 28;

		private const int MOUSE_Joystick2Left = 31;

		private const int MOUSE_Joystick2Right = 32;

		private const int MOUSE_Joystick2Up = 33;

		private const int MOUSE_Joystick2Down = 34;

		private const int MOUSE_Joystick2Button1 = 36;

		private const int MOUSE_Joystick2Button2 = 37;

		private const int MOUSE_Joystick2Button3 = 38;

		private const int MOUSE_Joystick2Button4 = 39;

		private const int MOUSE_Joystick2Button5 = 40;

		private const int MOUSE_Joystick2Button6 = 41;

		private const int MOUSE_Joystick2Button7 = 42;

		private const int MOUSE_Joystick2Button8 = 43;

		private const int MOUSE_GlobLeftButton = 50;

		private const int MOUSE_GlobRightButton = 51;

		private const int MOUSE_GlobMiddleButton = 52;

		private const int MOUSE_GlobLeftPressed = 53;

		private const int MOUSE_GlobRightPressed = 54;

		private const int MOUSE_GlobMiddlePressed = 55;

		private const int MOUSE_GlobLeftReleased = 56;

		private const int MOUSE_GlobRightReleased = 57;

		private const int MOUSE_GlobMiddleReleased = 58;

		private const int MOUSE_MouseWheelUp = 60;

		private const int MOUSE_MouseWheelDown = 61;

		private static TexturePage ms_tpageSprites;

		private static List<Wave> ms_Waves = new List<Wave>();

		private static List<string> ms_SoundFilenames = new List<string>();

		private static bool ms_fWriteNulls = true;

		private static Dictionary<string, GMLCode> ms_codeToCompile = new Dictionary<string, GMLCode>();

		private static string[] event2Name = new string[12]
		{
			"Create",
			"Destroy",
			"Alarm",
			"Step",
			"Collision",
			"Keyboard",
			"Mouse",
			"Other",
			"Draw",
			"KeyPress",
			"KeyRelease",
			"Trigger"
		};

		private static string baseDir = string.Empty;

		private static string baseGameName = string.Empty;

		private static List<string> ms_obfuscateKeywords = new List<string>();

		private static GMAssets ms_assets;

		private static Dictionary<string, string> ms_simpleConstants = new Dictionary<string, string>();

		public static GMAssets Assets
		{
			get;
			private set;
		}

		private static string EnsureValidId(string _name)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _name.Length; i++)
			{
				char c = _name[i];
				if (!char.IsLetterOrDigit(c) && c != '_')
				{
					c = '_';
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		public static void Save(GMAssets _assets, string _name)
		{
			if (Program.RemoveDND)
			{
				Assets = _assets;
				_assets.RemoveDND();
			}
			string fileName = Path.GetFileName(_name);
			string text = Path.Combine(Program.OutputDir, "html5game");
			string text2 = Path.Combine(Program.OutputDir, "scripts");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			fileName = Path.Combine(text, fileName);
			Program.Out.WriteLine("Saving HTML5 file... {0}", fileName);
			ms_tpageSprites = new TexturePage(2, 2, 0, 0, Program.MachineType.TPageWidth, Program.MachineType.TPageHeight);
			ms_Waves.Clear();
			ms_SoundFilenames.Clear();
			ms_codeToCompile.Clear();
			ms_obfuscateKeywords.Clear();
			TexturePageEntry.ms_count = 0;
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			StringWriter s = new StringWriter(stringBuilder);
			baseDir = text;
			baseGameName = Path.GetFileNameWithoutExtension(fileName);
			Save(_assets, s);
			if (!Program.DoObfuscate)
			{
				File.WriteAllText(fileName, stringBuilder.ToString());
			}
			else
			{
				list.Add(stringBuilder.ToString());
			}
			if (!Program.NoIndexHTML)
			{
				GMRoom value = _assets.Rooms[_assets.RoomOrder[0]].Value;
				int num = value.Width;
				int num2 = value.Height;
				if (value.EnableViews)
				{
					num = value.Views[0].WPort;
					num2 = value.Views[0].HPort;
				}
				StreamWriter streamWriter = File.CreateText(Path.Combine(Program.OutputDir, "index.html"));
				streamWriter.WriteLine("<!DOCTYPE html>");
				streamWriter.WriteLine("<html lang=\"en\">");
				streamWriter.WriteLine("    <head>");
				streamWriter.WriteLine("        <!-- Generated by GameMaker:HTML5 http://www.yoyogames.com/gamemaker/html5 -->");
				streamWriter.WriteLine("        <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />");
				if (Program.NoCache)
				{
					streamWriter.WriteLine("        <meta http-equiv=\"pragma\" content=\"no-cache\"/>");
				}
				streamWriter.WriteLine("        <meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />");
				streamWriter.WriteLine("        <meta name =\"viewport\" content=\"width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;\" />");
				streamWriter.WriteLine("        <meta name=\"apple-mobile-web-app-status-bar-style\" content=\"black-translucent\" />");
				streamWriter.WriteLine("        <meta charset=\"utf-8\"/>");
				streamWriter.WriteLine("");
				streamWriter.WriteLine("        <!-- Set the title bar of the page -->");
				streamWriter.WriteLine("        <title>Game Maker HTML 5</title>");
				streamWriter.WriteLine("");
				streamWriter.WriteLine("        <!-- Set the background colour of the document -->");
				streamWriter.WriteLine("        <style>");
				streamWriter.WriteLine("            body {");
				streamWriter.WriteLine("              background: #222;");
				streamWriter.WriteLine("              color:#cccccc;");
				streamWriter.WriteLine("              margin: 0px;");
				streamWriter.WriteLine("              padding: 0px;");
				streamWriter.WriteLine("              border: 0px;");
				streamWriter.WriteLine("            }");
				streamWriter.WriteLine("            canvas {");
				streamWriter.WriteLine("                      image-rendering: optimizeSpeed;");
				streamWriter.WriteLine("                      -webkit-interpolation-mode: nearest-neighbor;");
				streamWriter.WriteLine("                      margin: 0px;");
				streamWriter.WriteLine("                      padding: 0px;");
				streamWriter.WriteLine("                      border: 0px;");
				streamWriter.WriteLine("            }");
				streamWriter.WriteLine("            div.gm4html5_div_class");
				streamWriter.WriteLine("            {");
				if (Program.CenterHTML5Game)
				{
					streamWriter.WriteLine("              position: fixed;");
					streamWriter.WriteLine("              top: 50%;");
					streamWriter.WriteLine("              left: 50%;");
					streamWriter.WriteLine("              margin-left: -{0}px;", num / 2);
					streamWriter.WriteLine("              margin-top: -{0}px;", num2 / 2);
				}
				else
				{
					streamWriter.WriteLine("              margin: 0px;");
				}
				streamWriter.WriteLine("              padding: 0px;");
				streamWriter.WriteLine("              border: 0px;");
				streamWriter.WriteLine("            }");
				streamWriter.WriteLine("            :-webkit-full-screen {");
				streamWriter.WriteLine("               width: 100%;");
				streamWriter.WriteLine("               height: 100%;");
				streamWriter.WriteLine("            }");
				streamWriter.WriteLine("        </style>");
				streamWriter.WriteLine("    </head>");
				streamWriter.WriteLine("");
				streamWriter.WriteLine("    <body>");
				streamWriter.WriteLine("        <div class=\"gm4html5_div_class\" id=\"gm4html5_div_id\">");
				bool centerHTML5Game = Program.CenterHTML5Game;
				if (_assets.Options.LoadImage != null && Program.CustomLoadingScreen)
				{
					streamWriter.WriteLine("        <img src=\"html5game/loadingscreen.png\" id=\"GM4HTML5_loadingscreen\" alt=\"GameMaker:HTML5 loading screen\" style=\"display:none;\"/>");
				}
				streamWriter.WriteLine("            <!-- Create the canvas element the game draws to -->");
				streamWriter.WriteLine("            <canvas id=\"canvas\" width=\"{0}\" height=\"{1}\">", num, num2);
				streamWriter.WriteLine("               <p>Your browser doesn't support HTML5 canvas.</p>");
				streamWriter.WriteLine("            </canvas>");
				bool centerHTML5Game2 = Program.CenterHTML5Game;
				streamWriter.WriteLine("        </div>");
				streamWriter.WriteLine("");
				streamWriter.WriteLine("        <!-- Run the game code -->");
				Random random = new Random();
				random.Next();
				streamWriter.WriteLine("        <script type=\"text/javascript\" src=\"html5game/{0}?{1}={2}\"></script>", Path.GetFileName(_name), NewName(874871 + random.Next(61521)), random.Next(int.MaxValue));
				if (!Program.DoObfuscate)
				{
					streamWriter.WriteLine("        <script type=\"text/javascript\" src=\"scripts/runner.js\"></script>");
				}
				streamWriter.WriteLine("    </body>");
				streamWriter.WriteLine("</html>");
				streamWriter.Close();
			}
			if (!string.IsNullOrEmpty(Program.HTMLRunner))
			{
				if (!Program.DoObfuscate)
				{
					if (File.Exists(Program.HTMLRunner))
					{
						UnzipFromDir(text2, Program.HTMLRunner, new string[1]
						{
							".svn"
						});
					}
					else
					{
						CopyDirFromDir(text2, Program.HTMLRunner, new string[1]
						{
							".svn"
						});
					}
				}
				else if (File.Exists(Program.HTMLRunner))
				{
					GetFilesFromZip(Program.HTMLRunner, new string[3]
					{
						".svn",
						"runner.js",
						"particles"
					}, list);
				}
				else
				{
					GetFiles(Program.HTMLRunner, new string[3]
					{
						".svn",
						"runner.js",
						"particles"
					}, list);
				}
			}
			if (_assets.Debug && list.Count == 2)
			{
				Program.Out.WriteLine("Debug version skipping obfuscation...");
				stringBuilder.Length = 0;
				stringBuilder.Append(list[0]);
				stringBuilder.AppendLine("");
				stringBuilder.Append(list[1]);
				File.WriteAllText(fileName, stringBuilder.ToString());
			}
			else if (Program.DoObfuscate)
			{
				Program.Out.WriteLine("Obfuscating...");
				YYObfuscate yYObfuscate = new YYObfuscate();
				yYObfuscate.Obfuscate = Program.ObfuscateObfuscate;
				yYObfuscate.Verbose = Program.Verbose;
				yYObfuscate.PrettyPrint = Program.ObfuscatePrettyPrint;
				yYObfuscate.RemovedUnused = Program.ObfuscateRemoveUnused;
				yYObfuscate.EncodeStrings = Program.ObfuscateEncodeStrings;
				yYObfuscate.DoObfuscate(list, fileName, ms_obfuscateKeywords);
			}
			if (File.Exists(Program.HTMLRunner))
			{
				CopyDirFromZip(Path.Combine(text, "particles"), Program.HTMLRunner, "particles", new string[1]
				{
					".svn"
				});
			}
			else
			{
				CopyDirFromDir(Path.Combine(text, "particles"), Path.Combine(Program.HTMLRunner, "particles"), new string[1]
				{
					".svn"
				});
			}
			Program.Out.WriteLine("Done! ");
		}

		private static string NewName(int _count)
		{
			string text = "";
			for (int num = _count; num > 0; num /= 26)
			{
				text += (char)(num % 26 + 65);
			}
			_count++;
			return text;
		}

		private static string DecodeString(string _inp)
		{
			string text = "";
			for (int i = 0; i < _inp.Length; i++)
			{
				text += (char)(_inp[i] ^ 0x1A);
			}
			return text;
		}

		public static void GetFiles(string _sourceDir, string[] _excludes, List<string> _files)
		{
			string[] directories = Directory.GetDirectories(_sourceDir);
			string[] array = directories;
			foreach (string path in array)
			{
				if (!_excludes.Contains(Path.GetFileName(path)))
				{
					string sourceDir = Path.Combine(_sourceDir, Path.GetFileName(path));
					GetFiles(sourceDir, _excludes, _files);
				}
			}
			string[] files = Directory.GetFiles(_sourceDir);
			string[] array2 = files;
			foreach (string path2 in array2)
			{
				if (!_excludes.Contains(Path.GetFileName(path2)))
				{
					_files.Add(File.ReadAllText(path2));
				}
			}
		}

		private static void GetFilesFromZip(string _sourceZip, string[] _excludes, List<string> _files)
		{
			try
			{
				using (ZipInputStream zipInputStream = new ZipInputStream(_sourceZip))
				{
					zipInputStream.Password = DecodeString("+\"(9EZjZu)m>s>hE[^^7E>9");
					ZipEntry nextEntry;
					while ((nextEntry = zipInputStream.GetNextEntry()) != null)
					{
						string[] array = nextEntry.FileName.Split(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
						bool flag = false;
						string[] array2 = array;
						foreach (string value in array2)
						{
							flag = (flag || _excludes.Contains(value));
						}
						if (!nextEntry.IsDirectory && !flag)
						{
							byte[] buffer = new byte[nextEntry.UncompressedSize];
							zipInputStream.Read(buffer, 0, (int)nextEntry.UncompressedSize);
							MemoryStream stream = new MemoryStream(buffer);
							StreamReader streamReader = new StreamReader(stream);
							_files.Add(streamReader.ReadToEnd());
						}
					}
				}
			}
			catch
			{
			}
		}

		private static void CopyDirFromDir(string _destDir, string _sourceDir, string[] _excludes)
		{
			if (!Directory.Exists(_destDir))
			{
				Directory.CreateDirectory(_destDir);
			}
			try
			{
				string[] directories = Directory.GetDirectories(_sourceDir);
				string[] array = directories;
				foreach (string path in array)
				{
					if (!_excludes.Contains(Path.GetFileName(path)))
					{
						string destDir = Path.Combine(_destDir, Path.GetFileName(path));
						string sourceDir = Path.Combine(_sourceDir, Path.GetFileName(path));
						CopyDirFromDir(destDir, sourceDir, _excludes);
					}
				}
				string[] files = Directory.GetFiles(_sourceDir);
				string[] array2 = files;
				foreach (string text in array2)
				{
					string dest = Path.Combine(_destDir, Path.GetFileName(text));
					CopyFile(text, dest);
				}
			}
			catch (IOException ex)
			{
				Console.WriteLine("CopyDirFromDir File {0}->{1} :: Caught IOException : {0}", _destDir, _sourceDir, ex.ToString());
			}
		}

		private static void UnzipFromDir(string _destDir, string _sourceZip, string[] _excludes)
		{
			if (!Directory.Exists(_destDir))
			{
				Directory.CreateDirectory(_destDir);
			}
			byte[] array = new byte[2048];
			using (ZipInputStream zipInputStream = new ZipInputStream(_sourceZip))
			{
				zipInputStream.Password = DecodeString("+\"(9EZjZu)m>s>hE[^^7E>9");
				ZipEntry nextEntry;
				while ((nextEntry = zipInputStream.GetNextEntry()) != null)
				{
					string text = Path.Combine(_destDir, nextEntry.FileName);
					if (nextEntry.IsDirectory)
					{
						if (!Directory.Exists(text) && !_excludes.Contains(Path.GetFileName(text)))
						{
							Directory.CreateDirectory(text);
						}
					}
					else if (!_excludes.Contains(Path.GetFileName(text)))
					{
						using (FileStream fileStream = File.Open(text, FileMode.Create, FileAccess.ReadWrite))
						{
							long position = zipInputStream.Position;
							long position2 = fileStream.Position;
							bool flag = false;
							while (!flag)
							{
								zipInputStream.Position = position;
								fileStream.Position = position2;
								try
								{
									int count;
									while ((count = zipInputStream.Read(array, 0, array.Length)) > 0)
									{
										fileStream.Write(array, 0, count);
									}
									flag = true;
								}
								catch (IOException ex)
								{
									Console.WriteLine("IOException while unzipping file {0} : {1}", text, ex.ToString());
								}
							}
						}
					}
				}
			}
		}

		private static void CopyDirFromZip(string _destDir, string _sourceZip, string _sourceDir, string[] _excludes)
		{
			if (!Directory.Exists(_destDir))
			{
				Directory.CreateDirectory(_destDir);
			}
			byte[] array = new byte[2048];
			using (ZipInputStream zipInputStream = new ZipInputStream(_sourceZip))
			{
				zipInputStream.Password = DecodeString("+\"(9EZjZu)m>s>hE[^^7E>9");
				ZipEntry nextEntry;
				while ((nextEntry = zipInputStream.GetNextEntry()) != null)
				{
					if (nextEntry.FileName.StartsWith(_sourceDir))
					{
						string text = Path.Combine(_destDir, nextEntry.FileName.Remove(0, _sourceDir.Length + 1));
						if (nextEntry.IsDirectory)
						{
							if (!Directory.Exists(text) && !_excludes.Contains(Path.GetFileName(text)))
							{
								Directory.CreateDirectory(text);
							}
						}
						else if (!_excludes.Contains(Path.GetFileName(text)))
						{
							using (FileStream fileStream = File.Open(text, FileMode.Create, FileAccess.ReadWrite))
							{
								long position = zipInputStream.Position;
								long position2 = fileStream.Position;
								bool flag = false;
								while (!flag)
								{
									zipInputStream.Position = position;
									fileStream.Position = position2;
									try
									{
										int count;
										while ((count = zipInputStream.Read(array, 0, array.Length)) > 0)
										{
											fileStream.Write(array, 0, count);
										}
										flag = true;
									}
									catch (IOException ex)
									{
										Console.WriteLine("IOException while unzipping file {0} : {1}", text, ex.ToString());
									}
								}
							}
						}
					}
				}
			}
		}

		private static void CopyFile(string f, string dest)
		{
			bool flag = false;
			while (!flag)
			{
				try
				{
					File.Copy(f, dest, true);
					flag = true;
				}
				catch (IOException ex)
				{
					Console.WriteLine("Copy File {0}->{1} :: Caught IOException : {0}", f, dest, ex.ToString());
				}
			}
		}

		private static void WriteDataKVP<T>(IList<KeyValuePair<string, T>> _data, TextWriter _s, WriteDelegateKVP<T> _del)
		{
			int num = 0;
			foreach (KeyValuePair<string, T> _datum in _data)
			{
				if (_datum.Value != null)
				{
					_del(_datum, _s, num);
				}
				else if (ms_fWriteNulls)
				{
					if (num > 0)
					{
						_s.WriteLine(",");
					}
					_s.Write("\t\tnull");
				}
				num++;
			}
		}

		private static void WriteDataList<T>(IList<T> _data, TextWriter _s, WriteDelegate<T> _del)
		{
			int num = 0;
			foreach (T _datum in _data)
			{
				_del(_datum, _s, num);
				num++;
			}
		}

		private static void WriteHeader(GMAssets _assets, TextWriter _s)
		{
		}

		private static bool IsSimpleConstant(GMLCode _code, out string _ret)
		{
			bool result = false;
			_ret = "";
			if (_code.Token != null && _code.Token.Token == eToken.eBlock && _code.Token.Children.Count == 1 && _code.Token.Children[0].Token == eToken.eReturn && _code.Token.Children[0].Children.Count == 1 && _code.Token.Children[0].Children[0].Token == eToken.eConstant)
			{
				result = true;
				switch (_code.Token.Children[0].Children[0].Value.Kind)
				{
				case eKind.eNumber:
					_ret = Convert.ToString(_code.Token.Children[0].Children[0].Value.ValueI, CultureInfo.InvariantCulture.NumberFormat);
					break;
				case eKind.eString:
					_ret = _code.Token.Children[0].Children[0].Value.ValueS;
					if (_ret[0] != '"')
					{
						_ret = '"' + _ret + '"';
					}
					break;
				}
			}
			return result;
		}

		private static void WriteOptions(GMAssets _assets, TextWriter _s)
		{
			_s.Write("\tOptions: {");
			_s.WriteLine("\t\tdebugMode: {0},", _assets.Debug.ToString().ToLower());
			_s.WriteLine("\t\tgameId: {0},", _assets.GameID.ToString().ToLower());
			_s.WriteLine("\t\tgameGuid: \"{0}\",", _assets.GameGUID.ToString().ToLower());
			_s.WriteLine("\t\tfullScreen: {0},", _assets.Options.FullScreen.ToString().ToLower());
			_s.WriteLine("\t\tinterpolatePixels: {0},", _assets.Options.InterpolatePixels.ToString().ToLower());
			_s.WriteLine("\t\tshowCursor: {0},", _assets.Options.ShowCursor.ToString().ToLower());
			_s.WriteLine("\t\tscale: {0},", _assets.Options.Scale);
			_s.WriteLine("\t\tallowFullScreenKey: {0},", _assets.Options.ScreenKey.ToString().ToLower());
			_s.WriteLine("\t\tfreezeOnLostFocus: {0},", _assets.Options.Freeze.ToString().ToLower());
			_s.WriteLine("\t\tshowLoadingBar: {0},", _assets.Options.ShowProgress.ToString().ToLower());
			_s.WriteLine("\t\tdisplayErrors: {0},", _assets.Options.DisplayErrors.ToString().ToLower());
			_s.WriteLine("\t\twriteErrors: {0},", _assets.Options.WriteErrors.ToString().ToLower());
			_s.WriteLine("\t\tabortErrors: {0},", _assets.Options.AbortErrors.ToString().ToLower());
			_s.WriteLine("\t\tvariableErrors: {0},", _assets.Options.VariableErrors.ToString().ToLower());
			_s.WriteLine("\t\tWebGL: {0},", _assets.Options.WebGL.ToString().ToLower());
			_s.WriteLine("\t\tCreateEventOrder: {0}", _assets.Options.CreationEventOrder.ToString().ToLower());
			if (Program.LoadingBarName != null)
			{
				_s.WriteLine(",\t\tloadingBarCallback: \"{0}\"", Program.LoadingBarName);
			}
			_s.WriteLine("}");
			if (_assets.Options.LoadImage != null && Program.CustomLoadingScreen)
			{
				_assets.Options.LoadImage.Save(baseDir + "\\loadingscreen.png");
			}
			ms_simpleConstants.Clear();
			foreach (KeyValuePair<string, string> constant in _assets.Options.Constants)
			{
				int value = 0;
				if (GMLCompile.ms_ConstantCount.TryGetValue(constant.Key, out value) && value > 0)
				{
					GMLCode gMLCode = new GMLCode(ms_assets, "const_" + constant.Key, "return " + constant.Value, eGMLCodeType.eConstant);
					string _ret;
					if (!IsSimpleConstant(gMLCode, out _ret))
					{
						ms_codeToCompile.Add(gMLCode.Name, gMLCode);
					}
					else
					{
						ms_simpleConstants.Add(constant.Key, _ret);
					}
				}
			}
		}

		private static void WriteTextures(GMAssets _assets, TextWriter _s)
		{
			_s.Write("\tTextures: [");
			int num = 0;
			if (ms_tpageSprites.Textures != null)
			{
				foreach (Texture texture in ms_tpageSprites.Textures)
				{
					Program.Out.Write("{0} Compressing texture... ", num);
					Image _dest = null;
					byte[] bytes = Form1.createOutTexture(texture.Bitmap, eSquishFlags.kDxt5 | eSquishFlags.kColourMetricPerceptual | eSquishFlags.kClusterFitMaxIteration8, out _dest, Program.TextureType[texture.Group]);
					string path = Path.Combine(baseDir, string.Format("{2}_texture_{0}{1}", num, Program.TextureTypeExtension(Program.TextureType[texture.Group]), baseGameName));
					Program.Out.WriteLine("writing texture {0}... ", Path.GetFileName(path));
					if (num > 0)
					{
						_s.Write(", ");
					}
					_s.Write("\"{0}\"", Path.GetFileName(path));
					if (Program.WriteTextures)
					{
						if (string.Compare(Path.GetExtension(path), ".png", true) != 0)
						{
							texture.Bitmap.Save(Path.ChangeExtension(path, ".original.png"), ImageFormat.Png);
						}
						if (_dest != null)
						{
							_dest.Save(Path.ChangeExtension(path, ".png"), ImageFormat.Png);
						}
						File.WriteAllBytes(path, bytes);
					}
					Application.DoEvents();
					num++;
				}
			}
			_s.WriteLine("],");
		}

		private static void WriteWaveforms(GMAssets _assets, TextWriter _s)
		{
		}

		private static void WriteHelp(GMAssets _assets, TextWriter _s)
		{
		}

		private static void WriteTriggers(IList<GMTrigger> _triggers, TextWriter _s)
		{
			_s.Write("\tTriggers: [{ },");
			int num = 0;
			foreach (GMTrigger _trigger in _triggers)
			{
				if (num > 0)
				{
					_s.Write(",");
				}
				_s.Write("{");
				_s.Write("pName:\"{0}\",", _trigger.Name);
				_s.Write("moment: {0},", _trigger.Moment);
				_s.Write("constant: \"{0}\",", _trigger.ConstName);
				string text = "gml_Trigger_" + EnsureValidId(_trigger.Name);
				_s.Write("pFunc:{0}", text);
				GMLCode gMLCode = new GMLCode(ms_assets, text, _trigger.Condition, eGMLCodeType.eTrigger);
				ms_codeToCompile.Add(gMLCode.Name, gMLCode);
				_s.WriteLine("}");
				num++;
			}
			_s.WriteLine("],");
		}

		private static void WriteExtensions(IList<GMExtension> _extensions, TextWriter _s)
		{
			_s.Write("\tExtensions: [");
			List<string> list = new List<string>();
			List<byte[]> list2 = new List<byte[]>();
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>();
			int num = 0;
			foreach (GMExtension _extension in _extensions)
			{
				int num2 = 0;
				list.Clear();
				list2.Clear();
				foreach (GMExtensionInclude include in _extension.Includes)
				{
					if (!include.Filename.EndsWith(".dll") && !include.Filename.EndsWith(".gml") && _extension.ExtensionDLL[num2] != null)
					{
						File.WriteAllBytes(Path.Combine(baseDir, include.Filename), _extension.ExtensionDLL[num2]);
					}
					if (include.Filename.EndsWith(".js"))
					{
						list.Add(include.Filename);
					}
					if (include.Filename.EndsWith(".gml"))
					{
						list2.Add(_extension.ExtensionDLL[num2]);
						list3.Add(_extension.Includes[num2].Init);
						list4.Add(_extension.Includes[num2].Final);
						foreach (GMExtensionConstant constant in include.Constants)
						{
							ms_assets.Options.Constants[constant.Name] = constant.Value;
						}
					}
					num2++;
				}
				if (list.Count > 0)
				{
					string text = "";
					string text2 = "";
					foreach (GMExtensionInclude include2 in _extension.Includes)
					{
						foreach (GMExtensionFunction function in include2.Functions)
						{
							ms_obfuscateKeywords.Add(function.Name);
						}
						foreach (GMExtensionConstant constant2 in include2.Constants)
						{
							ms_obfuscateKeywords.Add(constant2.Name);
						}
						if (!string.IsNullOrEmpty(include2.Init))
						{
							text = include2.Init;
						}
						if (!string.IsNullOrEmpty(include2.Final))
						{
							text2 = include2.Final;
						}
					}
					if (num > 0)
					{
						_s.Write(",");
					}
					_s.WriteLine("\t{");
					_s.Write("\t\tjsFiles:[");
					num2 = 0;
					foreach (string item in list)
					{
						if (num2 > 0)
						{
							_s.Write(", ");
						}
						_s.Write("\"{0}\"", item);
						num2++;
					}
					_s.WriteLine("]");
					if (!string.IsNullOrEmpty(text))
					{
						_s.WriteLine(",\t\t init:\"{0}\"", text);
					}
					if (!string.IsNullOrEmpty(text2))
					{
						_s.WriteLine(",\t\tfinal:\"{0}\"", text2);
					}
					_s.WriteLine("}");
					num++;
				}
				if (list2.Count > 0)
				{
					for (num2 = 0; num2 < list2.Count; num2++)
					{
						byte[] bytes = list2[num2];
						string @string = Encoding.ASCII.GetString(bytes);
						string[] array = Regex.Split(@string, "\r\n");
						StringBuilder stringBuilder = new StringBuilder();
						string text3 = "";
						string[] array2 = array;
						foreach (string text4 in array2)
						{
							string text5 = text4.Trim();
							if (text5.StartsWith("#define"))
							{
								if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(stringBuilder.ToString()))
								{
									GMLCode gMLCode = new GMLCode(ms_assets, "gml_Script_" + text3, stringBuilder.ToString(), eGMLCodeType.eScript);
									ms_codeToCompile.Add(gMLCode.Name, gMLCode);
								}
								string[] array3 = text5.Split(' ', '\t');
								text3 = ((array3.Length <= 1) ? string.Empty : array3[1]);
								stringBuilder.Length = 0;
							}
							else
							{
								stringBuilder.AppendLine(text4);
							}
						}
						if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(stringBuilder.ToString()))
						{
							GMLCode gMLCode2 = new GMLCode(ms_assets, "gml_Script_" + text3, stringBuilder.ToString(), eGMLCodeType.eScript);
							ms_codeToCompile.Add(gMLCode2.Name, gMLCode2);
						}
						if (num > 0)
						{
							_s.Write(",");
						}
						_s.Write("\t{");
						if (!string.IsNullOrEmpty(list3[num2]))
						{
							_s.Write(" init:\"gml_Script_{0}\"", list3[num2]);
							ms_obfuscateKeywords.Add(string.Format("gml_Script_{0}", list3[num2]));
							if (!string.IsNullOrEmpty(list4[num2]))
							{
								_s.Write(",");
							}
						}
						if (!string.IsNullOrEmpty(list4[num2]))
						{
							_s.Write(" final:\"gml_Script_{0}\"", list4[num2]);
							ms_obfuscateKeywords.Add(string.Format("gml_Script_{0}", list4[num2]));
						}
						_s.WriteLine("}");
						num++;
					}
				}
			}
			_s.WriteLine("],");
		}

		private static void ConvertToMP3(string _exe, string _origName, GMSound _snd)
		{
			string text = Path.Combine(baseDir, string.Format("{0}{1}", Path.GetFileNameWithoutExtension(_origName), ".mp3"));
			if (Path.GetExtension(_origName) != ".mp3")
			{
				if (!File.Exists(text) || File.GetLastWriteTimeUtc(ms_assets.FileName) > File.GetLastWriteTimeUtc(text))
				{
					string tempFileName = Path.GetTempFileName();
					File.WriteAllBytes(tempFileName, _snd.Data);
					ProcessStartInfo processStartInfo = new ProcessStartInfo(_exe, string.Format("-y -i \"{0}\" -acodec libmp3lame -ab 128k \"{1}\"", tempFileName, text));
					processStartInfo.UseShellExecute = false;
					processStartInfo.RedirectStandardOutput = true;
					processStartInfo.RedirectStandardError = true;
					Process process = new Process();
					process.StartInfo = processStartInfo;
					process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs args)
					{
						if (Program.Verbose)
						{
							Program.Out.WriteLine(args.Data);
						}
					};
					process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs args)
					{
						if (Program.Verbose)
						{
							Program.Out.WriteLine(args.Data);
						}
					};
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					process.WaitForExit();
					File.Delete(tempFileName);
				}
			}
			else
			{
				File.WriteAllBytes(text, _snd.Data);
			}
		}

		private static void ConvertToOgg(string _exe, string _origName, GMSound _snd)
		{
			string text = Path.Combine(baseDir, string.Format("{0}{1}", Path.GetFileNameWithoutExtension(_origName), ".ogg"));
			if (Path.GetExtension(_origName) != ".ogg")
			{
				if (!File.Exists(text) || File.GetLastWriteTimeUtc(ms_assets.FileName) > File.GetLastWriteTimeUtc(text))
				{
					string tempFileName = Path.GetTempFileName();
					File.WriteAllBytes(tempFileName, _snd.Data);
					ProcessStartInfo processStartInfo = new ProcessStartInfo(_exe, string.Format("-y -i \"{0}\" -acodec libvorbis -aq 60 \"{1}\"", tempFileName, text));
					processStartInfo.UseShellExecute = false;
					processStartInfo.RedirectStandardOutput = true;
					processStartInfo.RedirectStandardError = true;
					Process process = new Process();
					process.StartInfo = processStartInfo;
					process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs args)
					{
						if (Program.Verbose)
						{
							Program.Out.WriteLine(args.Data);
						}
					};
					process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs args)
					{
						if (Program.Verbose)
						{
							Program.Out.WriteLine(args.Data);
						}
					};
					process.Start();
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
					process.WaitForExit();
					File.Delete(tempFileName);
				}
			}
			else
			{
				File.WriteAllBytes(text, _snd.Data);
			}
		}

		private static void WriteSounds(IList<KeyValuePair<string, GMSound>> _sounds, TextWriter _s)
		{
			_s.WriteLine("\tSounds: [");
			WriteDataKVP(_sounds, _s, delegate(KeyValuePair<string, GMSound> _kvp, TextWriter __s, int __n)
			{
				GMSound value = _kvp.Value;
				string text = Path.GetFileName(value.OrigName);
				bool flag = true;
				while (flag)
				{
					flag = false;
					foreach (string ms_SoundFilename in ms_SoundFilenames)
					{
						if (ms_SoundFilename == text)
						{
							text = string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(text), "_a", Path.GetExtension(text));
							flag = true;
						}
					}
				}
				if (value.Data != null)
				{
					string text2 = Path.Combine(baseDir, text);
					string text3 = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ffmpeg.exe");
					if (File.Exists(text3))
					{
						string text4 = Path.ChangeExtension(value.OrigName, ".mp3");
						string text5 = Path.ChangeExtension(value.OrigName, ".ogg");
						if (File.Exists(text4))
						{
							CopyFile(text4, Path.ChangeExtension(text2, ".mp3"));
						}
						if (File.Exists(text5))
						{
							CopyFile(text5, Path.ChangeExtension(text2, ".ogg"));
						}
						if (!File.Exists(text4) || !File.Exists(text5))
						{
							Program.Out.Write("Converting {0} to MP3...", text);
							ConvertToMP3(text3, text2, value);
							Program.Out.Write("OGG...");
							ConvertToOgg(text3, text2, value);
							Program.Out.WriteLine("Finished!");
						}
						text = Path.GetFileNameWithoutExtension(text);
					}
					else
					{
						File.WriteAllBytes(text2, value.Data);
					}
				}
				ms_SoundFilenames.Add(text);
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				_s.WriteLine("\t    {");
				_s.WriteLine("\t\tpName: \"{0}\",", _kvp.Key);
				_s.WriteLine("\t\tkind: {0},", value.Kind);
				_s.WriteLine("\t\textension: \"{0}\",", value.Extension);
				_s.WriteLine("\t\torigName: \"{0}\",", text);
				_s.WriteLine("\t\teffects: {0},", value.Effects);
				_s.WriteLine("\t\tvolume: {0},", value.Volume.ToString(CultureInfo.InvariantCulture.NumberFormat));
				_s.WriteLine("\t\tpan: {0},", value.Pan);
				_s.WriteLine("\t\tpreload: {0}", value.Preload.ToString().ToLower());
				_s.Write("\t    }");
			});
			_s.WriteLine("\t],");
		}

		private static byte[] RLEEncode(byte[] _source)
		{
			List<byte> list = new List<byte>();
			int num = 0;
			while (num < _source.Length)
			{
				if (num + 2 < _source.Length && _source[num] == _source[num + 1] && _source[num + 1] == _source[num + 2])
				{
					int i;
					for (i = 1; i < 127 && num + i + 1 < _source.Length && _source[num + i] == _source[num + i + 1]; i++)
					{
					}
					i++;
					list.Add((byte)(((i - 1) & 0x7F) | 0x80));
					list.Add(_source[num]);
					num += i;
					continue;
				}
				int j;
				for (j = 1; j < 127 && num + j + 1 < _source.Length && (num + j + 2 >= _source.Length || _source[num + j] != _source[num + j + 1] || _source[num + j] != _source[num + j + 2]); j++)
				{
				}
				list.Add((byte)((j - 1) & 0x7F));
				for (int k = num; k < num + j; k++)
				{
					list.Add(_source[k]);
				}
				num += j;
			}
			return list.ToArray();
		}

		private static void WriteSprites(IList<KeyValuePair<string, GMSprite>> _sprites, TextWriter _s)
		{
			_s.WriteLine("\tSprites: [");
			WriteDataKVP(_sprites, _s, delegate(KeyValuePair<string, GMSprite> _kvp, TextWriter __s, int __n)
			{
				GMSprite value = _kvp.Value;
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				_s.WriteLine("\t    {");
				_s.WriteLine("\t\tpName: \"{0}\",", _kvp.Key);
				if (value.Width != 16)
				{
					_s.Write("\t\twidth: {0},", value.Width);
				}
				if (value.Height != 16)
				{
					_s.WriteLine(" height: {0},", value.Height);
				}
				if (value.BBoxMode != 0)
				{
					_s.WriteLine(" bboxMode: {0},", value.BBoxMode);
				}
				if (!value.Transparent)
				{
					_s.WriteLine("\t\ttransparent: {0},", value.Transparent.ToString().ToLower());
				}
				if (!value.Smooth)
				{
					_s.WriteLine("\t\tsmooth: {0},", value.Smooth.ToString().ToLower());
				}
				if (!value.Preload)
				{
					_s.WriteLine("\t\tpreload: {0},", value.Preload.ToString().ToLower());
				}
				if (value.ColCheck)
				{
					_s.WriteLine("\t\tcolCheck: {0},", value.ColCheck.ToString().ToLower());
				}
				if (value.XOrig != 0)
				{
					_s.Write("\t\txOrigin: {0},", value.XOrig);
				}
				if (value.YOrig != 0)
				{
					_s.WriteLine("yOrigin: {0},", value.YOrig);
				}
				if (value.BBoxLeft != 0)
				{
					_s.Write("\t\tbboxLeft: {0},", value.BBoxLeft);
				}
				if (value.BBoxRight != 0)
				{
					_s.Write(" bboxRight: {0},", value.BBoxRight);
				}
				if (value.BBoxTop != 0)
				{
					_s.Write(" bboxTop: {0},", value.BBoxTop);
				}
				if (value.BBoxBottom != 0)
				{
					_s.Write(" bboxBottom: {0},", value.BBoxBottom);
				}
				_s.Write("\t\tTPEntryIndex: [ ");
				for (int i = 0; i < value.Images.Count; i++)
				{
					TexturePageEntry texturePageEntry = ms_tpageSprites.AddImage(value.Images[i].Bitmap, true, false);
					if (i > 0)
					{
						_s.Write(", ");
					}
					_s.Write("{0}", texturePageEntry.Entry);
				}
				if (!value.ColCheck)
				{
					_s.WriteLine("]");
				}
				else
				{
					_s.WriteLine("],");
					IList<byte[]> masks = value.Masks;
					_s.Write("\t\tMasks: [ ");
					if (masks != null)
					{
						_s.WriteLine();
						int num = 0;
						foreach (byte[] item in masks)
						{
							byte[] array = RLEEncode(item);
							if (num > 0)
							{
								_s.WriteLine(",");
							}
							_s.Write("[\t");
							int num2 = 0;
							int num3 = 0;
							byte[] array2 = array;
							foreach (byte b in array2)
							{
								if (num2 > 0)
								{
									_s.Write(",");
								}
								if (num3 >= 32)
								{
									num3 = 0;
									_s.WriteLine();
									_s.Write("\t");
								}
								_s.Write(string.Format("0x{0:X2}", b));
								num2++;
								num3++;
							}
							_s.Write("]");
							num++;
						}
						if (num > 0)
						{
							_s.WriteLine();
						}
					}
					_s.WriteLine("]");
				}
				_s.Write("\t    }");
			});
			_s.WriteLine("\t],");
		}

		private static void WriteBackgrounds(IList<KeyValuePair<string, GMBackground>> _backgrounds, TextWriter _s)
		{
			_s.WriteLine("\tBackgrounds: [");
			WriteDataKVP(_backgrounds, _s, delegate(KeyValuePair<string, GMBackground> _kvp, TextWriter __s, int __n)
			{
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				GMBackground value = _kvp.Value;
				_s.Write("\t\t{");
				_s.Write(" pName: \"{0}\",", _kvp.Key);
				_s.Write(" transparent: {0},", value.Transparent.ToString().ToLower());
				_s.Write(" smooth: {0},", value.Smooth.ToString().ToLower());
				_s.Write(" preload: {0},", value.Smooth.ToString().ToLower());
				if (value.Bitmap != null && value.Bitmap.Data != null && value.Bitmap.Width * value.Bitmap.Height > 0)
				{
					TexturePageEntry texturePageEntry = ms_tpageSprites.AddImage(value.Bitmap.Bitmap, !value.Tileset, false);
					texturePageEntry.OriginalRepeatBorder = true;
					texturePageEntry.RepeatX = 2;
					texturePageEntry.RepeatY = 2;
					TextureOptions.SetTextureOptions(_kvp.Key, texturePageEntry);
					_s.Write(" TPEntryIndex: {0}", texturePageEntry.Entry);
				}
				else
				{
					_s.Write(" TPEntryIndex: -1");
				}
				_s.Write(" }");
			});
			_s.WriteLine("\t],");
		}

		private static void WritePaths(IList<KeyValuePair<string, GMPath>> _paths, TextWriter _s)
		{
			_s.WriteLine("\tPaths: [");
			ms_fWriteNulls = true;
			WriteDataKVP(_paths, _s, delegate(KeyValuePair<string, GMPath> _kvp, TextWriter __s, int __n)
			{
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				GMPath value = _kvp.Value;
				_s.Write("\t\t{");
				_s.Write(" pName: \"{0}\",", _kvp.Key);
				_s.Write(" kind: {0},", value.Kind.ToString().ToLower());
				_s.Write(" closed: {0},", value.Closed.ToString().ToLower());
				_s.Write(" precision: {0},", value.Precision);
				_s.WriteLine(" points : [");
				int num = 0;
				foreach (GMPathPoint point in value.Points)
				{
					if (num > 0)
					{
						_s.WriteLine(",");
					}
					_s.Write("{{ x:{0}, y:{1}, speed:{2} }}", point.X, point.Y, point.Speed);
					num++;
				}
				_s.Write(" ]}");
			});
			ms_fWriteNulls = true;
			_s.WriteLine("\t],");
		}

		private static void WriteScripts(IList<KeyValuePair<string, GMScript>> _scripts, TextWriter _s)
		{
			ms_fWriteNulls = true;
			_s.WriteLine("\tScripts: [");
			WriteDataKVP(_scripts, _s, delegate(KeyValuePair<string, GMScript> _kvp, TextWriter __s, int __n)
			{
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				GMScript value = _kvp.Value;
				if (value != null)
				{
					GMLCode gMLCode = new GMLCode(ms_assets, "gml_Script_" + _kvp.Key, value.Script, eGMLCodeType.eScript);
					ms_codeToCompile.Add(gMLCode.Name, gMLCode);
					_s.WriteLine("{0}", gMLCode.Name);
				}
			});
			ms_fWriteNulls = true;
			_s.WriteLine("\t],");
		}

		private static Encoding GetEncoding(int _codepage)
		{
			int codepage = 1252;
			switch (_codepage)
			{
			case 0:
				codepage = 1252;
				break;
			case 1:
				codepage = 1252;
				break;
			case 238:
				codepage = 1250;
				break;
			case 204:
				codepage = 1251;
				break;
			case 2:
				codepage = 1252;
				break;
			case 128:
				codepage = 932;
				break;
			case 129:
				codepage = 949;
				break;
			case 134:
				codepage = 936;
				break;
			case 136:
				codepage = 950;
				break;
			case 130:
				codepage = 1361;
				break;
			case 177:
				codepage = 1255;
				break;
			case 178:
				codepage = 1256;
				break;
			case 161:
				codepage = 1253;
				break;
			case 162:
				codepage = 1254;
				break;
			case 163:
				codepage = 1258;
				break;
			case 222:
				codepage = 874;
				break;
			case 77:
				codepage = 1252;
				break;
			case 186:
				codepage = 1257;
				break;
			case 255:
				codepage = 1252;
				break;
			}
			return Encoding.GetEncoding(codepage);
		}

		private static void WriteFonts(IList<KeyValuePair<string, GMFont>> _fonts, TextWriter _s)
		{
			_s.WriteLine("\tFonts: [");
			WriteDataKVP(_fonts, _s, delegate(KeyValuePair<string, GMFont> _kvp, TextWriter __s, int __n)
			{
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				GMFont value = _kvp.Value;
				__s.Write("\t\t{");
				__s.Write(" pName: \"{0}\",", _kvp.Key);
				__s.Write(" size: {0},", value.Size);
				__s.Write(" bold: {0},", value.Bold.ToString().ToLower());
				__s.Write(" italic: {0},", value.Italic.ToString().ToLower());
				__s.Write(" first: {0},", value.First);
				__s.Write(" last: {0},", value.Last);
				__s.Write(" charset: {0},", value.CharSet);
				__s.Write(" antialias: {0},", value.AntiAlias);
				__s.Write(" fontname: \"{0}\",", value.Name);
				TexturePageEntry texturePageEntry = ms_tpageSprites.AddImage(value.Bitmap, false, true);
				__s.Write(" TPageEntry: {0},", texturePageEntry.Entry);
				double num = 1.0;
				double num2 = 1.0;
				if (texturePageEntry.W != value.Bitmap.Width || texturePageEntry.H != value.Bitmap.Height)
				{
					num = (double)texturePageEntry.W / (double)value.Bitmap.Width;
					num2 = (double)texturePageEntry.H / (double)value.Bitmap.Height;
				}
				num = 1.0 / num;
				num2 = 1.0 / num2;
				__s.Write(" scaleX: {0},", (float)num);
				__s.Write(" scaleY: {0},", (float)num2);
				__s.WriteLine(" glyphs: [");
				int num3 = 0;
				Encoding encoding = GetEncoding(value.CharSet);
				byte[] array = new byte[1];
				foreach (GMGlyph glyph in value.Glyphs)
				{
					if (glyph.W * glyph.H != 0)
					{
						__s.Write("\t\t\t{");
						__s.Write(" i: {0},", num3);
						array[0] = (byte)num3;
						char[] chars = encoding.GetChars(array);
						char c = chars[0];
						switch (char.GetUnicodeCategory(c))
						{
						default:
							if (c == '\\' || c == '"')
							{
								__s.Write(" c: \"\\{0}\",", c);
							}
							else
							{
								__s.Write(" c: \"{0}\",", c);
							}
							break;
						case UnicodeCategory.Control:
						case UnicodeCategory.Format:
							break;
						}
						__s.Write(" x: {0},", (int)(((double)glyph.X + num - 1.0) / num));
						__s.Write(" y: {0},", (int)(((double)glyph.Y + num2 - 1.0) / num2));
						__s.Write(" w: {0},", (int)(((double)glyph.W + num - 1.0) / num));
						__s.Write(" h: {0},", (int)(((double)glyph.H + num2 - 1.0) / num2));
						__s.Write(" shift: {0},", (int)(((double)glyph.Shift + num - 1.0) / num));
						__s.Write(" offset: {0} ", (int)(((double)glyph.Offset + num - 1.0) / num));
						__s.WriteLine(" },");
					}
					num3++;
				}
				__s.WriteLine("\t\t\t],");
				__s.Write("\t\t}");
			});
			_s.WriteLine("\t],");
		}

		private static void WriteGMEvent(GMEvent _event, TextWriter _s, string _basename)
		{
			WriteDataList(_event.Actions, _s, delegate(GMAction _action, TextWriter __s, int __n)
			{
				string text = _basename;
				for (int i = 0; i < _action.ArgumentCount; i++)
				{
					if (!string.IsNullOrEmpty(_action.Args[i]))
					{
						GMLCode value = new GMLCode(ms_assets, text, _action.Args[i], eGMLCodeType.eEvent);
						ms_codeToCompile[text] = value;
					}
				}
			});
		}

		private static void WriteTimelines(IList<KeyValuePair<string, GMTimeLine>> _timelines, TextWriter _s)
		{
			_s.WriteLine("\tTimelines: [");
			WriteDataKVP(_timelines, _s, delegate(KeyValuePair<string, GMTimeLine> _kvp, TextWriter __s, int __n)
			{
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				int num = 0;
				GMTimeLine value = _kvp.Value;
				_s.WriteLine("\t    {");
				_s.WriteLine("\t\t\tpName: \"{0}\",", _kvp.Key);
				_s.WriteLine("\t\t\tEvents: [");
				foreach (KeyValuePair<int, GMEvent> entry in value.Entries)
				{
					if (entry.Value.Actions.Count > 0)
					{
						if (num > 0)
						{
							_s.WriteLine(",");
						}
						_s.WriteLine("\t\t\t\t{Time: " + entry.Key + ", Event: Timeline_" + EnsureValidId(_kvp.Key) + "_" + num + "}");
						num++;
					}
				}
				_s.WriteLine("\t\t\t],");
				_s.Write("\t    }");
				num = 0;
				foreach (KeyValuePair<int, GMEvent> entry2 in value.Entries)
				{
					if (entry2.Value.Actions.Count > 0)
					{
						WriteGMEvent(entry2.Value, __s, "Timeline_" + EnsureValidId(_kvp.Key) + "_" + num);
						num++;
					}
				}
			});
			_s.WriteLine("\t],");
			ms_fWriteNulls = true;
		}

		private static string GetEventName(int _nEvent, int _nSubEvent)
		{
			return event2Name[_nEvent] + "_" + _nSubEvent;
		}

		private static void WriteObjects(IList<KeyValuePair<string, GMObject>> _objects, TextWriter _s)
		{
			_s.WriteLine("\tGMObjects: [");
			WriteDataKVP(_objects, _s, delegate(KeyValuePair<string, GMObject> _kvp, TextWriter __s, int __n)
			{
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				GMObject value = _kvp.Value;
				__s.Write("\t\t{");
				__s.Write("\t\t\tpName: \"{0}\", ", _kvp.Key);
				if (value.SpriteIndex != 0)
				{
					__s.Write(" spriteIndex: {0}, ", value.SpriteIndex);
				}
				if (value.Visible)
				{
					__s.Write(" visible: {0}, ", value.Visible.ToString().ToLower());
				}
				if (value.Solid)
				{
					__s.Write(" solid: {0}, ", value.Solid.ToString().ToLower());
				}
				if (value.Persistent)
				{
					__s.Write(" persistent: {0}, ", value.Persistent.ToString().ToLower());
				}
				if (value.Depth != 0)
				{
					__s.Write(" depth: {0}, ", value.Depth);
				}
				if (value.Parent != 0)
				{
					__s.Write(" parent: {0}, ", value.Parent);
				}
				if (value.Mask != -1)
				{
					__s.Write(" spritemask: {0}, ", value.Mask);
				}
				string basename = "gml_Object_" + _kvp.Key.Replace(' ', '_');
				int count = 0;
				WriteDataList(value.Events, __s, delegate(IList<KeyValuePair<int, GMEvent>> _list, TextWriter ___s, int _nEvent)
				{
					WriteDataList(_list, ___s, delegate(KeyValuePair<int, GMEvent> _entry, TextWriter ____s, int _nSubEvent)
					{
						string jSEventName = GetJSEventName(_nEvent, 0);
						if (jSEventName != "CollisionEvent" && jSEventName != "Trigger")
						{
							jSEventName = GetJSEventName(_nEvent, _entry.Key);
							string text3 = basename + "_" + GetEventName(_nEvent, _entry.Key);
							WriteGMEvent(_entry.Value, ____s, text3);
							if (count > 0)
							{
								__s.WriteLine(",");
							}
							__s.Write(" {0}: {1}", jSEventName, text3);
							count++;
						}
					});
				});
				if (count > 0)
				{
					__s.WriteLine(",");
				}
				count = 0;
				__s.Write(" TriggerEvents: [ ", value.Mask);
				WriteDataList(value.Events, __s, delegate(IList<KeyValuePair<int, GMEvent>> _list, TextWriter ___s, int _nEvent)
				{
					WriteDataList(_list, ___s, delegate(KeyValuePair<int, GMEvent> _entry, TextWriter ____s, int _nSubEvent)
					{
						if (GetJSEventName(_nEvent, 0) == "Trigger")
						{
							string text2 = basename + "_" + GetEventName(_nEvent, _entry.Key);
							WriteGMEvent(_entry.Value, ____s, text2);
							if (count > 0)
							{
								__s.Write(",");
							}
							__s.Write(" {0},{1}", _entry.Key, text2);
							count++;
						}
					});
				});
				__s.WriteLine(" ],", value.Mask);
				count = 0;
				__s.Write(" CollisionEvents: [ ", value.Mask);
				WriteDataList(value.Events, __s, delegate(IList<KeyValuePair<int, GMEvent>> _list, TextWriter ___s, int _nEvent)
				{
					WriteDataList(_list, ___s, delegate(KeyValuePair<int, GMEvent> _entry, TextWriter ____s, int _nSubEvent)
					{
						if (GetJSEventName(_nEvent, 0) == "CollisionEvent")
						{
							string arg = _entry.Key.ToString();
							string text = basename + "_" + GetEventName(_nEvent, _entry.Key);
							WriteGMEvent(_entry.Value, ____s, text);
							if (count > 0)
							{
								__s.Write(",");
							}
							__s.Write(" {0}, {1}", arg, text);
							count++;
						}
					});
				});
				__s.WriteLine(" ]", value.Mask);
				__s.Write(" }");
			});
			_s.WriteLine("\t],");
		}

		private static string EncodeString(string _str)
		{
			_str = _str.Replace("\\", "\\");
			_str = _str.Replace("\"", "\\\"");
			return _str;
		}

		private static void WriteRooms(IList<KeyValuePair<string, GMRoom>> _rooms, TextWriter _s)
		{
			_s.WriteLine("\tGMRooms: [");
			WriteDataKVP(_rooms, _s, delegate(KeyValuePair<string, GMRoom> _kvp, TextWriter __s, int __n)
			{
				GMRoom value = _kvp.Value;
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				_s.WriteLine("\t\t{\t");
				_s.WriteLine("\t\t\tpName:\"{0}\",", _kvp.Key);
				if (!string.IsNullOrEmpty(value.Caption))
				{
					_s.WriteLine("\t\t\tpCaption:\"{0}\",", EncodeString(value.Caption));
				}
				if (value.Width != 1024)
				{
					_s.WriteLine("\t\t\twidth:{0},", value.Width);
				}
				if (value.Height != 768)
				{
					_s.WriteLine("\t\t\theight:{0},", value.Height);
				}
				if (value.Speed != 30)
				{
					_s.WriteLine("\t\t\tspeed:{0},", value.Speed);
				}
				if (value.Persistent)
				{
					_s.WriteLine("\t\t\tpersistent:{0},", value.Persistent.ToString().ToLower());
				}
				if (value.Colour != 12632256)
				{
					_s.WriteLine("\t\t\tcolour:{0},", value.Colour);
				}
				if (!value.ShowColour)
				{
					_s.WriteLine("\t\t\tshowColour:{0},", value.ShowColour.ToString().ToLower());
				}
				if (value.EnableViews)
				{
					_s.WriteLine("\t\t\tenableViews:{0},", value.EnableViews.ToString().ToLower());
				}
				if (!value.ViewClearScreen)
				{
					_s.WriteLine("\t\t\tviewClearScreen:{0},", value.ViewClearScreen.ToString().ToLower());
				}
				if (!string.IsNullOrEmpty(value.Code))
				{
					string text = string.Format("gml_Room_{0}_Create", _kvp.Key);
					GMLCode value2 = new GMLCode(ms_assets, text, value.Code, eGMLCodeType.eRoomCreate);
					ms_codeToCompile.Add(text, value2);
					_s.WriteLine("\t\t\tpCode: {0},", text);
				}
				_s.WriteLine("\t\t\tbackgrounds:[");
				int num = 0;
				foreach (GMBack background in value.Backgrounds)
				{
					if (num > 0)
					{
						_s.WriteLine(",");
					}
					_s.Write("\t\t\t\t{");
					if (background.Visible)
					{
						_s.Write(" visible:{0}, ", background.Visible.ToString().ToLower());
					}
					if (background.Foreground)
					{
						_s.Write(" foreground:{0}, ", background.Foreground.ToString().ToLower());
					}
					if (background.Index != -1)
					{
						_s.Write(" index:{0}, ", background.Index);
					}
					if (background.X != 0)
					{
						_s.Write(" x:{0}, ", background.X);
					}
					if (background.Y != 0)
					{
						_s.Write(" y:{0}, ", background.Y);
					}
					if (!background.HTiled)
					{
						_s.Write(" htiled:{0}, ", background.HTiled.ToString().ToLower());
					}
					if (!background.VTiled)
					{
						_s.Write(" vtiled:{0}, ", background.VTiled.ToString().ToLower());
					}
					if (background.HSpeed != 0)
					{
						_s.Write(" hspeed:{0}, ", background.HSpeed);
					}
					if (background.VSpeed != 0)
					{
						_s.Write(" vspeed:{0}, ", background.VSpeed);
					}
					if (background.Stretch)
					{
						_s.Write(" stretch:{0} ", background.Stretch.ToString().ToLower());
					}
					_s.Write(" }");
					num++;
				}
				_s.WriteLine("\t\t\t],");
				_s.WriteLine("\t\t\tviews:[");
				num = 0;
				foreach (GMView view in value.Views)
				{
					if (num > 0)
					{
						_s.WriteLine(",");
					}
					_s.Write("\t\t\t\t{");
					if (view.Visible)
					{
						_s.Write(" visible:{0}, ", view.Visible.ToString().ToLower());
					}
					if (view.XView != 0)
					{
						_s.Write(" xview:{0}, ", view.XView);
					}
					if (view.YView != 0)
					{
						_s.Write(" yview:{0}, ", view.YView);
					}
					if (view.WView != 640)
					{
						_s.Write(" wview:{0}, ", view.WView);
					}
					if (view.HView != 480)
					{
						_s.Write(" hview:{0}, ", view.HView);
					}
					if (view.XPort != 0)
					{
						_s.Write(" xport:{0}, ", view.XPort);
					}
					if (view.YPort != 0)
					{
						_s.Write(" yport:{0}, ", view.YPort);
					}
					if (view.WPort != 640)
					{
						_s.Write(" wport:{0}, ", view.WPort);
					}
					if (view.HPort != 480)
					{
						_s.Write(" hport:{0}, ", view.HPort);
					}
					if (view.HBorder != 32)
					{
						_s.Write(" hborder:{0}, ", view.HBorder);
					}
					if (view.VBorder != 32)
					{
						_s.Write(" vborder:{0}, ", view.VBorder);
					}
					if (view.HSpeed != -1)
					{
						_s.Write(" hspeed:{0}, ", view.HSpeed);
					}
					if (view.VSpeed != -1)
					{
						_s.Write(" vspeed:{0}, ", view.VSpeed);
					}
					if (view.Index != -1)
					{
						_s.Write(" index:{0} ", view.Index);
					}
					_s.Write(" }");
					num++;
				}
				_s.WriteLine("\t\t\t],");
				_s.WriteLine("\t\t\tpInstances:[");
				int num2 = 0;
				foreach (GMInstance instance in value.Instances)
				{
					if (num2 > 0)
					{
						_s.WriteLine(",");
					}
					_s.Write("\t\t\t\t{");
					_s.Write(" x:{0}, ", instance.X);
					_s.Write(" y:{0}, ", instance.Y);
					_s.Write(" index:{0}, ", instance.Index);
					_s.Write(" id:{0}, ", instance.Id);
					if (!string.IsNullOrEmpty(instance.Code))
					{
						string text2 = string.Format("gml_RoomCC_{0}_{1}_Create", _kvp.Key, num2);
						GMLCode value3 = new GMLCode(ms_assets, text2, instance.Code, eGMLCodeType.eRoomInstanceCreate);
						ms_codeToCompile.Add(text2, value3);
						_s.Write(" pCode: {0}, ", text2);
					}
					_s.Write(" scaleX:{0}, ", instance.ScaleX);
					_s.Write(" scaleY:{0}, ", instance.ScaleY);
					_s.Write(" colour:{0} ", instance.Colour);
					_s.Write(" }");
					num2++;
				}
				_s.WriteLine("\t\t\t],");
				_s.WriteLine("\t\t\ttiles:[");
				num = 0;
				foreach (GMTile tile in value.Tiles)
				{
					if (num > 0)
					{
						_s.WriteLine(",");
					}
					_s.Write("\t\t\t\t{");
					int num3 = 0;
					if (tile.X != 0)
					{
						_s.Write(" x:{0} ", tile.X);
						num3++;
					}
					if (tile.Y != 0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" y:{0} ", tile.Y);
						num3++;
					}
					if (tile.Index != 0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" index:{0} ", tile.Index);
						num3++;
					}
					if (tile.XO != 0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" xo:{0} ", tile.XO);
						num3++;
					}
					if (tile.YO != 0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" yo:{0} ", tile.YO);
						num3++;
					}
					if (tile.W != 0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" w:{0} ", tile.W);
						num3++;
					}
					if (tile.H != 0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" h:{0} ", tile.H);
						num3++;
					}
					if (tile.Depth != 0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" depth:{0} ", tile.Depth);
						num3++;
					}
					if (tile.Id != 0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" id:{0} ", tile.Id);
						num3++;
					}
					if (tile.XScale != 1.0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" scaleX:{0} ", tile.XScale);
						num3++;
					}
					if (tile.YScale != 1.0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" scaleY:{0} ", tile.YScale);
						num3++;
					}
					if (tile.Blend != 1048575 || tile.Alpha != 1.0)
					{
						if (num3 > 0)
						{
							_s.Write(",");
						}
						_s.Write(" colour:{0} ", ((int)(tile.Alpha * 255.0) << 24) | tile.Blend);
						num3++;
					}
					_s.Write(" }");
					num++;
				}
				_s.WriteLine("\t\t\t]\t");
				_s.Write("\t\t}\t");
			});
			_s.WriteLine("\t],");
		}

		private static void WriteRoomOrder(IList<int> _roomOrder, TextWriter _sw)
		{
			_sw.Write("\tRoomOrder: [");
			int num = 0;
			foreach (int item in _roomOrder)
			{
				if (num > 0)
				{
					_sw.Write(",");
				}
				_sw.Write("{0}", item);
				num++;
			}
			_sw.WriteLine("\t],");
		}

		private static void WriteDataFiles(IList<KeyValuePair<string, GMDataFile>> _datafiles, TextWriter _s)
		{
			ms_fWriteNulls = false;
			WriteDataKVP(_datafiles, _s, delegate(KeyValuePair<string, GMDataFile> _kvp, TextWriter __s, int __n)
			{
				GMDataFile value = _kvp.Value;
				string text = Path.Combine(baseDir, Path.GetFileName(value.FileName));
				Program.Out.WriteLine("Writing datafile {0} to {1}", value.FileName, text);
				if (value.Data != null && value.Data.Length > 0)
				{
					File.WriteAllBytes(text, value.Data);
				}
				else
				{
					FileStream fileStream = File.Create(text);
					fileStream.Close();
					fileStream.Dispose();
				}
			});
			ms_fWriteNulls = true;
		}

		private static void WriteTexturePages(GMAssets _assets, TextWriter _s)
		{
			if (ms_tpageSprites.Entries.Count > 0)
			{
				ms_tpageSprites.Compile();
			}
			IOrderedEnumerable<TexturePageEntry> source = ms_tpageSprites.Entries.OrderBy((TexturePageEntry e) => e.Entry);
			_s.WriteLine("\tTPageEntries: [");
			WriteDataList(source.ToList(), _s, delegate(TexturePageEntry _tpe, TextWriter __s, int __n)
			{
				if (__n > 0)
				{
					_s.WriteLine(",");
				}
				_s.Write("\t\t{");
				_s.Write(" x:{0},", _tpe.X);
				_s.Write(" y:{0},", _tpe.Y);
				_s.Write(" w:{0},", _tpe.W);
				_s.Write(" h:{0},", _tpe.H);
				_s.Write(" XOffset:{0},", _tpe.XOffset);
				_s.Write(" YOffset:{0},", _tpe.YOffset);
				_s.Write(" CropWidth:{0},", _tpe.CropWidth);
				_s.Write(" CropHeight:{0},", _tpe.CropHeight);
				_s.Write(" ow:{0},", _tpe.OW);
				_s.Write(" oh:{0},", _tpe.OH);
				_s.Write(" tp:{0}", _tpe.TP.TP);
				_s.Write("}");
			});
			_s.WriteLine("\t],");
		}

		public static void Save(GMAssets _assets, TextWriter _s)
		{
			ms_assets = _assets;
			_s.WriteLine("var JSON_game = {");
			WriteHeader(_assets, _s);
			WriteExtensions(_assets.Extensions, _s);
			WriteHelp(_assets, _s);
			WriteSounds(_assets.Sounds, _s);
			WriteSprites(_assets.Sprites, _s);
			WriteBackgrounds(_assets.Backgrounds, _s);
			WritePaths(_assets.Paths, _s);
			WriteScripts(_assets.Scripts, _s);
			WriteFonts(_assets.Fonts, _s);
			WriteTimelines(_assets.TimeLines, _s);
			WriteTriggers(_assets.Triggers, _s);
			WriteObjects(_assets.Objects, _s);
			WriteRooms(_assets.Rooms, _s);
			WriteRoomOrder(_assets.RoomOrder, _s);
			WriteDataFiles(_assets.DataFiles, _s);
			WriteTexturePages(_assets, _s);
			WriteTextures(_assets, _s);
			WriteWaveforms(_assets, _s);
			WriteOptions(_assets, _s);
			_s.WriteLine("};\n");
			_s.WriteLine("function gmlConst() {");
			foreach (KeyValuePair<string, string> constant in _assets.Options.Constants)
			{
				int value = 0;
				if (GMLCompile.ms_ConstantCount.TryGetValue(constant.Key, out value) && value > 0)
				{
					string value2;
					if (!ms_simpleConstants.TryGetValue(constant.Key, out value2))
					{
						_s.WriteLine("this.{0} = const_{0}();", constant.Key);
					}
					else
					{
						_s.WriteLine("this.{0} = {1};", constant.Key, value2);
					}
				}
			}
			_s.WriteLine("}");
			_s.WriteLine("function gmlInitGlobal() {");
			foreach (KeyValuePair<string, string> ms_global in GML2JavaScript.ms_globals)
			{
				_s.WriteLine("global.{1}{0} = 0;", ms_global.Value, GML2JavaScript.ms_varPrefix);
			}
			_s.WriteLine("}");
			foreach (KeyValuePair<string, GMLCode> item in ms_codeToCompile)
			{
				GMLCode value3 = item.Value;
				GML2JavaScript.Compile(ms_assets, value3, _s);
				_s.Flush();
			}
			_s.Close();
		}

		public static string GetKeyBoardEvent(int _SubEvent)
		{
			switch (_SubEvent)
			{
			case 0:
				return "NOKEY";
			case 1:
				return "ANYKEY";
			case 8:
				return "BACKSPACE";
			case 9:
				return "TAB";
			case 13:
				return "ENTER";
			case 16:
				return "SHIFT";
			case 17:
				return "CTRL";
			case 18:
				return "ALT";
			case 19:
				return "PAUSE";
			case 27:
				return "ESCAPE";
			case 32:
				return "SPACE";
			case 33:
				return "PAGEUP";
			case 34:
				return "PAGEDOWN";
			case 35:
				return "END";
			case 36:
				return "HOME";
			case 37:
				return "LEFT";
			case 38:
				return "UP";
			case 39:
				return "RIGHT";
			case 40:
				return "DOWN";
			case 45:
				return "INSERT";
			case 46:
				return "DELETE";
			case 48:
				return "0";
			case 49:
				return "1";
			case 50:
				return "2";
			case 51:
				return "3";
			case 52:
				return "4";
			case 53:
				return "5";
			case 54:
				return "6";
			case 55:
				return "7";
			case 56:
				return "8";
			case 57:
				return "9";
			case 65:
				return "A";
			case 66:
				return "B";
			case 67:
				return "C";
			case 68:
				return "D";
			case 69:
				return "E";
			case 70:
				return "F";
			case 71:
				return "G";
			case 72:
				return "H";
			case 73:
				return "I";
			case 74:
				return "J";
			case 75:
				return "K";
			case 76:
				return "L";
			case 77:
				return "M";
			case 78:
				return "N";
			case 79:
				return "O";
			case 80:
				return "P";
			case 81:
				return "Q";
			case 82:
				return "R";
			case 83:
				return "S";
			case 84:
				return "T";
			case 85:
				return "U";
			case 86:
				return "V";
			case 87:
				return "W";
			case 88:
				return "X";
			case 89:
				return "Y";
			case 90:
				return "Z";
			case 112:
				return "F1";
			case 113:
				return "F2";
			case 114:
				return "F3";
			case 115:
				return "F4";
			case 116:
				return "F5";
			case 117:
				return "F6";
			case 118:
				return "F7";
			case 119:
				return "F8";
			case 120:
				return "F9";
			case 121:
				return "F10";
			case 122:
				return "F11";
			case 123:
				return "F12";
			case 145:
				return "SCROLL_LOCK";
			case 186:
				return "SEMICOLON";
			case 187:
				return "PLUS";
			case 188:
				return "COMMA";
			case 189:
				return "MINUS";
			case 190:
				return "FULLSTOP";
			case 191:
				return "FWSLASH";
			case 192:
				return "AT";
			case 219:
				return "RIGHTSQBR";
			case 220:
				return "BKSLASH";
			case 221:
				return "LEFTSQBR";
			case 222:
				return "HASH";
			case 223:
				return "TILD";
			case 144:
				return "NUM_LOCK";
			case 96:
				return "NUM_0";
			case 97:
				return "NUM_1";
			case 98:
				return "NUM_2";
			case 99:
				return "NUM_3";
			case 100:
				return "NUM_4";
			case 101:
				return "NUM_5";
			case 102:
				return "NUM_6";
			case 103:
				return "NUM_7";
			case 104:
				return "NUM_8";
			case 105:
				return "NUM_9";
			case 106:
				return "NUM_STAR";
			case 107:
				return "NUM_PLUS";
			case 109:
				return "NUM_MINUS";
			case 110:
				return "NUM_DOT";
			case 111:
				return "NUM_DIV";
			default:
				return "unknown";
			}
		}

		public static string GetJSEventName(int _nEvent, int _nSubEvent)
		{
			switch (_nEvent)
			{
			case 0:
				return "CreateEvent";
			case 1:
				return "DestroyEvent";
			case 2:
				switch (_nSubEvent)
				{
				case 0:
					return "ObjAlarm0";
				case 1:
					return "ObjAlarm1";
				case 2:
					return "ObjAlarm2";
				case 3:
					return "ObjAlarm3";
				case 4:
					return "ObjAlarm4";
				case 5:
					return "ObjAlarm5";
				case 6:
					return "ObjAlarm6";
				case 7:
					return "ObjAlarm7";
				case 8:
					return "ObjAlarm8";
				case 9:
					return "ObjAlarm9";
				case 10:
					return "ObjAlarm10";
				case 11:
					return "ObjAlarm11";
				default:
					return "unknown";
				}
			case 3:
				switch (_nSubEvent)
				{
				case 1:
					return "StepBeginEvent";
				case 0:
					return "StepNormalEvent";
				case 2:
					return "StepEndEvent";
				default:
					return "unknown";
				}
			case 4:
				return "CollisionEvent";
			case 5:
				return "Key_" + GetKeyBoardEvent(_nSubEvent);
			case 6:
				switch (_nSubEvent)
				{
				case 0:
					return "LeftButtonDown";
				case 1:
					return "RightButtonDown";
				case 2:
					return "MiddleButtonDown";
				case 3:
					return "NoButtonPressed";
				case 4:
					return "LeftButtonPressed";
				case 5:
					return "RightButtonPressed";
				case 6:
					return "MiddleButtonPressed";
				case 7:
					return "LeftButtonReleased";
				case 8:
					return "RightButtonReleased";
				case 9:
					return "MiddleButtonReleased";
				case 10:
					return "MouseEnter";
				case 11:
					return "MouseLeave";
				case 16:
					return "Joystick1Left";
				case 17:
					return "Joystick1Right";
				case 18:
					return "Joystick1Up";
				case 19:
					return "Joystick1Down";
				case 21:
					return "Joystick1Button1";
				case 22:
					return "Joystick1Button2";
				case 23:
					return "Joystick1Button3";
				case 24:
					return "Joystick1Button4";
				case 25:
					return "Joystick1Button5";
				case 26:
					return "Joystick1Button6";
				case 27:
					return "Joystick1Button7";
				case 28:
					return "Joystick1Button8";
				case 31:
					return "Joystick2Left";
				case 32:
					return "Joystick2Right";
				case 33:
					return "Joystick2Up";
				case 34:
					return "Joystick2Down";
				case 36:
					return "Joystick2Button1";
				case 37:
					return "Joystick2Button2";
				case 38:
					return "Joystick2Button3";
				case 39:
					return "Joystick2Button4";
				case 40:
					return "Joystick2Button5";
				case 41:
					return "Joystick2Button6";
				case 42:
					return "Joystick2Button7";
				case 43:
					return "Joystick2Button8";
				case 50:
					return "GlobalLeftButtonDown";
				case 51:
					return "GlobalRightButtonDown";
				case 52:
					return "GlobalMiddleButtonDown";
				case 53:
					return "GlobalLeftButtonPressed";
				case 54:
					return "GlobalRightButtonPressed";
				case 55:
					return "GlobalMiddleButtonPressed";
				case 56:
					return "GlobalLeftButtonReleased";
				case 57:
					return "GlobalRightButtonReleased";
				case 58:
					return "GlobalMiddleButtonReleased";
				case 60:
					return "MouseWheelUp";
				case 61:
					return "MouseWheelDown";
				default:
					return "unknown";
				}
			case 7:
				switch (_nSubEvent)
				{
				case 0:
					return "OutsideEvent";
				case 1:
					return "BoundaryEvent";
				case 2:
					return "StartGameEvent";
				case 3:
					return "EndGameEvent";
				case 4:
					return "StartRoomEvent";
				case 5:
					return "EndRoomEvent";
				case 6:
					return "NoLivesEvent";
				case 7:
					return "AnimationEndEvent";
				case 8:
					return "EndOfPathEvent";
				case 9:
					return "NoHealthEvent";
				case 30:
					return "CloseButtonEvent";
				case 40:
					return "OutsideView0Event";
				case 50:
					return "BoundaryView0Event";
				case 60:
					return "WebImageLoadedEvent";
				case 61:
					return "WebSoundLoadedEvent";
				case 62:
					return "WebAsyncEvent";
				case 10:
					return "UserEvent0";
				case 11:
					return "UserEvent1";
				case 12:
					return "UserEvent2";
				case 13:
					return "UserEvent3";
				case 14:
					return "UserEvent4";
				case 15:
					return "UserEvent5";
				case 16:
					return "UserEvent6";
				case 17:
					return "UserEvent7";
				case 18:
					return "UserEvent8";
				case 19:
					return "UserEvent9";
				case 20:
					return "UserEvent10";
				case 21:
					return "UserEvent11";
				case 22:
					return "UserEvent12";
				case 23:
					return "UserEvent13";
				case 24:
					return "UserEvent14";
				case 25:
					return "UserEvent15";
				default:
					return "unknown";
				}
			case 8:
				return "DrawEvent";
			case 9:
				return "KeyPressed_" + GetKeyBoardEvent(_nSubEvent);
			case 10:
				return "KeyReleased_" + GetKeyBoardEvent(_nSubEvent);
			case 11:
				return "Trigger";
			default:
				return "unknown";
			}
		}
	}
}
