using System;

namespace OutsideEngine.Util
{
    /// <summary>
    /// Taken from http://lostechies.com/chrispatterson/2012/11/29/idisposable-done-right/
    /// </summary>
    public class DisposableClass : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// PUblic facing Dispose method, used to clean up a DisposableClass
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// When a DisposableClass instance is collected, invoke the Dispose method
        ///  for unmanaged assets.
        /// </summary>
        ~DisposableClass()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose of this object, by cleaning managed resources, COM objects,
        ///  and unmanaged memory.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Free any IDisposables
            }

            // Release any unmanaged objects
            _disposed = true;
        }
    }
}
