using System.Data.Services.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableStoreTester
{
    public class Person : TableEntity
    {
        private string _index;
        public string Index
        {
            get { return _index; }
            set
            {
                _index = value;
                RowKey = value;
            }
        }
        public string Gender { get; set; }
        public string Title { get; set; }

        public string GivenName { get; set; }
        public string MiddleInitial { get; set; }

        private string _surname;
        public string Surname
        {
            get { return _surname; }
            set
            {
                _surname = value;
                PartitionKey = value;
            }
        }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string EmailAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TelephoneNumber { get; set; }
        public string Birthday { get; set; }
        public string Occupation { get; set; }
        public string Company { get; set; }

        public bool Persisted { get; set; }

        public Person()
        {
            Persisted = false;
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            IDictionary<string, EntityProperty> val = base.WriteEntity(operationContext);
            val.Remove("Persisted");
            return val;
        }


        public void CopyInto(Person p)
        {
            // copy all attributes other than the row (Index) and parition (Surname) keys.
            GivenName = p.GivenName;
            Gender = p.Gender;
            Title = p.Title;
            MiddleInitial = p.MiddleInitial;
            StreetAddress = p.StreetAddress;
            City = p.City;
            State = p.State;
            PostCode = p.PostCode;
            Country = p.Country;
            EmailAddress = p.EmailAddress;
            Username = p.Username;
            Password = p.Password;
            TelephoneNumber = p.TelephoneNumber;
            Birthday = p.Birthday;
            Occupation = p.Occupation;
            Company = p.Company;
        }
    }
}
