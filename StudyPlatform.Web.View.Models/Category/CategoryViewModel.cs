﻿using StudyPlatform.Web.View.Models.Category.Interfaces;
using StudyPlatform.Web.View.Models.Course;
using System.ComponentModel.DataAnnotations;
using static StudyPlatform.Common.ModelValidationConstants.Category;
namespace StudyPlatform.Web.View.Models.Category
{
    public class CategoryViewModel : IValidatableObject, ICategoryLink
    {
        public CategoryViewModel()
        {
            this.Courses = new List<CourseViewModel>();
        }  
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; } = null!;

        public ICollection<CourseViewModel> Courses { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name)) { yield return new ValidationResult("Name cannot be empty!"); }
        }
    }
}
