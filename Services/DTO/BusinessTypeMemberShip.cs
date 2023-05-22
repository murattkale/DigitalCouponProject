using System.ComponentModel.DataAnnotations;

public partial class BusinessTypeMemberShip : BaseModel
{
    public BusinessTypeMemberShip()
    {
    }

    [Required]
    public int BusinessTypeId { get; set; }
    public virtual BusinessType BusinessType { get; set; }

    [Required]
    public int MemberShipId { get; set; }
    public virtual MemberShip MemberShip { get; set; }



}
