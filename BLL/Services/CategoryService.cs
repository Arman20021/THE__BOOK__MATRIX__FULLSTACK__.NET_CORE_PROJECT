using BLL.DTOs;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CategoryService
    {
        CategoryRepository categoryRepository;

        public CategoryService(CategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public List<CategoryDto> GetActiveCategories()
        {
            var categories = categoryRepository.GetActiveCategories();

            List<CategoryDto> list = new List<CategoryDto>();

            foreach (var c in categories)
            {
                CategoryDto dto = new CategoryDto();

                dto.CategoryId = c.CategoryId;
                dto.CategoryName = c.CategoryName;
                dto.Description = c.Description;
                dto.IsActive = c.IsActive;

                list.Add(dto);
            }

            return list;
        }





        public List<CategoryDto> GetAllCategories()
        {
            var categories = categoryRepository.GetAllCategories();

            List<CategoryDto> categoryDtos = new List<CategoryDto>();

            foreach (var category in categories)
            {
                CategoryDto dto = new CategoryDto();

                dto.CategoryId = category.CategoryId;
                dto.CategoryName = category.CategoryName;

                categoryDtos.Add(dto);
            }

            return categoryDtos;
        }
    }
}
