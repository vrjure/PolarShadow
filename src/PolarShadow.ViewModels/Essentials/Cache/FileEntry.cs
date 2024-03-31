using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Essentials
{
    internal class FileEntry : IFileEntry
    {
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1,1);
        private readonly SemaphoreSlim _readLock = new SemaphoreSlim(0);
        private readonly FileInfo _fileInfo;
        private int _disposed;
        public long Size
        {
            get
            {
                try
                {
                    if (_readLock.Release() == 0)
                    {
                        _writeLock.Wait();
                    }
                    if (_fileInfo.Exists)
                    {
                        return _fileInfo.Length;
                    }
                }
                catch { }
                finally
                {
                    _readLock.Wait();
                    if (_readLock.CurrentCount == 0)
                    {
                        _writeLock.Release();
                    }
                }
                return 0;
            }
        }

        public DateTime CreateTime
        {
            get
            {
                try
                {
                    if (_readLock.Release() == 0)
                    {
                        _writeLock.Wait();
                    }

                    if (_fileInfo.Exists)
                    {
                        return _fileInfo.CreationTime;
                    }
                }
                catch { }
                finally
                {
                    _readLock.Wait();
                    if (_readLock.CurrentCount == 0)
                    {
                        _writeLock.Release();
                    }
                }
                return DateTime.MinValue;
            }
        }

        public FileEntry(string path)
        {
            _fileInfo = new FileInfo(path);
        }

        public void WriteFile(byte[] fileData)
        {
            CheckDispose();
            try
            {
                _writeLock.Wait();
                try
                {
                    using var fs = _fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write);
                    fs.SetLength(0);
                    fs.Write(fileData);
                    fs.Flush();
                    _fileInfo.Refresh();
                }
                finally
                {
                    _writeLock.Release();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"WriteFile:{ex}");
            }
        }

        public byte[] ReadFile()
        {
            CheckDispose();
            try
            {
                if (_readLock.Release() == 0)
                {
                    _writeLock.Wait();
                }
                try
                {
                    if (_fileInfo.Exists)
                    {
                        var buffer = new byte[_fileInfo.Length];
                        using var fs = _fileInfo.OpenRead();
                        fs.Read(buffer);
                        return buffer;
                    }
                }
                finally
                {
                    _readLock.Wait();
                    if (_readLock.CurrentCount == 0)
                    {
                        _writeLock.Release();
                    }
                }
            }
            catch  (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"ReadFile:{ex}");
            }
            return null;
        }

        public async Task WriteFileAsync(byte[] fileData)
        {
            CheckDispose();
            try
            {
                await _writeLock.WaitAsync();
                try
                {
                    using var fs = _fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write);
                    fs.SetLength(0);
                    await fs.WriteAsync(fileData);
                    await fs.FlushAsync();
                    _fileInfo.Refresh();
                }
                finally
                {
                    _writeLock.Release();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"WriteFileAsync:{ex}");
            }
        }

        public async Task<byte[]> ReadFileAsync()
        {
            CheckDispose();
            try
            {
                if (_readLock.Release() == 0)
                {
                    await _writeLock.WaitAsync();
                }

                try
                {
                    if (_fileInfo.Exists)
                    {
                        using var fs = _fileInfo.OpenRead();
                        var buffer = new byte[_fileInfo.Length];
                        await fs.ReadAsync(buffer);
                        return buffer;
                    }
                }
                finally
                {
                    await _readLock.WaitAsync();
                    if (_readLock.CurrentCount == 0)
                    {
                        _writeLock.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"ReadFileAsync:{ex}");
            }
            return null;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _disposed, 1);
            _writeLock.Dispose();
            _readLock.Dispose();
        }

        public void Dispose(bool delete)
        {
            if (delete)
            {
                Delete();
            }
            Dispose();
        }

        private void CheckDispose()
        {
            if(Interlocked.CompareExchange(ref _disposed, _disposed, 1) == 1)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private void Delete()
        {
            try
            {
                _writeLock.Wait();
                if (_fileInfo.Exists)
                {
                    _fileInfo.Delete();
                    _fileInfo.Refresh();
                }
            }
            catch { }
            finally
            {
                _writeLock.Release();
            }
        }
    }
}
