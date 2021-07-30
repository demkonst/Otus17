using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace DelegatesLib
{
    public class DocumentsReceiver: IDisposable
    {
        private readonly Dictionary<string, bool> _filesReady = new Dictionary<string, bool>
        {
            {nameof(FileNames.Passport), false},
            {nameof(FileNames.Application), false},
            {nameof(FileNames.Photo), false}
        };

        private FileSystemWatcher _fileSystemWatcher;
        private Timer _timer;

        public void Dispose()
        {
            _fileSystemWatcher?.Dispose();
            _timer?.Dispose();
        }

        public event Action DocumentsReady;
        public event Action TimedOut;

        public void Start(string targetDirectory, int waitingInterval)
        {
            _fileSystemWatcher = new FileSystemWatcher(targetDirectory)
            {
                EnableRaisingEvents = true
            };

            _fileSystemWatcher.Created += Fsw_Changed;
            _fileSystemWatcher.Deleted += Fsw_Changed;
            _fileSystemWatcher.Changed += Fsw_Changed;
            _fileSystemWatcher.Renamed += Fsw_Changed;

            _timer = new Timer(TimeSpan.FromSeconds(waitingInterval).TotalMilliseconds);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private void Stop()
        {
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Stop();

            _fileSystemWatcher.Created -= Fsw_Changed;
            _fileSystemWatcher.Deleted -= Fsw_Changed;
            _fileSystemWatcher.Changed -= Fsw_Changed;
            _fileSystemWatcher.Renamed -= Fsw_Changed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Stop();
            TimedOut?.Invoke();
        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            const WatcherChangeTypes changeTypes = WatcherChangeTypes.Created | WatcherChangeTypes.Changed | WatcherChangeTypes.Renamed;
            var fileReady = (e.ChangeType & changeTypes) == e.ChangeType;

            switch (e.Name)
            {
                case FileNames.Passport:
                    _filesReady[nameof(FileNames.Passport)] = fileReady;
                    break;
                case FileNames.Application:
                    _filesReady[nameof(FileNames.Application)] = fileReady;
                    break;
                case FileNames.Photo:
                    _filesReady[nameof(FileNames.Photo)] = fileReady;
                    break;
            }

            Console.WriteLine(e.ChangeType);
            Console.WriteLine(e.Name);

            if (_filesReady.Values.All(x => x))
            {
                Stop();
                DocumentsReady?.Invoke();
            }
        }
    }

    internal static class FileNames
    {
        public const string Passport = "Паспорт.jpg";
        public const string Application = "Заявление.txt";
        public const string Photo = "Фото.jpg";
    }
}
