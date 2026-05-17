using DAL.EF;
using DAL.EF.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository
    {
        BookShopDBContext db;

        public UserRepository(BookShopDBContext db)
        {
            this.db = db;
        }

        public User? GetUserByEmail(string email)
        {
            return db.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetUserById(int id)
        {
            return db.Users.FirstOrDefault(u => u.UserId == id);
        }

        public void AddUser(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            db.Users.Update(user);
            db.SaveChanges();
        }

        public string GetRoleNameByRoleId(int roleId)
        {
            var role = db.Roles.FirstOrDefault(r => r.RoleId == roleId);

            if (role == null)
            {
                return "";
            }

            return role.RoleName;
        }
    }
}
