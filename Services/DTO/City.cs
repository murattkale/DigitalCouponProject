using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public partial class City : BaseModel
{
    public City()
    {
        Partner = new HashSet<Partner>();
        User = new HashSet<User>();
    }
    [Required]
    public int CountryId { get; set; }
    public Country Country { get; set; }

    [Required]
    public string Name { get; set; }

    public string Code { get; set; }

    public virtual ICollection<Partner> Partner { get; set; }
    public virtual ICollection<User> User { get; set; }





}
