using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

public partial class User : BaseModel
{
    public User()
    {
        UserMemberShip = new HashSet<UserMemberShip>();
        UserCoupon = new HashSet<UserCoupon>();
        UserPartnerFavorites = new HashSet<UserPartnerFavorites>();
        UserCard = new HashSet<UserCard>();

    }

    [Description("Status")]
    public IsState IsState { get; set; }

    [NotMapped]
    public string IsStateName { get { return IsState.ExGetDescription(); } }

    [NotMapped]
    public List<EnumModel> IsStateList = Enum.GetValues(typeof(IsState)).Cast<int>().Select(x => new EnumModel { name = ((IsState)x).ToStr(), value = x.ToString(), text = ((IsState)x).ExGetDescription() }).ToList();


    [Required]
    public string Name { get; set; }


    public string Pass { get; set; }


    public string UserName { get; set; }

    public string Phone { get; set; }
    public string Mail { get; set; }

    public string AgeGroup { get; set; }



    public Gender Gender { get; set; }
    [NotMapped]
    public string GenderName { get { return Gender.ExGetDescription(); } }

    [NotMapped]
    public List<EnumModel> GenderList = Enum.GetValues(typeof(Gender)).Cast<int>().Select(x => new EnumModel { name = ((Gender)x).ToStr(), value = x.ToString(), text = ((Gender)x).ExGetDescription() }).ToList();



    public DeviceState DeviceState { get; set; }
    [NotMapped]
    public string DeviceStateName { get { return DeviceState.ExGetDescription(); } }

    [NotMapped]
    public List<EnumModel> DeviceStateList = Enum.GetValues(typeof(DeviceState)).Cast<int>().Select(x => new EnumModel { name = ((DeviceState)x).ToStr(), value = x.ToString(), text = ((DeviceState)x).ExGetDescription() }).ToList();




    public int? CountryId { get; set; }
    public virtual Country Country { get; set; }

    public string TCKN { get; set; }


    public int? CityId { get; set; }
    public virtual City City { get; set; }

    public int? PartnerId { get; set; }
    public virtual Partner Partner { get; set; }

    public string Adress { get; set; }
    public string Platform { get; set; }


    [NotMapped]
    public new List<string> UserMemberShipNames
    {
        get
        {
            var variable = UserMemberShip.Count > 0 ? UserMemberShip?.Select(o => o.MemberShip?.Name)?.ToList() : new List<string>();
            return variable;
        }
    }




    public virtual ICollection<UserMemberShip> UserMemberShip { get; set; }
    public virtual ICollection<UserCoupon> UserCoupon { get; set; }
    public virtual ICollection<UserPartnerFavorites> UserPartnerFavorites { get; set; }
    public virtual ICollection<UserCard> UserCard { get; set; }


}

public enum Gender
{
    [Description("Male")]
    Male = 1,
    [Description("Female")]
    Famale = 2,
    [Description("Other")]
    Other = 3
}



public enum DeviceState
{
    [Description("Android")]
    Android = 1,
    [Description("Ios")]
    Ios = 2,
    [Description("Web")]
    Web = 3
}