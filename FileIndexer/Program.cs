using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIndexer {
    class Program {
        static void Main(string[] args) {
            Indexer.GenerateFile(new Megabyte(1000));
            Indexer.GenerateIndexFile();
            var index = Indexer.CreateDictionaryFromIndex();
            var lineCount = Indexer.CountLines();
            Console.WriteLine($"Line count: {lineCount}");
            CompareSearchTime(index, 5);
            CompareSearchTime(index, 500);
            CompareSearchTime(index, 50000);
            CompareSearchTime(index, 250000);

            CompareSearchTime(index, lineCount / 8);
            CompareSearchTime(index, lineCount / 4);
            CompareSearchTime(index, lineCount / 2);
            CompareSearchTime(index, lineCount);

            Console.Read();
        }

        private static void CompareSearchTime(IDictionary<long, long> index, long lineNumber) {
            Console.WriteLine($"Line number: {lineNumber}");
            var sw = new Stopwatch();

            sw.Start();
            var line = Indexer.GetLineByLinearSearch(lineNumber);
            sw.Stop();

            Console.WriteLine(line);
            Console.WriteLine($"Linear time(ms): {sw.ElapsedMilliseconds}");
            sw.Reset();

            sw.Start();
            line = Indexer.GetLineByDictionaryLookup(index, lineNumber);
            sw.Stop();

            Console.WriteLine(line);
            Console.WriteLine($"Lookup time(ms): {sw.ElapsedMilliseconds}");

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
