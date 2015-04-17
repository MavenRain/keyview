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
        string currentKeyChord = "";
        Font big, small;

        public Form1()
        {
            InitializeComponent();

            watcher = KeyWatcher.Watcher.Start(stateChanged);
            this.DoubleBuffered = true;
            this.AllowTransparency = true;
            this.TransparencyKey = Color.FromKnownColor(KnownColor.LimeGreen);
        }

        static Tuple<Font, Font> createFonts(float size)
        {
            return new Tuple<Font, Font>(new Font("Segoe UI", size * 0.5f), new Font("Segoe UI", size * 0.15f));
        } 

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            var fonts = createFonts(this.ClientSize.Height);
            big = fonts.Item1;
            small = fonts.Item2;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(Color.FromKnownColor(KnownColor.LimeGreen));

            float xOffset = 0f;
            foreach (var message in messages)
            {
                e.Graphics.DrawString(message, small, Brushes.White, new PointF() { X = xOffset + 0, Y = 0 });
                e.Graphics.DrawString(message, small, Brushes.White, new PointF() { X = xOffset + 2, Y = 2 });
                e.Graphics.DrawString(message, small, Brushes.Black, new PointF() { X = xOffset + 1, Y = 1 });
                xOffset += e.Graphics.MeasureString(message, small).Width + 5f;
            }
            e.Graphics.DrawString(currentKeyChord, big, Brushes.White, new PointF() { X = 0, Y = 5 + 0 });
            e.Graphics.DrawString(currentKeyChord, big, Brushes.White, new PointF() { X = 2, Y = 5 + 2 });
            e.Graphics.DrawString(currentKeyChord, big, Brushes.Black, new PointF() { X = 1, Y = 5 + 1 });
            e.Graphics.Flush();
        }

        static bool notModifierKey(uint vkCode)
        {
            var vk = (KeyWatcher.VirtualKeys)vkCode;
            return vk != KeyWatcher.VirtualKeys.LeftShift
                && vk != KeyWatcher.VirtualKeys.RightShift
                && vk != KeyWatcher.VirtualKeys.Shift
                && vk != KeyWatcher.VirtualKeys.LeftMenu
                && vk != KeyWatcher.VirtualKeys.RightMenu
                && vk != KeyWatcher.VirtualKeys.Menu
                && vk != KeyWatcher.VirtualKeys.LeftControl
                && vk != KeyWatcher.VirtualKeys.RightControl
                && vk != KeyWatcher.VirtualKeys.Control
                && vk != KeyWatcher.VirtualKeys.CapsLock;
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


            var nonModifiersDown = downKeys.Keys.Where(notModifierKey).Count();
            var totalKeysDown = downKeys.Count;
            var currentlyDisplayingAnything = currentKeyChord.Length != 0;

            if (totalKeysDown != 0 && !currentlyDisplayingAnything)
            {
                currentKeyChord = downKeys.Keys.Select(displayFor).Aggregate((a, b) => a + "," + b);
            }
            else if (
                (totalKeysDown == 0 && currentlyDisplayingAnything) 
                || (nonModifiersDown == 0 && totalKeysDown > 0 && currentlyDisplayingAnything && !down))
            {
                messages.Add(currentKeyChord);
                currentKeyChord = "";
            }
            else if (currentlyDisplayingAnything)
            {
                currentKeyChord = downKeys.Keys.Select(displayFor).Aggregate((a, b) => a + "," + b);
            }

            if (messages.Count > 0)
            {
                // limit message length to 35 chars (add 2 spaces per join)... 
                while (messages.Aggregate((a, b) => a + "  " + b).Length > 35)
                {
                    messages.RemoveAt(0);
                }
            }
            Invalidate();
        }

        static string displayFor(uint vkCode)
        {
            switch ((KeyWatcher.VirtualKeys)vkCode)
            {
                case KeyWatcher.VirtualKeys.OEM1:
                    return ";";
                case KeyWatcher.VirtualKeys.OEM2:
                    return "/";
                case KeyWatcher.VirtualKeys.LeftControl:
                case KeyWatcher.VirtualKeys.RightControl:
                case KeyWatcher.VirtualKeys.Control:
                    return "Control";
                case KeyWatcher.VirtualKeys.LeftMenu:
                case KeyWatcher.VirtualKeys.RightMenu:
                case KeyWatcher.VirtualKeys.Menu:
                    return "Alt";
                case KeyWatcher.VirtualKeys.N0:
                case KeyWatcher.VirtualKeys.N1:
                case KeyWatcher.VirtualKeys.N2:
                case KeyWatcher.VirtualKeys.N3:
                case KeyWatcher.VirtualKeys.N4:
                case KeyWatcher.VirtualKeys.N5:
                case KeyWatcher.VirtualKeys.N6:
                case KeyWatcher.VirtualKeys.N7:
                case KeyWatcher.VirtualKeys.N8:
                case KeyWatcher.VirtualKeys.N9:
                    return (vkCode - KeyWatcher.VirtualKeys.N0).ToString();
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
