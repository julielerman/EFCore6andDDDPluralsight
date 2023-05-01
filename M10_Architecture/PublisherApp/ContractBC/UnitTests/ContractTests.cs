using ContractBC.ContractAggregate;
using ContractBC.Enums;
using ContractBC.ValueObjects;
using System.Reflection;

namespace ContractBC.UnitTests;

[TestClass]
public class ContractTests
{
    List<Author> _unsignedAuthors;
    Contract _contract;

    public ContractTests()
    {
        _unsignedAuthors = new List<Author> { Author.UnsignedAuthor("first", "last", "email", "phone") };
        _contract = new Contract(DateTime.Today, _unsignedAuthors, "booktitle");
    }

    [TestMethod]
    public void NewContractHasId()
    {
        Assert.AreNotEqual(Guid.Empty, _contract.Id);
    }

    [TestMethod]
    public void NewContractHasExpectedContractNumber()
    {
        Assert.AreEqual($"{DateTime.Today.ToShortDateString()}_firlas", _contract.ContractNumber);
    }

    [TestMethod]
    public void VersionRevisionResultsinChangeInCurrentVersionId()
    {
        var contract = new Contract(DateTime.Today, _unsignedAuthors, "booktitle");
        var firstVersionId = contract.CurrentVersion().Id;
        contract.CreateRevisionUsingSameSpecs(ModReason.Other, "abc", "xyz", _unsignedAuthors, null);
        Assert.AreNotEqual(firstVersionId, contract.CurrentVersionId);
    }

    [TestMethod]
    public void VersionRevisionResultsinNonEmptyVersionId()
    {
        var firstVersionId = _contract.CurrentVersion().Id;
        _contract.CreateRevisionUsingSameSpecs(ModReason.Other, "abc", "xyz", _unsignedAuthors, null);
        Assert.AreNotEqual(Guid.Empty, _contract.CurrentVersion().Id);
    }

    [TestMethod]
    public void AddingContractRevisionIncreasestheNumberOfVersions()
    {
        _contract.CreateRevisionUsingSameSpecs
            (ModReason.ChangeAttributes, "abc", "title", _unsignedAuthors, null);
        Assert.AreEqual(2, _contract.Versions.Count());
    }

    [TestMethod]
    public void ContractRevisionResultsInCorrectCurrentVersion()
    {
        _contract.CreateRevisionUsingSameSpecs(ModReason.ChangeAttributes, "abc", "title", _unsignedAuthors, null);
        var ccv = _contract.CurrentVersion();
        CollectionAssert.AreEqual
           (new string[] { ModReason.ChangeAttributes.ToString(), "abc", "title", "fl" },
            new string[] { ccv.ModificationReason.ToString(), ccv.ModificationDetails, 
                           ccv.WorkingTitle, ccv.Authors.FirstOrDefault().Name.SingleInitials });
    }

    [TestMethod]
    public void ContractRevisionWithSameSpecsSetsHasRevisedSpecsCorrectValue()
    {
        _contract.CreateRevisionUsingSameSpecs(ModReason.ChangeAttributes, "abc", "title",
                                               _unsignedAuthors, null);
        var ccv = _contract.CurrentVersion();
        var theField = typeof(ContractVersion)
            .GetField("_hasRevisedSpecSet",
                       BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        var hasRevisedSpecs = (bool)theField.GetValue(ccv);

        Assert.IsFalse(hasRevisedSpecs);
    }

    [TestMethod]
    public void DerivedContractIdIsProtected()
    {
        var prop = typeof(Contract).GetProperty("Id");
        Assert.IsTrue(prop.SetMethod.IsFamily);
    }
}

