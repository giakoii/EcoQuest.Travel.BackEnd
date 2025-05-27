using BackEnd.Controllers;

namespace BackEnd.Controllers.V1.User;

public class SelectUserResponse : AbstractApiResponse<SelectUserEntity>
{
    public override SelectUserEntity Response { get; set; }
}

public class SelectUserEntity
{
    public string AvartarUrl { get; set; }
    
    public string Email { get; set; }

    public string BirthDate { get; set; }

    public short? Gender { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
}