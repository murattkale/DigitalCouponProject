using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class PartnerDocument : BaseModel
{
    public PartnerDocument()
    {


    }

    public string Name { get; set; }

    [Required]
    public string Link { get; set; }
    public string Desc { get; set; }

    [Required]
    public DocType DocType { get; set; }
    public string FileSize { get; set; }

    [NotMapped]
    public string Camera { get; set; }

    [Required]
    public int PartnerId { get; set; }
    public virtual Partner Partner { get; set; }





}
public enum DocType : int
{
    [Description("Image")]
    Image = 1,
    [Description("Document")]
    Document = 2,
}



