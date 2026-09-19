namespace Models.Dto;

// Maps directly onto the vwInfoDb view (see MainDbContext.OnModelCreating: HasNoKey().ToView(...)).
// Doubles as the API response shape - no separate mapping needed for a flat read-only summary.
public class DbInfoDto
{
    public int NrUsers { get; set; }
    public int NrSeededUsers { get; set; }
    public int NrUnseededUsers { get; set; }

    public int NrCities { get; set; }
    public int NrSeededCities { get; set; }
    public int NrUnseededCities { get; set; }

    public int NrAttractions { get; set; }
    public int NrSeededAttractions { get; set; }
    public int NrUnseededAttractions { get; set; }

    public int NrAttractionsWithComments { get; set; }

    public int NrComments { get; set; }
    public int NrSeededComments { get; set; }
    public int NrUnseededComments { get; set; }

    public int NrAddresses { get; set; }
    public int NrSeededAddresses { get; set; }
    public int NrUnseededAddresses { get; set; }
}
