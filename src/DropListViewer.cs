using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VIANetWorkCard
{
    public partial class DropListViewer : Form
    {
        static bool s_bFormOpened = false;
        List<Main.Favourite> FavouriteCollection = new List<Main.Favourite>();
        ImageList iList = new ImageList();


        public DropListViewer()
        {
            InitializeComponent();
            iList.ImageSize = new Size(32, 32);
            iList.ColorDepth = ColorDepth.Depth32Bit;

        }

        public void AddFavourite(List<Main.Favourite> _FavouriteCollection)
        {
            if (FavouriteCollection == null) { FavouriteCollection = new List<Main.Favourite>(); }
            FavouriteCollection.AddRange(_FavouriteCollection.ToList());
        }
        public void SetFavourite(List<Main.Favourite> _FavouriteCollection)
        {
            FavouriteCollection = _FavouriteCollection;
        }
        public List<Main.Favourite> GetFavourite()
        {
            return FavouriteCollection;
        }
        public void ClearFavourite()
        {
            FavouriteCollection = null;
        }
        public void ListToView()
        {

            iList.Images.Clear();
            listView1.Items.Clear();

            FavouriteCollection.ToList().ForEach(x =>
             {
                 iList.Images.Add(x.Icon);
             });

            listView1.LargeImageList = iList;

            int count = 0;
            FavouriteCollection.ToList().ForEach(x =>
            {
                listView1.Items.Add(x.AppName, count);
                count += 1;
            });

        }

        private void DropListViewer_Load(object sender, EventArgs e)
        {
            if (!s_bFormOpened)
            {
                s_bFormOpened = true;
            }
            else {
                this.Dispose();
            }
        }

        private void DropListViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (s_bFormOpened)
            {
                s_bFormOpened = false;
            }
        }
    }
}
