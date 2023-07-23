﻿using Microsoft.AspNetCore.Mvc;
using StudyPlatform.Services.Category.Interfaces;
using StudyPlatform.Services.Course.Interfaces;
using StudyPlatform.Web.View.Models.Course;
using Microsoft.AspNetCore.Authorization;
using StudyPlatform.Services.Users.Interfaces;
using StudyPlatform.Infrastructure;
using static StudyPlatform.Common.GeneralConstants;
using StudyPlatform.Infrastructure.Infrastructure;

namespace StudyPlatform.Controllers
{
    [Authorize]
    // a method for displaying and accessing courses
    public class CourseController : Controller
    {
        private readonly ICourseViewService _courseViewService;
        private readonly ICategoryViewService _categoryViewService;
        private readonly ICourseViewFormService _courseViewFormService;

        public CourseController(
            ICourseViewService courseViewService,
            ICategoryViewService categoryViewService,
            ICourseViewFormService courseViewFormService)
        {
            this._courseViewService = courseViewService;
            this._categoryViewService = categoryViewService;
            this._courseViewFormService = courseViewFormService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourse(int id, string courseName)
        {
            var course = await this._courseViewService.GetById(id);
            if (course == null) {
                return RedirectToAction("Error", "Home");
            }

            if(courseName != course.GetNameUrl())
            {
                return BadRequest();
            }

            return View(course);
        }

        [HttpGet]
        [Authorize(Roles = $"{TeacherRoleName},{AdministratorRoleName}")]
        public async Task<IActionResult> Edit(int courseId)
        {
            if (!await this._courseViewService.AnyByIdAsync(courseId))
            {
                return BadRequest();
            }

            CourseViewFormModel model = await this._courseViewService.GetFormCourseAsync(courseId);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = $"{TeacherRoleName},{AdministratorRoleName}")]
        public async Task<IActionResult> Edit(CourseViewFormModel model)
        {
            if (!ModelState.IsValid){
                return View();
            }

            await this._courseViewFormService.EditAsync(model);

            return RedirectToAction("GetCourse", new { id = model.Id, courseName = model.GetNameUrl() });
        }

        [HttpGet]
        [Authorize(Roles = $"{TeacherRoleName},{AdministratorRoleName}")]
        public async Task<IActionResult> Remove(int id)
        {
            await this._courseViewFormService.RemoveAsync(id);
            // TODO: Return to the category of the deleted course
            return RedirectToAction("All", "Category");
        }

        [HttpGet]
        [Authorize(Roles = $"{TeacherRoleName},{AdministratorRoleName}")]
        public async Task<IActionResult> Add(int categoryId)
        {
            if (!await this._categoryViewService.AnyByIdAsync(categoryId))
            {
                return BadRequest();
            }

            CourseViewFormModel model = new CourseViewFormModel() {
                Categories = await this._categoryViewService.GetAllCategoriesAsync(),
                CategoryId = categoryId
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = $"{TeacherRoleName},{AdministratorRoleName}")]
        public async Task<IActionResult> Add(CourseViewFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            await this._courseViewFormService.AddAsync(model);
            int courseId = await this._courseViewService.GetIdByNameAsync(model.Name);
            return RedirectToAction("GetCourse", new { id = courseId, courseName = model.GetNameUrl() });
        }
    }
}
