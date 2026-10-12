using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CoinCounter
{
    public partial class Form1 : Form
    {
        Bitmap original;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp";
            if (dlg.ShowDialog() != DialogResult.OK) return;

            original = new Bitmap(dlg.FileName);
            picOriginal.Image = original;
            picResult.Image = null;
            txtResults.Text = "Image loaded. Click \"Count Coins\".";
            btnCount.Enabled = true;
        }

        private void btnCount_Click(object sender, EventArgs e)
        {
            if (original == null) return;
            Cursor = Cursors.WaitCursor;
            try
            {
                int threshold;
                List<CoinInfo> coins = CoinDetector.Detect(original, out threshold);
                picResult.Image = CoinDetector.Annotate(original, coins);

                StringBuilder sb = new StringBuilder();
                decimal total = 0;
                foreach (Denomination d in CoinDetector.Denominations)
                {
                    int n = 0;
                    foreach (CoinInfo c in coins) if (c.Denom == d) n++;
                    decimal sub = n * d.Value;
                    total += sub;
                    sb.AppendLine(string.Format("{0,-11} x {1,3}  =  \u20B1{2,7:0.00}", d.Name, n, sub));
                }
                sb.AppendLine("--------------------------------");
                sb.AppendLine(string.Format("Coins found: {0}", coins.Count));
                sb.AppendLine(string.Format("TOTAL: \u20B1{0:0.00}", total));
                sb.AppendLine();
                sb.AppendLine("(threshold used: " + threshold + ")");
                txtResults.Text = sb.ToString();
            }
            finally { Cursor = Cursors.Default; }
        }

        // Convenience: load CoinsImage.jpeg automatically if it sits next to the exe.
        private void Form1_Load(object sender, EventArgs e)
        {
            string p = Path.Combine(Application.StartupPath, "CoinsImage.jpeg");
            if (File.Exists(p))
            {
                original = new Bitmap(p);
                picOriginal.Image = original;
                btnCount.Enabled = true;
            }
        }
    }
}
