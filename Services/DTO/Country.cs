using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public partial class Country : BaseModel
{
    public Country()
    {
        City = new HashSet<City>();
        Partner = new HashSet<Partner>();
        User = new HashSet<User>();

    }

    [Required]
    public string Name { get; set; }

    public string Code { get; set; }

    public virtual ICollection<City> City { get; set; }
    public virtual ICollection<Partner> Partner { get; set; }
    public virtual ICollection<User> User { get; set; }




}
