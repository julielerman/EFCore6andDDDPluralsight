

using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContractBC.ValueObjects
{
    public class SpecificationSet
    {
       public static SpecificationSet Default() { 
            return GetDefault();
        }

        [JsonConstructor]
        public SpecificationSet(int advanceAmountUSD, int hardCoverRoyaltyPct,
            int softCoverRoyaltyPct, int digitalRoyaltyPct,
            int translationRoyaltyUSD, bool publicityProvided,
            bool authorAvailableForPR, int promoCopiesForAuthor,
            decimal priceForAddlAuthorCopiesUSD)
        {
            AdvanceAmountUSD = advanceAmountUSD;
            HardCoverRoyaltyPct = hardCoverRoyaltyPct;
            SoftCoverRoyaltyPct = softCoverRoyaltyPct;
            DigitalRoyaltyPct = digitalRoyaltyPct;
            TranslationRoyaltyUSD = translationRoyaltyUSD;
            PublicityProvided = publicityProvided;
            AuthorAvailableForPR = authorAvailableForPR;
            PromoCopiesForAuthor = promoCopiesForAuthor;
            PriceForAddlAuthorCopiesUSD = priceForAddlAuthorCopiesUSD;
        }

        public int AdvanceAmountUSD { get; private set; }
        public int HardCoverRoyaltyPct { get; private set; }
        public int SoftCoverRoyaltyPct { get; private set; }
        public int DigitalRoyaltyPct { get; private set; }
        public int TranslationRoyaltyUSD { get; private set; }
        public bool PublicityProvided { get; private set; }
        public bool AuthorAvailableForPR { get; private set; }
        public int PromoCopiesForAuthor { get; private set; }
        public decimal PriceForAddlAuthorCopiesUSD { get; private set; }

        public override bool Equals(object? obj)
        {
            return obj is SpecificationSet set &&
                   AdvanceAmountUSD == set.AdvanceAmountUSD &&
                   HardCoverRoyaltyPct == set.HardCoverRoyaltyPct &&
                   SoftCoverRoyaltyPct == set.SoftCoverRoyaltyPct &&
                   DigitalRoyaltyPct == set.DigitalRoyaltyPct &&
                   TranslationRoyaltyUSD == set.TranslationRoyaltyUSD &&
                   PublicityProvided == set.PublicityProvided &&
                   AuthorAvailableForPR == set.AuthorAvailableForPR &&
                   PromoCopiesForAuthor == set.PromoCopiesForAuthor &&
                   PriceForAddlAuthorCopiesUSD == set.PriceForAddlAuthorCopiesUSD;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(AdvanceAmountUSD);
            hash.Add(HardCoverRoyaltyPct);
            hash.Add(SoftCoverRoyaltyPct);
            hash.Add(DigitalRoyaltyPct);
            hash.Add(TranslationRoyaltyUSD);
            hash.Add(PublicityProvided);
            hash.Add(AuthorAvailableForPR);
            hash.Add(PromoCopiesForAuthor);
            hash.Add(PriceForAddlAuthorCopiesUSD);
            return hash.ToHashCode();
        }
        private static SpecificationSet GetDefault()
        { //read from json file

            string fileName = "DefaultSpecificationSet.json";
            string jsonString = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<SpecificationSet>(jsonString)!;

        }

        public SpecificationSet Copy()
        { 
            return new SpecificationSet(

         AdvanceAmountUSD,
         HardCoverRoyaltyPct,
         SoftCoverRoyaltyPct,
         DigitalRoyaltyPct,
         TranslationRoyaltyUSD,
         PublicityProvided,
         AuthorAvailableForPR,
         PromoCopiesForAuthor,
         PriceForAddlAuthorCopiesUSD);

        }
    }
}
