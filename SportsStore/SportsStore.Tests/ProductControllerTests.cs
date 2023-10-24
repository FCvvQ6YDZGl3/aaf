using System.Collections.Generic;
using System.Linq;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using Xunit;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SportsStore.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public void Can_Paginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            }).AsQueryable<Product>());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            ProductsListViewModel result =
                controller.List(null, 2).ViewData.Model as ProductsListViewModel;

            Product[] prodArray = result.Products.ToArray();

            Assert.True(prodArray.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }

        [Fact]
        public void Can_send_Pagination_View_Model()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns((new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            }).AsQueryable<Product>());
            ProductController controller =
                new ProductController(mock.Object) { PageSize = 3 };
            ProductsListViewModel result = controller.List(null, 2).ViewData.Model as ProductsListViewModel;
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(5, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        private IEnumerable<Product> getProductWithCat(IEnumerable<int> catIds)
        {
            int[] cat = catIds.ToArray();
            for(int i = 1; i != catIds.Count()+1; i++)
            {
                yield return (new Product { ProductID = i, Name = $"P{i}", Category = $"Cat{cat[i-1]}" });
            }
        }

        [Fact]
        public void Can_Filter_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(getProductWithCat(new int[] { 1, 2, 1, 2, 3 }).AsQueryable<Product>());

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            Product[] result = (controller.List("Cat2", 1).ViewData.Model as ProductsListViewModel).Products.ToArray();

            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
        }
        [Fact]
        public void Generate_Category_Specific_Product_Count()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(getProductWithCat(new int[] { 1, 2, 1, 2, 3 }).AsQueryable<Product>());
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;

            Func<ViewResult, ProductsListViewModel> GetModel = result =>
            result?.ViewData?.Model as ProductsListViewModel;

            int? res1 = GetModel(target.List("Cat1"))?.PagingInfo.TotalItems;
            int? res2 = GetModel(target.List("Cat2"))?.PagingInfo.TotalItems;
            int? res3 = GetModel(target.List("Cat3"))?.PagingInfo.TotalItems;
            int? resAll = GetModel(target.List(null))?.PagingInfo.TotalItems;

            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}
