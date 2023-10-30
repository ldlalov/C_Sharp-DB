using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            var db = new ProductShopContext();
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();
            //string users = "../../../Datasets/users.xml";
            //Console.WriteLine(ImportUsers(db, users));
            //string products = File.ReadAllText("../../../Datasets/products.xml");
            //Console.WriteLine(ImportProducts(db, products));
            //string categories = File.ReadAllText("../../../Datasets/categories.xml");
            //Console.WriteLine(ImportCategories(db, categories));
            //string categoriesProducts = File.ReadAllText("../../../Datasets/categories-products.xml");
            //Console.WriteLine(ImportCategoryProducts(db, categoriesProducts));

            string products = "../../../Datasets/products-in-range.xml";
            File.WriteAllText(products, GetProductsInRange(db));
            //string users = "../../../Datasets/users-sold-products.xml";
            //File.WriteAllText(users, GetSoldProducts(db));
            //string categories = "../../../Datasets/users-and-products.xml";
            //File.WriteAllText(categories, GetUsersWithProducts(db));
        }
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Include(x => x.ProductsSold)
                .ToList()
            .Where(u => u.ProductsSold.Any(b => b.BuyerId != null))
            .Select(u => new
            {
                firstName = u.FirstName,
                lastName = u.LastName,
                age = u.Age,
                soldProducts = new
                {
                    count = u.ProductsSold.Count(),
                    products = u.ProductsSold
                 .Where(p => p.BuyerId != null)
                 .Select(p => new
                 {
                     name = p.Name,
                     price = p.Price
                 })
                 .OrderByDescending(p => p.price)
                }
            })
            .OrderByDescending(u => u.soldProducts.products.Count())
            .ToList();
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
            XElement element = new XElement("Users");
            XElement allUsers = new XElement("users");
            element.Add(new XElement("count", users.Count()));
            foreach (var user in users.Take(10))
            {
                XElement currentUser = new XElement("User");
                var sProductsCount = new XElement("count",user.soldProducts.products.Count());
                var allProducts = new XElement("products");
                var products = user.soldProducts;
                foreach (var product in products.products)
                {
                    allProducts.Add(new XElement("Product", 
                        new XElement("name", product.name),
                        new XElement("price", product.price)));
                }
                currentUser.Add(
                 new XElement("firstName", user.firstName),
                 new XElement("lastName", user.lastName),
                 new XElement("age", user.age),
                 new XElement("SoldProducts",sProductsCount, allProducts));
                allUsers.Add(currentUser);
            }
            element.Add(new XElement(allUsers));
            XDocument xDoc = new XDocument(declaration);
            xDoc.Add(element);
            var sw = new StringWriter();
            xDoc.Save(sw);
            return sw.ToString().Trim();
        }
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new { category = c.Name, productsCount = c.CategoryProducts.Count(), averagePrice = c.CategoryProducts.Average(price => price.Product.Price), totalRevenue = c.CategoryProducts.Sum(price => price.Product.Price) })
                .OrderByDescending(c => c.productsCount)
                .ThenBy(c => c.totalRevenue)
                .ToList();
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
            XElement element = new XElement("Categories");
            foreach (var c in categories)
            {
                element.Add(new XElement("Category", 
                    new XElement("name", c.category),
                    new XElement("count", c.productsCount),
                    new XElement("averagePrice",c.averagePrice),
                    new XElement("totalRevenue",c.totalRevenue)
                    ));
            }
            XDocument xDoc = new XDocument(declaration);
            xDoc.Add(element);
            var sw = new StringWriter();
            xDoc.Save(sw);
            return sw.ToString().Trim();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Count() != null && x.ProductsSold.Any(p => p.Buyer.LastName != null))
                .Select(u => new { firstName = u.FirstName, lastName = u.LastName, soldProducts = u.ProductsSold.Select(p => new { name = p.Name, price = p.Price, buyerFirstName = p.Buyer.FirstName, buyerLastName = p.Buyer.LastName }) })
                .OrderBy(u => u.lastName)
                .ThenBy(u => u.firstName)
                .Take(5)
                .ToList();
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
            XElement element = new XElement("Users");
            foreach (var user in users)
            {
                var currentUser = new XElement("User",
                                            new XElement("firstName", user.firstName),
                                            new XElement("lastName", user.lastName));
                var soldProducts = new XElement("soldProducts", null); 
                var products = user.soldProducts;
                foreach (var product in products)
                {
                    soldProducts.Add(new XElement("Product",new XElement("name",product.name),
                                     new XElement("price",product.price)));
                }
                currentUser.Add(soldProducts);
                element.Add(currentUser);
                        
            }
            XDocument xDoc = new XDocument(declaration);
            xDoc.Add(element);
            var sw = new StringWriter();
            xDoc.Save(sw);
            return sw.ToString().Trim();
        }
        public static string GetProductsInRange(ProductShopContext context)
        {
            var selectedProducts = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new { p.Name, price = p.Price.ToString("G29"), buyer = $"{p.Buyer.FirstName} {p.Buyer.LastName}" })
                .Take(10)
                .ToArray();
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
            XElement element = new XElement("Products");
            foreach (var sel in selectedProducts)
            {
                element.Add(
                    new XElement("Product",
                    new XElement("name", sel.Name),
                    new XElement("price", sel.price),
                    sel.buyer != " " ? new XElement("buyer", sel.buyer) : null)
                    );
            }
            XDocument xDoc = new XDocument(declaration);
            xDoc.Add(element);
            var sw = new StringWriter();
            xDoc.Save(sw);
            return sw.ToString().Trim();
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlData = new XmlSerializer(typeof(ImportedCategoryProduct[]), new XmlRootAttribute("CategoryProducts"));
            using StringReader data = new StringReader(inputXml);
            ImportedCategoryProduct[] categoriesProductsData = (ImportedCategoryProduct[])xmlData.Deserialize(data);
            var categoriesProducts = new List<CategoryProduct>();
            foreach (var u in categoriesProductsData)
            {
                categoriesProducts.Add(new CategoryProduct { CategoryId = u.CategoryId, ProductId = u.ProductId });
            }

            context.AddRange(categoriesProducts);
            context.SaveChanges();
            return $"Successfully imported {categoriesProducts.Count()}";

        }
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlData = new XmlSerializer(typeof(ImportedCategory[]), new XmlRootAttribute("Categories"));
            using StringReader data = new StringReader(inputXml);
            ImportedCategory[] categoryData = (ImportedCategory[])xmlData.Deserialize(data);
            var categories = new List<Category>();
            foreach (var u in categoryData)
            {
                categories.Add(new Category { Name = u.name });
            }

            context.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count()}";

        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlData = new XmlSerializer(typeof(ImportedProduct[]), new XmlRootAttribute("Products"));
            using StringReader data = new StringReader(inputXml);
            ImportedProduct[] productData = (ImportedProduct[])xmlData.Deserialize(data);
            var products = new List<Product>();
            foreach (var u in productData)
            {
                products.Add(new Product { Name = u.name, Price = u.price, SellerId = u.sellerId, BuyerId = u.buyerId });
            }
            context.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count()}";

        }
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var file = File.OpenRead(inputXml);
            var xmlData = new XmlSerializer(typeof(ImportedUsers[]), new XmlRootAttribute("Users"));
            ImportedUsers[] usersData = (ImportedUsers[])xmlData.Deserialize(file);
            var users = new List<User>();
            foreach (var u in usersData)
            {
                users.Add(new User{FirstName = u.firstName,LastName = u.lastName,Age = u.age});
            }
            context.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Count()}";
        }
    }
}