using ContractBC.ContractAggregate;
using ContractBC.Enums;
using ContractBC.ValueObjects;
using Infrastructure.Data;
using Infrastructure.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IntegrationTests
{
    [TestClass]
    public class ServiceWorkflowTests
    {
        Contract _contract = new Contract(
           DateTime.Today, new List<Author> { Author.UnsignedAuthor("first", "last", "email", "phone") }, "booktitle"
           );
        ContractContext _context;
        ContractServices _service;

        public ServiceWorkflowTests()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContractContext>();
            optionsBuilder.UseSqlServer(
                 "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=PubRepoTests");
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
            var _options = optionsBuilder.Options;
            _context = new ContractContext(_options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            _service = new ContractServices(_context);
        }
     

        private JsonSerializerOptions CustomJsonOptions()
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new CustomDateTimeConverter("yyyy-MM-ddTHH:mm:ss"));
            options.Converters.Add(new CustomDecimalConverter("F"));
            return options;
        }

        private Contract CreateNewContractAndAddRevision()
        {
            _context.Contracts.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            var contractFromDB = _context.Contracts.Include(c => c.Versions).FirstOrDefault();
            var v1 = contractFromDB.CurrentVersion();
            var authors = new List<Author>();
            v1.Authors.ToList().ForEach(author => authors.Add(author.Copy()));
            contractFromDB.CreateRevisionUsingSameSpecs
              (ContractBC.Enums.ModReason.Other, "abc", v1.WorkingTitle, authors, null);
            return contractFromDB;
        }
        
        private Guid StoreContractAggregate()
        {
            var cid = _contract.Id; 
            _service.Add(_contract);
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
            return cid;

        }
        [TestMethod]
        public void CanAddANewVersionToAPartialAggregate()
        {
           //arrange
            var cid= StoreContractAggregate();
            //act
            var cDB1 = _service.GetContractWithCurrentVersionOnly(cid);
            var authors = new List<Author>();
            cDB1.CurrentVersion().Authors.ToList().ForEach(author => authors.Add(author.Copy()));
            cDB1.CreateRevisionUsingSameSpecs(ModReason.Other,"did a thing", "the title",authors, null);
            var newVID = cDB1.CurrentVersionId;
            _service.AddRevision(cDB1);
           _context.ChangeTracker.Clear();
            //assert
            var cDB2= _service.GetContractAggregateById(cid);
           CollectionAssert.AreEqual(new Object[] { newVID, 2 }, new Object[] { cDB2.CurrentVersionId, cDB2.Versions.Count() });
          }

    }
}