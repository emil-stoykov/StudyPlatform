﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudyPlatform.Data;
using StudyPlatform.Data.Models;
using StudyPlatform.Services.LearningMaterial.Interfaces;
using StudyPlatform.Services.Users.Interfaces;
using StudyPlatform.Web.View.Models.LearningMaterial;
using StudyPlatform.Web.View.Models.Teacher;
using static StudyPlatform.Common.GeneralConstants;

namespace StudyPlatform.Services.LearningMaterial
{
    public class LearningMaterialService : ILearningMaterialService
    {
        /// <summary>
        /// DataBase injection.
        /// </summary>
        private readonly StudyPlatformDbContext _db;

        public LearningMaterialService(
            StudyPlatformDbContext db)
        {
            this._db = db;
        }

        /// <summary>
        /// Searches learning material through the given id parameter in the DB.
        /// </summary>
        public async Task<bool> AnyByIdAsync(int id)
        {
            bool lmExists 
                = await this._db
                .LearningMaterials
                .AnyAsync(lm => lm.Id.Equals(id));

            return lmExists;
        }

        /// <summary>
        /// Searches learning material through the given name parameter in the DB.
        /// </summary>
        public async Task<bool> AnyByNameAsync(string name)
        {
            bool lmExists
                = await this._db
                .LearningMaterials
                .AnyAsync(lm => lm.LearningMaterialName.Equals(name));

            return lmExists;
        }

        /// <summary>
        /// Returns every learning material model that has the given lessonId foreign key. Not asynchronous.
        /// </summary>
        public ICollection<LearningMaterialViewModel> GetAllLmModelsByLesson(int lessonId)
        {
            ICollection<LearningMaterialViewModel> lms
                = this._db
                .LearningMaterials
                .Where(lm => lm.LessonId.Equals(lessonId))
                .Select(lm => new LearningMaterialViewModel()
                {
                    Id = lm.Id,
                    FileName = lm.FileName,
                    Title = lm.LearningMaterialName,
                    LinkName = lm.FileName.Split(".pdf", StringSplitOptions.RemoveEmptyEntries)[0].ToString(),
                })
                .ToList();

            return lms;
        }

        /// <summary>
        /// Returns every learning material model that has the given lessonId foreign key.
        /// </summary>
        public async Task<ICollection<LearningMaterialViewModel>> GetAllLmModelsByLessonAsync(int lessonId)
        {
            ICollection<LearningMaterialViewModel> lms
                = await this._db
                .LearningMaterials
                .Where(lm => lm.LessonId.Equals(lessonId))
                .Select(lm => new LearningMaterialViewModel()
                {
                    Id = lm.Id,
                    FileName = lm.FileName,
                    Title = lm.LearningMaterialName,
                    LinkName = lm.FileName.Split(".pdf", StringSplitOptions.RemoveEmptyEntries)[0].ToString(),
                })
                .ToListAsync();

            return lms;
        }

        public async Task<int> GetIdByNameAsync(string lmName)
        {
            int lmId
                = await this._db
                .LearningMaterials
                .Where(c => c.LearningMaterialName.Equals(lmName))
                .Select(c => c.Id)
                .FirstAsync();

            return lmId;
        }

        /// <summary>
        /// Returns a model for viewing learning material objects.
        /// </summary>
        public async Task<LearningMaterialViewModel> GetViewModelAsync(int lmId)
        {
            LearningMaterialViewModel lmModel =
                await this._db
                .LearningMaterials
                .Where(lm => lm.Id.Equals(lmId))
                .Select(lm => new LearningMaterialViewModel()
                {
                    Id = lm.Id,
                    FileName = lm.FileName,
                    LinkName = lm.FileName.Split(".pdf", StringSplitOptions.RemoveEmptyEntries)[0].ToString(),
                    FullPath = LmFilePath + lm.FileName,
                    Title = lm.LearningMaterialName,
                    LessonLink = new LearningMaterialLessonLinkModel()
                    {
                        LessonId = lm.LessonId,
                        LessonName = lm.Lesson.Name,
                        LessonNameLink = lm.Lesson.Name.Replace(" ", "-")
                    }
                })
                .FirstOrDefaultAsync();
            
            return lmModel;
        }
    }
}
