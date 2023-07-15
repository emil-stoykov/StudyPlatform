﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StudyPlatform.Services.Course.Interfaces;
using StudyPlatform.Services.Lesson.Interfaces;
using StudyPlatform.Web.View.Models.Lesson;

namespace StudyPlatform.Controllers
{
    public class LessonController : Controller
    {
        private readonly ILessonViewService _lessonViewService;
        private readonly ILessonFormService _lessonFormService;
        private readonly ICourseViewService _courseViewService;

        public LessonController(
            ILessonViewService lessonViewService,
            ILessonFormService lessonFormService,
            ICourseViewService courseViewService)
        {
            this._lessonFormService = lessonFormService;
            this._lessonViewService = lessonViewService;
            this._courseViewService = courseViewService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetLesson(int id)
        {
            LessonViewModel lessonsModel = await this._lessonViewService.GetLessonByIdAsync(id);

            return View(lessonsModel);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            bool lessonExists = await this._lessonViewService.AnyByIdAsync(id);
            if (!lessonExists)
            {
                return RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            int courseId = await this._lessonViewService.GetCourseIdByLessonId(id);

            await this._lessonFormService.RemoveAsync(id);

            return RedirectToAction("GetCourse", "Course", new {id = courseId});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            bool lessonExists = await this._lessonViewService.AnyByIdAsync(id);
            if (!lessonExists)
            {
                return RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            LessonViewFormModel model = await this._lessonFormService.GetLessonFormByIdAsync(id);
            model.Courses = await this._courseViewService.GetAllAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LessonViewFormModel model)
        {
            bool lessonExists = await this._lessonViewService.AnyByIdAsync(model.Id);
            if (!lessonExists)
            {
                return RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            if (!ModelState.IsValid)
            {
                LessonViewFormModel getModel = await this._lessonFormService.GetLessonFormByIdAsync(model.Id);
                getModel.Courses = await this._courseViewService.GetAllAsync();
                return View(getModel);
            }

            await this._lessonFormService.EditAsync(model);

            return RedirectToAction("GetLesson", "Lesson", new { id = model.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Add(int id)
        {
            LessonViewFormModel model = new LessonViewFormModel()
            {
                CourseId = id,
                Courses = await this._courseViewService.GetAllAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(LessonViewFormModel model)
        {
            bool lessonExists = await this._lessonViewService.AnyByIdAsync(model.Id);
            if (lessonExists)
            {
                ModelState.AddModelError(nameof(model.Id), "Lesson by this id already exists.");
            }

            bool existsByName = await this._lessonViewService.AnyByNameAsync(model.Name);
            if (existsByName)
            {
                ModelState.AddModelError(nameof(model.Name), "Lesson by this name already exists.");
            }

            if (!ModelState.IsValid)
            {
                LessonViewFormModel getModel = new LessonViewFormModel()
                {
                    Courses = await this._courseViewService.GetAllAsync()
                };
                return View(getModel);
            }

            await this._lessonFormService.AddAsync(model);

            int courseId = await this._lessonViewService.GetCourseIdByLessonId(model.CourseId);

            return RedirectToAction("GetCourse", "Course", new { id = courseId });
        }
    }
}