using Sellorio.Substripper.Models;
using System;

namespace Sellorio.Substripper
{
    internal static class Constants
    {
        public static TimeSpan PollingRate { get; } = TimeSpan.FromMinutes(5);

        public static string MediaFilePath { get; } = "/src";
        public static string HistoryPath { get; } = "/data/history.json";

        public static class Subtitles
        {
            public static Language[] LanguagesToKeep { get; } = [Language.English, Language.Japanese];
        }
    }
}
