using BackEnd.Controllers;
using BackEnd.Controllers.V1.User;
using BackEnd.Logics;
using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.SystemClient;
using BackEnd.Utils;
using BackEnd.Utils.Const;
using Microsoft.EntityFrameworkCore;
using SystemConfig = BackEnd.Models.SystemConfig;

namespace BackEnd.Services;

public class UserService : IUserService
{
    private readonly IBaseRepository<User, Guid> _userRepository;
    private readonly IBaseRepository<Account, Guid> _accountRepository;
    private readonly IBaseRepository<Role, Guid> _roleRepository;
    private readonly IBaseRepository<SystemConfig, string> _systemConfigRepository;
    private readonly IEmailTemplateRepository _emailTemplateRepository;
    private readonly CloudinaryLogic _cloudinary;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userRepository"></param>
    /// <param name="accountRepository"></param>
    /// <param name="roleRepository"></param>
    /// <param name="systemConfigRepository"></param>
    /// <param name="cloudinary"></param>
    /// <param name="emailTemplateRepository"></param>
    public UserService(IBaseRepository<User, Guid> userRepository, IBaseRepository<Account, Guid> accountRepository, 
        IBaseRepository<Role, Guid> roleRepository, IBaseRepository<SystemConfig, string> systemConfigRepository, CloudinaryLogic cloudinary, 
        IEmailTemplateRepository emailTemplateRepository)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _roleRepository = roleRepository;
        _systemConfigRepository = systemConfigRepository;
        _cloudinary = cloudinary;
        _emailTemplateRepository = emailTemplateRepository;
    }

    /// <summary>
    /// Insert a new user into the system
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<InsertUserResponse> InsertUser(InsertUserRequest request)
    {
        var response = new InsertUserResponse {Success = false};
        
        // Check if account already exists
        var existingUser = await _accountRepository.Find(x => x.Email == request.Email).FirstOrDefaultAsync();
        if (existingUser != null)
        {
            response.SetMessage(MessageId.E11004);
            return response;
        }
        
        // Check if role is not exists
        var role = await _roleRepository.Find(x => x.Name == ConstantEnum.UserRole.Customer.ToString()).FirstOrDefaultAsync();
        if (role == null)
        {
            response.SetMessage(MessageId.E11003);
            return response;
        }
        
        // Create key
        var key = $"{request.Email}";
        key = CommonLogic.EncryptText(key, _systemConfigRepository);
        
        // Begin transaction
        await _userRepository.ExecuteInTransactionAsync(async () =>
        {
            // Insert new account
            var newAccount = new Account
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
                LockoutEnd = null,
                EmailConfirmed = false,
                Key = key,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString(),
                RoleId = role.Id,
            };
            await _accountRepository.AddAsync(newAccount);
            await _accountRepository.SaveChangesAsync(newAccount.Email);
            
            // Insert new user
            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                AuthId = newAccount.AccountId,
            };
            
            // Save changes to the database
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync(newAccount.Email);
            
            // Send email verification
            var detailErrorList = new List<DetailError>();
            await UserInsertSendMail.SendMailVerifyInformation(_emailTemplateRepository, newAccount.Email, key, detailErrorList);
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        
        return response;
    }

    /// <summary>
    /// Verify user after registration
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<InsertUserVerifyResponse> InsertUserVerify(InsertUserVerifyRequest request)
    {
        var response = new InsertUserVerifyResponse() { Success = false };
        
        // Decrypt
        var keyDecrypt = CommonLogic.DecryptText(request.Key, _systemConfigRepository);
        string[] values = keyDecrypt.Split(",");
        string emailDecrypt = values[0];
        
        // Check user exists
        var userExist = await _accountRepository.Find(x => x.Email == emailDecrypt && 
                                                        x.EmailConfirmed == false &&
                                                        x.IsActive == true &&
                                                        x.Key == request.Key).FirstOrDefaultAsync();
        if (userExist == null)
        {
            response.SetMessage(MessageId.E11001);
            return response;
        }

        // Begin transaction
        await _userRepository.ExecuteInTransactionAsync(async () =>
        {
            // Update information user
            userExist.Key = null;
            userExist.LockoutEnd = null;
            userExist.IsActive = true;
            userExist.EmailConfirmed = true;

            _accountRepository.Update(userExist);
            await _accountRepository.SaveChangesAsync(userExist.Email!);
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, IdentityEntity identityEntity)
    {
        var response = new UpdateUserResponse() { Success = false };
        
        // Get account
        var account = await _accountRepository.Find(x => x.Email == identityEntity.Email).FirstOrDefaultAsync();
        
        // Get user profile
        var user = await _userRepository.Find(x => x.UserId == account!.AccountId).FirstOrDefaultAsync();
        if (user == null)
        {
            response.SetMessage(MessageId.E11001);
            return response;
        }
        
        // Update user profile
        await _userRepository.ExecuteInTransactionAsync(async () =>
        {
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Gender = request.Gender;
            user.DateOfBirth = request.BirthDate;
            user.AvatarUrl = await _cloudinary.UploadImageAsync(request.ImageUrl!);
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync(identityEntity.Email);
            
            // True
            response.Success = true;
            response.SetMessage(MessageId.I00001);
            return true;
        });
        return response;
    }

    /// <summary>
    /// Select user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="identityEntity"></param>
    /// <returns></returns>
    public Task<SelectUserResponse> SelectUser(SelectUserRequest request, IdentityEntity identityEntity)
    {
        var response = new SelectUserResponse { Success = false };
        
        // Get account
        var account = _accountRepository.Find(x => x.Email == identityEntity.Email).FirstOrDefault();
        if (account == null)
        {
            response.SetMessage(MessageId.E11001);
            return Task.FromResult(response);
        }
        
        // Get user information
        var user = _userRepository.Find(x => x.AuthId == account.AccountId).FirstOrDefault();
        if (user == null)
        {
            response.SetMessage(MessageId.E11001);
            return Task.FromResult(response);
        }
        
        // Set response
        response.Response = new SelectUserEntity
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = StringUtil.ConvertToDateAsDdMmYyyy(user.DateOfBirth),
            Email = account.Email!,
            Gender = user.Gender,
            AvartarUrl = user.AvatarUrl!,
        };
        
        // True
        response.Success = true;
        response.SetMessage(MessageId.I00001);
        return Task.FromResult(response);
    }
}