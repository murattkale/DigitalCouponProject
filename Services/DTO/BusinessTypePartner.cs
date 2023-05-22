using System.ComponentModel.DataAnnotations;

public partial class BusinessTypePartner : BaseModel
{
    public BusinessTypePartner()
    {
    }

    [Required]
    public int BusinessTypeId { get; set; }
    public virtual BusinessType BusinessType { get; set; }

    [Required]
    public int PartnerId { get; set; }
    public virtual Partner Partner { get; set; }



}
