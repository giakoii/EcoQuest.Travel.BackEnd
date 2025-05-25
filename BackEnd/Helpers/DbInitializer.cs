using BackEnd.Models;
using BackEnd.Utils.Const;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.Helpers;

public static class DbInitializer
{
    public static async Task SeedRoles(RoleManager<Role> roleManager)
    {
        string[] roleNames = { ConstantEnum.UserRole.Customer.ToString(), 
            ConstantEnum.UserRole.Owner.ToString(), 
            ConstantEnum.UserRole.SaleEmployee.ToString(),
            ConstantEnum.UserRole.PlannedCustomer.ToString()
        };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                });
            }
        }
    }
}