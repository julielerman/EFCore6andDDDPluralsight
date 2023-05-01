using ContractBC.ContractAggregate;
using ContractBC.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using PublisherSystem.SharedKernel.DTOs;
using System.Text.Json;

namespace IntegrationTests
{
    [TestClass]
    public class ContractTests
    {
        Contract _contract = new Contract(
           DateTime.Today, new List<Author> { Author.UnsignedAuthor("first", "last","email","phone") }, "booktitle"
           );
        ContractContext _context;

        public ContractTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContractContext>();
            optionsBuilder.UseSqlServer(
                 "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=PubContractTests");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
            var _options = optionsBuilder.Options;
            _context = new ContractContext(_options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
        [TestMethod]
        public void NewContractHasNonEmptyId()
        {
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts.FirstOrDefault();
            Assert.AreNotEqual(Guid.Empty, contractFromDB.Id);
        }

        [TestMethod]
        public void NewContractStoresCorrectId()
        {
            var assignedId = _contract.Id;
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts.FirstOrDefault();
            Assert.AreEqual(assignedId, contractFromDB.Id);
        }

        [TestMethod]
        public void NewContractHasVersionWithSpecDefaults()
        {
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var defaultSpecs = SpecificationSet.Default();
            var contractFromDB = _context.Contracts.Include(c => c.Versions).FirstOrDefault();
            Assert.AreEqual(defaultSpecs, contractFromDB.Versions.First().Specs);
        }



        [TestMethod]
        public void NewContractHasContractNumberWhenQueried()
        {
            var calculatedContractNumber = _contract.ContractNumber;
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts.FirstOrDefault();
            Assert.AreEqual(calculatedContractNumber, contractFromDB.ContractNumber);
        }

        [TestMethod]
        public void NewContractHasDateInitiatedWhenQueried()
        {
            var calculatedDateInit = _contract.DateInitiated;
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts.FirstOrDefault();
            Assert.AreEqual(calculatedDateInit, contractFromDB.DateInitiated);
        }

        [TestMethod]
        public void NewContractValuesPersistedandRetrieved()
        {
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts.Include(c => c.Versions).ThenInclude(v => v.Specs).FirstOrDefault();
           //to force the test to fail, you can make the following modification, changing one of the contract instances
            // contractFromDB.FinalVersionSignedByAllParties();
            var expected = JsonSerializer.Serialize(_contract, CustomJsonOptions());
            var actual = JsonSerializer.Serialize(contractFromDB, CustomJsonOptions());
            Assert.AreEqual(expected, actual);
          
        }

        private JsonSerializerOptions CustomJsonOptions()
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new CustomDateTimeConverter("yyyy-MM-ddTHH:mm:ss"));
             options.Converters.Add(new CustomDecimalConverter("F"));
            return options;

        }

        [TestMethod]
        public void HasRevisedSpecSetIsFalseForNewContractNewVersion()
        {
           
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var value = _context.Set<ContractVersion>().Select(v=>EF.Property<bool>(v, "_hasRevisedSpecSet")).FirstOrDefault();
            //EF COre 7 would allow:
            //var value = _context.Database.SqlQuery<int>(@"SELECT TOP 1 [_hasRevisedSpecSet] 
            //                                              FROM [ContractVersions]")
            //            .FirstOrDefault();
            Assert.AreEqual(false, value);
        }

        [TestMethod]
        public void PersistedContractWithRevisonReturnsTwoVersions()
        {
            CreateNewContractAndAddRevision();
            _context.SaveChanges();
            _context.ChangeTracker.Clear(); 
            var contractFromDB = _context.Contracts.Include(c => c.Versions)
                .FirstOrDefault();

            Assert.AreEqual(2, contractFromDB.Versions.Count());

        }


        [TestMethod]
        public void RevisonUsingSameSpecsStoresHasRevisedAsFalse()
        {

            CreateNewContractAndAddRevision();
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts.Include(c => c.Versions)
                .FirstOrDefault();

            var value = _context.Set<ContractVersion>().Select(v => EF.Property<bool>(v, "_hasRevisedSpecSet")).FirstOrDefault();
            //EF COre 7 would allow:
            //var value = _context.Database.SqlQuery<int>(@"SELECT TOP 1 [_hasRevisedSpecSet] 
            //                                              FROM [ContractVersions]")
            //            .FirstOrDefault();

            Assert.AreEqual(false, value);

        }

        [TestMethod]
        public void PersistedContractWithRevisonAndSameAuthorsDuplicatesAuthorInDatabase()
        {
            var authorsv1 = _contract.CurrentVersion().Authors.ToList();
            CreateNewContractAndAddRevision();
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts
                .Select(c => new { c.CurrentVersionId, current=c.Versions.FirstOrDefault(v => v.Id == c.CurrentVersionId) })
                .FirstOrDefault();
          //Wrench: authorsv1= new List<Author>{ authorsv1.First().FixName("new", "name")};
            var authorsv2 = contractFromDB.current.Authors.ToList();
            Assert.AreEqual(JsonSerializer.Serialize(authorsv1, CustomJsonOptions()),
                JsonSerializer.Serialize(authorsv2, CustomJsonOptions()));

        }

        private void CreateNewContractAndAddRevision()
        {
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts.Include(c => c.Versions).FirstOrDefault();
            var v1 = contractFromDB.CurrentVersion();
            var authors = new List<Author>();
            v1.Authors.ToList().ForEach(author => authors.Add(author.Copy()));
            //contractFromDB.CreateRevisionUsingSameSpecs
            //    (ContractBC.Enums.ModReason.Other, "abc", v1.WorkingTitle, v1.Authors.ToList(), null);
            contractFromDB.CreateRevisionUsingSameSpecs
                (ContractBC.Enums.ModReason.Other, "abc", v1.WorkingTitle, authors, null);

        }
        private void CreateNewContractAndAddRevisionWithNewSpecs()
        {
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts.Include(c => c.Versions).FirstOrDefault();
            var v1 = contractFromDB.CurrentVersion();
            var authors = new List<Author>();
            v1.Authors.ToList().ForEach(author => authors.Add(author.Copy()));
            contractFromDB.CreateRevisionUsingNewSpecs
                (ContractBC.Enums.ModReason.Other, "abc", v1.WorkingTitle, authors, null,SpecificationSet.Default());

        }
    }
}