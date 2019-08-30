using GMAssetCompiler.Machines;
using GMAssetCompiler.Output;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GMAssetCompiler
{
    internal static class Program
    {
        private static OptionSet m_options;

        //public static bool IsPSPFileLoaded = false;

        public static eTexType[] TextureType;

        public static IMachineType MachineType
        {
            get;
            set;
        }

        public static string OutputDir
        {
            get;
            set;
        }

        public static bool WriteTextures
        {
            get;
            set;
        }

        public static bool WriteWaves
        {
            get;
            set;
        }

        public static bool CompileOnly
        {
            get;
            set;
        }

        public static string TitleID
        {
            get;
            set;
        }

        public static bool SplashOmit
		{
			get;
			set;
		}

		public static bool SeparateOpaqueAndAlpha
		{
			get;
			set;
		}

		public static bool DisplaySortedTextures
		{
			get;
			set;
		}

		public static int TexturePageWidth
		{
			get;
			set;
		}

		public static int TexturePageHeight
		{
			get;
			set;
		}

		public static int TextureScale
		{
			get;
			set;
		}

		public static bool Verbose
		{
			get;
			set;
		}

		public static bool CompileVerbose
		{
			get;
			set;
		}

		public static bool RemoveDND
		{
			get;
			set;
		}

		public static bool CenterHTML5Game
		{
			get;
			set;
		}

		public static bool NoCache
		{
			get;
			set;
		}

		public static bool NoIndexHTML
		{
			get;
			set;
		}

		public static string HTMLRunner
		{
			get;
			set;
		}

		public static bool DoObfuscate
		{
			get;
			set;
		}

		public static bool ObfuscateObfuscate
		{
			get;
			set;
		}

		public static bool ObfuscatePrettyPrint
		{
			get;
			set;
		}

		public static bool ObfuscateRemoveUnused
		{
			get;
			set;
		}

		public static bool ObfuscateEncodeStrings
		{
			get;
			set;
		}

		public static string LoadingBarName
		{
			get;
			set;
		}

		public static bool CustomLoadingScreen
		{
			get;
			set;
		}

		public static int ExitCode
		{
			get;
			set;
		}

		public static bool InhibitErrorOutput
		{
			get;
			set;
		}

		public static GMAssets Assets
		{
			get;
			set;
		}

		public static bool Studio
		{
			get;
			set;
		}

		public static Dictionary<string, List<string>> TextureGroups
		{
			get;
			set;
		}

		public static TextWriter Out
		{
			get;
			set;
		}

		public static string TextureTypeExtension(eTexType _textureType)
		{
			switch (_textureType)
			{
			case eTexType.eDXT:
				return ".dds";
			case eTexType.ePVR:
				return ".pvr";
			case eTexType.eRaw:
			case eTexType.e4444:
				return ".raw";
			case eTexType.ePNG:
				return ".png";
			default:
				return ".dds";
			}
		}

		private static void ShowHelp()
		{
			Console.WriteLine("Usage is:");
			Console.WriteLine("\tSilicaAssetCompiler <options> [<filenam>]+");
			m_options.WriteOptionDescriptions(Console.Out);
			Environment.Exit(0);
		}

		public static void SetMachineType(string _m)
		{
            _m = "psp"; // Allways PSP
			switch (_m.ToLower())
			{
			case "nokia":
			case "symbian":
			case "qt":
				MachineType = new Symbian();
				break;
			case "droid":
			case "android":
				MachineType = new Android();
				break;
			case "ipad":
			case "iphone":
			case "ipod":
			case "ios":
				MachineType = new IOS();
				break;
			case "psp":
				MachineType = new PSP();
				break;
			case "windows":
			case "win":
				MachineType = new Windows();
				break;
			case "html5":
				MachineType = new HTML5();
				break;
			}
			if (MachineType != null)
			{
				TextureType[0] = MachineType.OpaqueTextureType;
				TextureType[1] = MachineType.AlphaTextureType;
				TexturePageWidth = MachineType.TPageWidth;
				TexturePageHeight = MachineType.TPageHeight;
			}
		}

		private static eTexType SetTextureType(string _t)
		{
			eTexType result = eTexType.eRaw;
			switch (_t.ToLower())
			{
			case "png":
				result = eTexType.ePNG;
				break;
			case "raw":
				result = eTexType.eRaw;
				break;
			case "dxt":
				result = eTexType.eDXT;
				break;
			case "pvr":
				result = eTexType.ePVR;
				break;
			case "16b":
			case "16bit":
			case "4444":
				result = eTexType.e4444;
				break;
			}
			return result;
		}

		private static void DoPreObfuscateLib(string _sourceDir, string _outputDir)
		{
			List<string> files = new List<string>();
			HTML5Saver.GetFiles(_sourceDir, new string[3]
			{
				".svn",
				"runner.js",
				"particles"
			}, files);
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, GMLFunction> ms_func in GMLCompile.ms_funcs)
			{
				list.Add(ms_func.Key);
			}
			foreach (KeyValuePair<string, GMLVariable> ms_builtin in GMLCompile.ms_builtins)
			{
				list.Add(ms_builtin.Key);
				if (!string.IsNullOrEmpty(ms_builtin.Value.setFunction))
				{
					list.Add(ms_builtin.Value.setFunction);
				}
				if (!string.IsNullOrEmpty(ms_builtin.Value.getFunction))
				{
					list.Add(ms_builtin.Value.getFunction);
				}
			}
			foreach (KeyValuePair<string, GMLVariable> item2 in GMLCompile.ms_builtinsArray)
			{
				list.Add(item2.Key);
				if (!string.IsNullOrEmpty(item2.Value.setFunction))
				{
					list.Add(item2.Value.setFunction);
				}
				if (!string.IsNullOrEmpty(item2.Value.getFunction))
				{
					list.Add(item2.Value.getFunction);
				}
			}
			foreach (KeyValuePair<string, GMLVariable> item3 in GMLCompile.ms_builtinsLocal)
			{
				list.Add(item3.Key);
				if (!string.IsNullOrEmpty(item3.Value.setFunction))
				{
					list.Add(item3.Value.setFunction);
				}
				if (!string.IsNullOrEmpty(item3.Value.getFunction))
				{
					list.Add(item3.Value.getFunction);
				}
			}
			foreach (KeyValuePair<string, GMLVariable> item4 in GMLCompile.ms_builtinsLocalArray)
			{
				list.Add(item4.Key);
				if (!string.IsNullOrEmpty(item4.Value.setFunction))
				{
					list.Add(item4.Value.setFunction);
				}
				if (!string.IsNullOrEmpty(item4.Value.getFunction))
				{
					list.Add(item4.Value.getFunction);
				}
			}
			foreach (KeyValuePair<string, double> ms_constant in GMLCompile.ms_constants)
			{
				list.Add(ms_constant.Key);
			}
			string[] array = new string[]
			{
				"antialias",
				"charset",
				"i",
				"c",
				"JSON_game",
				"Options",
				"debugMode",
				"loadingBarCallback",
				"gameId",
				"gameGuid",
				"fullScreen",
				"interpolatePixels",
				"showCursor",
				"scale",
				"allowFullScreenKey",
				"freezeOnLostFocus",
				"showLoadingBar",
				"displayErrors",
				"writeErrors",
				"abortErrors",
				"variableErrors",
				"Textures",
				"Triggers",
				"pName",
				"moment",
				"pFunc",
				"Extensions",
				"TriggerEvents",
				"jsFiles",
				"init",
				"final",
				"Sounds",
				"pName",
				"kind",
				"extension",
				"origName",
				"effects",
				"volume",
				"pan",
				"preload",
				"Sprites",
				"width",
				"height",
				"bboxMode",
				"transparent",
				"smooth",
				"preload",
				"colCheck",
				"xOrigin",
				"yOrigin",
				"bboxLeft",
				"bboxRight",
				"bboxTop",
				"bboxBottom",
				"TPEntryIndex",
				"Masks",
				"Backgrounds",
				"Paths",
				"kind",
				"closed",
				"precision",
				"points",
				"speed",
				"Fonts",
				"size",
				"bold",
				"italic",
				"first",
				"last",
				"TPageEntry",
				"scaleX",
				"scaleY",
				"glyphs",
				"w",
				"h",
				"shift",
				"offset",
				"Timelines",
				"Events",
				"Time",
				"Event",
				"GMObjects",
				"spriteIndex",
				"visible",
				"solid",
				"persistent",
				"depth",
				"parent",
				"spritemask",
				"CollisionEvents",
				"CreateEvent",
				"DestroyEvent",
				"StepBeginEvent",
				"StepNormalEvent",
				"StepEndEvent",
				"DrawEvent",
				"NoButtonPressed",
				"LeftButtonDown",
				"RightButtonDown",
				"MiddleButtonDown",
				"LeftButtonPressed",
				"RightButtonPressed",
				"MiddleButtonPressed",
				"LeftButtonReleased",
				"RightButtonReleased",
				"MiddleButtonReleased",
				"GlobalLeftButtonDown",
				"GlobalRightButtonDown",
				"GlobalMiddleButtonDown",
				"GlobalLeftButtonPressed",
				"GlobalRightButtonPressed",
				"GlobalMiddleButtonPressed",
				"GlobalLeftButtonReleased",
				"GlobalRightButtonReleased",
				"GlobalMiddleButtonReleased",
				"MouseEnter",
				"MouseLeave",
				"OutsideEvent",
				"BoundaryEvent",
				"StartGameEvent",
				"EndGameEvent",
				"StartRoomEvent",
				"EndRoomEvent",
				"NoLivesEvent",
				"AnimationEndEvent",
				"EndOfPathEvent",
				"NoHealthEvent",
				"CloseButtonEvent",
				"OutsideView0Event",
				"BoundaryView0Event",
				"UserEvent0",
				"UserEvent1",
				"UserEvent2",
				"UserEvent3",
				"UserEvent4",
				"UserEvent5",
				"UserEvent6",
				"UserEvent7",
				"UserEvent8",
				"UserEvent9",
				"UserEvent10",
				"UserEvent11",
				"UserEvent12",
				"UserEvent13",
				"UserEvent14",
				"UserEvent15",
				"WebImageLoadedEvent",
				"WebSoundLoadedEvent",
				"WebAsyncEvent",
				"ObjAlarm0",
				"ObjAlarm1",
				"ObjAlarm2",
				"ObjAlarm3",
				"ObjAlarm4",
				"ObjAlarm5",
				"ObjAlarm6",
				"ObjAlarm7",
				"ObjAlarm8",
				"ObjAlarm9",
				"ObjAlarm10",
				"ObjAlarm11",
				"KeyPressed_",
				"KeyPressed_NOKEY",
				"KeyPressed_ANYKEY",
				"KeyPressed_BACKSPACE",
				"KeyPressed_TAB",
				"KeyPressed_ENTER",
				"KeyPressed_SHIFT",
				"KeyPressed_CTRL",
				"KeyPressed_ALT",
				"KeyPressed_PAUSE",
				"KeyPressed_ESCAPE",
				"KeyPressed_SPACE",
				"KeyPressed_PAGEUP",
				"KeyPressed_PAGEDOWN",
				"KeyPressed_END",
				"KeyPressed_HOME",
				"KeyPressed_LEFT",
				"KeyPressed_UP",
				"KeyPressed_RIGHT",
				"KeyPressed_DOWN",
				"KeyPressed_INSERT",
				"KeyPressed_DELETE",
				"KeyPressed_0",
				"KeyPressed_1",
				"KeyPressed_2",
				"KeyPressed_3",
				"KeyPressed_4",
				"KeyPressed_5",
				"KeyPressed_6",
				"KeyPressed_7",
				"KeyPressed_8",
				"KeyPressed_9",
				"KeyPressed_A",
				"KeyPressed_B",
				"KeyPressed_C",
				"KeyPressed_D",
				"KeyPressed_E",
				"KeyPressed_F",
				"KeyPressed_G",
				"KeyPressed_H",
				"KeyPressed_I",
				"KeyPressed_J",
				"KeyPressed_K",
				"KeyPressed_L",
				"KeyPressed_M",
				"KeyPressed_N",
				"KeyPressed_O",
				"KeyPressed_P",
				"KeyPressed_Q",
				"KeyPressed_R",
				"KeyPressed_S",
				"KeyPressed_T",
				"KeyPressed_U",
				"KeyPressed_V",
				"KeyPressed_W",
				"KeyPressed_X",
				"KeyPressed_Y",
				"KeyPressed_Z",
				"KeyPressed_F1",
				"KeyPressed_F2",
				"KeyPressed_F3",
				"KeyPressed_F4",
				"KeyPressed_F5",
				"KeyPressed_F6",
				"KeyPressed_F7",
				"KeyPressed_F8",
				"KeyPressed_F9",
				"KeyPressed_F10",
				"KeyPressed_F11",
				"KeyPressed_F12",
				"KeyPressed_NUM_LOCK",
				"KeyPressed_NUM_0",
				"KeyPressed_NUM_1",
				"KeyPressed_NUM_2",
				"KeyPressed_NUM_3",
				"KeyPressed_NUM_4",
				"KeyPressed_NUM_5",
				"KeyPressed_NUM_6",
				"KeyPressed_NUM_7",
				"KeyPressed_NUM_8",
				"KeyPressed_NUM_9",
				"KeyPressed_NUM_STAR",
				"KeyPressed_NUM_PLUS",
				"KeyPressed_NUM_MINUS",
				"KeyPressed_NUM_DOT",
				"KeyPressed_NUM_DIV",
				"Key_NOKEY",
				"Key_ANYKEY",
				"Key_BACKSPACE",
				"Key_TAB",
				"Key_ENTER",
				"Key_SHIFT",
				"Key_CTRL",
				"Key_ALT",
				"Key_PAUSE",
				"Key_ESCAPE",
				"Key_SPACE",
				"Key_PAGEUP",
				"Key_PAGEDOWN",
				"Key_END",
				"Key_HOME",
				"Key_LEFT",
				"Key_UP",
				"Key_RIGHT",
				"Key_DOWN",
				"Key_INSERT",
				"Key_DELETE",
				"Key_0",
				"Key_1",
				"Key_2",
				"Key_3",
				"Key_4",
				"Key_5",
				"Key_6",
				"Key_7",
				"Key_8",
				"Key_9",
				"Key_A",
				"Key_B",
				"Key_C",
				"Key_D",
				"Key_E",
				"Key_F",
				"Key_G",
				"Key_H",
				"Key_I",
				"Key_J",
				"Key_K",
				"Key_L",
				"Key_M",
				"Key_N",
				"Key_O",
				"Key_P",
				"Key_Q",
				"Key_R",
				"Key_S",
				"Key_T",
				"Key_U",
				"Key_V",
				"Key_W",
				"Key_X",
				"Key_Y",
				"Key_Z",
				"Key_F1",
				"Key_F2",
				"Key_F3",
				"Key_F4",
				"Key_F5",
				"Key_F6",
				"Key_F7",
				"Key_F8",
				"Key_F9",
				"Key_F10",
				"Key_F11",
				"Key_F12",
				"Key_NUM_LOCK",
				"Key_NUM_0",
				"Key_NUM_1",
				"Key_NUM_2",
				"Key_NUM_3",
				"Key_NUM_4",
				"Key_NUM_5",
				"Key_NUM_6",
				"Key_NUM_7",
				"Key_NUM_8",
				"Key_NUM_9",
				"Key_NUM_STAR",
				"Key_NUM_PLUS",
				"Key_NUM_MINUS",
				"Key_NUM_DOT",
				"Key_NUM_DIV",
				"KeyReleased_NOKEY",
				"KeyReleased_ANYKEY",
				"KeyReleased_BACKSPACE",
				"KeyReleased_TAB",
				"KeyReleased_ENTER",
				"KeyReleased_SHIFT",
				"KeyReleased_CTRL",
				"KeyReleased_ALT",
				"KeyReleased_PAUSE",
				"KeyReleased_ESCAPE",
				"KeyReleased_SPACE",
				"KeyReleased_PAGEUP",
				"KeyReleased_PAGEDOWN",
				"KeyReleased_END",
				"KeyReleased_HOME",
				"KeyReleased_LEFT",
				"KeyReleased_UP",
				"KeyReleased_RIGHT",
				"KeyReleased_DOWN",
				"KeyReleased_INSERT",
				"KeyReleased_DELETE",
				"KeyReleased_0",
				"KeyReleased_1",
				"KeyReleased_2",
				"KeyReleased_3",
				"KeyReleased_4",
				"KeyReleased_5",
				"KeyReleased_6",
				"KeyReleased_7",
				"KeyReleased_8",
				"KeyReleased_9",
				"KeyReleased_A",
				"KeyReleased_B",
				"KeyReleased_C",
				"KeyReleased_D",
				"KeyReleased_E",
				"KeyReleased_F",
				"KeyReleased_G",
				"KeyReleased_H",
				"KeyReleased_I",
				"KeyReleased_J",
				"KeyReleased_K",
				"KeyReleased_L",
				"KeyReleased_M",
				"KeyReleased_N",
				"KeyReleased_O",
				"KeyReleased_P",
				"KeyReleased_Q",
				"KeyReleased_R",
				"KeyReleased_S",
				"KeyReleased_T",
				"KeyReleased_U",
				"KeyReleased_V",
				"KeyReleased_W",
				"KeyReleased_X",
				"KeyReleased_Y",
				"KeyReleased_Z",
				"KeyReleased_F1",
				"KeyReleased_F2",
				"KeyReleased_F3",
				"KeyReleased_F4",
				"KeyReleased_F5",
				"KeyReleased_F6",
				"KeyReleased_F7",
				"KeyReleased_F8",
				"KeyReleased_F9",
				"KeyReleased_F10",
				"KeyReleased_F11",
				"KeyReleased_F12",
				"KeyReleased_NUM_LOCK",
				"KeyReleased_NUM_0",
				"KeyReleased_NUM_1",
				"KeyReleased_NUM_2",
				"KeyReleased_NUM_3",
				"KeyReleased_NUM_4",
				"KeyReleased_NUM_5",
				"KeyReleased_NUM_6",
				"KeyReleased_NUM_7",
				"KeyReleased_NUM_8",
				"KeyReleased_NUM_9",
				"KeyReleased_NUM_STAR",
				"KeyReleased_NUM_PLUS",
				"KeyReleased_NUM_MINUS",
				"KeyReleased_NUM_DOT",
				"KeyReleased_NUM_DIV",
				"GMRooms",
				"pCaption",
				"width",
				"height",
				"speed",
				"persistent",
				"colour",
				"showColour",
				"enableViews",
				"viewClearScreen",
				"pCode",
				"visible",
				"foreground",
				"index",
				"htiled",
				"vtiled",
				"hspeed",
				"vspeed",
				"stretch",
				"backgrounds",
				"views",
				"visible",
				"xview",
				"yview",
				"wview",
				"hview",
				"xport",
				"yport",
				"wport",
				"hport",
				"hborder",
				"vborder",
				"hspeed",
				"vspeed",
				"index",
				"pInstances",
				"id",
				"tiles",
				"xo",
				"yo",
				"depth",
				"RoomOrder",
				"TPageEntries",
				"XOffset",
				"YOffset",
				"CropWidth",
				"CropHeight",
				"ow",
				"oh",
				"tp",
				"fontname",
				"array_set_2D",
				"array_set_1D",
				"array_get_2D",
				"array_get_1D",
				"g_pBuiltIn",
				"GetWithArray",
				"yyInst",
				"Scripts",
				"WebGL",
				"CreateEventOrder",
			};
			string[] array2 = array;
			foreach (string item in array2)
			{
				list.Add(item);
			}
			string outputfilename = Path.Combine(_outputDir, "gmRunner.js");
			Out.WriteLine("Obfuscating...");
			YYObfuscate yYObfuscate = new YYObfuscate();
			yYObfuscate.Obfuscate = ObfuscateObfuscate;
			yYObfuscate.Verbose = Verbose;
			yYObfuscate.PrettyPrint = ObfuscatePrettyPrint;
			yYObfuscate.RemovedUnused = ObfuscateRemoveUnused;
			yYObfuscate.EncodeStrings = ObfuscateEncodeStrings;
			yYObfuscate.DoObfuscate(files, outputfilename, list);
		}

		[STAThread]
		private static int Main(string[] _args)
		{
			Trace.Write("------------ {1} {0} --------------", Assembly.GetExecutingAssembly().GetName().Version.ToString(), Assembly.GetExecutingAssembly().GetName().Name);
            /*if (!Debugger.IsAttached)
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			}*/
            //We dont care if you debug this process :D 


            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			GMLCompile.Test();
			GMLCompile.Code_Init();
			HTMLRunner = "C:\\source\\GameMaker\\Runner\\HTML5\\scripts";
			Verbose = false;
			CompileVerbose = false;
			Out = Console.Out;
			SplashOmit = true;
			TextureType = new eTexType[2];
			SeparateOpaqueAndAlpha = false;
			DisplaySortedTextures = false;
            //RemoveDND = true;
            RemoveDND = false;
            CenterHTML5Game = false;
			NoCache = false;
			HTMLRunner = string.Empty;
			NoIndexHTML = false;
			TextureGroups = new Dictionary<string, List<string>>();
			DoObfuscate = false;
			ObfuscateObfuscate = true;
			ObfuscatePrettyPrint = false;
			ObfuscateRemoveUnused = true;
			ObfuscateEncodeStrings = true;
			LoadingBarName = null;
			CustomLoadingScreen = false;
			ExitCode = 0;
			InhibitErrorOutput = false;
			string PreObfuscateLib = string.Empty;
			TextureScale = 1;
			Studio = false;

            ChovyUI.ChovyUI CUI = new ChovyUI.ChovyUI();
            CUI.ShowDialog();
            TitleID = CUI.GetTitleID();
            string GMPath = CUI.GetGMPath();
            if(!CUI.WasClicked())
            {
                Environment.Exit(-1);
            }

            _args = new string[] { "-c", GMPath };
            CUI.Dispose();

            m_options = new OptionSet().Add("?|help", "display help usage", (Action<string>)delegate
			{
				ShowHelp();
			}).Add("m=|machine=", "set machine type (ios, psp, win, droid)", delegate(string m)
			{
				SetMachineType(m);
			}).Add("t=|tex=", "override opaque texture type (dxt,raw,pvr,png)", delegate(string s)
			{
				TextureType[0] = SetTextureType(s);
			})
				.Add("at=|alphatex=", "override alpha texture type (dxt,raw,pvr,png)", delegate(string s)
				{
					TextureType[1] = SetTextureType(s);
				})
				.Add("o=|outputDir=", "set output directory", delegate(string d)
				{
					OutputDir = d;
				})
				.Add("w=|tpageWidth=", "set texture page width", delegate(int w)
				{
					TexturePageWidth = w;
				})
				.Add("h=|tpageHeight=", "set texture page height", delegate(int h)
				{
					TexturePageHeight = h;
				})
				.Add("wt|writeTextures", "optionally write textures generated to output directory", (Action<string>)delegate
				{
					WriteTextures = true;
				})
				.Add("ww|writeWaves", "optionally write audio waves generated to output directory", (Action<string>)delegate
				{
					WriteTextures = true;
				})
				.Add("so|splashOmit", "optionally disable writing of the Spash screen to output file", (Action<string>)delegate
				{
					SplashOmit = true;
				})
				.Add("dst|DisplaySortedTextures", "optionally display sorted texture information", (Action<string>)delegate
				{
					DisplaySortedTextures = true;
				})
				.Add("c|compile", "do not display gui compile only", (Action<string>)delegate
				{
					CompileOnly = true;
				})
				.Add("s|separate", "separate the alpha and opaque textures (false by default)", (Action<string>)delegate
				{
					SeparateOpaqueAndAlpha = true;
				})
				.Add("v|verbose", "output verbose debug info", (Action<string>)delegate
				{
					Verbose = true;
				})
				.Add("nohtml", "do not output index.html", (Action<string>)delegate
				{
					NoIndexHTML = true;
				})
				.Add("HTMLRunner=", "directory with HTML Runner (will be copied as scripts)", delegate(string d)
				{
					HTMLRunner = d;
				})
				.Add("tg=|TextureGroup=", "Group resources onto texture pages comma param is a filename, file has format <groupname> : <comma delim list of resourcenames>, use # for comment (NOTE: entries MUST all be on the same line", delegate(string f)
				{
					string[] array4 = File.ReadAllLines(f);
					string[] array5 = array4;
					foreach (string text3 in array5)
					{
						string text4 = text3.Trim().Replace(" ", "").Replace("\t", "");
						if (!string.IsNullOrEmpty(text4) && !text4.StartsWith("#"))
						{
							string[] array6 = text4.Split(':');
							if (array6.Length == 2)
							{
								string[] collection2 = array6[1].Split(',');
								string key = array6[0];
								List<string> value = new List<string>(collection2);
								TextureGroups.Add(key, value);
							}
						}
					}
				})
				.Add("to=|TextureOption=", "Set an option for a set of textures via <option> : <comma delimited list of resourcenames>. Valid options are: " + TextureOptions.ValidTextureOptions() + ")", delegate(string f)
				{
					string[] array = File.ReadAllLines(f);
					string[] array2 = array;
					foreach (string text in array2)
					{
						string text2 = text.Trim().Replace(" ", "").Replace("\t", "");
						if (!string.IsNullOrEmpty(text2) && !text2.StartsWith("#"))
						{
							string[] array3 = text2.Split(':');
							if (array3.Length == 2)
							{
								string[] collection = array3[1].Split(',');
								string optionName = array3[0];
								List<string> resources = new List<string>(collection);
								TextureOptions.AddResourceOptions(optionName, resources);
							}
						}
					}
				})
				.Add("nodnd", "remove any Drag and Drop (dnd)", (Action<string>)delegate
				{
					RemoveDND = true;
				})
				.Add("obfuscate|ob", "obfuscate the Javascript output", (Action<string>)delegate
				{
					DoObfuscate = true;
				})
				.Add("obfuscateDo=|obob=", "Really do the obfuscation (default true)", delegate(bool v)
				{
					ObfuscateObfuscate = v;
				})
				.Add("obfuscatePrettyPrint=|obpp=", "when obfuscating Pretty Print the output (default false)", delegate(bool v)
				{
					ObfuscatePrettyPrint = v;
				})
				.Add("obfuscateRemoveUnused=|obru=", "when obfuscating Remove Unused functions from the output (default true)", delegate(bool v)
				{
					ObfuscateRemoveUnused = v;
				})
				.Add("obfuscateEncodeStrings=|obes=", "when obfuscating Encode strings in the output (default true)", delegate(bool v)
				{
					ObfuscateEncodeStrings = v;
				})
				.Add("compileVerbose|cv", "switch on verbose mode when compiling the GML (default false)", (Action<string>)delegate
				{
					CompileVerbose = true;
				})
				.Add("c_html5", "Center the HTML5 game in the browser", (Action<string>)delegate
				{
					CenterHTML5Game = true;
				})
				.Add("nocache_html5", "Add the \"no cache\" option to the default index.html page", (Action<string>)delegate
				{
					NoCache = true;
				})
				.Add("preObfuscateLib=|pob=", "Pre-Obfuscate a directory of JS files into a single file - keep GM public interface", delegate(string v)
				{
					PreObfuscateLib = v;
				})
				.Add("textureScale=|ts=", "scale textures by an integer amount (default 1)", delegate(int v)
				{
					TextureScale = v;
				})
				.Add("loadingbarcallback=", "Name of the loading bar callback function", delegate(string v)
				{
					LoadingBarName = v;
				})
				.Add("customloadingimage", "Use the custom screen provided?", (Action<string>)delegate
				{
					CustomLoadingScreen = true;
				})
				.Add("studio", "Enable Studio use...", (Action<string>)delegate
				{
					Studio = true;
				});
			List<string> list = m_options.Parse(_args);
			if (!string.IsNullOrEmpty(PreObfuscateLib))
			{
				DoPreObfuscateLib(PreObfuscateLib, OutputDir);
				return ExitCode;
			}
			GMAssets gMAssets = null;
			foreach (string item in list)
			{
				if (File.Exists(item))
				{
					gMAssets = Loader.Load(item);
				}
			}
			if (MachineType == null)
			{
				SetMachineType("psp");
			}
			if (gMAssets == null)
			{

                return -1;

				/*
                OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.Filter = "Exe files (*.exe, *.ios, *.psp, *.win, *.droid)|*.exe;*.ios;*.psp;*.win;*.droid|All files (*.*)|*.*";
				openFileDialog.FilterIndex = 1;
				openFileDialog.RestoreDirectory = true;
				if (openFileDialog.ShowDialog() != DialogResult.OK)
				{
					return -1;
				}
				gMAssets = Loader.Load(openFileDialog.FileName);
			    */
            }
			if (OutputDir == null)
			{
				if (gMAssets != null) OutputDir = Path.GetDirectoryName(gMAssets.FileName);
			}
			Assets = gMAssets;
			if (!CompileOnly)
			{

                Application.Run(new Form1(gMAssets));
			}
			else
			{
				string extension = MachineType.Extension;
				if (Studio)
				{
					extension = ".zip";
				}
				string name = Path.Combine(OutputDir, Path.ChangeExtension(Path.GetFileName(gMAssets.FileName), extension));
				switch (MachineType.OutputType)
				{
				case eOutputType.eWAD:
					IFFSaver.Save(gMAssets, name);
					break;
				case eOutputType.eHTML5:
					HTML5Saver.Save(gMAssets, name);
					break;
				}
			}
			return ExitCode;
		}

		private static string EncodeText(string _inp)
		{
			string text = "";
			for (int i = 0; i < _inp.Length; i++)
			{
				text += (char)(_inp[i] ^ 0x15);
			}
			return text;
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				Exception ex = (Exception)e.ExceptionObject;
				Console.Out.WriteLine("Error : Exception : {0}", ex.ToString());
				StringBuilder stringBuilder = new StringBuilder();
				string[] array = File.ReadAllLines(Trace.Filename);
				for (int i = (array.Length > 30) ? (array.Length - 30) : 0; i < array.Length; i++)
				{
					stringBuilder.AppendLine(array[i]);
				}
				Trace.Write("############## Unhandled Exception! ##################");
				Trace.Write(ex.ToString());
				MailMessage mailMessage = new MailMessage(EncodeText("lzlzvy|p{aUlzlzrtxpf;vzx"), EncodeText("lzlzvy|p{aUlzlzrtxpf;vzx"));
				mailMessage.IsBodyHtml = true;
				mailMessage.Subject = string.Format("YoYoClient Unhandled Exception : {0}", Environment.MachineName);
				mailMessage.Body = "<html><body><pre>Exception : " + Environment.NewLine + ex.ToString() + "</pre><table><tr><td>OS</td><td>" + Environment.OSVersion.ToString() + "</td><tr><td>ProcessorCount</td><td>" + Environment.ProcessorCount.ToString() + "</td><tr><td>CLR version</td><td>" + Environment.Version.ToString() + "</td><tr><td>UserName</td><td>" + Environment.UserName.ToString() + "</td><tr><td>MachineName</td><td>" + Environment.MachineName.ToString() + "</td><tr><td>Domain</td><td>" + Environment.UserDomainName.ToString() + "</td><tr><td>Working Set size</td><td>" + Environment.WorkingSet.ToString() + "</td></table><pre>TraceLog (last 100 lines)" + stringBuilder.ToString() + "</pre></body></html>";
				string str = "Exception : " + Environment.NewLine + ex.ToString() + Environment.NewLine + "OS=" + Environment.OSVersion.ToString() + Environment.NewLine + "ProcessorCount=" + Environment.ProcessorCount.ToString() + Environment.NewLine + "CLR version=" + Environment.Version.ToString() + Environment.NewLine + "UserName=" + Environment.UserName.ToString() + Environment.NewLine + "MachineName=" + Environment.MachineName.ToString() + Environment.NewLine + "Domain=" + Environment.UserDomainName.ToString() + Environment.NewLine + "Working Set size=" + Environment.WorkingSet.ToString() + Environment.NewLine + "TraceLog (last 100 lines)" + Environment.NewLine + stringBuilder.ToString() + Environment.NewLine + Environment.NewLine;
				if (MessageBox.Show(str + "GMAssetCompiler has encountered an exception, can I send an email to the YoYo Games development team with the information?", "Exception - email YoYo games?", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					SmtpClient smtpClient = new SmtpClient(EncodeText("t`a};fxae;$t{q$;vz;`~"), 25);
					smtpClient.Credentials = new NetworkCredential(EncodeText("lzlzvy|p{aUlzlzrtxpf;vzx"), EncodeText("lzlzvy|p{a$"));
					smtpClient.Send(mailMessage);
				}
			}
			finally
			{
				Environment.Exit(-2);
			}
		}
	}
}
