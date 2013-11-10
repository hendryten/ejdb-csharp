﻿using System;
using System.IO;
using Nejdb;
using Nejdb.Bson;
using NUnit.Framework;

namespace Ejdb.Tests
{
	[TestFixture]
	public class CollectionTests
	{
		private Database _dataBase;
		private Collection _collection;
		private BsonDocument _origin;
		private const string DbName = "test.db";

		[SetUp]
		public void Setup()
		{
			if (File.Exists(DbName))
			{
				File.Delete(DbName);
			}
			_dataBase = Library.Instance.CreateDatabase();

			_dataBase.Open(DbName, Database.DefaultOpenMode | OpenMode.TruncateOnOpen);

			_collection = _dataBase.CreateCollection("default", new CollectionOptions());

			_origin = BsonDocument.ValueOf(new
				{
					name = "Grenny",
					type = "African Grey",
					male = true,
					age = 1,
					birthdate = DateTime.Now,
					likes = new[] { "green color", "night", "toys" },
					extra = BsonNull.VALUE
				});
		}

		[Test]
		public void TEST()
		{
			using (var library = Library.Create())
			using (var database = library.CreateDatabase())
			{
				database.Open("MyDB.db");
				using (var collection = database.CreateCollection("Parrots", CollectionOptions.None))
				{
					var parrot = BsonDocument.ValueOf(new
					{
						name = "Grenny",
						type = "African Grey",
						male = true,
						age = 1,
						birthdate = DateTime.Now,
						likes = new[] { "green color", "night", "toys" },
						extra = BsonNull.VALUE
					});
					collection.Save(parrot, false);
				}
			}
		}


		[TearDown]
		public void TearDown()
		{
			_dataBase.Dispose();
		}

		[Test]
		public void Can_begin_and_commit_transaction()
		{
			_collection.BeginTransaction();

			var isActive = _collection.TransactionActive;

			_collection.CommitTransaction();

			var notActiveTransaction = !_collection.TransactionActive;

			Assert.That(isActive, Is.True, "Transaction should be active after begin");
			Assert.That(notActiveTransaction, Is.True, "Transaction should be active after commit");
		}

		[Test]
		public void Can_synchronize_collection()
		{
			_collection.Synchronize();

			//TODO: Assert something?
		}

		[Test]
		public void Can_save_and_load_document()
		{
			_collection.Save(_origin, false);

			var id = _origin.GetBsonValue("_id");

			var reloaded = _collection.Load((BsonOid)id.Value);
			//TODO: made more string assertion
			Assert.That(reloaded, Is.Not.Null);
		}


		[Test]
		public void Can_delete_document()
		{
			_collection.Save(_origin, false);
			var id = _origin.GetBsonValue("_id");
			var BsonOid = (BsonOid)id.Value;

			_collection.Delete(BsonOid);

			var reloaded = _collection.Load(BsonOid);

			Assert.That(reloaded, Is.Not.Null);
		}

		[Test]
		public void Can_create_index()
		{
			_collection.Index("name", IndexOperations.String);

			var meta = _dataBase.DatabaseMetadata;

			var metaAsString = meta.ToString();
			const string indexEvidence = "test.db_default.idx.sname.lex";

			StringAssert.Contains(indexEvidence, metaAsString);
		}
	}
}