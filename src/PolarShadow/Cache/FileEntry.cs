using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Cache
{
    internal class FileEntry : IFileEntry
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly string _path;
        private int _disposed;
        public FileEntry(string path)
        {
            _path = path;
        }

        public void WriteFile(byte[] fileData)
        {
            CheckDispose();
            try
            {
                _lock.EnterWriteLock();
                try
                {
                    using var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write);
                    fs.SetLength(0);
                    fs.Write(fileData);
                    fs.Flush();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public byte[] ReadFile()
        {
            CheckDispose();
            try
            {
                _lock.EnterReadLock();
                try
                {
                    return File.ReadAllBytes(_path);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task WriteFileAsync(byte[] fileData)
        {
            CheckDispose();
            try
            {
                _lock.EnterWriteLock();
                try
                {
                    using var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write);
                    fs.SetLength(0);
                    await fs.WriteAsync(fileData);
                    await fs.FlushAsync();
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<byte[]> ReadFileAsync()
        {
            CheckDispose();
            try
            {
                _lock.EnterReadLock();
                try
                {
                    return await File.ReadAllBytesAsync(_path);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _disposed, 1);
            _lock.Dispose();
        }

        public void Dispose(bool delete)
        {
            Dispose();
            if (delete)
            {
                Delete();
            }
        }

        private void CheckDispose()
        {
            if(Interlocked.CompareExchange(ref _disposed, 1, 1) == 1)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private async void Delete()
        {
            try
            {
                await Task.Run(() =>
                {
                    if (File.Exists(_path))
                    {
                        File.Delete(_path);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
