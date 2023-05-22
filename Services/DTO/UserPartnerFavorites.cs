using System.ComponentModel.DataAnnotations;

public partial class UserPartnerFavorites : BaseModel
{
    public UserPartnerFavorites()
    {
    }

    [Required]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    [Required]
    public int PartnerId { get; set; }
    public virtual Partner Partner { get; set; }



}
