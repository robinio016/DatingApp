using System.Collections.Generic;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManger;
        private readonly RoleManager<Role> _roleManager;
        // private readonly DataContext _context;
        // public Seed(DataContext context)
        // {
        //     _context = context;
        // }
        
        public Seed(UserManager<User> userManger, RoleManager<Role> role)
        {
            _roleManager= role;
            _userManger = userManger;
        }
        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            var roles = new List<Role>
            {
                new Role{Name = "Member"},
                new Role{Name = "Admin"},
                new Role{Name = "Moderator"},
                new Role{Name = "VIP"},
            };
            foreach(var role in roles)
            {
                _roleManager.CreateAsync(role).Wait();
            }

            foreach (var user in users)
            {
                // byte[] passwordHash, passwordSalt;
                // CreatePasswordHash("password",out passwordHash, out passwordSalt);
                // user.PasswordHash = passwordHash;
                // user.PasswordSalt = passwordSalt;
                //user.UserName = user.UserName.ToLower();

                //_context.Users.Add(user);
                _userManger.CreateAsync(user, "password").Wait();
                _userManger.AddToRoleAsync(user,"Member").Wait();
            }

            //_context.SaveChanges();
            var adminUser = new User
            {
                UserName = "Admin"
            };

            IdentityResult result = _userManger.CreateAsync(adminUser,"password").Result;

            if(result.Succeeded)
            {
                var admin = _userManger.FindByNameAsync("Admin").Result;
                _userManger.AddToRolesAsync(admin, new [] {"Admin","Moderator"}).Wait();
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }

}