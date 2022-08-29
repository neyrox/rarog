using System;
using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Engine.Storage;

namespace UnitTests
{
    [TestClass]
    public class TransactionTest
    {
        Database db;
        Shell shell;

        [TestInitialize]
        public void Setup()
        {
            db = new Database(new MemoryStorage());
            shell = new Shell(db);
        }

        [TestMethod]
        public void HashsetDetectsSameObjects()
        {
            var setOfObjects = new HashSet<object>();

            var obj1 = new object();
            var obj2 = new object();

            Assert.IsTrue(setOfObjects.Add(obj1));
            Assert.IsTrue(setOfObjects.Add(obj2));
            Assert.AreEqual(2, setOfObjects.Count);
            
            Assert.IsFalse(setOfObjects.Add(obj1));
            Assert.IsFalse(setOfObjects.Add(obj2));
            Assert.AreEqual(2, setOfObjects.Count);
        }
        
        [TestMethod]
        public void TransactionLifecycle()
        {
            Transaction tx = new SingleQueryTransaction();

            var beginCommand = new BeginTransactionNode();

            beginCommand.Execute(db, ref tx);

            var endCommand = new EndTransactionNode();

            endCommand.Execute(db, ref tx);
        }

        [TestMethod]
        public void ForbidNestedTransactions()
        {
            Transaction tx = new SingleQueryTransaction();

            var beginCommand1 = new BeginTransactionNode();

            beginCommand1.Execute(db, ref tx);

            var beginCommand2 = new BeginTransactionNode();

            Assert.ThrowsException<Exception>(() =>
            {
                beginCommand2.Execute(db, ref tx);
            });
        }
        
        [TestMethod]
        public void MultipleQueriesInTransaction()
        {
            Transaction tx = new SingleQueryTransaction();

            var beginCommand = new BeginTransactionNode();
            beginCommand.Execute(db, ref tx);

            CreateTable(ref tx);
            Insert(ref tx);

            var endCommand = new EndTransactionNode();
            endCommand.Execute(db, ref tx);
        }

        private void CreateTable(ref Transaction tx)
        {
            var command = Parser.Convert(new [] {"CREATE", "TABLE", "t1", "(", "c1", "int", ")", ";"});
            var create = command.Execute(db, ref tx);
            Assert.IsTrue(create.IsOK);
        }
        
        private void Insert(ref Transaction tx)
        {
            var command = Parser.Convert(new [] {"INSERT", "INTO", "t1", "(", "c1", ")", "VALUES", "(", "0", ")", ";"});
            var insert = command.Execute(db, ref tx);
            Assert.IsTrue(insert.IsOK);
        }

    }
}
