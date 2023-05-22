using System.ComponentModel.DataAnnotations;

public partial class UserMemberShip : BaseModel
{
    public UserMemberShip()
    {
    }

    [Required]
    public int UserId { get; set; }
    public virtual User User { get; set; }

    [Required]
    public int MemberShipId { get; set; }
    public virtual MemberShip MemberShip { get; set; }



}
