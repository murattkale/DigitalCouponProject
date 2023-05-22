using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class TextValue
{
    public object Id { get; set; }
    public object Name { get; set; }
    public object text { get; set; }
    public object value { get; set; }
    public object dataid { get; set; }
}

public class TextValueId
{
    public int? id { get; set; }
    public string text { get; set; }
    public decimal value { get; set; }
}

public class ParentChild
{
    public int Id { get; set; }
    public int? ParentId { get; set; }

}


public class uploadModel
{
    public IEnumerable<IFormFile> files { get; set; }
    public int Id { get; set; }
    public string type { get; set; }
}



public class whereCaseModel
{

    public string name { get; set; }
    public string whereCase { get; set; }
}

public class LocationModel
{
    public int Id { get; set; }
    public string Location { get; set; }
}



public class EnumModel
{

    public string name { get; set; }
    public string value { get; set; }
    public string value2 { get; set; }
    public string text { get; set; }
    public bool selected { get; set; }
}

public class SelectTextValue
{

    public string value { get; set; }
    public string label { get; set; }

}




public class SubjectModel
{

    public int Yes { get; set; }
    public int No { get; set; }
    public int NotAvailable { get; set; }
    public int Total { get; set; }
    public int TotalSubject { get; set; }
    public int TotalRequest { get; set; }
    public string TotalRating { get; set; }
}


