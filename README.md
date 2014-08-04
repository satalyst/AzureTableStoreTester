Azure Table Store Tester
==============

Introduction
------------

The Azure Table Store Tester uses a CSV file of data and will perform basic CRUD operations using the data against the Azure Table Store. The table is automatically created before the tests and is deleted upon completion of the tests.

Times for each of the CRUD operations (as well as the initial data load) will be printed to the console in a CSV format for consumption in Microsoft Excel.


Arguments
---------

--accountName=\(string\) : The Azure account name to use \(__mandatory__\)

--accessKey=\(string\) : The access key for the azure account to use \(__mandatory__\)

--data=\(string\) : The path to the data file to use as source data \(defaults to the included _people.csv_\)

--useHttps=\(string\) : Whether to use HTTPS or not \(_true_ or _false_, defaults to _true_\)

--tableName=\(string\) : The name of the table to create in the Azure Table Store \(defaults to _testtable_\)

--count=\(n\) : The number of tests to run \(defaults to _1000_\)

Percentages
-----------

The table store tester will randomly perform operations against the table store, to adjust the relative weightings of the operations you can use the following command line arguments.


--initialLoad=\(n\) : The percentage of the data file to upload before starting testing  \(defaults to _0.001_\)

__The values below should add up to 1.0, otherwise NOP\'s will be performed__.

--insert=\(n\) : The percentage chance of an insert operation being performed \(defaults to _0.25_\)

--update=\(n\) : The percentance chance of an update operation being performed \(defaults to _0.25_\)

--delete=\(n\) : The percentage chance of a delete operation being performed \(defaults to _0.1_\)

--load=\(n\) : The percentage chance of a retrieve operation being performed \(defaults to _0.4_\)

References
----------

The included sample CSV file was generated using the awesome free online service [Fake Name Generator](http://www.fakenamegenerator.com/)
