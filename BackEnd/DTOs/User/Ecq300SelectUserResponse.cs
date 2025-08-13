using BackEnd.Controllers;

namespace BackEnd.Controllers.V1.User;

public class Ecq300SelectUserResponse : AbstractApiResponse<Ecq300SelectUserEntity>
{
    public override Ecq300SelectUserEntity Response { get; set; }
}

public class Ecq300SelectUserEntity
{
    public string AvartarUrl { get; set; }
    
    public string Email { get; set; }

    public string BirthDate { get; set; }

    public short? Gender { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string UserType { get; set; }
}