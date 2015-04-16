using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyViewWF
{
    public partial class Form1 : Form
    {
        IDisposable watcher;
        Dictionary<uint, bool> downKeys = new Dictionary<uint, bool>();
        List<string> messages = new List<string>();

        public Form1()
        {
            InitializeComponent();

            watcher = KeyWatcher.Watcher.Start(stateChanged);
            this.DoubleBuffered = true;
            this.AllowTransparency = true;
            this.TransparencyKey = Color.FromKnownColor(KnownColor.LimeGreen);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            label1.Font = new Font(label1.Font.FontFamily, ((float)this.ClientSize.Height) * 0.6f);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.FromKnownColor(KnownColor.LimeGreen));
            foreach (var message in messages)
            {
                e.Graphics.DrawString(message, label1.Font, Brushes.White, new PointF() { X = 0, Y = 0 });
                e.Graphics.DrawString(message, label1.Font, Brushes.White, new PointF() { X = 2, Y = 2 });
                e.Graphics.DrawString(message, label1.Font, Brushes.Black, new PointF() { X = 1, Y = 1 });
            }
            e.Graphics.Flush();
        }

        void stateChanged(uint vkCode, bool down, bool up)
        {
            if (up)
            {
                if (downKeys.ContainsKey(vkCode))
                {
                    downKeys.Remove(vkCode);
                }
            }
            else if (down)
            {
                downKeys[vkCode] = true;
            }
            messages.Clear();
            if (downKeys.Count > 0)
            {
                messages.Add(downKeys.Keys.Select(displayFor).Aggregate((a, b) => a + "," + b));
            }
            Invalidate();
        }

        static string displayFor(uint vkCode)
        {
            switch ((KeyWatcher.VirtualKeys)vkCode)
            {
                // UNDONE: do we want any friendly aliases here?
                default:
                    return ((KeyWatcher.VirtualKeys)vkCode).ToString();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (watcher != null)
            {
                watcher.Dispose();
                watcher = null;
            }
            
            base.OnClosing(e);
        }
    }
}
