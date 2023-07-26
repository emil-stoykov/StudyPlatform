﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using StudyPlatform.Controllers;
using StudyPlatform.Data;
using StudyPlatform.Services.Category;
using StudyPlatform.Services.Category.Interfaces;
using StudyPlatform.Web.View.Models.Category;
using StudyPlatform.Web.View.Models.Course;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using static StudyPlatform.Common.CacheConstants;

namespace StudyPlatform.Tests
{
    [TestFixture]
    public class CategoryViewServiceTests
    {
        private CategoryController _categoryController;
        private Mock<CategoryViewService> _categoryViewServiceMock;
        private Mock<CategoryViewFormService> _categoryViewFormServiceMock;
        private Mock<IMemoryCache> _memoryCacheMock;
        private StudyPlatformDbContext dbContext;


        private Mock<AllCategoriesViewModel> _allCategoriesViewModelMock;
        private Mock<CategoryViewModel> _categoryViewModelMock;

        [SetUp]
        public void Setup()
        {
            //Arrange
            ICollection<Data.Models.Category> categoryData = new List<Data.Models.Category>()
            {
                new Data.Models.Category{Id = 1, Name = "In Memory Category 1"},
                new Data.Models.Category{Id = 2, Name = "In Memory Category 2"}
            };

            var dbOptions = new DbContextOptionsBuilder<StudyPlatformDbContext>()
                .UseInMemoryDatabase(databaseName: "StudyPlatformInMemory")
                .Options;

            this.dbContext = new StudyPlatformDbContext(dbOptions);
            this.dbContext.Categories.AddRange(categoryData);
            this.dbContext.SaveChanges();

            _categoryViewServiceMock = new Mock<CategoryViewService>(this.dbContext);
            _categoryViewFormServiceMock = new Mock<CategoryViewFormService>(this.dbContext);
            _memoryCacheMock = new Mock<IMemoryCache>();

            _allCategoriesViewModelMock = new Mock<AllCategoriesViewModel>();
            _categoryViewModelMock = new Mock<CategoryViewModel>();


            //_categoryViewModelMock.Setup(c => c.Id).Returns(1);
            //_categoryViewModelMock.Setup(c => c.Name).Returns("Mock Category");
            //// _categoryViewModelMock.Setup(c => c.Courses)
            //IEnumerable<CategoryViewModel> _categoryViewModelMocks
            //    = new List<CategoryViewModel>() {
            //    _categoryViewModelMock.Object
            //};

            //_allCategoriesViewModelMock.Setup(c => c.IsViewedByTeacher).Returns(true);
            //_allCategoriesViewModelMock.Setup(c => c.Categories).Returns(_categoryViewModelMocks.AsEnumerable());

            //_memoryCacheMock.Setup(c => c.Get(AllCategoriesCacheKey)).Returns(_allCategoriesViewModelMock.Object);

            //_categoryController = new(_categoryViewServiceMock.Object,
            //                          _categoryViewFormServiceMock.Object,
            //                          _memoryCacheMock.Object);

        }

        [Test]
        public async Task CategoryAllService_ShouldReturnById()
        {
            int categoryId = 1;
            // act
            var result = await _categoryViewServiceMock.Object.GetCategoryByIdAsync(categoryId);
            
            // assert
            Assert.AreEqual(categoryId, result.Id, "Didn't return a category view model with the same id.");
        }

        [Test]
        public async Task CategoryAllService_ShouldReturnName()
        {
            string name = "In Memory Category 1";
            int categoryId = 1;
            // act
            string result = await _categoryViewServiceMock.Object.GetNameByIdAsync(categoryId);

            // assert
            Assert.NotNull(result);
            Assert.That(result, Does.Contain(name));
        }

        [Test]
        public async Task CategoryAllService_AnyByIdShouldReturnTrue()
        {
            int categoryId = 1;
            // act
            bool result = await _categoryViewServiceMock.Object.AnyByIdAsync(categoryId);

            // assert
            Assert.True(result);
        }

        [Test]
        public async Task CategoryAllService_AnyByIdShouldReturnFalse()
        {
            int categoryId = -1;
            // act
            bool result = await _categoryViewServiceMock.Object.AnyByIdAsync(categoryId);

            // assert
            Assert.False(result);
        }

        [Test]
        public async Task CategoryAllService_AnyByNameShouldReturnTrue()
        {
            string name = "In Memory Category 1";
            // act
            bool result = await _categoryViewServiceMock.Object.AnyByNameAsync(name);

            // assert
            Assert.True(result);
        }

        [Test]
        public async Task CategoryAllService_AnyByNameShouldReturnFalse()
        {
            string name = "In Memory Category -1";
            // act
            bool result = await _categoryViewServiceMock.Object.AnyByNameAsync(name);

            // assert
            Assert.False(result);
        }

        [Test]
        public async Task CategoryAllService_AllShouldReturnAtLeast2()
        {
            // act
            var result = await _categoryViewServiceMock.Object.GetAllCategoriesAsync();

            // assert
            Assert.NotNull(result);
            Assert.That(result.Count >= 2);
        }

        [Test]
        public async Task CategoryAllService_AllCategoriesShouldHaveCategoriesWithCountOfAtLeast2()
        {
            // act
            var result = await _categoryViewServiceMock.Object.GetCategoriesForAllPageAsync();
            // assert
            Assert.NotNull(result);
            Assert.NotNull(result.Categories);
            Assert.True(result.Categories.Count() >= 2);
        }

        [Test]
        public async Task CategoryAllService_ShouldReturnFormById()
        {
            int categoryId = 1;
            // act
            var result = await _categoryViewServiceMock.Object.GetFormCategory(categoryId);
            // assert
            Assert.NotNull(result, "Form model is not null");
            Assert.That(categoryId == result.Id, "Form model has the same input as the param sent");
        }

        [Test]
        public async Task CategoryAllService_ShouldReturnNameUrlById()
        {
            int categoryId = 1;
            // act
            string result = await _categoryViewServiceMock.Object.GetNameUrlByIdAsync(categoryId);
            // assert
            Assert.NotNull(result);
        }

        [Test]
        public async Task CategoryAllService_NameUrlShouldHaveNoEmptySpaces()
        {
            int categoryId = 1;
            // act
            string result = await _categoryViewServiceMock.Object.GetNameUrlByIdAsync(categoryId);
            // assert
            Assert.That(!result.Contains(" "));
        }

        // WEB HOST IS STILL NOT CONFIGURED!!!
        //[Test]
        //public async Task GetAllTest()
        //{
        //    var httpClient = new HttpClient();

        //    var response = await httpClient.GetAsync("https://localhost:7289/Category/All");

        //    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        //}


    }
}