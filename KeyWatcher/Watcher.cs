using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyWatcher
{
    class DisposeToken : IDisposable 
    {
        internal DisposeToken() 
        {
            // capture state
        }
        ~DisposeToken() 
        {
            Dispose(false);
        }
        void Dispose(bool disposing) 
        {
            // stop watcher
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public delegate void KeyStateChanged();

    public static class Watcher
    {
        public static IDisposable Start(KeyStateChanged stateChanged)
        {
            // UNDONE
            return new DisposeToken();
        }
    }
}
