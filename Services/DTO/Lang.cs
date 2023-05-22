using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public partial class Lang : BaseModel
{
    public Lang()
    {
        //LangDisplay = new HashSet<LangDisplay>();
    }
    //public virtual ICollection<LangDisplay> LangDisplay { get; set; }


    [Required]
    public string Name { get; set; }

    public string Code { get; set; }

    public bool IsDefault { get; set; }


    [DataType("SingleDocument")]
    public string Logo { get; set; }





}
