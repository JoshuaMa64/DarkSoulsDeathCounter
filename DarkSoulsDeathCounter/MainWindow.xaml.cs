using System;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Text;


namespace DarkSoulsDeathCounter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            Tb.Text = "";
            var array = (Array) e.Data.GetData(DataFormats.FileDrop);
            if (array == null) return;
            string fileName = array.GetValue(0).ToString();
            ReadFile(fileName);
        }

        private void ReadFile(string fileName)
        {
            var byteName = new byte[32];
            var byteDeath = new byte[4];
            var info = "";
            try
            {
                var aFile = new FileStream(fileName, FileMode.Open);
                aFile.Seek(0x3C0, SeekOrigin.Begin);
                Debug.WriteLine("0x" + aFile.Position.ToString("X"), "POSITION");
                for (var i = 0; i < 10; i++)
                {
                    aFile.Read(byteName, 0, 32);
                    if (byteName[0] != '\0')
                    {
                        aFile.Seek(0x1F008, SeekOrigin.Current);
                        aFile.Read(byteDeath, 0, 4);
                        aFile.Seek(-0x1F12C, SeekOrigin.Current);
                        string playerName = Encoding.Unicode.GetString(byteName);
                        Debug.WriteLine(playerName.Split('\0')[0], "PLAYER NAME");
                        info += "玩家姓名: " + playerName.Split('\0')[0] + "\n";
                        int testDeath = byteDeath[0] + (byteDeath[1] << 8) + (byteDeath[2] << 16) + (byteDeath[3] << 32);
                        Debug.WriteLine(testDeath, "DEATH COUNT");
                        info += "死亡次数: " + testDeath + "\n\n";
                        aFile.Seek(0x100, SeekOrigin.Current);
                    }
                    aFile.Seek(0x60020, SeekOrigin.Current);
                }
                Tb.Text = info;
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }
    }
}
