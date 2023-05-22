using System.ComponentModel.DataAnnotations;

public partial class UserCard : BaseModel
{
    public UserCard()
    {
    }

    public int? UserId { get; set; }
    public virtual User User { get; set; }



    [Required]
    public int MemberShipId { get; set; }
    public virtual MemberShip MemberShip { get; set; }


    [Required]
    public string QrCode { get; set; }







}
