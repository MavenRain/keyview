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

        public Form1()
        {
            InitializeComponent();

            watcher = KeyWatcher.Watcher.Start(stateChanged);
        }

        Dictionary<uint, bool> downKeys = new Dictionary<uint, bool>();

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
            if (downKeys.Count > 0)
            {
                label1.Text = downKeys.Keys.Select(displayFor).Aggregate((a, b) => a + "," + b);
            }
            else
            {
                label1.Text = "";
            }
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
