﻿using System.IO;
using NUnit.Framework;

namespace Nejdb.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        private Database _dataBase;
        private const string DbName = "test.db";

        [SetUp]
        public void Setup()
        {
            if (File.Exists(DbName))
            {
                File.Delete(DbName);
            }
            _dataBase = Library.Instance.CreateDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            _dataBase.Dispose();
        }

        [Test]
        public void Can_open_data_base()
        {
            _dataBase.Open(DbName);

            var isOpen = _dataBase.IsOpen;

            Assert.That(isOpen, Is.True);
        }

        [Test]
        public void Can_sync_database()
        {
            _dataBase.Open(DbName);

            _dataBase.Synchronize();
        }


        [Test]
        public void Database_metadata_returns_info_about_collections()
        {
            _dataBase.Open(DbName);

            _dataBase.CreateCollection("TheFirst", new CollectionOptions());

            var metaData = _dataBase.DatabaseMetadata;

            var metaDataAsString = metaData.ToString();

            StringAssert.Contains("TheFirst", metaDataAsString);
        }

        [Test]
        public void Can_create_collection_data_base()
        {
            _dataBase.Open(DbName);

            var collection = _dataBase.CreateCollection("TheFirst", new CollectionOptions());

            collection.Drop();

            var metaData = _dataBase.DatabaseMetadata;

            var metaDataAsString = metaData.ToString();

            StringAssert.DoesNotContain("TheFirst", metaDataAsString);
        }

        [Test]
        public void Can_get_collection()
        {
            _dataBase.Open(DbName);

            _dataBase.CreateCollection("TheFirst", new CollectionOptions());

            Assert.DoesNotThrow(() => _dataBase.GetCollection("TheFirst"));
        }

        [Test]
        public void Get_collection_throws_if_not_exists()
        {
            _dataBase.Open(DbName);

            Assert.Throws<EjdbException>(() => _dataBase.GetCollection("TheFirst"));
        }

        
    }
}