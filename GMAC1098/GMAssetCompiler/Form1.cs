using Flobbster.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GMAssetCompiler
{
	public class Form1 : Form
	{
		public delegate PropertyBag Prepare(object _o);

		private GMAssets m_assets;

		private ToolStripMenuItem m_currentMachineSelected;

		private ToolStripMenuItem[] m_currentTextureSelected;

		private IContainer components;

		private SplitContainer splitContainer1;

		private TreeView treeView1;

		private SplitContainer splitContainer2;

		private PropertyGrid propertyGrid1;

		private PictureBox pictureBox1;

		private ToolStrip toolStrip1;

		private ToolStripButton compileButton;

		private ToolStripDropDownButton targetDDButton;

		private ToolStripMenuItem pSPToolStripMenuItem;

		private ToolStripMenuItem windowsToolStripMenuItem;

		private ToolStripMenuItem iOSToolStripMenuItem;

		private ToolStripButton toolStripButton1;

		private ToolStripTextBox SearchBox;

		private ToolStripDropDownButton toolStripOpaqueTextureButton;

		private ToolStripMenuItem bitToolStripMenuItem;

		private ToolStripMenuItem r8G8B8A8ToolStripMenuItem;

		private ToolStripMenuItem dXTC5ToolStripMenuItem;

		private ToolStripMenuItem pVRTCToolStripMenuItem;

		private ToolStripDropDownButton toolStripAlphaTextureButton;

		private ToolStripMenuItem toolStripMenuItem1;

		private ToolStripMenuItem toolStripMenuItem2;

		private ToolStripMenuItem toolStripMenuItem3;

		private ToolStripMenuItem toolStripMenuItem4;

		private ToolStripMenuItem pNGToolStripMenuItem;

		private ToolStripMenuItem pNGToolStripMenuItem1;

		private ToolStripMenuItem androidToolStripMenuItem;

		private ToolStripMenuItem symbianToolStripMenuItem;

		private ToolStripMenuItem hTML5ToolStripMenuItem;

		private ToolStripDropDownButton toolStripDropDownButton1;

		private ToolStripMenuItem writeTexturesToolStripMenuItem;

		private ToolStripMenuItem writeWavsToolStripMenuItem;

		private ToolStripMenuItem separateAlphaOpaqueTexturesToolStripMenuItem;

		private ToolStripMenuItem noCacheStripMenuItem;

		private ToolStripMenuItem verboseToolStripMenuItem;

		private ToolStripMenuItem obfuscateToolStripMenuItem;

		private ToolStripMenuItem prettyPrintingToolStripMenuItem;

		private ToolStripMenuItem RemoveUnusedFunctionsToolStripMenuItem;

		private ToolStripMenuItem encodeStringsToolStripMenuItem;

		private ToolStripMenuItem obfuscateToolStripMenuItem1;

		public Form1(GMAssets _assets)
		{
			m_assets = _assets;
			InitializeComponent();
			Text = string.Format("GMAssetCompiler : {0} : {1}", Program.MachineType.Name, _assets.FileName);
			m_currentTextureSelected = new ToolStripMenuItem[2];
			foreach (ToolStripMenuItem dropDownItem in targetDDButton.DropDownItems)
			{
				if (string.Compare(dropDownItem.Text, Program.MachineType.Name, true) == 0)
				{
					m_currentMachineSelected = dropDownItem;
					dropDownItem.Checked = true;
					break;
				}
			}
			foreach (ToolStripMenuItem dropDownItem2 in toolStripOpaqueTextureButton.DropDownItems)
			{
				dropDownItem2.Tag = (eTexType)Enum.Parse(typeof(eTexType), dropDownItem2.Tag as string);
				dropDownItem2.Checked = (Program.TextureType[0] == (eTexType)dropDownItem2.Tag);
				if (dropDownItem2.Checked)
				{
					m_currentTextureSelected[0] = dropDownItem2;
				}
			}
			foreach (ToolStripMenuItem dropDownItem3 in toolStripAlphaTextureButton.DropDownItems)
			{
				dropDownItem3.Tag = (eTexType)Enum.Parse(typeof(eTexType), dropDownItem3.Tag as string);
				dropDownItem3.Checked = (Program.TextureType[1] == (eTexType)dropDownItem3.Tag);
				if (dropDownItem3.Checked)
				{
					m_currentTextureSelected[1] = dropDownItem3;
				}
			}
			writeTexturesToolStripMenuItem.Checked = Program.WriteTextures;
			writeWavsToolStripMenuItem.Checked = Program.WriteWaves;
			separateAlphaOpaqueTexturesToolStripMenuItem.Checked = Program.SeparateOpaqueAndAlpha;
			noCacheStripMenuItem.Checked = Program.NoCache;
			verboseToolStripMenuItem.Checked = Program.Verbose;
			obfuscateToolStripMenuItem.Checked = Program.DoObfuscate;
			prettyPrintingToolStripMenuItem.Checked = Program.ObfuscatePrettyPrint;
			RemoveUnusedFunctionsToolStripMenuItem.Checked = Program.ObfuscateRemoveUnused;
			encodeStringsToolStripMenuItem.Checked = Program.ObfuscateEncodeStrings;
			obfuscateToolStripMenuItem1.Checked = Program.ObfuscateObfuscate;
			AddExtensions();
			AddSounds();
			AddSprites();
			AddBackgrounds();
			AddPaths();
			AddScripts();
			AddFonts();
			AddTimeLines();
			AddObjects();
			AddRooms();
			AddDataFiles();
			AddLibraries();
			AddRoomOrder();
			AddOptions();
		}

		private void AddEntry<T>(IList<KeyValuePair<string, T>> _collection, string _name, Type _tag)
		{
			List<TreeNode> list = new List<TreeNode>();
			int num = 0;
			foreach (KeyValuePair<string, T> item in _collection)
			{
				if (!string.IsNullOrEmpty(item.Key))
				{
					TreeNode treeNode = new TreeNode(item.Key + string.Format("({0})", num));
					treeNode.Tag = ((_tag != null) ? Activator.CreateInstance(_tag, item.Value) : null);
					list.Add(treeNode);
				}
				num++;
			}
			TreeNode node = new TreeNode(_name, list.ToArray());
			treeView1.Nodes.Add(node);
		}

		public void AddSprites()
		{
			AddEntry(m_assets.Sprites, "sprites", typeof(ViewSprite));
		}

		public void AddExtensions()
		{
			List<TreeNode> list = new List<TreeNode>();
			foreach (GMExtension extension in m_assets.Extensions)
			{
				list.Add(new TreeNode(extension.Name));
			}
			TreeNode node = new TreeNode("extensions", list.ToArray());
			treeView1.Nodes.Add(node);
		}

		public void AddSounds()
		{
			AddEntry(m_assets.Sounds, "sounds", typeof(View<GMSound>));
		}

		public void AddBackgrounds()
		{
			AddEntry(m_assets.Backgrounds, "backgrounds", typeof(ViewBackground));
		}

		public void AddPaths()
		{
			AddEntry(m_assets.Paths, "paths", typeof(View<GMPath>));
		}

		public void AddScripts()
		{
			AddEntry(m_assets.Scripts, "scripts", typeof(View<GMScript>));
		}

		public void AddFonts()
		{
			AddEntry(m_assets.Fonts, "fonts", typeof(ViewFont));
		}

		public void AddTimeLines()
		{
			AddEntry(m_assets.TimeLines, "timelines", typeof(View<GMTimeLine>));
		}

		public void AddObjects()
		{
			AddEntry(m_assets.Objects, "objects", typeof(View<GMObject>));
		}

		public void AddRooms()
		{
			AddEntry(m_assets.Rooms, "rooms", typeof(View<GMRoom>));
		}

		public void AddDataFiles()
		{
			AddEntry(m_assets.DataFiles, "datafiles", typeof(View<GMDataFile>));
		}

		public void AddLibraries()
		{
			List<TreeNode> list = new List<TreeNode>();
			foreach (string library in m_assets.Libraries)
			{
				list.Add(new TreeNode(library));
			}
			TreeNode node = new TreeNode("libraries", list.ToArray());
			treeView1.Nodes.Add(node);
		}

		public void AddRoomOrder()
		{
			List<TreeNode> list = new List<TreeNode>();
			foreach (int item in m_assets.RoomOrder)
			{
				list.Add(new TreeNode(m_assets.Rooms[item].Key));
			}
			TreeNode node = new TreeNode("roomOrder", list.ToArray());
			treeView1.Nodes.Add(node);
		}

		public void AddOptions()
		{
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertyGrid1.SelectedObject = null;
			if (e.Node.Tag != null)
			{
				IPropertyGrid propertyGrid = (IPropertyGrid)e.Node.Tag;
				PropertyBag selectedObject = propertyGrid.Prepare();
				propertyGrid1.SelectedObject = selectedObject;
				Image image = propertyGrid.PrepareImage();
				pictureBox1.Image = image;
			}
		}

		public static byte[] createOutTexture(Image image, eSquishFlags _flags, out Image _dest, eTexType _type)
		{
			_dest = null;
			switch (_type)
			{
			case eTexType.e4444:
				return CompressImageRaw4444(image, out _dest);
			case eTexType.eRaw:
				return CompressImageRaw(image);
			case eTexType.ePVR:
				return CompressImagePVR(image);
			case eTexType.ePNG:
			{
				MemoryStream memoryStream = new MemoryStream();
				image.Save(memoryStream, ImageFormat.Png);
				return memoryStream.ToArray();
			}
			default:
				return CompressImageDXT(image, _flags);
			}
		}

		private unsafe static byte[] CompressImageRaw(Image image)
		{
			int num = 0;
			Bitmap bitmap = new Bitmap(image);
			Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
			BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			IMachineType machineType = Program.MachineType;
			byte[] array = new byte[image.Width * image.Height * 4 + 16];
			fixed (byte* ptr2 = &array[0])
			{
				IntPtr scan = bitmapData.Scan0;
				IntPtr intPtr = scan;
				int num2 = 0;
				while (num2 < image.Height)
				{
					IntPtr ptr = intPtr;
					int num3 = 0;
					while (num3 < image.Width)
					{
						int num4 = Marshal.ReadInt32(ptr);
						*(uint*)(ptr2 + (long)(4 + num) * 4L) = machineType.Convert8888((num4 >> 24) & 0xFF, num4 & 0xFF, (num4 & 0xFF00) >> 8, (num4 >> 16) & 0xFF);
						num++;
						num3++;
						ptr = new IntPtr(ptr.ToInt64() + 4);
					}
					num2++;
					intPtr = new IntPtr(intPtr.ToInt64() + bitmapData.Stride);
				}
				*(int*)ptr2 = 542589266;
				*(int*)(ptr2 + 4) = image.Width;
				*(int*)(ptr2 + 8) = image.Height;
				*(int*)(ptr2 + 12) = 0;
			}
			bitmap.UnlockBits(bitmapData);
			return array;
		}

		private unsafe static byte[] CompressImageRaw4444(Image image, out Image _destImage)
		{
			Bitmap bitmap = (Bitmap)(_destImage = new Bitmap(image));
			int num = 0;
			Bitmap bitmap2 = new Bitmap(image);
			Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
			BitmapData bitmapData = bitmap2.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			BitmapData bitmapData2 = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			byte[] array = new byte[image.Width * image.Height * 2 + 16];
			fixed (byte* ptr3 = &array[0])
			{
				IntPtr scan = bitmapData.Scan0;
				IntPtr scan2 = bitmapData2.Scan0;
				IntPtr intPtr = scan;
				IntPtr intPtr2 = scan2;
				IMachineType machineType = Program.MachineType;
				int num2 = 0;
				while (num2 < image.Height)
				{
					IntPtr ptr = intPtr;
					IntPtr ptr2 = intPtr2;
					float num3 = 0.4375f;
					float num4 = 0.1875f;
					float num5 = 0.3125f;
					float num6 = 0.0625f;
					int num7 = 0;
					while (num7 < image.Width)
					{
						int num8 = Marshal.ReadInt32(ptr);
						int num9 = num8 & 0xFF;
						int num10 = (num8 >> 8) & 0xFF;
						int num11 = (num8 >> 16) & 0xFF;
						int num12 = (num8 >> 24) & 0xFF;
						*(ushort*)(ptr3 + (long)(8 + num) * 2L) = machineType.Convert4444(num12, num9, num10, num11);
						int num13 = num12 & 0xF0;
						if (num13 != 0)
						{
							num13 |= 0xF;
						}
						Marshal.WriteInt32(ptr2, (num13 << 24) | ((num11 & 0xF0) << 16) | ((num10 & 0xF0) << 8) | (num9 & 0xF0));
						int num14 = num9 & 0xF;
						int num15 = num10 & 0xF;
						int num16 = num11 & 0xF;
						if (num7 < image.Width - 1)
						{
							num8 = Marshal.ReadInt32(ptr, 4);
							num9 = (int)((float)(num8 & 0xFF) + num3 * (float)num14);
							num10 = (int)((float)((num8 >> 8) & 0xFF) + num3 * (float)num15);
							num11 = (int)((float)((num8 >> 16) & 0xFF) + num3 * (float)num16);
							num12 = ((num8 >> 24) & 0xFF);
							num9 = ((num9 >= 0) ? ((num9 > 255) ? 255 : num9) : 0);
							num10 = ((num10 >= 0) ? ((num10 > 255) ? 255 : num10) : 0);
							num11 = ((num11 >= 0) ? ((num11 > 255) ? 255 : num11) : 0);
							num8 = ((num12 << 24) | (num11 << 16) | (num10 << 8) | num9);
							Marshal.WriteInt32(ptr, 4, num8);
						}
						if (num7 > 0 && num2 < image.Height - 1)
						{
							num8 = Marshal.ReadInt32(ptr, -4 + bitmapData.Stride);
							num9 = (int)((float)(num8 & 0xFF) + num4 * (float)num14);
							num10 = (int)((float)((num8 >> 8) & 0xFF) + num4 * (float)num15);
							num11 = (int)((float)((num8 >> 16) & 0xFF) + num4 * (float)num16);
							num12 = ((num8 >> 24) & 0xFF);
							num9 = ((num9 >= 0) ? ((num9 > 255) ? 255 : num9) : 0);
							num10 = ((num10 >= 0) ? ((num10 > 255) ? 255 : num10) : 0);
							num11 = ((num11 >= 0) ? ((num11 > 255) ? 255 : num11) : 0);
							num8 = ((num12 << 24) | (num11 << 16) | (num10 << 8) | num9);
							Marshal.WriteInt32(ptr, -4 + bitmapData.Stride, num8);
						}
						if (num2 < image.Height - 1)
						{
							num8 = Marshal.ReadInt32(ptr, bitmapData.Stride);
							num9 = (int)((float)(num8 & 0xFF) + num5 * (float)num14);
							num10 = (int)((float)((num8 >> 8) & 0xFF) + num5 * (float)num15);
							num11 = (int)((float)((num8 >> 16) & 0xFF) + num5 * (float)num16);
							num12 = ((num8 >> 24) & 0xFF);
							num9 = ((num9 >= 0) ? ((num9 > 255) ? 255 : num9) : 0);
							num10 = ((num10 >= 0) ? ((num10 > 255) ? 255 : num10) : 0);
							num11 = ((num11 >= 0) ? ((num11 > 255) ? 255 : num11) : 0);
							num8 = ((num12 << 24) | (num11 << 16) | (num10 << 8) | num9);
							Marshal.WriteInt32(ptr, bitmapData.Stride, num8);
						}
						if (num7 < image.Width - 1 && num2 < image.Height - 1)
						{
							num8 = Marshal.ReadInt32(ptr, 4 + bitmapData.Stride);
							num9 = (int)((float)(num8 & 0xFF) + num6 * (float)num14);
							num10 = (int)((float)((num8 >> 8) & 0xFF) + num6 * (float)num15);
							num11 = (int)((float)((num8 >> 16) & 0xFF) + num6 * (float)num16);
							num12 = ((num8 >> 24) & 0xFF);
							num9 = ((num9 >= 0) ? ((num9 > 255) ? 255 : num9) : 0);
							num10 = ((num10 >= 0) ? ((num10 > 255) ? 255 : num10) : 0);
							num11 = ((num11 >= 0) ? ((num11 > 255) ? 255 : num11) : 0);
							num8 = ((num12 << 24) | (num11 << 16) | (num10 << 8) | num9);
							Marshal.WriteInt32(ptr, 4 + bitmapData.Stride, num8);
						}
						num++;
						num7++;
						ptr = new IntPtr(ptr.ToInt64() + 4);
						ptr2 = new IntPtr(ptr2.ToInt64() + 4);
					}
					num2++;
					intPtr = new IntPtr(intPtr.ToInt64() + bitmapData.Stride);
					intPtr2 = new IntPtr(intPtr2.ToInt64() + bitmapData2.Stride);
				}
				*(int*)ptr3 = 542589266;
				*(int*)(ptr3 + 4) = image.Width;
				*(int*)(ptr3 + 8) = image.Height;
				*(int*)(ptr3 + 12) = 1;
			}
			bitmap2.UnlockBits(bitmapData);
			return array;
		}

		private static byte[] CompressImagePVR(Image image)
		{
			Bitmap bitmap = new Bitmap(image);
			Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
			BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			IntPtr scan = bitmapData.Scan0;
			IntPtr intPtr = scan;
			int num = 0;
			while (num < image.Height)
			{
				IntPtr ptr = intPtr;
				int num2 = 0;
				while (num2 < image.Width)
				{
					int num3 = Marshal.ReadInt32(ptr);
					int val = (num3 & -16711936) | ((num3 & 0xFF) << 16) | ((num3 & 0xFF0000) >> 16);
					Marshal.WriteInt32(ptr, val);
					num2++;
					ptr = new IntPtr(ptr.ToInt64() + 4);
				}
				num++;
				intPtr = new IntPtr(intPtr.ToInt64() + bitmapData.Stride);
			}
			string text = Path.ChangeExtension(Path.GetTempFileName(), ".pvr");
			IntPtr intPtr2 = Marshal.StringToHGlobalAnsi(text);
			Squish.CompressPVRTC(bitmapData.Scan0, image.Width, image.Height, intPtr2, 0);
			Marshal.FreeHGlobal(intPtr2);
			scan = bitmapData.Scan0;
			intPtr = scan;
			int num4 = 0;
			while (num4 < image.Height)
			{
				IntPtr ptr2 = intPtr;
				int num5 = 0;
				while (num5 < image.Width)
				{
					int num6 = Marshal.ReadInt32(ptr2);
					int val2 = (num6 & -16711936) | ((num6 & 0xFF) << 16) | ((num6 & 0xFF0000) >> 16);
					Marshal.WriteInt32(ptr2, val2);
					num5++;
					ptr2 = new IntPtr(ptr2.ToInt64() + 4);
				}
				num4++;
				intPtr = new IntPtr(intPtr.ToInt64() + bitmapData.Stride);
			}
			byte[] result = File.ReadAllBytes(text);
			bitmap.UnlockBits(bitmapData);
			return result;
		}

		private unsafe static byte[] CompressImageDXT(Image image, eSquishFlags _flags)
		{
			Bitmap bitmap = new Bitmap(image);
			Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
			BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			int storageRequirements = Squish.GetStorageRequirements(image.Width, image.Height, _flags);
			int num = Marshal.SizeOf(typeof(DDS.SDDSHeader));
			storageRequirements += num;
			byte[] array = new byte[storageRequirements + 15];
			fixed (byte* value = &array[num])
			{
				IntPtr scan = bitmapData.Scan0;
				IntPtr intPtr = scan;
				int num2 = 0;
				while (num2 < image.Height)
				{
					IntPtr ptr = intPtr;
					int num3 = 0;
					while (num3 < image.Width)
					{
						int num4 = Marshal.ReadInt32(ptr);
						int val = (num4 & -16711936) | ((num4 & 0xFF) << 16) | ((num4 & 0xFF0000) >> 16);
						Marshal.WriteInt32(ptr, val);
						num3++;
						ptr = new IntPtr(ptr.ToInt64() + 4);
					}
					num2++;
					intPtr = new IntPtr(intPtr.ToInt64() + bitmapData.Stride);
				}
				Squish.CompressImage(bitmapData.Scan0, image.Width, image.Height, new IntPtr(value), _flags);
				intPtr = scan;
				int num5 = 0;
				while (num5 < image.Height)
				{
					IntPtr ptr2 = intPtr;
					int num6 = 0;
					while (num6 < image.Width)
					{
						int num7 = Marshal.ReadInt32(ptr2);
						int val2 = (num7 & -16711936) | ((num7 & 0xFF) << 16) | ((num7 & 0xFF0000) >> 16);
						Marshal.WriteInt32(ptr2, val2);
						num6++;
						ptr2 = new IntPtr(ptr2.ToInt64() + 4);
					}
					num5++;
					intPtr = new IntPtr(intPtr.ToInt64() + bitmapData.Stride);
				}
			}
			bitmap.UnlockBits(bitmapData);
			fixed (byte* ptr3 = &array[0])
			{
				DDS.SDDSHeader* ptr4 = (DDS.SDDSHeader*)ptr3;
				ptr4->Magic = 542327876u;
				ptr4->dwSize = (uint)Marshal.SizeOf(typeof(DDS.SDDSHeader));
				ptr4->dwFlags = 659463u;
				ptr4->dwMipMapCount = 0u;
				ptr4->dwWidth = (uint)image.Width;
				ptr4->dwHeight = (uint)image.Height;
				ptr4->ddpfPixelFormat.dwSize = (uint)Marshal.SizeOf(typeof(DDS.SDDPIXELFORMAT));
				if ((_flags & eSquishFlags.kDxt1) != 0)
				{
					ptr4->ddpfPixelFormat.dwFourCC = 827611204u;
				}
				else if ((_flags & eSquishFlags.kDxt3) != 0)
				{
					ptr4->ddpfPixelFormat.dwFourCC = 861165636u;
				}
				else if ((_flags & eSquishFlags.kDxt5) != 0)
				{
					ptr4->ddpfPixelFormat.dwFourCC = 894720068u;
				}
				ptr4->ddpfPixelFormat.dwFlags = 4u;
				ptr4->ddsCaps.dwCaps1 = 4096u;
			}
			return array;
		}

		private void compileButton_Click(object sender, EventArgs e)
		{
			string extension = Program.MachineType.Extension;
			if (Program.Studio)
			{
				extension = ".zip";
			}
			string name = Path.Combine(Program.OutputDir, Path.ChangeExtension(Path.GetFileName(m_assets.FileName), extension));
			switch (Program.MachineType.OutputType)
			{
			case eOutputType.eWAD:
				IFFSaver.Save(m_assets, name);
				break;
			case eOutputType.eHTML5:
				HTML5Saver.Save(m_assets, name);
				break;
			}
		}

		private void ToolStripMenuItem_MachineSelect_Click(object sender, EventArgs e)
		{
			m_currentMachineSelected.Checked = false;
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.SetMachineType(toolStripMenuItem.Text);
			toolStripMenuItem.Checked = true;
			m_currentMachineSelected = toolStripMenuItem;
			Text = string.Format("GMAssetCompiler : {0} : {1}", Program.MachineType.Name, m_assets.FileName);
			foreach (ToolStripMenuItem dropDownItem in toolStripOpaqueTextureButton.DropDownItems)
			{
				dropDownItem.Checked = (Program.TextureType[0] == (eTexType)dropDownItem.Tag);
				if (dropDownItem.Checked)
				{
					m_currentTextureSelected[0] = dropDownItem;
				}
			}
			foreach (ToolStripMenuItem dropDownItem2 in toolStripAlphaTextureButton.DropDownItems)
			{
				dropDownItem2.Checked = (Program.TextureType[1] == (eTexType)dropDownItem2.Tag);
				if (dropDownItem2.Checked)
				{
					m_currentTextureSelected[1] = dropDownItem2;
				}
			}
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			string text = SearchBox.Text;
			Console.WriteLine("------------- Finding '{0}' ----------------------", text);
			foreach (KeyValuePair<string, GMScript> script in m_assets.Scripts)
			{
				GMScript value = script.Value;
				if (value != null && !string.IsNullOrEmpty(value.Script) && value.Script.Contains(text))
				{
					Console.WriteLine("Found in script - {0}", script.Key);
				}
			}
			foreach (KeyValuePair<string, GMObject> @object in m_assets.Objects)
			{
				GMObject value2 = @object.Value;
				if (value2 != null)
				{
					foreach (IList<KeyValuePair<int, GMEvent>> @event in value2.Events)
					{
						foreach (KeyValuePair<int, GMEvent> item in @event)
						{
							GMEvent value3 = item.Value;
							foreach (GMAction action in value3.Actions)
							{
								foreach (string arg in action.Args)
								{
									if (!string.IsNullOrEmpty(arg) && arg.Contains(text))
									{
										Console.WriteLine("Found in object {0} in event {1} action {2}", @object.Key, item.Key, action.Name);
									}
								}
								if (!string.IsNullOrEmpty(action.Code) && action.Code.Contains(text))
								{
									Console.WriteLine("Found in object {0} in event {1} action {2}", @object.Key, item.Key, action.Name);
								}
							}
						}
					}
				}
			}
		}

		private void ToolStripMenuItem_OpaqueTextureSelect_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem_TextureSelect_Click(sender, e, 0);
		}

		private void ToolStripMenuItem_AlphaTextureSelect_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem_TextureSelect_Click(sender, e, 1);
		}

		private void ToolStripMenuItem_TextureSelect_Click(object sender, EventArgs e, int n)
		{
			m_currentTextureSelected[n].Checked = false;
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.TextureType[n] = (eTexType)toolStripMenuItem.Tag;
			toolStripMenuItem.Checked = true;
			m_currentTextureSelected[n] = toolStripMenuItem;
		}

		private void writeTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.WriteTextures = !Program.WriteTextures;
			toolStripMenuItem.Checked = Program.WriteTextures;
		}

		private void writeWavsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.WriteWaves = !Program.WriteWaves;
			toolStripMenuItem.Checked = Program.WriteWaves;
		}

		private void separateAlphaOpaqueTexturesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.SeparateOpaqueAndAlpha = !Program.SeparateOpaqueAndAlpha;
			toolStripMenuItem.Checked = Program.SeparateOpaqueAndAlpha;
		}

		private void noCacheStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.NoCache = !Program.NoCache;
			toolStripMenuItem.Checked = Program.NoCache;
		}

		private void verboseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.Verbose = !Program.Verbose;
			toolStripMenuItem.Checked = Program.Verbose;
		}

		private void obfuscateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.DoObfuscate = !Program.DoObfuscate;
			toolStripMenuItem.Checked = Program.DoObfuscate;
		}

		private void prettyPrintingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.ObfuscatePrettyPrint = !Program.ObfuscatePrettyPrint;
			toolStripMenuItem.Checked = Program.ObfuscatePrettyPrint;
		}

		private void reToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.ObfuscateRemoveUnused = !Program.ObfuscateRemoveUnused;
			toolStripMenuItem.Checked = Program.ObfuscateRemoveUnused;
		}

		private void encodeStringsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.ObfuscateEncodeStrings = !Program.ObfuscateEncodeStrings;
			toolStripMenuItem.Checked = Program.ObfuscateEncodeStrings;
		}

		private void obfuscateToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			Program.ObfuscateObfuscate = !Program.ObfuscateObfuscate;
			toolStripMenuItem.Checked = Program.ObfuscateObfuscate;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GMAssetCompiler.Form1));
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			treeView1 = new System.Windows.Forms.TreeView();
			splitContainer2 = new System.Windows.Forms.SplitContainer();
			propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			compileButton = new System.Windows.Forms.ToolStripButton();
			targetDDButton = new System.Windows.Forms.ToolStripDropDownButton();
			pSPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			iOSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			androidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			symbianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			hTML5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripOpaqueTextureButton = new System.Windows.Forms.ToolStripDropDownButton();
			bitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			r8G8B8A8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			dXTC5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			pVRTCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			pNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripAlphaTextureButton = new System.Windows.Forms.ToolStripDropDownButton();
			toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			pNGToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			writeTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			writeWavsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			separateAlphaOpaqueTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			noCacheStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			verboseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			obfuscateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			prettyPrintingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			RemoveUnusedFunctionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			encodeStringsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			SearchBox = new System.Windows.Forms.ToolStripTextBox();
			toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			obfuscateToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			splitContainer2.Panel1.SuspendLayout();
			splitContainer2.Panel2.SuspendLayout();
			splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			toolStrip1.SuspendLayout();
			SuspendLayout();
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.Location = new System.Drawing.Point(0, 25);
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Panel1.Controls.Add(treeView1);
			splitContainer1.Panel2.Controls.Add(splitContainer2);
			splitContainer1.Size = new System.Drawing.Size(921, 633);
			splitContainer1.SplitterDistance = 173;
			splitContainer1.TabIndex = 0;
			treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			treeView1.Location = new System.Drawing.Point(0, 0);
			treeView1.Name = "treeView1";
			treeView1.Size = new System.Drawing.Size(173, 633);
			treeView1.TabIndex = 0;
			treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(treeView1_AfterSelect);
			splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer2.Location = new System.Drawing.Point(0, 0);
			splitContainer2.Name = "splitContainer2";
			splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			splitContainer2.Panel1.Controls.Add(propertyGrid1);
			splitContainer2.Panel2.Controls.Add(pictureBox1);
			splitContainer2.Size = new System.Drawing.Size(744, 633);
			splitContainer2.SplitterDistance = 371;
			splitContainer2.TabIndex = 0;
			propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			propertyGrid1.Location = new System.Drawing.Point(0, 0);
			propertyGrid1.Name = "propertyGrid1";
			propertyGrid1.Size = new System.Drawing.Size(744, 371);
			propertyGrid1.TabIndex = 0;
			pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			pictureBox1.Location = new System.Drawing.Point(0, 0);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(744, 258);
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[7]
			{
				compileButton,
				targetDDButton,
				toolStripOpaqueTextureButton,
				toolStripAlphaTextureButton,
				toolStripDropDownButton1,
				SearchBox,
				toolStripButton1
			});
			toolStrip1.Location = new System.Drawing.Point(0, 0);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Size = new System.Drawing.Size(921, 25);
			toolStrip1.TabIndex = 1;
			toolStrip1.Text = "toolStrip1";
			compileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			compileButton.Image = (System.Drawing.Image)resources.GetObject("compileButton.Image");
			compileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			compileButton.Name = "compileButton";
			compileButton.Size = new System.Drawing.Size(56, 22);
			compileButton.Text = "Compile";
			compileButton.Click += new System.EventHandler(compileButton_Click);
			targetDDButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			targetDDButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[6]
			{
				pSPToolStripMenuItem,
				windowsToolStripMenuItem,
				iOSToolStripMenuItem,
				androidToolStripMenuItem,
				symbianToolStripMenuItem,
				hTML5ToolStripMenuItem
			});
			targetDDButton.Image = (System.Drawing.Image)resources.GetObject("targetDDButton.Image");
			targetDDButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			targetDDButton.Name = "targetDDButton";
			targetDDButton.Size = new System.Drawing.Size(54, 22);
			targetDDButton.Text = "Target";
			pSPToolStripMenuItem.Name = "pSPToolStripMenuItem";
			pSPToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			pSPToolStripMenuItem.Tag = "";
			pSPToolStripMenuItem.Text = "PSP";
			pSPToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_MachineSelect_Click);
			windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
			windowsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			windowsToolStripMenuItem.Text = "Windows";
			windowsToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_MachineSelect_Click);
			iOSToolStripMenuItem.Name = "iOSToolStripMenuItem";
			iOSToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			iOSToolStripMenuItem.Tag = "";
			iOSToolStripMenuItem.Text = "iOS";
			iOSToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_MachineSelect_Click);
			androidToolStripMenuItem.Name = "androidToolStripMenuItem";
			androidToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			androidToolStripMenuItem.Text = "Android";
			androidToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_MachineSelect_Click);
			symbianToolStripMenuItem.Name = "symbianToolStripMenuItem";
			symbianToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			symbianToolStripMenuItem.Text = "Symbian";
			symbianToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_MachineSelect_Click);
			hTML5ToolStripMenuItem.Name = "hTML5ToolStripMenuItem";
			hTML5ToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
			hTML5ToolStripMenuItem.Text = "HTML5";
			hTML5ToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_MachineSelect_Click);
			toolStripOpaqueTextureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			toolStripOpaqueTextureButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5]
			{
				bitToolStripMenuItem,
				r8G8B8A8ToolStripMenuItem,
				dXTC5ToolStripMenuItem,
				pVRTCToolStripMenuItem,
				pNGToolStripMenuItem
			});
			toolStripOpaqueTextureButton.Image = (System.Drawing.Image)resources.GetObject("toolStripOpaqueTextureButton.Image");
			toolStripOpaqueTextureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			toolStripOpaqueTextureButton.Name = "toolStripOpaqueTextureButton";
			toolStripOpaqueTextureButton.Size = new System.Drawing.Size(133, 22);
			toolStripOpaqueTextureButton.Text = "Opaque Texture Type";
			bitToolStripMenuItem.Name = "bitToolStripMenuItem";
			bitToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			bitToolStripMenuItem.Tag = "e4444";
			bitToolStripMenuItem.Text = "16bit - R4G4B4A4";
			bitToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_OpaqueTextureSelect_Click);
			r8G8B8A8ToolStripMenuItem.Name = "r8G8B8A8ToolStripMenuItem";
			r8G8B8A8ToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			r8G8B8A8ToolStripMenuItem.Tag = "eRaw";
			r8G8B8A8ToolStripMenuItem.Text = "32bit - R8G8B8A8";
			r8G8B8A8ToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_OpaqueTextureSelect_Click);
			dXTC5ToolStripMenuItem.Name = "dXTC5ToolStripMenuItem";
			dXTC5ToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			dXTC5ToolStripMenuItem.Tag = "eDXT";
			dXTC5ToolStripMenuItem.Text = "DXTC5";
			dXTC5ToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_OpaqueTextureSelect_Click);
			pVRTCToolStripMenuItem.Name = "pVRTCToolStripMenuItem";
			pVRTCToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			pVRTCToolStripMenuItem.Tag = "ePVR";
			pVRTCToolStripMenuItem.Text = "PVRTC";
			pVRTCToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_OpaqueTextureSelect_Click);
			pNGToolStripMenuItem.Name = "pNGToolStripMenuItem";
			pNGToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			pNGToolStripMenuItem.Tag = "ePNG";
			pNGToolStripMenuItem.Text = "PNG";
			pNGToolStripMenuItem.Click += new System.EventHandler(ToolStripMenuItem_OpaqueTextureSelect_Click);
			toolStripAlphaTextureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			toolStripAlphaTextureButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[5]
			{
				toolStripMenuItem1,
				toolStripMenuItem2,
				toolStripMenuItem3,
				toolStripMenuItem4,
				pNGToolStripMenuItem1
			});
			toolStripAlphaTextureButton.Image = (System.Drawing.Image)resources.GetObject("toolStripAlphaTextureButton.Image");
			toolStripAlphaTextureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			toolStripAlphaTextureButton.Name = "toolStripAlphaTextureButton";
			toolStripAlphaTextureButton.Size = new System.Drawing.Size(122, 22);
			toolStripAlphaTextureButton.Text = "Alpha Texture Type";
			toolStripMenuItem1.Name = "toolStripMenuItem1";
			toolStripMenuItem1.Size = new System.Drawing.Size(165, 22);
			toolStripMenuItem1.Tag = "e4444";
			toolStripMenuItem1.Text = "16bit - R4G4B4A4";
			toolStripMenuItem1.Click += new System.EventHandler(ToolStripMenuItem_AlphaTextureSelect_Click);
			toolStripMenuItem2.Name = "toolStripMenuItem2";
			toolStripMenuItem2.Size = new System.Drawing.Size(165, 22);
			toolStripMenuItem2.Tag = "eRaw";
			toolStripMenuItem2.Text = "32bit - R8G8B8A8";
			toolStripMenuItem2.Click += new System.EventHandler(ToolStripMenuItem_AlphaTextureSelect_Click);
			toolStripMenuItem3.Name = "toolStripMenuItem3";
			toolStripMenuItem3.Size = new System.Drawing.Size(165, 22);
			toolStripMenuItem3.Tag = "eDXT";
			toolStripMenuItem3.Text = "DXTC5";
			toolStripMenuItem3.Click += new System.EventHandler(ToolStripMenuItem_AlphaTextureSelect_Click);
			toolStripMenuItem4.Name = "toolStripMenuItem4";
			toolStripMenuItem4.Size = new System.Drawing.Size(165, 22);
			toolStripMenuItem4.Tag = "ePVR";
			toolStripMenuItem4.Text = "PVRTC";
			toolStripMenuItem4.Click += new System.EventHandler(ToolStripMenuItem_AlphaTextureSelect_Click);
			pNGToolStripMenuItem1.Name = "pNGToolStripMenuItem1";
			pNGToolStripMenuItem1.Size = new System.Drawing.Size(165, 22);
			pNGToolStripMenuItem1.Tag = "ePNG";
			pNGToolStripMenuItem1.Text = "PNG";
			pNGToolStripMenuItem1.Click += new System.EventHandler(ToolStripMenuItem_AlphaTextureSelect_Click);
			toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[6]
			{
				writeTexturesToolStripMenuItem,
				writeWavsToolStripMenuItem,
				separateAlphaOpaqueTexturesToolStripMenuItem,
				noCacheStripMenuItem,
				verboseToolStripMenuItem,
				obfuscateToolStripMenuItem
			});
			toolStripDropDownButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripDropDownButton1.Image");
			toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			toolStripDropDownButton1.Size = new System.Drawing.Size(62, 22);
			toolStripDropDownButton1.Text = "Options";
			writeTexturesToolStripMenuItem.Name = "writeTexturesToolStripMenuItem";
			writeTexturesToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			writeTexturesToolStripMenuItem.Text = "Write Textures";
			writeTexturesToolStripMenuItem.Click += new System.EventHandler(writeTexturesToolStripMenuItem_Click);
			writeWavsToolStripMenuItem.Name = "writeWavsToolStripMenuItem";
			writeWavsToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			writeWavsToolStripMenuItem.Text = "Write Wavs";
			writeWavsToolStripMenuItem.Click += new System.EventHandler(writeWavsToolStripMenuItem_Click);
			separateAlphaOpaqueTexturesToolStripMenuItem.Name = "separateAlphaOpaqueTexturesToolStripMenuItem";
			separateAlphaOpaqueTexturesToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			separateAlphaOpaqueTexturesToolStripMenuItem.Text = "Separate Alpha and Opaque Textures";
			separateAlphaOpaqueTexturesToolStripMenuItem.Click += new System.EventHandler(separateAlphaOpaqueTexturesToolStripMenuItem_Click);
			noCacheStripMenuItem.Name = "noCacheStripMenuItem";
			noCacheStripMenuItem.Size = new System.Drawing.Size(268, 22);
			noCacheStripMenuItem.Text = "No-cache (html5)";
			noCacheStripMenuItem.Click += new System.EventHandler(noCacheStripMenuItem_Click);
			verboseToolStripMenuItem.Name = "verboseToolStripMenuItem";
			verboseToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			verboseToolStripMenuItem.Text = "Verbose";
			verboseToolStripMenuItem.Click += new System.EventHandler(verboseToolStripMenuItem_Click);
			obfuscateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4]
			{
				prettyPrintingToolStripMenuItem,
				RemoveUnusedFunctionsToolStripMenuItem,
				encodeStringsToolStripMenuItem,
				obfuscateToolStripMenuItem1
			});
			obfuscateToolStripMenuItem.Name = "obfuscateToolStripMenuItem";
			obfuscateToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
			obfuscateToolStripMenuItem.Text = "Obfuscate";
			obfuscateToolStripMenuItem.Click += new System.EventHandler(obfuscateToolStripMenuItem_Click);
			prettyPrintingToolStripMenuItem.Name = "prettyPrintingToolStripMenuItem";
			prettyPrintingToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
			prettyPrintingToolStripMenuItem.Text = "Pretty Printing";
			prettyPrintingToolStripMenuItem.Click += new System.EventHandler(prettyPrintingToolStripMenuItem_Click);
			RemoveUnusedFunctionsToolStripMenuItem.Name = "RemoveUnusedFunctionsToolStripMenuItem";
			RemoveUnusedFunctionsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
			RemoveUnusedFunctionsToolStripMenuItem.Text = "Remove Unused Functions";
			RemoveUnusedFunctionsToolStripMenuItem.Click += new System.EventHandler(reToolStripMenuItem_Click);
			encodeStringsToolStripMenuItem.Name = "encodeStringsToolStripMenuItem";
			encodeStringsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
			encodeStringsToolStripMenuItem.Text = "Encode Strings";
			encodeStringsToolStripMenuItem.Click += new System.EventHandler(encodeStringsToolStripMenuItem_Click);
			SearchBox.Name = "SearchBox";
			SearchBox.Size = new System.Drawing.Size(150, 25);
			toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			toolStripButton1.Image = (System.Drawing.Image)resources.GetObject("toolStripButton1.Image");
			toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			toolStripButton1.Name = "toolStripButton1";
			toolStripButton1.Size = new System.Drawing.Size(46, 22);
			toolStripButton1.Text = "Search";
			toolStripButton1.Click += new System.EventHandler(toolStripButton1_Click);
			obfuscateToolStripMenuItem1.Name = "obfuscateToolStripMenuItem1";
			obfuscateToolStripMenuItem1.Size = new System.Drawing.Size(215, 22);
			obfuscateToolStripMenuItem1.Text = "Obfuscate";
			obfuscateToolStripMenuItem1.Click += new System.EventHandler(obfuscateToolStripMenuItem1_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(921, 658);
			base.Controls.Add(splitContainer1);
			base.Controls.Add(toolStrip1);
			base.Name = "Form1";
			Text = "GMAssetCompiler";
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.ResumeLayout(false);
			splitContainer2.Panel1.ResumeLayout(false);
			splitContainer2.Panel2.ResumeLayout(false);
			splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
