using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Text;

namespace DoubleCacheTests
{
    public class RedisFixture : IDisposable
    {
        
        private static readonly Random _random = new Random();
        private readonly Process _process;
        private bool _disposed = false;
        private static int _port = _random.Next(49152, 65535 + 1);

        public IConnectionMultiplexer ConnectionMultiplexer
        {
            get { return lazyConnection.Value;  }
        }

        private Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return StackExchange.Redis.ConnectionMultiplexer.Connect(string.Format("localhost:{0}", _port));
        });

        public RedisFixture()
        {
           
            var processStartInfo = new ProcessStartInfo(".\\Redis\\redis-server.exe")
            {
                UseShellExecute = false,
                Arguments = string.Format("--port {0} --bind 127.0.0.1 --maxheap 10MB", _port),
                WindowStyle = ProcessWindowStyle.Maximized,
                CreateNoWindow = true,
                LoadUserProfile = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.ASCII,
            };

            _process = Process.Start(processStartInfo);
            _process.BeginOutputReadLine();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;


            try
            {
                _process.CancelOutputRead();
                _process.Kill();
                _process.WaitForExit(2000);

                if (disposing)
                    _process.Dispose();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed disposing redis", ex);
            }

            _disposed = true;

        }

        ~RedisFixture()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
