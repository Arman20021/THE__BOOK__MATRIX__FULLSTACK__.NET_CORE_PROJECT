using DAL.EF;
using DAL.EF.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class EmailVerificationRepository
    {
        BookShopDBContext db;

        public EmailVerificationRepository(BookShopDBContext db)
        {
            this.db = db;
        }

        public void AddToken(EmailVerificationToken token)
        {
            db.EmailVerificationTokens.Add(token);
            db.SaveChanges();
        }

        public EmailVerificationToken? GetValidToken(string token)
        {
            return db.EmailVerificationTokens.FirstOrDefault(t =>
                t.Token == token &&
                t.IsUsed == false &&
                t.ExpireAt > DateTime.Now);
        }

        public void UpdateToken(EmailVerificationToken token)
        {
            db.EmailVerificationTokens.Update(token);
            db.SaveChanges();
        }
    }
}
