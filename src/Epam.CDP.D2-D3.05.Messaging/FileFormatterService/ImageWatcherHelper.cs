﻿using System.IO;

namespace FileFormatterService
{
    internal static class ImageWatcherHelper
    {
        public static void WatcherSettings(this FileSystemWatcher watcher, FileSystemEventHandler onCreated, ErrorEventHandler onError)
        {
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.Created += onCreated;
            watcher.InternalBufferSize = 81920;
            watcher.EnableRaisingEvents = true;
            watcher.Error += onError;
        }

        public static void CreateImageWatcher(out IImageWatcher watcher, EndOfFileEventHandler endOfFileEventHandler)
        {
            watcher = new ImageWatcher();
            watcher.EndOfFileEventDetected += endOfFileEventHandler;
            watcher.CheckDirectoriesForNewImages();
        }

        public static void DisposeImageWatcher(ref IImageWatcher watcher, EndOfFileEventHandler endOfFileEventHandler)
        {
            if (watcher == null)
                return;

            if (endOfFileEventHandler != null)
                watcher.EndOfFileEventDetected -= endOfFileEventHandler;
            watcher.Dispose();
            watcher = null;
        }

    }
}
