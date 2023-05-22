using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public partial class BusinessType : BaseModel
{
    public BusinessType()
    {
        BusinessTypePartner = new HashSet<BusinessTypePartner>();
        BusinessTypeMemberShip = new HashSet<BusinessTypeMemberShip>();
        Childs = new HashSet<BusinessType>();
        LangText = new HashSet<LangText>();

    }

    //[Description("Parent")]
    public int? ParentId { get; set; }
    public virtual BusinessType Parent { get; set; }

    //[Required]
    public string Name { get; set; }

    public string Code { get; set; }
    public string Desc { get; set; }

    public virtual ICollection<LangText> LangText { get; set; }


    [DataType("SingleDocument")]
    public string IconMain { get; set; }

    [DataType("SingleDocument")]
    public string IconFavorite { get; set; }



    [DataType("SingleDocument")]
    public string IconSilver { get; set; }

    [DataType("SingleDocument")]
    public string IconGold { get; set; }

    [DataType("SingleDocument")]
    public string IconDiomand { get; set; }

    [DataType("SingleDocument")]
    public string IconLocal { get; set; }


    public virtual ICollection<BusinessType> Childs { get; set; }
    public virtual ICollection<BusinessTypePartner> BusinessTypePartner { get; set; }
    public virtual ICollection<BusinessTypeMemberShip> BusinessTypeMemberShip { get; set; }
}
