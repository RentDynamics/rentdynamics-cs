using System;
using System.IO;

namespace RentDynamics.RdClient
{
    internal readonly struct StreamRewindScope : IDisposable
    {
        private readonly Stream? _stream;
        public StreamRewindScope(Stream? stream) => _stream = stream;
        public void Dispose() => _stream?.Seek(0, SeekOrigin.Begin);
    }
}