﻿// This file has been modified by Microsoft on 9/2017.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleTestAdapter.Helpers
{

    public static class Utils
    {

        public static string GetTempDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        public static bool DeleteDirectory(string directory)
        {
            string dummy;
            return DeleteDirectory(directory, out dummy);
        }

        public static bool DeleteDirectory(string directory, out string errorMessage)
        {
            try
            {
                Directory.Delete(directory, true);
                errorMessage = null;
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        public static string GetExtendedPath(string pathExtension)
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            return string.IsNullOrEmpty(pathExtension) ? path : $"{pathExtension};{path}";
        }

        public static void TimestampMessage(ref string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            message = $"{timestamp} - {message ?? ""}";
        }

        /// <exception cref="AggregateException">If at least one of the actions has thrown an exception</exception>
        public static bool SpawnAndWait(Action[] actions, int timeoutInMs = Timeout.Infinite)
        {
            var tasks = new Task[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                tasks[i] = Task.Factory.StartNew(actions[i]);
            }
      
            return Task.WaitAll(tasks, timeoutInMs);
        }

        public static bool BinaryFileContainsStrings(string executable, Encoding encoding, IEnumerable<string> strings)
        {
            byte[] file = File.ReadAllBytes(executable);
            return strings.All(s => file.IndexOf(encoding.GetBytes(s)) >= 0);
        }

        public static void ValidateRegex(string pattern)
        {
            try
            {
                Regex.Match(string.Empty, pattern);
            }
            catch (ArgumentException e)
            {
                throw new Exception(String.Format(Resources.InvalidRegularExpression, pattern, e.Message));
            }
        }

        public static void ValidateTraitRegexes(string value)
        {
            // The parser will throw if the value is not well formed.
            var parser = new RegexTraitParser(null);
            parser.ParseTraitsRegexesString(value, ignoreErrors: false);
        }
    }

}