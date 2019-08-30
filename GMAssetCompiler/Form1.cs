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
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.SearchBox = new System.Windows.Forms.ToolStripTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.compileButton = new System.Windows.Forms.ToolStripButton();
            this.targetDDButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.pSPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iOSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.androidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.symbianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hTML5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripOpaqueTextureButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.bitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.r8G8B8A8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dXTC5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pVRTCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripAlphaTextureButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.pNGToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.writeTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeWavsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separateAlphaOpaqueTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noCacheStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verboseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obfuscateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prettyPrintingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveUnusedFunctionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encodeStringsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.obfuscateToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(921, 633);
            this.splitContainer1.SplitterDistance = 173;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(173, 633);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.propertyGrid1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer2.Size = new System.Drawing.Size(744, 633);
            this.splitContainer2.SplitterDistance = 371;
            this.splitContainer2.TabIndex = 0;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(744, 371);
            this.propertyGrid1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.compileButton,
            this.targetDDButton,
            this.toolStripOpaqueTextureButton,
            this.toolStripAlphaTextureButton,
            this.toolStripDropDownButton1,
            this.SearchBox,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(921, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // SearchBox
            // 
            this.SearchBox.Name = "SearchBox";
            this.SearchBox.Size = new System.Drawing.Size(150, 25);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(744, 258);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // compileButton
            // 
            this.compileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.compileButton.Image = ((System.Drawing.Image)(resources.GetObject("compileButton.Image")));
            this.compileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.compileButton.Name = "compileButton";
            this.compileButton.Size = new System.Drawing.Size(56, 22);
            this.compileButton.Text = "Compile";
            this.compileButton.Click += new System.EventHandler(this.compileButton_Click);
            // 
            // targetDDButton
            // 
            this.targetDDButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.targetDDButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pSPToolStripMenuItem,
            this.windowsToolStripMenuItem,
            this.iOSToolStripMenuItem,
            this.androidToolStripMenuItem,
            this.symbianToolStripMenuItem,
            this.hTML5ToolStripMenuItem});
            this.targetDDButton.Image = ((System.Drawing.Image)(resources.GetObject("targetDDButton.Image")));
            this.targetDDButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.targetDDButton.Name = "targetDDButton";
            this.targetDDButton.Size = new System.Drawing.Size(53, 22);
            this.targetDDButton.Text = "Target";
            // 
            // pSPToolStripMenuItem
            // 
            this.pSPToolStripMenuItem.Name = "pSPToolStripMenuItem";
            this.pSPToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.pSPToolStripMenuItem.Tag = "";
            this.pSPToolStripMenuItem.Text = "PSP";
            this.pSPToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_MachineSelect_Click);
            // 
            // windowsToolStripMenuItem
            // 
            this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            this.windowsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.windowsToolStripMenuItem.Text = "Windows";
            this.windowsToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_MachineSelect_Click);
            // 
            // iOSToolStripMenuItem
            // 
            this.iOSToolStripMenuItem.Name = "iOSToolStripMenuItem";
            this.iOSToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.iOSToolStripMenuItem.Tag = "";
            this.iOSToolStripMenuItem.Text = "iOS";
            this.iOSToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_MachineSelect_Click);
            // 
            // androidToolStripMenuItem
            // 
            this.androidToolStripMenuItem.Name = "androidToolStripMenuItem";
            this.androidToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.androidToolStripMenuItem.Text = "Android";
            this.androidToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_MachineSelect_Click);
            // 
            // symbianToolStripMenuItem
            // 
            this.symbianToolStripMenuItem.Name = "symbianToolStripMenuItem";
            this.symbianToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.symbianToolStripMenuItem.Text = "Symbian";
            this.symbianToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_MachineSelect_Click);
            // 
            // hTML5ToolStripMenuItem
            // 
            this.hTML5ToolStripMenuItem.Name = "hTML5ToolStripMenuItem";
            this.hTML5ToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.hTML5ToolStripMenuItem.Text = "HTML5";
            this.hTML5ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_MachineSelect_Click);
            // 
            // toolStripOpaqueTextureButton
            // 
            this.toolStripOpaqueTextureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripOpaqueTextureButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bitToolStripMenuItem,
            this.r8G8B8A8ToolStripMenuItem,
            this.dXTC5ToolStripMenuItem,
            this.pVRTCToolStripMenuItem,
            this.pNGToolStripMenuItem});
            this.toolStripOpaqueTextureButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripOpaqueTextureButton.Image")));
            this.toolStripOpaqueTextureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpaqueTextureButton.Name = "toolStripOpaqueTextureButton";
            this.toolStripOpaqueTextureButton.Size = new System.Drawing.Size(131, 22);
            this.toolStripOpaqueTextureButton.Text = "Opaque Texture Type";
            // 
            // bitToolStripMenuItem
            // 
            this.bitToolStripMenuItem.Name = "bitToolStripMenuItem";
            this.bitToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.bitToolStripMenuItem.Tag = "e4444";
            this.bitToolStripMenuItem.Text = "16bit - R4G4B4A4";
            this.bitToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_OpaqueTextureSelect_Click);
            // 
            // r8G8B8A8ToolStripMenuItem
            // 
            this.r8G8B8A8ToolStripMenuItem.Name = "r8G8B8A8ToolStripMenuItem";
            this.r8G8B8A8ToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.r8G8B8A8ToolStripMenuItem.Tag = "eRaw";
            this.r8G8B8A8ToolStripMenuItem.Text = "32bit - R8G8B8A8";
            this.r8G8B8A8ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_OpaqueTextureSelect_Click);
            // 
            // dXTC5ToolStripMenuItem
            // 
            this.dXTC5ToolStripMenuItem.Name = "dXTC5ToolStripMenuItem";
            this.dXTC5ToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.dXTC5ToolStripMenuItem.Tag = "eDXT";
            this.dXTC5ToolStripMenuItem.Text = "DXTC5";
            this.dXTC5ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_OpaqueTextureSelect_Click);
            // 
            // pVRTCToolStripMenuItem
            // 
            this.pVRTCToolStripMenuItem.Name = "pVRTCToolStripMenuItem";
            this.pVRTCToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.pVRTCToolStripMenuItem.Tag = "ePVR";
            this.pVRTCToolStripMenuItem.Text = "PVRTC";
            this.pVRTCToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_OpaqueTextureSelect_Click);
            // 
            // pNGToolStripMenuItem
            // 
            this.pNGToolStripMenuItem.Name = "pNGToolStripMenuItem";
            this.pNGToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.pNGToolStripMenuItem.Tag = "ePNG";
            this.pNGToolStripMenuItem.Text = "PNG";
            this.pNGToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_OpaqueTextureSelect_Click);
            // 
            // toolStripAlphaTextureButton
            // 
            this.toolStripAlphaTextureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripAlphaTextureButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.pNGToolStripMenuItem1});
            this.toolStripAlphaTextureButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripAlphaTextureButton.Image")));
            this.toolStripAlphaTextureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAlphaTextureButton.Name = "toolStripAlphaTextureButton";
            this.toolStripAlphaTextureButton.Size = new System.Drawing.Size(120, 22);
            this.toolStripAlphaTextureButton.Text = "Alpha Texture Type";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem1.Tag = "e4444";
            this.toolStripMenuItem1.Text = "16bit - R4G4B4A4";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem_AlphaTextureSelect_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem2.Tag = "eRaw";
            this.toolStripMenuItem2.Text = "32bit - R8G8B8A8";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.ToolStripMenuItem_AlphaTextureSelect_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem3.Tag = "eDXT";
            this.toolStripMenuItem3.Text = "DXTC5";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.ToolStripMenuItem_AlphaTextureSelect_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(165, 22);
            this.toolStripMenuItem4.Tag = "ePVR";
            this.toolStripMenuItem4.Text = "PVRTC";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.ToolStripMenuItem_AlphaTextureSelect_Click);
            // 
            // pNGToolStripMenuItem1
            // 
            this.pNGToolStripMenuItem1.Name = "pNGToolStripMenuItem1";
            this.pNGToolStripMenuItem1.Size = new System.Drawing.Size(165, 22);
            this.pNGToolStripMenuItem1.Tag = "ePNG";
            this.pNGToolStripMenuItem1.Text = "PNG";
            this.pNGToolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem_AlphaTextureSelect_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.writeTexturesToolStripMenuItem,
            this.writeWavsToolStripMenuItem,
            this.separateAlphaOpaqueTexturesToolStripMenuItem,
            this.noCacheStripMenuItem,
            this.verboseToolStripMenuItem,
            this.obfuscateToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(62, 22);
            this.toolStripDropDownButton1.Text = "Options";
            // 
            // writeTexturesToolStripMenuItem
            // 
            this.writeTexturesToolStripMenuItem.Name = "writeTexturesToolStripMenuItem";
            this.writeTexturesToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.writeTexturesToolStripMenuItem.Text = "Write Textures";
            this.writeTexturesToolStripMenuItem.Click += new System.EventHandler(this.writeTexturesToolStripMenuItem_Click);
            // 
            // writeWavsToolStripMenuItem
            // 
            this.writeWavsToolStripMenuItem.Name = "writeWavsToolStripMenuItem";
            this.writeWavsToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.writeWavsToolStripMenuItem.Text = "Write Wavs";
            this.writeWavsToolStripMenuItem.Click += new System.EventHandler(this.writeWavsToolStripMenuItem_Click);
            // 
            // separateAlphaOpaqueTexturesToolStripMenuItem
            // 
            this.separateAlphaOpaqueTexturesToolStripMenuItem.Name = "separateAlphaOpaqueTexturesToolStripMenuItem";
            this.separateAlphaOpaqueTexturesToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.separateAlphaOpaqueTexturesToolStripMenuItem.Text = "Separate Alpha and Opaque Textures";
            this.separateAlphaOpaqueTexturesToolStripMenuItem.Click += new System.EventHandler(this.separateAlphaOpaqueTexturesToolStripMenuItem_Click);
            // 
            // noCacheStripMenuItem
            // 
            this.noCacheStripMenuItem.Name = "noCacheStripMenuItem";
            this.noCacheStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.noCacheStripMenuItem.Text = "No-cache (html5)";
            this.noCacheStripMenuItem.Click += new System.EventHandler(this.noCacheStripMenuItem_Click);
            // 
            // verboseToolStripMenuItem
            // 
            this.verboseToolStripMenuItem.Name = "verboseToolStripMenuItem";
            this.verboseToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.verboseToolStripMenuItem.Text = "Verbose";
            this.verboseToolStripMenuItem.Click += new System.EventHandler(this.verboseToolStripMenuItem_Click);
            // 
            // obfuscateToolStripMenuItem
            // 
            this.obfuscateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prettyPrintingToolStripMenuItem,
            this.RemoveUnusedFunctionsToolStripMenuItem,
            this.encodeStringsToolStripMenuItem,
            this.obfuscateToolStripMenuItem1});
            this.obfuscateToolStripMenuItem.Name = "obfuscateToolStripMenuItem";
            this.obfuscateToolStripMenuItem.Size = new System.Drawing.Size(267, 22);
            this.obfuscateToolStripMenuItem.Text = "Obfuscate";
            this.obfuscateToolStripMenuItem.Click += new System.EventHandler(this.obfuscateToolStripMenuItem_Click);
            // 
            // prettyPrintingToolStripMenuItem
            // 
            this.prettyPrintingToolStripMenuItem.Name = "prettyPrintingToolStripMenuItem";
            this.prettyPrintingToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.prettyPrintingToolStripMenuItem.Text = "Pretty Printing";
            this.prettyPrintingToolStripMenuItem.Click += new System.EventHandler(this.prettyPrintingToolStripMenuItem_Click);
            // 
            // RemoveUnusedFunctionsToolStripMenuItem
            // 
            this.RemoveUnusedFunctionsToolStripMenuItem.Name = "RemoveUnusedFunctionsToolStripMenuItem";
            this.RemoveUnusedFunctionsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.RemoveUnusedFunctionsToolStripMenuItem.Text = "Remove Unused Functions";
            this.RemoveUnusedFunctionsToolStripMenuItem.Click += new System.EventHandler(this.reToolStripMenuItem_Click);
            // 
            // encodeStringsToolStripMenuItem
            // 
            this.encodeStringsToolStripMenuItem.Name = "encodeStringsToolStripMenuItem";
            this.encodeStringsToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.encodeStringsToolStripMenuItem.Text = "Encode Strings";
            this.encodeStringsToolStripMenuItem.Click += new System.EventHandler(this.encodeStringsToolStripMenuItem_Click);
            // 
            // obfuscateToolStripMenuItem1
            // 
            this.obfuscateToolStripMenuItem1.Name = "obfuscateToolStripMenuItem1";
            this.obfuscateToolStripMenuItem1.Size = new System.Drawing.Size(215, 22);
            this.obfuscateToolStripMenuItem1.Text = "Obfuscate";
            this.obfuscateToolStripMenuItem1.Click += new System.EventHandler(this.obfuscateToolStripMenuItem1_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(46, 22);
            this.toolStripButton1.Text = "Search";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(921, 658);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "GMAssetCompiler";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
