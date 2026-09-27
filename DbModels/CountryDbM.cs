using System.ComponentModel.DataAnnotations;
using Models;
using Seido.Utilities.SeedGenerator;

namespace DbModels;

public sealed class CountryDbM : Country, ISeed<CountryDbM>, IEquatable<CountryDbM>
{
    [Key]
    public override Guid CountryId { get; set; }

    public List<CityDbM> Cities { get; set; } = new();

    #region constructors
    public CountryDbM()
        : base() { }

    public CountryDbM(CountryDbM org)
        : base(org) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(CountryDbM other) =>
        (other != null) && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as CountryDbM);

    public override int GetHashCode() => Name?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion

    #region randomly seed this instance
    // Hides the base Country.Seed(SeedGenerator) so this satisfies ISeed<CountryDbM> exactly
    // (the interface requires the return type to match TItem precisely) - needed for
    // UniqueItemsToList<CountryDbM>(...) to work.
    public new CountryDbM Seed(SeedGenerator seedGenerator)
    {
        base.Seed(seedGenerator);
        return this;
    }
    #endregion
}
