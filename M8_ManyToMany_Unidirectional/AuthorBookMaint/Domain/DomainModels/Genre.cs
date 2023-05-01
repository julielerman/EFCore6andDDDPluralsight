namespace AuthorAndBookMaintenance.DomainModels;
/// <summary>
/// This enum is intented for bitwise use. Values must be powers of two
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum
/// interesting take by Steve Smith: https://ardalis.com/enum-alternatives-in-c/
/// </summary>
[Flags]
public enum Genres
{
    None=0,
    Fantasy = 1,
    SciFi = 2,
    Mystery = 4,
    Dystopian = 8,
    Contemporary = 16,
    HistoricalFiction = 32
}
