using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p =>p.Photos).FirstOrDefaultAsync(x =>x.UserName == username);
            if(user == null)
            {
                return null;
            }
            if(!VerifyPasswordHash(password,user.PasswordSalt,user.PasswordHash))
            return null;
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using( var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i =0;i < computedHash.Length;i++)
                {

                    if(computedHash[i] != passwordHash[i])
                    return false;
                }
            }
            return true;
            
        }

        public async Task<User> Register(User user, string passWord)
        {
            byte[] passWordHash, passWordSalt;
            CreatePassWordHash(passWord,out passWordHash,out passWordSalt);
            user.PasswordHash = passWordHash;
            user.PasswordSalt = passWordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private void CreatePassWordHash(string passWord, out byte[] passWordHash, out byte[] passWordSalt)
        {
            using( var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passWordSalt = hmac.Key;
                passWordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passWord));
            }
        
        }

        public async Task<bool> UserExists(string username)
        {
            if( await _context.Users.AnyAsync(x=>x.UserName == username)) 
            return true ;
           else
           {
              return  false;
            }
        }
    }
}