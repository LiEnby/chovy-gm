using GMAssetCompiler.Output;
using Ionic.Zip;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UmdGen;

namespace GMAssetCompiler
{
	internal class IFFSaver
	{
		private delegate void WriteDelegateKVP<T>(KeyValuePair<string, T> _kvp, Stream _s, IFF _iff, long _index);

		private delegate void WriteDelegate<T>(T _t, Stream _s, IFF _iff, long _index);

		private const int WAD_VERSION_NUMBER = 6;

		private static TexturePage ms_tpageSprites;

		private static List<Wave> ms_Waves;

		private static List<GML2VM> ms_code;

		static IFFSaver()
		{
			ms_Waves = new List<Wave>(20);
			ms_code = new List<GML2VM>();
		}

		public static void Save(GMAssets _assets, string _name)
		{
            String InputFolder = Path.Combine(Program.OutputDir, "_iso_temp");
            _name = Path.Combine(Program.OutputDir, "_iso_temp", "PSP_GAME", "USRDIR", "games", "game.psp"); // Allways "game.psp"

            ms_code.Clear();
			if (Program.RemoveDND)
			{
				_assets.RemoveDND();
				foreach (KeyValuePair<string, GMScript> script in _assets.Scripts)
				{
					if (script.Value != null && !string.IsNullOrEmpty(script.Value.Script))
					{
						GMLCode code = new GMLCode(_assets, script.Key, script.Value.Script, eGMLCodeType.eScript);
						GML2VM gML2VM = new GML2VM();
						gML2VM.Compile(_assets, code);
						ms_code.Add(gML2VM);
					}
				}
				foreach (KeyValuePair<string, GMObject> @object in _assets.Objects)
				{
					if (@object.Value != null)
					{
						foreach (IList<KeyValuePair<int, GMEvent>> @event in @object.Value.Events)
						{
							foreach (KeyValuePair<int, GMEvent> item in @event)
							{
								if (item.Value != null && item.Value.Actions != null && item.Value.Actions.Count > 0 && item.Value.Actions[0].Kind == eAction.ACT_CODE)
								{
									GMLCode code2 = new GMLCode(_assets, @object.Key, item.Value.Actions[0].Args[0], eGMLCodeType.eEvent);
									GML2VM gML2VM2 = new GML2VM();
									gML2VM2.Compile(_assets, code2);
									ms_code.Add(gML2VM2);
								}
							}
						}
					}
				}
				foreach (KeyValuePair<string, GMTimeLine> timeLine in _assets.TimeLines)
				{
					if (timeLine.Value != null)
					{
						foreach (KeyValuePair<int, GMEvent> entry in timeLine.Value.Entries)
						{
							if (entry.Value != null && entry.Value.Actions != null && entry.Value.Actions.Count > 0 && entry.Value.Actions[0].Kind == eAction.ACT_CODE)
							{
								GMLCode code3 = new GMLCode(_assets, timeLine.Key, entry.Value.Actions[0].Args[0], eGMLCodeType.eEvent);
								GML2VM gML2VM3 = new GML2VM();
								gML2VM3.Compile(_assets, code3);
								ms_code.Add(gML2VM3);
							}
						}
					}
				}
				foreach (GMTrigger trigger in _assets.Triggers)
				{
					if (trigger != null)
					{
						GMLCode code4 = new GMLCode(_assets, trigger.Name, trigger.Condition, eGMLCodeType.eTrigger);
						GML2VM gML2VM4 = new GML2VM();
						gML2VM4.Compile(_assets, code4);
						ms_code.Add(gML2VM4);
					}
				}
				foreach (KeyValuePair<string, GMRoom> room in _assets.Rooms)
				{
					if (room.Value != null)
					{
						if (!string.IsNullOrEmpty(room.Value.Code))
						{
							GMLCode code5 = new GMLCode(_assets, room.Key, room.Value.Code, eGMLCodeType.eRoomCreate);
							GML2VM gML2VM5 = new GML2VM();
							gML2VM5.Compile(_assets, code5);
							ms_code.Add(gML2VM5);
						}
						foreach (GMInstance instance in room.Value.Instances)
						{
							if (!string.IsNullOrEmpty(instance.Code))
							{
								GMLCode code6 = new GMLCode(_assets, room.Key, instance.Code, eGMLCodeType.eRoomInstanceCreate);
								GML2VM gML2VM6 = new GML2VM();
								gML2VM6.Compile(_assets, code6);
								ms_code.Add(gML2VM6);
							}
						}
					}
				}
			}
			ms_tpageSprites = new TexturePage(2, 2, 0, 0, Program.MachineType.TPageWidth, Program.MachineType.TPageHeight);
			ms_Waves.Clear();
			Program.Out.WriteLine("Saving IFF file... {0}", _name);
			List<string> list = new List<string>();
			if (Path.GetExtension(_name) == ".zip")
			{
				using (FileStream stream = File.Create(_name))
				{
					ZipOutputStream zipOutputStream = new ZipOutputStream(stream);
					zipOutputStream.PutNextEntry(string.Format("assets\\game{0}", Program.MachineType.Extension));
					Save(_assets, zipOutputStream, list);
					foreach (string item2 in list)
					{
						if (File.Exists(item2))
						{
							string entryName = Path.Combine("assets", Path.GetFileName(item2));
							zipOutputStream.PutNextEntry(entryName);
							byte[] array = File.ReadAllBytes(item2);
							zipOutputStream.Write(array, 0, array.Length);
						}
					}
					zipOutputStream.Close();
				}
			}
			else
			{
				using (FileStream stream2 = File.Create(_name))
				{
					Save(_assets, stream2, list);
				}
			}
            String ISOPath = Path.Combine(Program.OutputDir, Path.ChangeExtension(Program.Assets.FileName, "ISO"));
            Console.WriteLine("Building ISO");

            String UmiFile = Path.Combine(Program.OutputDir, "UmiFile.umi");
            String UflFile = Path.ChangeExtension(UmiFile, "ufl");
            UMDGEN.CreateUmi(UmiFile);
            UMDGEN.CreateUfl(UflFile, InputFolder);
            UMDGEN.CreateISO(UmiFile, Program.OutputDir);

            File.Delete(Path.Combine(Program.OutputDir, "UMD_AUTH.DAT"));
            File.Delete(Path.Combine(Program.OutputDir, "CONT_L0.IMG"));
            File.Delete(Path.Combine(Program.OutputDir, "MDI.IMG"));
            File.Delete(Path.Combine(Program.OutputDir, "UmiFile.ufl"));
            File.Delete(Path.Combine(Program.OutputDir, "UmiFile.umi"));
            if(File.Exists(ISOPath))
            {
                File.Delete(ISOPath);
            }

            File.Move(Path.Combine(Program.OutputDir, "USER_L0.IMG"), ISOPath);
            if (Directory.Exists(InputFolder))
            {
                Directory.Delete(InputFolder, true);
            }
            Console.WriteLine("Done!");

            MessageBox.Show("ISO Built @ " + ISOPath, "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public static void Save(GMAssets _assets, Stream _stream, List<string> _extraFilenames)
		{
			IFF iFF = new IFF();
			iFF.ExternalFiles = _extraFilenames;
			string name = "GENL";
			switch (_assets.Version)
			{
			case 700:
			case 701:
				name = "GEN7";
				break;
			case 800:
			case 810:
				name = "GEN8";
				break;
			}
			iFF.RegisterChunk(name, WriteHeader, _assets, IFFChunkType.CPU);
			iFF.RegisterChunk("TXTR", WriteTextures, _assets, IFFChunkType.GPU | IFFChunkType.Align, 128);
			iFF.RegisterChunk("AUDO", WriteWaveforms, _assets, IFFChunkType.Audio);
			iFF.RegisterChunk("HELP", WriteHelp, _assets.Help, IFFChunkType.CPU);
			iFF.RegisterChunk("OPTN", WriteOptions, _assets.Options, IFFChunkType.CPU);
			iFF.RegisterChunk("EXTN", WriteExtensions, _assets.Extensions, IFFChunkType.CPU);
			iFF.RegisterChunk("SOND", WriteSounds, _assets.Sounds, IFFChunkType.CPU);
			iFF.RegisterChunk("SPRT", WriteSprites, _assets.Sprites, IFFChunkType.CPU);
			iFF.RegisterChunk("BGND", WriteBackgrounds, _assets.Backgrounds, IFFChunkType.CPU);
			iFF.RegisterChunk("PATH", WritePaths, _assets.Paths, IFFChunkType.CPU);
			iFF.RegisterChunk("SCPT", WriteScripts, _assets.Scripts, IFFChunkType.CPU);
			iFF.RegisterChunk("FONT", WriteFonts, _assets.Fonts, IFFChunkType.CPU);
			iFF.RegisterChunk("TMLN", WriteTimelines, _assets.TimeLines, IFFChunkType.CPU);
			iFF.RegisterChunk("OBJT", WriteObjects, _assets.Objects, IFFChunkType.CPU);
			iFF.RegisterChunk("ROOM", WriteRooms, _assets.Rooms, IFFChunkType.CPU);
			iFF.RegisterChunk("DAFL", WriteDataFiles, _assets.DataFiles, IFFChunkType.CPU);
			iFF.RegisterChunk("TPAGE", WriteTexturePages, _assets, IFFChunkType.CPU);
			iFF.RegisterChunk("STRG", (IFFChunkSaver<IList<IFFString>>)WriteStrings, (IList<IFFString>)iFF.Strings, IFFChunkType.CPU);
			iFF.WriteChunks(_stream);
		}

		private static void WriteDataKVP<T>(IList<KeyValuePair<string, T>> _data, Stream _s, IFF _iff, WriteDelegateKVP<T> _del)
		{
			List<long> list = new List<long>();
			_s.WriteInteger(_data.Count);
			for (int i = 0; i < _data.Count; i++)
			{
				list.Add(_s.Position);
				_s.WriteInteger(0);
			}
			int num = 0;
			foreach (KeyValuePair<string, T> _datum in _data)
			{
				if (_datum.Value != null)
				{
					_del(_datum, _s, _iff, list[num]);
				}
				num++;
			}
		}

		private static void WriteDataList<T>(IList<T> _data, Stream _s, IFF _iff, WriteDelegate<T> _del)
		{
			List<long> list = new List<long>();
			_s.WriteInteger(_data.Count);
			for (int i = 0; i < _data.Count; i++)
			{
				list.Add(_s.Position);
				_s.WriteInteger(0);
			}
			int num = 0;
			foreach (T _datum in _data)
			{
				_del(_datum, _s, _iff, list[num]);
				num++;
			}
		}

		private static void WriteOptions(GMOptions _data, Stream _s, IFF _iff)
		{
			_s.WriteBoolean(_data.FullScreen);
			_s.WriteBoolean(_data.InterpolatePixels);
			_s.WriteBoolean(_data.NoBorder);
			_s.WriteBoolean(_data.ShowCursor);
			_s.WriteInteger(_data.Scale);
			_s.WriteBoolean(_data.Sizeable);
			_s.WriteBoolean(_data.StayOnTop);
			_s.WriteInteger(_data.WindowColour);
			_s.WriteBoolean(_data.ChangeResolution);
			_s.WriteInteger(_data.ColorDepth);
			_s.WriteInteger(_data.Resolution);
			_s.WriteInteger(_data.Frequency);
			_s.WriteBoolean(_data.NoButtons);
			_s.WriteInteger(_data.Sync_Vertex);
			_s.WriteBoolean(_data.ScreenKey);
			_s.WriteBoolean(_data.HelpKey);
			_s.WriteBoolean(_data.QuitKey);
			_s.WriteBoolean(_data.SaveKey);
			_s.WriteBoolean(_data.ScreenShotKey);
			_s.WriteBoolean(_data.CloseSec);
			_s.WriteInteger(_data.Priority);
			_s.WriteBoolean(_data.Freeze);
			_s.WriteBoolean(_data.ShowProgress);
			if (_data.BackImage != null && !Program.SplashOmit)
			{
				TexturePageEntry o = ms_tpageSprites.AddImage(_data.BackImage, true, false);
				_iff.AddPatch(_s, o);
			}
			else
			{
				_s.WriteInteger(0);
			}
			if (_data.FrontImage != null && !Program.SplashOmit)
			{
				TexturePageEntry o2 = ms_tpageSprites.AddImage(_data.FrontImage, true, false);
				_iff.AddPatch(_s, o2);
			}
			else
			{
				_s.WriteInteger(0);
			}
			if (_data.LoadImage != null && !Program.SplashOmit)
			{
				TexturePageEntry o3 = ms_tpageSprites.AddImage(_data.LoadImage, true, false);
				_iff.AddPatch(_s, o3);
			}
			else
			{
				_s.WriteInteger(0);
			}
			_s.WriteBoolean(_data.LoadTransparent);
			_s.WriteInteger(_data.LoadAlpha);
			_s.WriteBoolean(_data.ScaleProgress);
			_s.WriteBoolean(_data.DisplayErrors);
			_s.WriteBoolean(_data.WriteErrors);
			_s.WriteBoolean(_data.AbortErrors);
			_s.WriteBoolean(_data.VariableErrors);
			_s.WriteBoolean(_data.CreationEventOrder);
			int num = 0;
			foreach (KeyValuePair<string, string> constant in _data.Constants)
			{
				int value = 0;
				if (GMLCompile.ms_ConstantCount.TryGetValue(constant.Key, out value) && value > 0)
				{
					num++;
				}
			}
			_s.WriteInteger(num);
			foreach (KeyValuePair<string, string> constant2 in _data.Constants)
			{
				int value2 = 0;
				if (GMLCompile.ms_ConstantCount.TryGetValue(constant2.Key, out value2) && value2 > 0)
				{
					_iff.AddString(_s, constant2.Key);
					_iff.AddString(_s, constant2.Value);
				}
			}
		}

		private static void WriteHelp(GMHelp _data, Stream _s, IFF _iff)
		{
			_s.WriteInteger(_data.BackgroundColour);
			_s.WriteBoolean(_data.Mimic);
			_iff.AddString(_s, _data.Caption);
			_s.WriteInteger(_data.Left);
			_s.WriteInteger(_data.Top);
			_s.WriteInteger(_data.Width);
			_s.WriteInteger(_data.Height);
			_s.WriteBoolean(_data.Border);
			_s.WriteBoolean(_data.Sizable);
			_s.WriteBoolean(_data.OnTop);
			_s.WriteBoolean(_data.Modal);
			_iff.AddString(_s, _data.Text);
		}

		private static void WriteExtensions(IList<GMExtension> _data, Stream _s, IFF _iff)
		{
		}

		private static void WriteSounds(IList<KeyValuePair<string, GMSound>> _data, Stream _s, IFF _iff)
		{
            Random rnd = new Random();
            int NumSounds = _data.Count;
            Console.WriteLine("Making Audio Files ISO9660 Compatible");

            for (int i = 0; i < NumSounds; i++)
            {
                if (!(_data[i].ToString() == "[, ]"))
                {
                    GMSound Sound = _data[i].Value;
                    byte[] AudioName = new byte[0x5];
                    rnd.NextBytes(AudioName);
                    string OriginalName = Sound.OrigName;
                    string Extension = Path.GetExtension(Sound.OrigName);
                    Sound.OrigName = Path.ChangeExtension(BitConverter.ToString(AudioName).Replace("-", ""), Extension);
                    Console.WriteLine("Renaming: " + OriginalName + "->" + Sound.OrigName);
                }
            }

            WriteDataKVP(_data, _s, _iff, delegate(KeyValuePair<string, GMSound> _kvp, Stream __s, IFF __iff, long __index)
			{
				__s.PatchOffset(__index);
				__iff.AddString(__s, _kvp.Key);
				GMSound value = _kvp.Value;
				string[] source = value.OrigName.Split('\\', '/', ':');
				string text = Path.GetFileName(source.Last());
				bool flag = true;
				while (flag)
				{
					flag = false;
					foreach (Wave ms_Wave in ms_Waves)
					{
						if (ms_Wave.FileName == text)
						{
							text = string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(text), "_a", Path.GetExtension(text));
							flag = true;
						}
					}
				}
				if (value.Data != null)
				{
					ms_Waves.Add(new Wave(_iff, value.Data, text));
				}
				__s.WriteInteger(value.Kind);
				__iff.AddString(__s, value.Extension);
				__iff.AddString(__s, text);
				__s.WriteInteger(value.Effects);
				__s.WriteSingle((float)value.Volume);
				__s.WriteSingle((float)value.Pan);
				__s.WriteBoolean(value.Preload);
				__s.WriteInteger(ms_Waves.Count - 1);
			});
		}

		private static void WriteSprites(IList<KeyValuePair<string, GMSprite>> _data, Stream _s, IFF _iff)
		{
			WriteDataKVP(_data, _s, _iff, delegate(KeyValuePair<string, GMSprite> _kvp, Stream __s, IFF __iff, long __index)
			{
				__s.Align(4);
				__s.PatchOffset(__index);
				__iff.AddString(__s, _kvp.Key);
				GMSprite value = _kvp.Value;
				__s.WriteInteger(value.Width);
				__s.WriteInteger(value.Height);
				__s.WriteInteger(value.BBoxLeft);
				__s.WriteInteger(value.BBoxRight);
				__s.WriteInteger(value.BBoxBottom);
				__s.WriteInteger(value.BBoxTop);
				__s.WriteBoolean(value.Transparent);
				__s.WriteBoolean(value.Smooth);
				__s.WriteBoolean(value.Preload);
				__s.WriteInteger(value.BBoxMode);
				__s.WriteBoolean(value.ColCheck);
				__s.WriteInteger(value.XOrig);
				__s.WriteInteger(value.YOrig);
				__s.WriteInteger(value.Images.Count);
				ms_tpageSprites.BeginGroup(_kvp.Key);
				for (int i = 0; i < value.Images.Count; i++)
				{
					if (value.Images[i].Width * value.Images[i].Height > 0)
					{
						TexturePageEntry o = ms_tpageSprites.AddImage(value.Images[i].Bitmap, true, false);
						__iff.AddPatch(__s, o);
					}
					else
					{
						__s.WriteInteger(0);
					}
				}
				ms_tpageSprites.EndGroup();
				IList<byte[]> masks = value.Masks;
				if (masks != null)
				{
					__s.WriteInteger(value.Masks.Count);
					foreach (byte[] item in masks)
					{
						__s.Write(item, 0, item.Length);
					}
				}
				else
				{
					__s.WriteInteger(0);
				}
			});
		}

		private static void WriteBackgrounds(IList<KeyValuePair<string, GMBackground>> _data, Stream _s, IFF _iff)
		{
			WriteDataKVP(_data, _s, _iff, delegate(KeyValuePair<string, GMBackground> _kvp, Stream __s, IFF __iff, long __index)
			{
				__s.PatchOffset(__index);
				__iff.AddString(__s, _kvp.Key);
				GMBackground value = _kvp.Value;
				__s.WriteBoolean(value.Transparent);
				__s.WriteBoolean(value.Smooth);
				__s.WriteBoolean(value.Preload);
				if (value.Bitmap != null && value.Bitmap.Width * value.Bitmap.Height > 0)
				{
					ms_tpageSprites.BeginGroup(_kvp.Key);
					TexturePageEntry texturePageEntry = ms_tpageSprites.AddImage(value.Bitmap.Bitmap, true, false);
					ms_tpageSprites.EndGroup();
					texturePageEntry.OriginalRepeatBorder = true;
					texturePageEntry.RepeatX = 2;
					texturePageEntry.RepeatY = 2;
					TextureOptions.SetTextureOptions(_kvp.Key, texturePageEntry);
					__iff.AddPatch(__s, texturePageEntry);
				}
				else
				{
					__s.WriteInteger(0);
				}
			});
		}

		private static void WritePaths(IList<KeyValuePair<string, GMPath>> _data, Stream _s, IFF _iff)
		{
			WriteDataKVP(_data, _s, _iff, delegate(KeyValuePair<string, GMPath> _kvp, Stream __s, IFF __iff, long __index)
			{
				__s.PatchOffset(__index);
				__iff.AddString(__s, _kvp.Key);
				GMPath value = _kvp.Value;
				__s.WriteInteger(value.Kind);
				__s.WriteBoolean(value.Closed);
				__s.WriteInteger(value.Precision);
				__s.WriteInteger(value.Points.Count);
				foreach (GMPathPoint point in value.Points)
				{
					__s.WriteSingle((float)point.X);
					__s.WriteSingle((float)point.Y);
					__s.WriteSingle((float)point.Speed);
				}
			});
		}

		private static void WriteScripts(IList<KeyValuePair<string, GMScript>> _data, Stream _s, IFF _iff)
		{
			WriteDataKVP(_data, _s, _iff, delegate(KeyValuePair<string, GMScript> _kvp, Stream __s, IFF __iff, long __index)
			{
				__s.PatchOffset(__index);
				__iff.AddString(__s, _kvp.Key);
				GMScript value = _kvp.Value;
				__iff.AddString(__s, value.Script);
			});
		}

		private static Bitmap Crop(Bitmap _bmp, int _x, int _y, int _w, int _h)
		{
			Bitmap bitmap = new Bitmap(_w, _h, PixelFormat.Format32bppArgb);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawImage(_bmp, new Rectangle(0, 0, _w, _h), new Rectangle(_x, _y, _w, _h), GraphicsUnit.Pixel);
				return bitmap;
			}
		}

		private static void WriteFonts(IList<KeyValuePair<string, GMFont>> _data, Stream _s, IFF _iff)
		{
			WriteDataKVP(_data, _s, _iff, delegate(KeyValuePair<string, GMFont> _kvp, Stream __s, IFF __iff, long __index)
			{
				__s.PatchOffset(__index);
				__iff.AddString(__s, _kvp.Key);
				GMFont value = _kvp.Value;
				__iff.AddString(__s, value.Name);
				__s.WriteInteger(value.Size);
				__s.WriteBoolean(value.Bold);
				__s.WriteBoolean(value.Italic);
				__s.WriteInteger(value.First);
				__s.WriteInteger(value.Last);
				ms_tpageSprites.BeginGroup(_kvp.Key);
				TexturePageEntry texturePageEntry = ms_tpageSprites.AddImage(value.Bitmap, false, true);
				ms_tpageSprites.EndGroup();
				__iff.AddPatch(__s, texturePageEntry);
				double num = 1.0;
				double num2 = 1.0;
				if (texturePageEntry.W != value.Bitmap.Width || texturePageEntry.H != value.Bitmap.Height)
				{
					num = (double)texturePageEntry.W / (double)value.Bitmap.Width;
					num2 = (double)texturePageEntry.H / (double)value.Bitmap.Height;
				}
				num = 1.0 / num;
				num2 = 1.0 / num2;
				__s.WriteSingle((float)num);
				__s.WriteSingle((float)num2);
				__s.WriteInteger(value.Glyphs.Count);
				foreach (GMGlyph glyph in value.Glyphs)
				{
					__s.WriteInteger((int)(((double)glyph.X + num - 1.0) / num));
					__s.WriteInteger((int)(((double)glyph.Y + num2 - 1.0) / num2));
					__s.WriteInteger((int)(((double)glyph.W + num - 1.0) / num));
					__s.WriteInteger((int)(((double)glyph.H + num2 - 1.0) / num2));
					__s.WriteInteger((int)(((double)glyph.Shift + num - 1.0) / num));
					__s.WriteInteger((int)(((double)glyph.Offset + num - 1.0) / num));
				}
			});
		}

		private static void WriteGMEvent(GMEvent _event, Stream _s, IFF _iff)
		{
			WriteDataList(_event.Actions, _s, _iff, delegate(GMAction _action, Stream __s, IFF __iff, long __index)
			{
				__s.PatchOffset(__index);
				__s.WriteInteger(_action.LibID);
				__s.WriteInteger(_action.ID);
				__s.WriteInteger((int)_action.Kind);
				__s.WriteBoolean(_action.UseRelative);
				__s.WriteBoolean(_action.IsQuestion);
				__s.WriteBoolean(_action.UseApplyTo);
				__s.WriteInteger((int)_action.ExeType);
				__iff.AddString(__s, _action.Name);
				__iff.AddString(__s, _action.Code);
				__s.WriteInteger(_action.ArgumentCount);
				__s.WriteInteger(_action.Who);
				__s.WriteBoolean(_action.Relative);
				__s.WriteBoolean(_action.IsNot);
				if (_action.ArgTypes.Count != _action.Args.Count)
				{
					Console.WriteLine("We have a problem here!!");
				}
				__s.WriteInteger(_action.ArgTypes.Count);
				for (int i = 0; i < _action.ArgTypes.Count; i++)
				{
					__s.WriteInteger((int)_action.ArgTypes[i]);
					__iff.AddString(__s, _action.Args[i]);
				}
			});
		}

		private static void WriteTimelines(IList<KeyValuePair<string, GMTimeLine>> _data, Stream _s, IFF _iff)
		{
			WriteDataKVP(_data, _s, _iff, delegate(KeyValuePair<string, GMTimeLine> _kvp, Stream __s, IFF __iff, long __index)
			{
				__s.PatchOffset(__index);
				__iff.AddString(__s, _kvp.Key);
				GMTimeLine value = _kvp.Value;
				__s.WriteInteger(value.Entries.Count);
				foreach (KeyValuePair<int, GMEvent> entry in value.Entries)
				{
					__s.WriteInteger(entry.Key);
					__iff.AddPatch(__s, entry.Value);
				}
				foreach (KeyValuePair<int, GMEvent> entry2 in value.Entries)
				{
					__iff.SetOffset(__s, entry2.Value, __s.Position);
					WriteGMEvent(entry2.Value, __s, __iff);
				}
			});
		}

        private static void WriteObjects(IList<KeyValuePair<string, GMObject>> _data, Stream _s, IFF _iff)
        {
            IFFSaver.WriteDataKVP<GMObject>(_data, _s, _iff, delegate (KeyValuePair<string, GMObject> _kvp, Stream __s, IFF __iff, long __index)
            {
                __s.PatchOffset(__index);
                __iff.AddString(__s, _kvp.Key);
                GMObject value = _kvp.Value;
                __s.WriteInteger(value.SpriteIndex);
                __s.WriteBoolean(value.Visible);
                __s.WriteBoolean(value.Solid);
                __s.WriteInteger(value.Depth);
                __s.WriteBoolean(value.Persistent);
                __s.WriteInteger(value.Parent);
                __s.WriteInteger(value.Mask);
             //   Console.WriteLine("DEBUG:\nOBJECT EVENTS: " + value.Events.ToString());
                IFFSaver.WriteDataList<IList<KeyValuePair<int, GMEvent>>>(value.Events, __s, __iff, delegate (IList<KeyValuePair<int, GMEvent>> _list, Stream ___s, IFF ___iff, long ___index)
                {
                    ___s.PatchOffset(___index);
                    IFFSaver.WriteDataList<KeyValuePair<int, GMEvent>>(_list, ___s, ___iff, delegate (KeyValuePair<int, GMEvent> _entry, Stream ____s, IFF ____iff, long ____index)
                    {
                        ____s.PatchOffset(____index);
                        ____s.WriteInteger(_entry.Key);
                        IFFSaver.WriteGMEvent(_entry.Value, ____s, ____iff);
                    });
                });
            });
        }

        private static void WriteRooms(IList<KeyValuePair<string, GMRoom>> _data, Stream _s, IFF _iff)
        {
            IFFSaver.WriteDataKVP<GMRoom>(_data, _s, _iff, delegate (KeyValuePair<string, GMRoom> _kvp, Stream __s, IFF __iff, long __index)
            {
                __s.PatchOffset(__index);
                __iff.AddString(__s, _kvp.Key);
                GMRoom value = _kvp.Value;
                __iff.AddString(__s, value.Caption);
                __s.WriteInteger(value.Width);
                __s.WriteInteger(value.Height);
                __s.WriteInteger(value.Speed);
                __s.WriteBoolean(value.Persistent);
                __s.WriteInteger(value.Colour);
                __s.WriteBoolean(value.ShowColour);
                __iff.AddString(__s, value.Code);
                __s.WriteBoolean(value.EnableViews);
                long position = __s.Position;
                __s.WriteInteger(0);
                long position2 = __s.Position;
                __s.WriteInteger(0);
                long position3 = __s.Position;
                __s.WriteInteger(0);
                long position4 = __s.Position;
                __s.WriteInteger(0);
                __s.PatchOffset(position);
                IFFSaver.WriteDataList<GMBack>(value.Backgrounds, __s, __iff, delegate (GMBack _back, Stream ___s, IFF ___iff, long ___index)
                {
                    ___s.PatchOffset(___index);
                    ___s.WriteBoolean(_back.Visible);
                    ___s.WriteBoolean(_back.Foreground);
                    ___s.WriteInteger(_back.Index);
                    ___s.WriteInteger(_back.X);
                    ___s.WriteInteger(_back.Y);
                    ___s.WriteBoolean(_back.HTiled);
                    ___s.WriteBoolean(_back.VTiled);
                    ___s.WriteInteger(_back.HSpeed);
                    ___s.WriteInteger(_back.VSpeed);
                    ___s.WriteBoolean(_back.Stretch);
                });
                __s.PatchOffset(position2);
                IFFSaver.WriteDataList<GMView>(value.Views, __s, __iff, delegate (GMView _view, Stream ___s, IFF ___iff, long ___index)
                {
                    ___s.PatchOffset(___index);
                    ___s.WriteBoolean(_view.Visible);
                    ___s.WriteInteger(_view.XView);
                    ___s.WriteInteger(_view.YView);
                    ___s.WriteInteger(_view.WView);
                    ___s.WriteInteger(_view.HView);
                    ___s.WriteInteger(_view.XPort);
                    ___s.WriteInteger(_view.YPort);
                    ___s.WriteInteger(_view.WPort);
                    ___s.WriteInteger(_view.HPort);
                    ___s.WriteInteger(_view.HBorder);
                    ___s.WriteInteger(_view.VBorder);
                    ___s.WriteInteger(_view.HSpeed);
                    ___s.WriteInteger(_view.VSpeed);
                    ___s.WriteInteger(_view.Index);
                });
                __s.PatchOffset(position3);
                IFFSaver.WriteDataList<GMInstance>(value.Instances, __s, __iff, delegate (GMInstance _inst, Stream ___s, IFF ___iff, long ___index)
                {
                    ___s.PatchOffset(___index);
                    ___s.WriteInteger(_inst.X);
                    ___s.WriteInteger(_inst.Y);
                    ___s.WriteInteger(_inst.Index);
                    ___s.WriteInteger(_inst.Id);
                    ___iff.AddString(___s, _inst.Code);
                    ___s.WriteSingle((float)_inst.ScaleX);
                    ___s.WriteSingle((float)_inst.ScaleY);
                    ___s.WriteInteger((int)_inst.Colour);
                    ___s.WriteSingle((float)_inst.Rotation);
                });
                __s.PatchOffset(position4);
                IFFSaver.WriteDataList<GMTile>(value.Tiles, __s, __iff, delegate (GMTile _tile, Stream ___s, IFF ___iff, long ___index)
                {
                    ___s.PatchOffset(___index);
                    ___s.WriteInteger(_tile.X);
                    ___s.WriteInteger(_tile.Y);
                    ___s.WriteInteger(_tile.Index);
                    ___s.WriteInteger(_tile.XO);
                    ___s.WriteInteger(_tile.YO);
                    ___s.WriteInteger(_tile.W);
                    ___s.WriteInteger(_tile.H);
                    ___s.WriteInteger(_tile.Depth);
                    ___s.WriteInteger(_tile.Id);
                    ___s.WriteSingle((float)_tile.XScale);
                    ___s.WriteSingle((float)_tile.YScale);
                    ___s.WriteInteger(_tile.Blend + ((int)(_tile.Alpha * 255.0) << 24));
                });
            });
        }

        private static void WriteDataFiles(IList<KeyValuePair<string, GMDataFile>> _data, Stream _s, IFF _iff)
		{
			foreach (KeyValuePair<string, GMDataFile> _datum in _data)
			{
				GMDataFile value = _datum.Value;
				string text = Path.Combine(Program.OutputDir, value.FileName);
				File.WriteAllBytes(text, value.Data);
				_iff.ExternalFiles.Add(text);
			}
		}

		private static void WriteHeader(GMAssets _data, Stream _s, IFF _iff)
		{
			_s.WriteInteger(((!_data.Debug) ? 1 : 0) | 0x201);
			_iff.AddString(_s, Path.GetFileNameWithoutExtension(_data.FileName));
			_s.WriteInteger(_data.RoomMaxId);
			_s.WriteInteger(_data.RoomMaxTileId);
			_s.WriteInteger(_data.GameID);
			_s.WriteInteger(0);
			_s.WriteInteger(0);
			_s.WriteInteger(0);
			_s.WriteInteger(0);
			_s.WriteInteger(_data.RoomOrder.Count);
			foreach (int item in _data.RoomOrder)
			{
				_s.WriteInteger(item);
			}
		}

		private static void WriteTextures(GMAssets _data, Stream _s, IFF _iff)
		{
			List<byte[]> list = new List<byte[]>();
			if (ms_tpageSprites.Textures != null)
			{
				int num = 0;
				foreach (Texture texture in ms_tpageSprites.Textures)
				{
					Program.Out.Write("{0} Compressing texture... ", num);
					Image _dest = null;
					Bitmap bitmap = texture.Bitmap;
					byte[] array = Form1.createOutTexture(bitmap, eSquishFlags.kDxt5 | eSquishFlags.kColourMetricPerceptual | eSquishFlags.kClusterFitMaxIteration8, out _dest, Program.TextureType[texture.Group]);
					string path = Path.Combine(Program.OutputDir, string.Format("texture_{0}{1}", num, Program.TextureTypeExtension(Program.TextureType[texture.Group])));
					Program.Out.WriteLine("writing texture {0}... ", Path.GetFileName(path));
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
						File.WriteAllBytes(path, array);
					}
					list.Add(array);
					Application.DoEvents();
					num++;
				}
			}
			WriteDataList(list, _s, _iff, delegate(byte[] __tex, Stream __s, IFF __iff, long __index)
			{
				int num2 = 128;
				int num3 = num2 - 1;
				while ((__s.Position & num3) != 0)
				{
					__s.WriteByte(0);
				}
				__s.PatchOffset(__index);
				__s.Write(__tex, 0, __tex.Length);
			});
		}

		private static void WriteTexturePages(GMAssets _data, Stream _s, IFF _iff)
		{
			if (ms_tpageSprites.Entries.Count > 0)
			{
				ms_tpageSprites.Compile();
			}
			IOrderedEnumerable<TexturePageEntry> source = ms_tpageSprites.Entries.OrderBy((TexturePageEntry e) => e.Entry);
			WriteDataList(source.ToList(), _s, _iff, delegate(TexturePageEntry _tpe, Stream __s, IFF __iff, long __index)
			{
				__s.PatchOffset(__index);
				__iff.SetOffset(__s, _tpe, __s.Position);
				__s.WriteShort((short)_tpe.X);
				__s.WriteShort((short)_tpe.Y);
				__s.WriteShort((short)_tpe.W);
				__s.WriteShort((short)_tpe.H);
				__s.WriteShort((short)_tpe.XOffset);
				__s.WriteShort((short)_tpe.YOffset);
				__s.WriteShort((short)_tpe.CropWidth);
				__s.WriteShort((short)_tpe.CropHeight);
				__s.WriteShort((short)_tpe.OW);
				__s.WriteShort((short)_tpe.OH);
				__s.WriteShort((short)_tpe.TP.TP);
			});
		}
        private static void WriteWaveforms(GMAssets _data, Stream _s, IFF _iff)
        {
 

            IFFSaver.WriteDataList<Wave>(IFFSaver.ms_Waves, _s, _iff, delegate (Wave _wave, Stream __s, IFF __iff, long __index)
            {
                int num = 4;
                int num2 = num - 1;
                while ((__s.Position & (long)num2) != 0L)
                {
                    __s.WriteByte(0);
                }
                __s.PatchOffset(__index);
                __s.WriteInteger(_wave.RawWavFile.Length);
                __s.Write(_wave.RawWavFile, 0, _wave.RawWavFile.Length);
                if (Program.WriteWaves && _wave.FileName.ToLower().EndsWith("wav"))
                {
                    File.WriteAllBytes(Path.Combine(Program.OutputDir, Path.GetFileName(_wave.FileName)), _wave.RawWavFile);
                }
            });
            Console.WriteLine("Converting to at3..");
            int NumSounds = _data.Sounds.Count;
            
            for (int i = 0; i < NumSounds; i++)
            {
                if (!(_data.Sounds[i].ToString() == "[, ]"))
                {
                    GMSound Sound = _data.Sounds[i].Value;
                    string OriginalName = Sound.OrigName;
                    string Extension = Path.GetExtension(Sound.OrigName);
                    string AudioFile = Path.Combine(Program.OutputDir, OriginalName);
                    if (Extension.ToLower() == ".mid" || Extension.ToLower() == ".midi")
                    {
                        Console.WriteLine("Converting " + OriginalName + " To .WAV");
                        File.WriteAllBytes(AudioFile, Sound.Data);
                        Process FluidSynth = new Process();
                        FluidSynth.StartInfo.FileName = Path.Combine(Application.StartupPath, "fluidsynth.exe");
                        FluidSynth.StartInfo.WorkingDirectory = Application.StartupPath;
                        FluidSynth.StartInfo.CreateNoWindow = true;
                        FluidSynth.StartInfo.UseShellExecute = false;
                        FluidSynth.StartInfo.RedirectStandardOutput = true;
                        FluidSynth.StartInfo.RedirectStandardError = true;
                        //Change "gm.sf2" to whatever soundfont you want.
                        FluidSynth.StartInfo.Arguments = "-F \"" + Path.ChangeExtension(AudioFile, "wav") + "\" \"gm.sf2\" \"" + AudioFile+"\"";
                        Console.WriteLine(FluidSynth.StartInfo.FileName + " " + FluidSynth.StartInfo.Arguments);
                        FluidSynth.Start();
                        FluidSynth.WaitForExit();
                        if (FluidSynth.ExitCode != 0)
                        {
                            Console.WriteLine(FluidSynth.StandardOutput.ReadToEnd() + FluidSynth.StandardError.ReadToEnd());
                            return;
                        }
                        FluidSynth.Dispose();

                    }
                    else if (Path.GetExtension(OriginalName).ToLower() == ".mp3")
                    {
                        Console.WriteLine("Converting " + OriginalName + " To .WAV");
                        MemoryStream Mp3Stream = new MemoryStream(Sound.Data);
                        Mp3FileReader mp3 = new Mp3FileReader(Mp3Stream);
                        WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3);
                        WaveFileWriter.CreateWaveFile(Path.ChangeExtension(AudioFile, "wav"), pcm);
                        pcm.Close();
                        mp3.Close();
                        pcm.Dispose();
                        mp3.Dispose();
                     }
                     else
                     {
                            continue;
                     }

                    String OutputPath = Path.Combine(Program.OutputDir, "_iso_temp", "PSP_GAME", "USRDIR",Path.ChangeExtension(OriginalName,"at3"));
                    Console.WriteLine("Output: " + OutputPath);
                    Process At3Tool = new Process();
                    At3Tool.StartInfo.FileName = Path.Combine(Application.StartupPath, "at3tool.exe");
                    At3Tool.StartInfo.WorkingDirectory = Application.StartupPath;
                    At3Tool.StartInfo.CreateNoWindow = true;
                    At3Tool.StartInfo.UseShellExecute = false;
                    At3Tool.StartInfo.RedirectStandardOutput = true;
                    At3Tool.StartInfo.RedirectStandardError = true;
                    At3Tool.StartInfo.Arguments = "-e \"" + Path.ChangeExtension(AudioFile, "wav") + "\" \"" + OutputPath + "\"";
                    Console.WriteLine(At3Tool.StartInfo.FileName + " " + At3Tool.StartInfo.Arguments);
                    At3Tool.Start();
                    At3Tool.WaitForExit();
                    if (At3Tool.ExitCode != 0)
                    {
                        Console.WriteLine(At3Tool.StandardOutput.ReadToEnd() + At3Tool.StandardError.ReadToEnd());
                        return;
                    }
                    At3Tool.Dispose();
                  
                    if(File.Exists(AudioFile))
                    {
                        File.Delete(AudioFile);
                    }
                    File.Delete(Path.ChangeExtension(AudioFile, "wav"));
                    }
                }
            
            Console.WriteLine("Done");
        }

        private static void WriteStrings(IList<IFFString> _strings, Stream _s, IFF _iff)
		{
			WriteDataList(_strings, _s, _iff, delegate(IFFString _string, Stream __s, IFF __iff, long __index)
			{
				__s.PatchOffset(__index);
				_s.WriteInteger(_string.String.Length);
				__iff.SetOffset(__s, _string, __s.Position);
				for (int i = 0; i < _string.String.Length; i++)
				{
					_s.WriteByte((byte)_string.String[i]);
				}
				_s.WriteByte(0);
			});
		}
	}
}
