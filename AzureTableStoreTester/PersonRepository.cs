using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

namespace AzureTableStoreTester
{
    public class PersonRepository : IDisposable
    {
        private readonly Random _random;
        public IList<Person> People { get; private set; }
        private readonly TestParameters _testParameters;
        private CloudTable _table;

        public static PersonRepository CreateAndInitialize(TestParameters p)
        {
            PersonRepository pr = new PersonRepository(p);
            pr.Initialise();

            return pr;
        }

        private PersonRepository(TestParameters parameters, bool createTable = true)
        {
            _random = new Random();
            People = new List<Person>();
            _testParameters = parameters;
            _table = GetOrCreateTable(createTable);
        }

        private void Initialise()
        {
            using (var reader = new StreamReader(File.OpenRead(_testParameters.DataFile)))
            {
                // throw away the header line...
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    Person p = new Person()
                    {
                        Index = values[0],
                        Gender = values[1],
                        Title = values[2],
                        GivenName = values[3],
                        MiddleInitial = values[4],
                        Surname = values[5],
                        StreetAddress = values[6],
                        City = values[7],
                        State = values[8],
                        PostCode = values[9],
                        Country = values[10],
                        EmailAddress = values[11],
                        Username = values[12],
                        Password = values[13],
                        TelephoneNumber = values[14],
                        Birthday = values[15],
                        Occupation = values[16],
                        Company = values[17]
                    };

                    People.Add(p);
                }
            }
        }

        public int Count { get { return People.Count; } }

        public void PerformInitialLoad()
        {
            long number = (long)Math.Ceiling(((double)Count) * _testParameters.InitialLoadPercentage);

            for (long i = 1; i <= number; i++)
            {
                TestResult r = Insert(RandomDetachedPerson());
                r.Index = i;
                r.Operation = "Initial Load";

                r.Print(Console.Out);
            }
        }

        public Person RandomAttachedPerson()
        {
            IEnumerable<Person> people = People.Where(x => x.Persisted).ToList();

            if (!people.Any())
            {
                return null;
            }

            int index = (int)Math.Floor(((double)people.Count()) * _random.NextDouble());
            return people.ElementAt(index);
        }

        public Person RandomDetachedPerson()
        {
            IEnumerable<Person> people = People.Where(x => x.Persisted == false).ToList();

            if (!people.Any())
            {
                return null;
            }

            int index = (int)Math.Floor(((double)people.Count()) * _random.NextDouble());
            return people.ElementAt(index);
        }


        public TestResult Insert(Person p)
        {
            if (p == null)
            {
                return CreateNoOpTestResult();
            }

            TestResult r = CreateTestResult(p, "Insert");

            r.Start = DateTime.Now;
            TableOperation insert = TableOperation.Insert(p);
            _table.Execute(insert);

            r.End = DateTime.Now;

            p.Persisted = true;

            return r;
        }


        public TestResult Delete(Person p)
        {
            if (p == null)
            {
                return CreateNoOpTestResult();
            }

            TestResult r = CreateTestResult(p, "Delete");

            r.Start = DateTime.Now;

            TableOperation insert = TableOperation.Delete(p);
            _table.Execute(insert);

            r.End = DateTime.Now;

            p.Persisted = false;

            return r;
        }

        public TestResult Update(Person p)
        {
            if (p == null)
            {
                return CreateNoOpTestResult();
            }

            Person replaceWith = RandomDetachedPerson();
            p.CopyInto(replaceWith);

            TestResult r = CreateTestResult(p, "Replace");
            r.Start = DateTime.Now;

            TableOperation update = TableOperation.Replace(p);
            _table.Execute(update);
            r.End = DateTime.Now;

            p.Persisted = true;

            return r;
        }

        public TestResult Load(Person p)
        {
            if (p == null)
            {
                return CreateNoOpTestResult();
            }

            TestResult r = CreateTestResult(p, "Load");

            r.Start = DateTime.Now;

            TableOperation retrieve = TableOperation.Retrieve(p.PartitionKey, p.RowKey);
            _table.Execute(retrieve);

            r.End = DateTime.Now;

            p.Persisted = true;

            return r;
        }

        private static TestResult CreateTestResult(Person p, string operation)
        {
            TestResult r = new TestResult
            {
                ParititonKey = p.PartitionKey,
                RowKey = p.RowKey,
                Operation = operation

            };
            return r;
        }

        private static TestResult CreateNoOpTestResult()
        {
            return new TestResult
            {
                ParititonKey = null,
                RowKey =  null,
                Operation = "No-Op",
                Start = DateTime.Now,
                End = DateTime.Now
            };
        }


        /// <summary>
        /// Breaks this <see cref="PersonRepository"/> down into <paramref name="number"/> of subset <see cref="PersonRepository"/> instances, splitting
        /// the internal <see cref="IList"/> of <see cref="Person"/>s equally between all repositories.
        /// </summary>
        public IEnumerable<PersonRepository> Split(long number)
        {
            long countPerRepo = Count / number;

            IList<PersonRepository> result = new List<PersonRepository>();

            long cumulative = 0;
            for (long i = 0; i < number; i++)
            {
                PersonRepository repo = new PersonRepository(_testParameters, false);
                result.Add(repo);

                for (long j = cumulative; j < cumulative + countPerRepo; j++)
                {
                    repo.People.Add(People[(int)j]);
                }

                cumulative += countPerRepo;
            }

            return result;
        }


        private CloudTable GetOrCreateTable(bool create)
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(_testParameters.AccountName, _testParameters.KeyValue), _testParameters.UseHttps);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(_testParameters.TableName);

            if (create)
            {
                // make sure the table doesn't exist...
                table.DeleteIfExists();
                table.CreateIfNotExists();
            }

            return table;
        }

        public void Dispose()
        {
            if (_table != null)
            {
                _table.Delete();
            }
        }
    }
}
