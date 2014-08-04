using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableStoreTester
{
    public class TestResult
    {
        public long Index { get; set; }
        public string Operation { get; set; }
        public string ParititonKey { get; set; }
        public string RowKey { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public double Duration
        {
            get { return End.Subtract(Start).TotalMilliseconds; }
        }

        public TestResult()
        {
            Index = -1;
        }

        public void Print(TextWriter txt)
        {
            txt.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", Index, Operation, Start, End, Duration, ParititonKey, RowKey));
        }
    }
}
