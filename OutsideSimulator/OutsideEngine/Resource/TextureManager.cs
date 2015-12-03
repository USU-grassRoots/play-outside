using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OutsideEngine.Util;

using SlimDX.Direct3D11;
using System.IO;

namespace OutsideEngine.Resource
{
    // TODO KAM: You need to have a way to release textures after a time. These are exactly what you want to be ephemeral.

    /// <summary>
    /// Manages texture resources for the OutsideEngine instance. May be used as a single global
    ///  texture manager, or individual texture managers for various levels or whatever.
    /// Glorified dictionary of shader resource views, as created from textures from a file.
    /// </summary>
    public class TextureManager : DisposableClass
    {
        private bool _disposed;
        private Device _device;
        private readonly Dictionary<string, ShaderResourceView> _textureSRVs;

        /// <summary>
        /// Initialize a texture manager by creating a blank dictionary
        /// </summary>
        public TextureManager()
        {
            _textureSRVs = new Dictionary<string, ShaderResourceView>();
        }

        /// <summary>
        /// Cleanup any unmanaged resources that may be in use
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var key in _textureSRVs.Keys)
                    {
                        var srv = _textureSRVs[key];
                        Util.Util.ReleaseCom(ref srv);
                    }
                    _textureSRVs.Clear();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialize the texture manager to use the given device
        /// </summary>
        /// <param name="device">Direct3D device of the application using the texture manager</param>
        public void Init(Device device)
        {
            _device = device;
        }

        /// <summary>
        /// Get the texture in the given file path as a ShaderResourceView
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ShaderResourceView GetTexture(string path)
        {
            if (!_textureSRVs.ContainsKey(path))
            {
                if (File.Exists(path))
                {
                    _textureSRVs[path] = ShaderResourceView.FromFile(_device, path);
                }
                else
                {
                    return null;
                }
            }

            return _textureSRVs[path];
        }
    }
}
