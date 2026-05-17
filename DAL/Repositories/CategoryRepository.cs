using DAL.EF;
using DAL.EF.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class CategoryRepository
    {
        BookShopDBContext db;

        public CategoryRepository(BookShopDBContext db)
        {
            this.db = db;
        }

        public List<Category> GetAllCategories()
        {
            return db.Categories.ToList();
        }

        public List<Category> GetActiveCategories()
        {
            return db.Categories
                .Where(c => c.IsActive == true)
                .ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return db.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public void AddCategory(Category category)
        {
            db.Categories.Add(category);
            db.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            db.Categories.Update(category);
            db.SaveChanges();
        }
    }
}
