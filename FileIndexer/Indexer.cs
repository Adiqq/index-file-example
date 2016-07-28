using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Schema;

namespace FileIndexer {
    public class Indexer {
        private static readonly string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
        private const string _filename = "test.txt";
        private const string _indexFile = "test-index.txt";

        private static readonly string _filePath = Path.Combine(_baseDir, _filename);
        private static readonly string _indexPath = Path.Combine(_baseDir, _indexFile);

        private static readonly Random _random = new Random();
        private static readonly StringBuilder _builder = new StringBuilder();

        private static readonly long _indexSize = 1000;

        public static void GenerateFile(Megabyte fileSize) {
            if (File.Exists(_filePath)) {
                File.Delete(_filePath);
            }
            using (StreamWriter sw = File.CreateText(_filePath)) {
                long size = 0;
                while (size <= fileSize.Value) {
                    var length = _random.Next(1, 200);
                    for (var i = 0; i < length; i++) {
                        var ch = _random.Next(32, 126);
                        _builder.Append((char)ch);
                    }
                    var line = _builder.ToString();
                    size += Encoding.UTF8.GetByteCount(line);
                    sw.WriteLine(line);
                    _builder.Clear();
                }

            }

        }

        public static void GenerateIndexFile() {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(baseDir, _indexFile);
            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }

            using (StreamReader sr = File.OpenText(_filePath))
            using (StreamWriter sw = File.CreateText(_indexPath)) {
                string line;
                long size = 0;
                long lineNumber = 0;
                while ((line = sr.ReadLine()) != null) {
                    size += Encoding.UTF8.GetByteCount(line + Environment.NewLine);
                    if (lineNumber% _indexSize == 0) {
                        var indexEntry = $"{lineNumber} {size}";
                        sw.WriteLine(indexEntry);
                    }
                    lineNumber++;
                }

            }
        }

        public static IDictionary<long, long> CreateDictionaryFromIndex() {
            var dic = new Dictionary<long, long>();

            using (StreamReader sr = File.OpenText(_indexPath)) {
                string line;
                while ((line = sr.ReadLine()) != null) {
                    var splitted = line.Split(' ');
                    dic.Add(long.Parse(splitted[0]), long.Parse(splitted[1]));
                }

            }
            return dic;
        }

        public static string GetLineByLinearSearch(long lineNumber) {
            using (StreamReader sr = File.OpenText(_filePath)) {
                for (var i = 0l; i < lineNumber - 1; i++) {
                    sr.ReadLine();
                }
                return sr.ReadLine();
            }
        }

        public static string GetLineByDictionaryLookup(IDictionary<long,long> index, long lineNumber) {
            using (StreamReader sr = File.OpenText(_filePath)) {
                var roundDownNumber = Math.Max((((int)lineNumber - 2) / _indexSize) * _indexSize, 0);
                var offset = index[roundDownNumber];
                sr.BaseStream.Seek(offset, SeekOrigin.Begin);
                offset = (lineNumber - 2) - roundDownNumber;
                for (var i = 0; i < offset; i++) {
                    sr.ReadLine();
                }
                return sr.ReadLine();
            }
        }

        public static long CountLines() {
            using (StreamReader sr = File.OpenText(_filePath)) {
                var length = 0l;
                while ((sr.ReadLine()) != null) {
                    length++;
                }
                return length;
            }
        }

    }


    public class Megabyte {
        private readonly long _value;

        public long Value {
            get { return _value * 1024 * 1024; }
        }

        public Megabyte(long value) {
            _value = value;
        }
    }
}