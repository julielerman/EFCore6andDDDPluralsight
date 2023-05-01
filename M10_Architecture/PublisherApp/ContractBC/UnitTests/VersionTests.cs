using ContractBC.ContractAggregate;
using ContractBC.Enums;
using ContractBC.Services;
using ContractBC.ValueObjects;
using PublisherSystem.SharedKernel.DTOs;
using System.Reflection;

namespace ContractBC.UnitTests;

[TestClass]
public class VersionTests
{
    List<Author> _unsignedAuthors = new List<Author> { Author.UnsignedAuthor( "first", "last", "email", "phone") };

    [TestMethod]
    public void NewVersionHasSpecDefaults()
    {
        var versionattribs = VersionAttributeFactory.Create(Guid.Empty, " ", new List<Author>(), ModReason.NewContract, "");
        var version = ContractVersion
            .CreateNew(versionattribs);
        var defaultSpecs = SpecificationSet.Default();
        Assert.AreEqual(defaultSpecs, version.Specs);
    }

    [TestMethod]
    public void NewContractHasVersionWithSpecDefaults()
    {
        var contract = new Contract(DateTime.Today
            , _unsignedAuthors,
            "booktitle");
        var defaultSpecs = SpecificationSet.Default();
        Assert.AreEqual(defaultSpecs, contract.CurrentVersion().Specs);
    }

    [TestMethod]
    public void NewContractVersionValueEqualsNewContract()
    {
        var contract = new Contract(DateTime.Today,
           _unsignedAuthors,
            "booktitle");

        Assert.AreEqual(ModReason.NewContract, contract.CurrentVersion().ModificationReason);
    }

    [TestMethod]
    public void NewContractCurrentVersionHasCorrectAuthor()
    {
        var contract = new Contract(DateTime.Today,
            _unsignedAuthors,
            "booktitle");
        var nameFromContract =
            contract.CurrentVersion().Authors.FirstOrDefault().Name.FullName;
        Assert.AreEqual("first last", nameFromContract);
    }
    [TestMethod]
    public void NewContractHasId()
    {
        var contract = new Contract(DateTime.Today,
    _unsignedAuthors,
    "booktitle");
        Assert.AreNotEqual(Guid.Empty, contract.Id);
    }
    [TestMethod]
    public void NewContractVersionHasId()
    {
        var contract = new Contract(DateTime.Today,
    _unsignedAuthors,
    "booktitle");
        Assert.AreNotEqual(Guid.Empty, contract.CurrentVersion().Id);
    }
    [TestMethod]
    public void VersionRevisionResultsinCurrentVersionWithNewId()
    {
        var contract = new Contract(DateTime.Today,
    _unsignedAuthors,
    "booktitle");
        var firstVersionId=contract.CurrentVersion().Id;
        contract.CreateRevisionUsingSameSpecs(ModReason.Other, "abc", "xyz", _unsignedAuthors, null);
        Assert.AreNotEqual(firstVersionId, contract.CurrentVersion().Id);
    }
    [TestMethod]
    public void VersionRevisionResultsinCurrentVersionWithValidId()
    {
        var contract = new Contract(DateTime.Today,
    _unsignedAuthors,
    "booktitle");
        var firstVersionId = contract.CurrentVersion().Id;
        contract.CreateRevisionUsingSameSpecs(ModReason.Other, "abc", "xyz", _unsignedAuthors, null);
        Assert.AreNotEqual(Guid.Empty, contract.CurrentVersion().Id);
    }

    [TestMethod]
    public void NewContractHasCurrentVersionWithSameDetails()
    {
        var contract = new Contract(DateTime.Today,
         _unsignedAuthors,
        "booktitle");
        var namefromContract =
            contract.CurrentVersion().Authors.FirstOrDefault().FullName;
        Assert.AreEqual(
            "first last booktitle",
            $"{namefromContract} booktitle"
            );
    }

    [TestMethod]
    public void AddingContractRevisionIncreasestheNumberOfVersions()
    {
        var contract = new Contract(DateTime.Today,
       _unsignedAuthors,
       "booktitle");

        contract.CreateRevisionUsingSameSpecs
            (ModReason.ChangeAttributes, "abc", "title", _unsignedAuthors, null);
        Assert.AreEqual(2, contract.Versions.Count());
    }
    [TestMethod]
    public void ContractRevisionResultsInCorrectCurrentVersion()
    {
        var contract = new Contract
            (DateTime.Today,_unsignedAuthors, "booktitle");
        contract.CreateRevisionUsingSameSpecs(ModReason.ChangeAttributes, "abc", "title", _unsignedAuthors, null);
        var ccv = contract.CurrentVersion();
        CollectionAssert.AreEqual
           (new string[] { ModReason.ChangeAttributes.ToString(), "abc", "title", "fl" },
            new string[] { ccv.ModificationReason.ToString(), ccv.ModificationDetails, ccv.WorkingTitle, ccv.Authors.FirstOrDefault().Name.SingleInitials });
    }
    [TestMethod]
    public void ContractRevisionWithSameSpecsSetsHasRevisedSpecsCorrectValue()
    {
        var contract = new Contract
            (DateTime.Today, _unsignedAuthors, "booktitle");
        contract.CreateRevisionUsingSameSpecs(ModReason.ChangeAttributes, "abc", "title", _unsignedAuthors, null);
        var ccv = contract.CurrentVersion();
        var theField = typeof(ContractVersion).GetField("_hasRevisedSpecSet",
                               BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
       var hasRevisedSpecs = (bool)theField.GetValue(ccv);

        Assert.IsFalse(hasRevisedSpecs);
           }

    [TestMethod]
    public void DerivedVersionIdIsProtected()
    {
        var prop = typeof(ContractVersion).GetProperty("Id");
        Assert.IsTrue(prop.SetMethod.IsFamily);
    }
}

