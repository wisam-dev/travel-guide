using System.Text.RegularExpressions;

namespace Models.Dto;

// Cu(Create/update) DTOs: simplistic, fully instantiable subsets of the DbModels, used only to
// carry create/update payloads in and out of the controllers.

public class UserCuDto
{
    public virtual Guid? UserId { get; set; }
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }

    public UserCuDto() { }

    public UserCuDto(IUser org)
    {
        UserId = org.UserId;
        Name = org.Name;
        Email = org.Email;
    }

    public void EnsureValidity()
    {
        if (!string.IsNullOrEmpty(Name) && !Regex.IsMatch(Name, @"^[a-zA-Z0-9\s]*$"))
            throw new ArgumentException(
                "Name can only contain letters (a-z), numbers (0-9), and spaces."
            );
        if (!string.IsNullOrEmpty(Email) && !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Email has to be a valid email address.");
    }
}

public class AddressCuDto
{
    public virtual Guid? AddressId { get; set; }

    public virtual string Street { get; set; }
    public virtual int ZipCode { get; set; }
    public virtual Guid CityId { get; set; }

    public AddressCuDto() { }

    public AddressCuDto(IAddress org)
    {
        AddressId = org.AddressId;
        Street = org.Street;
        ZipCode = org.ZipCode;
        CityId = org.CityId;
    }

    public void EnsureValidity()
    {
        if (!string.IsNullOrEmpty(Street) && !Regex.IsMatch(Street, @"^[a-zA-Z0-9\s]*$"))
            throw new ArgumentException(
                "Street can only contain letters (a-z), numbers (0-9), and spaces."
            );
        if (ZipCode <= 0)
            throw new ArgumentException("ZipCode has to be larger than zero");
        if (CityId == Guid.Empty)
            throw new ArgumentException("CityId must be set");
    }
}

// Covers task 10's "add a sight" / "change a sight's category, country, city, title, description".
// AddressId points at an already-existing Address (create one first via AddressesController if
// needed) - changing a sight's country/city means repointing AddressId at a different address,
// not editing the address in place.
public class AttractionCuDto
{
    public virtual Guid? AttractionId { get; set; }

    public virtual Guid CategoryId { get; set; }
    public virtual Guid AddressId { get; set; }

    public virtual string Title { get; set; }
    public virtual string Description { get; set; }

    public AttractionCuDto() { }

    public void EnsureValidity()
    {
        if (CategoryId == Guid.Empty)
            throw new ArgumentException("CategoryId must be set");
        if (AddressId == Guid.Empty)
            throw new ArgumentException("AddressId must be set");
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title must be set");
    }
}

// Covers task 10's "add a comment, linked to a user and a sight". No update per tasks 10/11 -
// comments are only ever created or deleted.
public class CommentCuDto
{
    public virtual Guid? CommentId { get; set; }

    public virtual Guid UserId { get; set; }
    public virtual Guid AttractionId { get; set; }
    public virtual string Text { get; set; }

    public CommentCuDto() { }

    public void EnsureValidity()
    {
        if (UserId == Guid.Empty)
            throw new ArgumentException("UserId must be set");
        if (AttractionId == Guid.Empty)
            throw new ArgumentException("AttractionId must be set");
        if (string.IsNullOrWhiteSpace(Text))
            throw new ArgumentException("Text must be set");
        // Comment text is free-form (unlike names/titles), so no character-set restriction here -
        // just require it to be non-empty and within the varchar(1000) column limit.
        if (Text.Length > 1000)
            throw new ArgumentException("Text must be 1000 characters or fewer");
    }
}
