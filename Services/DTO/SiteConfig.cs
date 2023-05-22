using System;

public partial class SiteConfig : BaseModel
{

    public string Title { get; set; }
    public string StartPage { get; set; }
    public string StartAction { get; set; }
    public string version { get; set; }
    public string layoutId { get; set; }
    public string BaseUrl { get; set; }
    public string layoutUrlBase { get; set; }
    public string layoutUrl { get; set; }
    public string Logo { get; set; }
    public string JokerPass { get; set; }
    public string Copyright { get; set; }
    public string Map { get; set; }
    public string DefaultImage { get; set; }
    public string ImageUrl { get; set; }
    public string MetaKeywords { get; set; }
    public string MetaDescription { get; set; }
    public string Adress { get; set; }
    public string Phone { get; set; }
    public string Mail { get; set; }
    public string MailGorunenAd { get; set; }
    public string SmtpHost { get; set; }
    public string SmtpPort { get; set; }
    public string SmtpMail { get; set; }
    public string SmtpMailPass { get; set; }
    public bool? SmtpSsl { get; set; }
    public int? MaxDocumentSize { get; set; }
    public int? MaxImageSize { get; set; }
    public string HeadScript { get; set; }
    public string HeadStyle { get; set; }
    public string BodyScript { get; set; }
    public string FooterScript { get; set; }
    public string FooterStyle { get; set; }


    public DateTime? MaintenanceStartDate { get; set; }
    public DateTime? MaintenanceEndDate { get; set; }
    public bool? IsMaintenance { get; set; }

    public string Android_Min { get; set; }
    public string Android_Current { get; set; }

    public string Ios_Min { get; set; }

    public string Ios_Current { get; set; }








}
