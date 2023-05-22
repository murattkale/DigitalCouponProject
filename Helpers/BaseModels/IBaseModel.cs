using System;


public interface IBaseModel
{
    int Id { get; set; }
    DateTime CreaDate { get; set; }
    int CreaUser { get; set; }
    int? ModUser { get; set; }
    DateTime? ModDate { get; set; }
    int? OrderNo { get; set; }
    DateTime? IsDeleted { get; set; }

    //bool? IsActive { get; set; }
    //int? IsStatus { get; set; }
    string Joker { get; set; }
    int? LanguageId { get; set; }
}

