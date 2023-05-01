using AuthorAndBookMaintenance.DomainModels;
using AuthorBook.Infrastructure.Data;

namespace AuthorBook.API;

public class NewContractService
{
    AuthorBookContext _context;
    public NewContractService(AuthorBookContext context)
    {
        _context = context;
    }
    public void AddAuthorsAndBookForNewContract(ContractSignedEventMessage contract)
    {
        var book = new Book(contract.Title,contract.ContractId, contract.SignedDate);
        _context.Books.Add(book);
        foreach (var authorDto in contract.Authors)
            if (authorDto.Signed)
            {
                //get the author and update with book and contractID
                var existingauthor=_context.Authors.Find(authorDto.SignedAuthorId);
                existingauthor.AddExistingBook(book);
                existingauthor.AddContractViaId(contract.ContractId);
            }
            else
            {
                var newAuthor=Author.CreateFromNewContractMessage
                    (authorDto.Name, authorDto.Email, contract.ContractId);
                newAuthor.AddExistingBook(book);
                _context.Add(newAuthor);
            }
        _context.SaveChanges();
    }
}
