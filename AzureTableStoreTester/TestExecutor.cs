using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace AzureTableStoreTester
{
    public class TestExecutor
    {
        private readonly PersonRepository _repository;
        private readonly RouletteWheelSelector _selector;

        public TestExecutor(PersonRepository rep, RouletteWheelSelector selector)
        {
            this._repository = rep;
            this._selector = selector;
        }

        public TestResult Execute(long i)
        {
            TestResult result = null;
            Person person = null;
            Action? a = _selector.Random();

            try
            {
                if (a.HasValue)
                {
                    switch (a.Value)
                    {
                        case Action.Insert:
                        {
                            person = _repository.RandomDetachedPerson();
                            result = _repository.Insert(person);
                            break;
                        }

                        case Action.Update:
                        {
                            person = _repository.RandomAttachedPerson();
                            result = _repository.Update(person);
                            break;
                        }

                        case Action.Delete:
                        {
                            person = _repository.RandomAttachedPerson();
                            result = _repository.Delete(person);
                            break;
                        }

                        case Action.Retrieve:
                        {
                            person = _repository.RandomAttachedPerson();
                            result = _repository.Load(person);
                            break;
                        }

                        default:
                        {
                            throw new InvalidOperationException("Could not determine action: " + a.Value.ToString() + ".");
                        }
                    }
                }
            }
            catch
            {
                Console.Error.WriteLine("Exception occurred on Person: " + person.Index);

                if (a.HasValue)
                {
                    Console.Error.WriteLine("Attempted action: " + a.Value.ToString());
                }
                throw;
            }

            if (result != null)
            {
                result.Index = i;
            }

            return result;
        }
    }
}
