using PublisherSystem.SharedKernel.ValueObjects;

namespace AuthorBookBC.DomainModels
{
    public class PubContract
    {
        private PubContract()
        {
            
        }
        public PubContract(PersonName authorName, string email, string phone, string booktitle, Guid contractId, DateTime contractDate)
        {
            Author=new Author(authorName, email, phone);
            Book= new Book(booktitle);
            PreCreatedContractId = contractId;
            ContractedDate = contractDate;
            NewlyContractedRequiresDetails = true;
        }
 
        public PubContract CreateCoAuthorContract(PersonName authorName, string email, string phone)
        {
            var contract = new PubContract();
            contract.Author = new Author(authorName, email, phone);
            contract.BookId = Book.Id;
            contract.PreCreatedContractId = this.PreCreatedContractId;
            contract.ContractedDate = this.ContractedDate;
            contract.NewlyContractedRequiresDetails = true;
            return contract;

        }
        public PubContract(Guid existingAuthorId, string booktitle, Guid contractId, DateTime contractDate)
        {
            AuthorId = existingAuthorId;
            Book = new Book(booktitle);
            PreCreatedContractId = contractId;
            ContractedDate = contractDate;
            NewlyContractedRequiresDetails = true;
        }

        public Author Author { get; private set; } 
        public Guid AuthorId { get; private set; }
        public Book Book { get; private set; }
        public Guid BookId { get; private set; }
        public Guid PreCreatedContractId { get; private set; }
        public bool NewlyContractedRequiresDetails { get; private set; }
        public DateTime ContractedDate { get; private set; }
    }
}
