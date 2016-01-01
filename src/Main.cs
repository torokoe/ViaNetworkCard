using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VIANetWorkCard;

namespace VIANetWorkCard
{
    public partial class Main : Form
    {

        public class NetworkCard
        {
            public string Name { get; set; }
            public string IP { get; set; }
            public string Description { get; set; }
            public string OperationalStatus { get; set; }

            public NetworkCard(string name, string ip, string desc, string status)
            { Name = name; IP = ip; Description = desc; OperationalStatus = status; }
        }
        public class ApplicationData
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public Icon Icon { get; set; }
            public Boolean Elevated { get; set; }

            public ApplicationData(string name, string path, string extension, Icon icon, Boolean elevated)
            { Name = name; Path = path; Extension = extension; Icon = icon; Elevated = elevated; }
        }
        public class Favourite
        {
            public string AppName { get; set; }
            public string NetworkCardName { get; set; }
            public string Path { get; set; }
            public string IP { get; set; }
            public Icon Icon { get; set; }

            public Favourite(string appName, string networkcardname, string path, string ip, Icon icon)
            { AppName = appName; NetworkCardName = networkcardname; Path = path; IP = ip; Icon = icon; }
        }
        List<NetworkCard> NetworkCardCollection = new List<NetworkCard>();
        List<ApplicationData> ApplicationDataCollection = new List<ApplicationData>();
        List<Favourite> FavouriteCollection = new List<Favourite>();
        WINAPI WINAPI = new WINAPI();
        Bitmap UacShield;
        DropListViewer dlv = new DropListViewer();
        Favourite fav;

        public Main()
        {
            InitializeComponent();
            InitializeOpenFileDialog();
            GetNetworkCardDetails();
            NetworkCardCollectionTolistView(listView1);

            try { listView1.Items[0].Selected = true; } catch { MessageBox.Show("Cannot Found NetworkCard."); }
            UacShield = WINAPI.GetUacShieldImage();
            installToolStripMenuItem1.ImageScaling = ToolStripItemImageScaling.None;
            installToolStripMenuItem1.Image = UacShield;
            ElevatedDragDropManager.Instance.EnableDragDrop(button2.Handle);
            ElevatedDragDropManager.Instance.EnableDragDrop(this.Handle);
            ElevatedDragDropManager.Instance.ElevatedDragDrop += ElevatedDragDropController;
            try
            {
                MessageBox.Show(Properties.Settings.Default.Favourite.ToList()[0].AppName);
                dlv.SetFavourite(Properties.Settings.Default.Favourite);
                dlv.ListToView();
            }
            catch { }
        }
        private void InitializeOpenFileDialog()
        {
            this.openFileDialog1.Filter ="Application |*.URL;*.EXE";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "Application";
            this.openFileDialog1.CheckFileExists = true;

        }
        public void GetNetworkCardDetails()
        {
            var NWCname = NetworkInterface.GetAllNetworkInterfaces()
            .Where(i => i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
            i.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            .Select(i => i.Name)
            .ToList();

            var NWCip = NetworkInterface.GetAllNetworkInterfaces()
            .Where(i => i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
            i.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            .SelectMany(i => i.GetIPProperties().UnicastAddresses)
            .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
            .Select(a => a.Address.ToString())
            .ToList();

            var NWCDescription = NetworkInterface.GetAllNetworkInterfaces()
            .Where(i => i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        i.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
             .Select(i => i.Description)
             .ToList();

            var NWCOperationalStatus = NetworkInterface.GetAllNetworkInterfaces()
            .Where(i => i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        i.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
             .Select(i => i.OperationalStatus.ToString() == "Up" ? "On" : "Off")
             .ToList();

            NetworkCardCollection =
                NWCname.Zip(NWCip, NWCDescription, NWCOperationalStatus,
                    (name, ip, desc, status) => new NetworkCard(name, ip, desc, status)
                )
                .ToList();
        }
        public void NetworkCardCollectionTolistView(ListView lv)
        {
            foreach (var ni in NetworkCardCollection)
            {
                if (ni.OperationalStatus == "On")
                {
                    ListViewItem item = new ListViewItem(ni.OperationalStatus);
                    item.SubItems.Add("[" + ni.Name + "] ");
                    item.SubItems.Add(ni.IP);

                    lv.Items.Add(item);
                }
            }
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        public void ForeachFileToList(List<string> e)
        {
            List<string> FileName = new List<string>();
            List<string> FilePath = new List<string>();
            List<string> FileExtension = new List<string>();
            List<Icon> FileIcon = new List<Icon>();
            List<Boolean> FileElevated = new List<Boolean>();
            Boolean find = false;
            string pattern = @"URL=(.*)";
            int NewFileOnlist = 0;
            String NewFilePath = "";

            try
            {
                    ApplicationDataCollection.ToList().ForEach(x =>
                    {
                        FileName.Add(x.Name);
                        FilePath.Add(x.Path);
                        FileExtension.Add(x.Extension);
                        FileIcon.Add(x.Icon);
                        FileElevated.Add(x.Elevated);
                    });

            }catch{}

            e.ForEach(x =>
            {
                try
                {
                    if (Path.GetExtension(WINAPI.GetShortcutTargetFile(x)) == ".exe")
                    {
                        FilePath.ToList().ForEach(y => { if (y == WINAPI.GetShortcutTargetFile(x)) { find = true; } });
                        if (!find)
                        {
                            NewFileOnlist += 1;
                            FileName.Add(Path.GetFileNameWithoutExtension(x));
                            FilePath.Add(WINAPI.GetShortcutTargetFile(x));
                            FileExtension.Add(Path.GetExtension(x));
                            try
                            {
                                FileIcon.Add(WINAPI.ExtractAssociatedIcon(WINAPI.GetShortcutTargetFile(x)));
                                FileElevated.Add(WINAPI.RequiresElevation(WINAPI.GetShortcutTargetFile(x)));
                            }
                            catch { }
                        }
                        else
                        {
                            NewFilePath = WINAPI.GetShortcutTargetFile(x);
                        }
                    }
                    else if (Path.GetExtension(WINAPI.GetShortcutTargetFile(x)) == ".url")
                    {
                        string data = System.IO.File.ReadAllText(WINAPI.GetShortcutTargetFile(x));
                        MatchCollection matches = Regex.Matches(data, pattern);

                        string path = String.Format(@"{0} ""{1}""", WINAPI.GetSystemDefaultBrowser(), matches[0].Groups[1].Value);

                        FilePath.ToList().ForEach(y =>
                        { if (y == path) { find = true; } });

                        if (!find)
                        {
                            NewFileOnlist += 1;
                            FileName.Add(Path.GetFileNameWithoutExtension(x));
                            FilePath.Add(path);
                            FileExtension.Add(Path.GetExtension(x));
                            try
                            {
                                FileIcon.Add(WINAPI.ExtractAssociatedIcon(WINAPI.GetShortcutTargetFile(x)));
                                FileElevated.Add(WINAPI.RequiresElevation(path));
                            }
                            catch { }
                        }
                        else
                        {
                            NewFilePath = path;
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex); }
            });

            listBox2.SelectedIndex = listBox2.Items.Count - 1;

            if (NewFileOnlist >= 1)
            {
                listBox2.Items.Clear();
                FilePath.ToList().ForEach(x =>
                {
                    listBox2.Items.Add(x);
                    listBox2.SelectedIndex += 1;
                });
            }
            else
            {
                listBox2.SelectedIndex = FilePath.IndexOf(NewFilePath);
            }
            ApplicationDataCollection = FileName.Zip(FilePath, FileExtension, FileIcon, FileElevated,
            (name, path, extension, icon, elevated) => new ApplicationData(name, path, extension, icon, elevated)).ToList();

            listBox2_SelectedIndexChanged(new object(), new EventArgs());
            if (checkBox1.Checked)
            {
                button1_Click(new object(), new EventArgs());
            }
        }

        private void ElevatedDragDropController(System.Object sender, ElevatedDragDropArgs e)
        {
            if (e.HWnd == this.Handle || e.HWnd == button1.Handle)
            {
                ForeachFileToList(e.Files.ToList());
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                ForeachFileToList(openFileDialog1.FileNames.ToList());

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            String fileName, arguments;
            fileName = "ForceBindIP.exe";
            try
            {
                arguments = String.Format(@"{0} ""{1}""",
                NetworkCardCollection[listView1.SelectedIndices.Count == 0 ? 0 : listView1.SelectedIndices[0]].IP, ApplicationDataCollection[listBox2.SelectedIndex].Path);

                ProcessStartInfo psi_normal = new ProcessStartInfo(fileName, arguments)
                {
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                };
                ProcessStartInfo psi_admin = new ProcessStartInfo(fileName, arguments)
                {
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    Verb = "runas"
                };

                try
                {
                    if (WINAPI.RequiresElevation(listBox2.SelectedItem.ToString()))
                    {
                        Process.Start(psi_admin);
                    }
                    else
                    {
                        Process.Start(psi_normal);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ApplicationDataCollection[listBox2.SelectedIndex].Elevated)
                {
                    WINAPI.AddShieldToButton(button1);
                }
                else
                {
                    WINAPI.RemoveShieldFromButton(button1);
                }

                textBox1.Text = ApplicationDataCollection[listBox2.SelectedIndex].Name;
            }
            catch { }
        }

        private void clearToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            GetNetworkCardDetails();
            listView1.Items.Clear();
            NetworkCardCollectionTolistView(listView1);
            ApplicationDataCollection = null;
        }
        private void listToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            dlv.Show();
            dlv.Location = new Point(this.Location.X + this.Size.Width, this.Location.Y);
        }
        private void seetingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void installToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }


        private void button4_Click(object sender, EventArgs e)
        {
            fav = new Favourite(textBox1.Text, NetworkCardCollection[listView1.SelectedIndices[0]].Name,ApplicationDataCollection[listBox2.SelectedIndex].Path,
                NetworkCardCollection[listView1.SelectedIndices[0]].IP, ApplicationDataCollection[listBox2.SelectedIndex].Icon);
            FavouriteCollection.Clear();
            FavouriteCollection.Add(fav);
            dlv.AddFavourite(FavouriteCollection);
            dlv.ListToView();
            Properties.Settings.Default.Favourite = dlv.GetFavourite();
            Properties.Settings.Default.Upgrade();
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            MessageBox.Show(Properties.Settings.Default.Favourite.ToList()[0].AppName);
            Application.Restart();
        }
    }
}
