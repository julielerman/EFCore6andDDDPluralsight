using ContractBC.Enums;
using ContractBC.ValueObjects;

namespace ContractBC.Services
{
    public class VersionAttributeFactory
    {
        public static BaseAttributes Create(Guid contractId, string workingTitle, List<Author> authors, ModReason modReason, string modDescription)
        {
            return new BaseAttributes(contractId, workingTitle, authors, modReason, modDescription);
        }
        public struct BaseAttributes
        {
            internal BaseAttributes(Guid contractId, string workingTitle, List<Author> authors, ModReason modReason, string modDescription)
            {
                ContractId = contractId;
                WorkingTitle = workingTitle;
                Authors = authors;
                ModReason = modReason;
                ModDescription = modDescription;
            }

            public Guid ContractId { get; }
            public string WorkingTitle { get; }
            public List<Author> Authors { get; }
            public ModReason ModReason { get; }
            public string ModDescription { get; }
        }
    }
}
