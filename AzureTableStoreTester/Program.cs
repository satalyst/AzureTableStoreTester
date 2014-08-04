using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureTableStoreTester
{
    class Program
    {
        public static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(args);
        }

        public void Run(string[] args)
        {
            TestParameters parameters = TestParameters.Parse(args);
            RouletteWheelSelector selector = RouletteWheelSelector.Create(parameters);

            Console.WriteLine("INDEX, OPERATION, START_TIME, END_TIME, DURATION, PARTITION_KEY, ROW_KEY");

            using (PersonRepository repository = PersonRepository.CreateAndInitialize(parameters))
            {
                repository.PerformInitialLoad();
                Console.WriteLine();
                Console.WriteLine();

                PerformRuns(parameters, repository, selector);
            }

        }

        private void PerformRuns(TestParameters parameters, PersonRepository repository, RouletteWheelSelector selector)
        {
            long numberOfSteps = parameters.TestCount / parameters.ThreadCount;
            CountdownLatch latch = new CountdownLatch(parameters.ThreadCount);

            long total = 0;

            foreach (PersonRepository pr in repository.Split(parameters.ThreadCount))
            {
                BackgroundArg arg = new BackgroundArg
                {
                    Latch = latch,
                    Repository = pr,
                    Selector = selector,
                    StepCount = numberOfSteps,
                    StartIndex = total
                };

                BackgroundWorker worker = new BackgroundWorker { WorkerSupportsCancellation = true };
                worker.DoWork += bw_DoWork;

                worker.RunWorkerAsync(arg);

                total += numberOfSteps;
            }

            // wait until all background workers have finished running (or they've somehow been cancelled.
            latch.Wait();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs eventArgs)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            BackgroundArg arg = eventArgs.Argument as BackgroundArg;

            try
            {
                TestExecutor testExecutor = new TestExecutor(arg.Repository, arg.Selector);

                for (long j = 0; j < arg.StepCount; j++)
                {
                    if (worker.CancellationPending)
                    {
                        eventArgs.Cancel = true;
                        arg.Latch.Signal();
                        return;
                    }

                    TestResult result = testExecutor.Execute(arg.StartIndex + j);

                    if (result != null)
                    {
                        result.Print(Console.Out);
                    }
                }
            }
            finally
            {
                arg.Latch.Signal();
            }
        }

        private class BackgroundArg
        {
            public CountdownLatch Latch { get; set; }
            public PersonRepository Repository { get; set; }
            public RouletteWheelSelector Selector { get; set; }
            public long StepCount { get; set; }
            public long StartIndex { get; set; }
        }
    }
}
