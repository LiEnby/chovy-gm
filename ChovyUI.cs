using GMAssetCompiler;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ChovyUI
{
    public partial class ChovyUI : Form
    {
        bool wasClicked = false;
        public ChovyUI()
        {
            InitializeComponent();
            IconPath.Text = Path.Combine(Application.StartupPath, "IMG", "ICON0.PNG");
            PicPath.Text = Path.Combine(Application.StartupPath, "IMG", "PIC0.PNG");

            try
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\CHOVYProject\Chovy-GM");

                TitleId.Text = key.GetValue("TITLEID", TitleId.Text).ToString();
                Title.Text = key.GetValue("TITLE", Title.Text).ToString();
                GMPath.Text = key.GetValue("GMEXE", GMPath.Text).ToString();
                IconPath.Text = key.GetValue("ICONPATH", IconPath.Text).ToString();
                PicPath.Text = key.GetValue("PICPATH", PicPath.Text).ToString();


                LBumper.Text = key.GetValue("SCE_CTRL_L", LBumper.Text.ToUpper()).ToString();
                RBumper.Text = key.GetValue("SCE_CTRL_R", RBumper.Text.ToUpper()).ToString();
                LeftDPAD.Text = key.GetValue("SCE_CTRL_LEFT", LeftDPAD.Text.ToUpper()).ToString();
                RightDPAD.Text = key.GetValue("SCE_CTRL_RIGHT", RightDPAD.Text.ToUpper()).ToString();
                UpDPAD.Text = key.GetValue("SCE_CTRL_UP", UpDPAD.Text.ToUpper()).ToString();
                DownDPAD.Text = key.GetValue("SCE_CTRL_DOWN", DownDPAD.Text.ToUpper()).ToString();

                CircleButton.Text = key.GetValue("SCE_CTRL_CIRCLE", CircleButton.Text.ToUpper()).ToString();
                CrossButton.Text = key.GetValue("SCE_CTRL_CROSS", CrossButton.Text.ToUpper()).ToString();
                SquareButton.Text = key.GetValue("SCE_CTRL_SQUARE", SquareButton.Text.ToUpper()).ToString();
                TriangleButton.Text = key.GetValue("SCE_CTRL_TRIANGLE", TriangleButton.Text.ToUpper()).ToString();

                SelectButton.Text = key.GetValue("SCE_CTRL_SELECT", SelectButton.Text.ToUpper()).ToString();
                StartButton.Text =  key.GetValue("SCE_CTRL_START", StartButton.Text.ToUpper()).ToString();
                key.Close();
            }
            catch (Exception) { };
            Check();

        }

        private void BrowseIcon_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG files (*.png)|*.png;";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                IconPath.Text = openFileDialog.FileName;
            }
        }

        private void BrowsePic_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG files (*.png)|*.png;";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                PicPath.Text = openFileDialog.FileName;
            }
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GameMaker 7/8/8.1 Executable files (*.exe)|*.exe;";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                GMPath.Text = openFileDialog.FileName;
            }
            
        }

        public static void CopyDirTree(string SourcePath, string DestinationPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
        }

        public static void WriteString(Stream s, string Str)
        {
            char[] carr = Str.ToCharArray();
            foreach (char c in carr)
            {
                s.WriteByte((byte)c);
            }
        }
        public static void WriteValueStr(Stream s, string Key, string Value)
        {
            WriteString(s, Key + " = ");
            WriteString(s, Value);
            WriteString(s, "\r\n");
        }

        private void BuildISO_Click(object sender, EventArgs e)
        {
            wasClicked = true;
            Program.OutputDir = Path.GetDirectoryName(GMPath.Text);

            String InputFolder = Path.Combine(Program.OutputDir, "_iso_temp");
            if (Directory.Exists(InputFolder))
            {
                Directory.Delete(InputFolder, true);
            }
            CopyDirTree(Path.Combine(Application.StartupPath, "RUNNER"), InputFolder);

            //Write to PARAM.SFO:
            FileStream sfo = new FileStream(Path.Combine(InputFolder, "PSP_GAME", "PARAM.SFO"),FileMode.OpenOrCreate,FileAccess.ReadWrite);
            sfo.Seek(0x128, SeekOrigin.Begin);
            WriteString(sfo, TitleId.Text);
            sfo.Seek(0x158, SeekOrigin.Begin);
            WriteString(sfo, Title.Text);
            sfo.Close();

            //Copy icon and pic0:
            File.Copy(IconPath.Text, Path.Combine(InputFolder, "PSP_GAME", "ICON0.PNG"), true);
            File.Copy(PicPath.Text, Path.Combine(InputFolder, "PSP_GAME", "PIC0.PNG"), true);
            File.Copy(PicPath.Text, Path.Combine(InputFolder, "PSP_GAME", "USRDIR", "games","ICON0.PNG"), true);

            //Write game.ini
            FileStream ini = new FileStream(Path.Combine(InputFolder, "PSP_GAME", "USRDIR","games","game.ini"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            ini.SetLength(0);

            WriteString(ini, "[PSP_LOADSAVE]\r\n");
            WriteValueStr(ini, "ICON","games /ICON0.PNG");
            WriteValueStr(ini, "TITLEID", TitleId.Text);
            WriteValueStr(ini, "SHORT_NAME", Title.Text);
            WriteValueStr(ini, "DESC_NAME", Title.Text+ " save game.");
            WriteValueStr(ini, "PARENTAL_LEVEL", "0");

            WriteString(ini, "[PSP_IO]\r\n");
            WriteValueStr(ini, "SCE_CTRL_L", LBumper.Text.ToUpper());
            WriteValueStr(ini, "SCE_CTRL_R", RBumper.Text.ToUpper());
            WriteValueStr(ini, "SCE_CTRL_LEFT", LeftDPAD.Text.ToUpper());
            WriteValueStr(ini, "SCE_CTRL_RIGHT", RightDPAD.Text.ToUpper());
            WriteValueStr(ini, "SCE_CTRL_UP", UpDPAD.Text.ToUpper());
            WriteValueStr(ini, "SCE_CTRL_DOWN", DownDPAD.Text.ToUpper());

            WriteValueStr(ini, "SCE_CTRL_CIRCLE", CircleButton.Text.ToUpper());
            WriteValueStr(ini, "SCE_CTRL_CROSS", CrossButton.Text.ToUpper());
            WriteValueStr(ini, "SCE_CTRL_SQUARE", SquareButton.Text.ToUpper());
            WriteValueStr(ini, "SCE_CTRL_TRIANGLE", TriangleButton.Text.ToUpper());

            WriteValueStr(ini, "SCE_CTRL_SELECT", SelectButton.Text.ToUpper());
            WriteValueStr(ini, "SCE_CTRL_START", StartButton.Text.ToUpper());

            ini.Close();

            this.Close();

        }

        public string GetGMPath()
        {
            return GMPath.Text;
        }

        public string GetTitleID()
        {
            return TitleId.Text;
        }

        public bool WasClicked()
        {
            return wasClicked;
        }

        public void Check()
        {
            if (File.Exists(GMPath.Text))
            {
                BuildISO.Enabled = true;
            }
            else
            {
                BuildISO.Enabled = false;
                return;
            }

            if (File.Exists(IconPath.Text))
            {
                BuildISO.Enabled = false;

                try
                {
                    Bitmap bmp = new Bitmap(IconPath.Text);
                    if(bmp.Width != 144 || bmp.Height != 80)
                    {
                        return;
                    }
                }
                catch(Exception)
                {
                    return;
                }
                BuildISO.Enabled = true;

            }
            else
            {
                BuildISO.Enabled = false;
                return;
            }

            if (File.Exists(PicPath.Text))
            {
                BuildISO.Enabled = true;
            }
            else
            {
                BuildISO.Enabled = false;
                return;
            }

            BuildISO.Enabled = false;

            if (TitleId.Text.Length == 9)
            {
                char[] TitleArray = TitleId.Text.ToArray();
                for (int i = 0; i < 4; i++)
                {
                    if (!Char.IsLetter(TitleArray[i]))
                    {
                        return;
                    }
                }
                for (int i = 4; i < 9; i++)
                {
                    if (!Char.IsNumber(TitleArray[i]))
                    {
                        return;
                    }
                }

                BuildISO.Enabled = true;
            }

        }
        private void GMPath_TextChanged(object sender, EventArgs e)
        {
            Check();
        }
        private void IconPath_TextChanged(object sender, EventArgs e)
        {
            Check();
        }

        private void PicPath_TextChanged(object sender, EventArgs e)
        {
            Check();
        }
        private void TitleId_TextChanged(object sender, EventArgs e)
        {
            int Cursor = TitleId.SelectionStart;
            TitleId.Text = TitleId.Text.ToUpper();
            TitleId.SelectionStart = Cursor;

            Check();
        }

        private void ChovyUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\CHOVYProject\Chovy-GM");

                key.SetValue("TITLEID", TitleId.Text);
                key.SetValue("TITLE", Title.Text);
                key.SetValue("GMEXE", GMPath.Text);
                key.SetValue("ICONPATH", IconPath.Text);
                key.SetValue("PICPATH", PicPath.Text);
                

                key.SetValue("SCE_CTRL_L", LBumper.Text.ToUpper());
                key.SetValue("SCE_CTRL_R", RBumper.Text.ToUpper());
                key.SetValue("SCE_CTRL_LEFT", LeftDPAD.Text.ToUpper());
                key.SetValue("SCE_CTRL_RIGHT", RightDPAD.Text.ToUpper());
                key.SetValue("SCE_CTRL_UP", UpDPAD.Text.ToUpper());
                key.SetValue("SCE_CTRL_DOWN", DownDPAD.Text.ToUpper());

                key.SetValue("SCE_CTRL_CIRCLE", CircleButton.Text.ToUpper());
                key.SetValue("SCE_CTRL_CROSS", CrossButton.Text.ToUpper());
                key.SetValue("SCE_CTRL_SQUARE", SquareButton.Text.ToUpper());
                key.SetValue("SCE_CTRL_TRIANGLE", TriangleButton.Text.ToUpper());

                key.SetValue("SCE_CTRL_SELECT", SelectButton.Text.ToUpper());
                key.SetValue("SCE_CTRL_START", StartButton.Text.ToUpper());
                key.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to save settings to registry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
