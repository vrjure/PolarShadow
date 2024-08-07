using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Options;

namespace PolarShadow.Api.Utilities
{
    public sealed class FileSafeOperate
    {
        private object _lock = new object();
        private int _fileCount;
        private int _concurrentCount;
        private Timer? _timer;
        private ReaderWriterLockSlim _readWriterLockSlim;
        private ReaderWriterLockSlim _readFilelockSlim;

        private readonly PolarShadowSetting _setting;
        private readonly ILogger<FileSafeOperate> _logger;
        public FileSafeOperate(IOptions<PolarShadowSetting> setting, ILogger<FileSafeOperate> logger)
        {
            _setting = setting.Value;
            _logger = logger;
            _readWriterLockSlim = new ReaderWriterLockSlim();
            _readFilelockSlim = new ReaderWriterLockSlim();
            if (!string.IsNullOrEmpty(_setting.SourceFileFolder) && !Directory.Exists(_setting.SourceFileFolder))
            {
                Directory.CreateDirectory(_setting.SourceFileFolder);
            }
        }

        public string _filePath => Path.Combine(_setting.SourceFileFolder!, _setting.SourceFileName!);
        public string _readFilePath => Path.Combine(_setting.SourceFileFolder!, $"{_setting.SourceFileName}.reader");
        public void Initialize()
        {
            StartBackgourndDeal();
        }

        public FileStream CreateFileStream()
        {
            var filePathWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
            var extension = Path.GetExtension(_filePath);
            var index = Interlocked.Increment(ref _fileCount);

            var fs = new FileStream($"{filePathWithoutExtension}_{index}.{extension}", FileMode.OpenOrCreate, FileAccess.Write);
            lock (_lock)
            {
                if(_timer == null)
                {
                    _timer = new Timer(BackgroundDeal, null, 0, Timeout.Infinite);
                }
                else
                {
                    _timer.Change(0, Timeout.Infinite);
                }
            }
            return fs;
        }

        public void Increment()
        {
            Interlocked.Increment(ref _concurrentCount);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _concurrentCount);
        }

        public Stream FileRead()
        {
            _readFilelockSlim.EnterReadLock();
            try
            {
                var filePath = _filePath;
                if (!File.Exists(filePath))
                {
                    EnsureReadFileCreate();
                }

                if (!File.Exists(filePath))
                {
                    return new MemoryStream();
                }
                else
                {
                    return new FileStream(_readFilePath, FileMode.Open, FileAccess.Read);
                }
            }
            finally
            {
                _readFilelockSlim.ExitReadLock();
            }
        }

        private void BackgroundDeal(object? state)
        {
            if (string.IsNullOrEmpty(_setting.SourceFileFolder) || string.IsNullOrEmpty(_setting.SourceFileName))
            {
                return;
            }
            var count = Volatile.Read(ref _concurrentCount);
            if (count > 0)
            {
                _timer?.Change(1000, Timeout.Infinite);
                return;
            }

            var fileCount = Volatile.Read(ref _fileCount);

            try
            {
                var filePath = _filePath;
                var filePathWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                var extension = Path.GetExtension(filePath);
                for (int i = 1; i <= fileCount; i++)
                {
                    var targetPath = $"{filePathWithoutExtension}_{i}.{extension}";
                    if (File.Exists(targetPath))
                    {
                        _readWriterLockSlim.EnterWriteLock();
                        try
                        {
                            File.Copy(targetPath, filePath, true);
                        }
                        finally
                        {
                            _readWriterLockSlim.ExitWriteLock();
                        }
                        File.Delete(targetPath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Combine file error");
            }
            finally
            {
                if (Volatile.Read(ref _fileCount) == fileCount)
                {
                    Interlocked.Exchange(ref _fileCount, 0);
                    EnsureReadFileCreate();
                }
                else
                {
                    _timer?.Change(1000, Timeout.Infinite);
                }
            }
        }

        private void StartBackgourndDeal()
        {
            lock (_lock)
            {
                if (_timer == null)
                {
                    _timer = new Timer(BackgroundDeal, null, 0, Timeout.Infinite);
                }
                else
                {
                    _timer.Change(0, Timeout.Infinite);
                }
            }
        }

        private string EnsureReadFileCreate()
        {
            if (string.IsNullOrEmpty( _setting.SourceFileFolder) || string.IsNullOrEmpty(_setting.SourceFileName))
            {
                return "";
            }
            string filePath = _filePath;
            var readFilePath = _readFilePath;
            if (File.Exists(filePath))
            {
                _readWriterLockSlim.EnterReadLock();
                _readFilelockSlim.EnterWriteLock();
                try
                {
                    if (!File.Exists(readFilePath))
                    {
                        File.Create(readFilePath);
                    }
                    File.Copy(filePath, readFilePath, true);
                }
                finally
                {
                    _readFilelockSlim.ExitWriteLock();
                    _readWriterLockSlim.ExitReadLock();
                }
            }
            return readFilePath;
        }
    }
}
