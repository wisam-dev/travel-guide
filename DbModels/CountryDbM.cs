using System.ComponentModel.DataAnnotations;
using Models;

namespace DbModels;

sealed public class CountryDbM : Country, IEquatable<CountryDbM>
{
    [Key]
    public override Guid CountryId { get; set; }

    public List<CityDbM> Cities { get; set; } = new();

    #region constructors
    public CountryDbM() : base() { }
    public CountryDbM(string name) : base(name) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(CountryDbM other) => (other != null) && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower());
    public override bool Equals(object obj) => Equals(obj as CountryDbM);
    public override int GetHashCode() => Name?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion
}
