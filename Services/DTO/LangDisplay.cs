using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


public partial class LangDisplay : BaseModel
{
    public LangDisplay()
    {
        LangText = new HashSet<LangText>();

    }

    //[DisplayName("Language")]
    //public int LangId { get; set; }
    //public virtual Lang Lang { get; set; }


    [DisplayName("Paramater")]
    [Required]
    public string ParamName { get; set; }

    public virtual ICollection<LangText> LangText { get; set; }


    //[DisplayName("Value")]

    //public string LangValue { get; set; }


    //[DisplayName("Description")]
    //public string LangDesc { get; set; }

}

