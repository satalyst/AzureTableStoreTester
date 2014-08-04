using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableStoreTester
{
    public class TestParameters
    {
        public const string ACCOUNT_NAME_ARG = "--accountName";
        public const string KEY_VALUE_ARG = "--accessKey";

        public const string INITIAL_LOAD_ARG = "--initialLoad";
        public const string INSERT_ARG = "--insert";
        public const string UPDATE_ARG = "--update";
        public const string DELETE_ARG = "--delete";
        public const string RETRIEVE_ARG = "--load";

        public const string DATA_FILE_ARG = "--data";
        public const string USE_HTTPS_ARG = "--useHttps";
        public const string TABLE_NAME_ARG = "--tableName";
        public const string TEST_COUNT_ARG = "--count";

        public const string THREAD_COUNT_ARG = "--threadCount";

        public double InitialLoadPercentage { get; set; }
        public double InsertProbability { get; set; }
        public double UpdateProbability { get; set; }
        public double DeleteProbability { get; set; }
        public double RetrieveProbability { get; set; }

        public string DataFile { get; set; }
        public string AccountName { get; set; }
        public string KeyValue { get; set; }
        public bool UseHttps { get; set; }
        public string TableName { get; set; }
        public long TestCount { get; set; }
        public int ThreadCount { get; set; }


        private TestParameters()
        {
            InitialLoadPercentage = 0.001;

            RetrieveProbability = 0.4;
            InsertProbability = 0.25;
            UpdateProbability = 0.25;
            DeleteProbability = 0.1;

            TestCount = 1000;
            ThreadCount = 1;

            DataFile = "people.csv";
            UseHttps = true;
            TableName = "testtable";
        }

        public static TestParameters Parse(string[] args)
        {
            TestParameters parameters = new TestParameters();

            foreach (string arg in args)
            {
                string commandName = arg.Split('=')[0];

                switch (commandName)
                {
                    case INITIAL_LOAD_ARG:
                        {
                            parameters.InitialLoadPercentage = DoubleValue(arg, 0.0);
                            break;
                        }

                    case INSERT_ARG:
                        {
                            parameters.InsertProbability = DoubleValue(arg, 0.0);
                            break;
                        }

                    case UPDATE_ARG:
                        {
                            parameters.UpdateProbability = DoubleValue(arg, 0.0);
                            break;
                        }

                    case DELETE_ARG:
                        {
                            parameters.DeleteProbability = DoubleValue(arg, 0.0);
                            break;
                        }

                    case RETRIEVE_ARG:
                        {
                            parameters.RetrieveProbability = DoubleValue(arg, 0.0);
                            break;
                        }

                    case DATA_FILE_ARG:
                        {
                            parameters.DataFile = StringValue(arg);
                            break;
                        }

                    case ACCOUNT_NAME_ARG:
                        {
                            parameters.AccountName = StringValue(arg);
                            break;
                        }

                    case KEY_VALUE_ARG:
                        {
                            parameters.KeyValue = StringValue(arg);
                            break;
                        }

                    case USE_HTTPS_ARG:
                        {
                            parameters.UseHttps = BoolValue(arg);
                            break;
                        }

                    case TABLE_NAME_ARG:
                        {
                            parameters.TableName = StringValue(arg);
                            break;
                        }

                    case TEST_COUNT_ARG:
                        {
                            parameters.TestCount = LongValue(arg, 1);
                            break;
                        }

                    case THREAD_COUNT_ARG:
                    {
                        parameters.ThreadCount = (int)LongValue(arg, 1);
                        break;
                    }

                    default:
                        {
                            throw new ArgumentException(string.Format("Unrecognised argument: {0}", arg));
                        }
                }
            }

            return parameters;
        }

        private static long LongValue(string arg, long min)
        {
            string val = StringValue(arg);

            long value;

            if (!long.TryParse(val, out value))
            {
                throw new ArgumentException(string.Format("Could not extract number from argument: {0}", arg));
            }

            if (value < min)
            {
                throw new ArgumentException(string.Format("Could not set value, {0} is less than {1}", value, min));
            }

            return value;
        }

        private static double DoubleValue(string arg, double min)
        {
            string val = StringValue(arg);

            double value;

            if (!double.TryParse(val, out value))
            {
                throw new ArgumentException(string.Format("Could not extract number from argument: {0}", arg));
            }

            if (value < min)
            {
                throw new ArgumentException(string.Format("Could not set value, {0} is less than {1}", value, min));
            }

            return value;
        }

        private static string StringValue(string arg)
        {
            int index = arg.IndexOf("=", StringComparison.Ordinal);
            string val = arg.Substring(index + 1).Trim();

            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException(string.Format("Could not extract value from argument: {0}", arg));
            }

            return val;
        }

        private static bool BoolValue(string arg)
        {
            string val = StringValue(arg);

            bool value = false;

            if (bool.TryParse(val, out value))
            {
                return value;
            }

            throw new ArgumentException(string.Format("Could not extract bool from argument: {0}", arg));
        }

    }
}
