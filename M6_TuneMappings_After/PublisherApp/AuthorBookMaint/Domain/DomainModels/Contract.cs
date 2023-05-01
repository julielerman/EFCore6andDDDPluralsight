namespace AuthorAndBookMaintenance.ValueObjects;

public class Contract
{
    
    public static Contract Setup(Guid contractId, List<Guid> authorIds, Guid bookId)
    { 
        return new Contract(contractId, authorIds, bookId); 
    }
    private Contract(Guid contractId, List<Guid> authorIds, Guid bookId)
    {
        ContractId = contractId;
        AuthorIds = authorIds;
        BookId = bookId;
    }

    public Guid ContractId { get; private set; }
    public List<Guid> AuthorIds { get; private set; }
    public Guid BookId { get; private   set; }

    public void AddAuthor(Guid authorId) 
    { 
       //tbd: ensure this triggers efcore to update the contract
        if (!AuthorIds.Contains(authorId))
        { 
            AuthorIds.Add(authorId);
        }
    }
}
