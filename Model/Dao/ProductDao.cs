using Model.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class ProductDao
    {
        OnlineShopDbContext db = null;
        public ProductDao()
        {
            db = new OnlineShopDbContext();
        }

        public List<Product> ListNewProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        public List<string> ListName(string keyword)
        {
            return db.Products.Where(x => x.Name.Contains(keyword)).Select(x => x.Name).ToList();
        }
        public List<Product> ListByCategoryId(long categoryID)
        {
            return db.Products.Where(x => x.CategoryID == categoryID).ToList();
        }
        public IEnumerable<Product> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Product> model = db.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString) || x.Name.Contains(searchString));
            }

            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public List<Product> ListFeatureProduct(int top)
        {
            return db.Products.Where(x => x.TopHot != null && x.TopHot > DateTime.Now).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        public List<Product> ListRelatedProducts(long productId)
        {
            var product = db.Products.Find(productId);
            return db.Products.Where(x => x.ID != productId && x.CategoryID == product.CategoryID).ToList();
        }

        public void UpdateImages(long productId, string images)
        {
            var product = db.Products.Find(productId);
            product.MoreImages = images;
            db.SaveChanges();
        }
        public Product ViewDetail(long id)
        {
            return db.Products.Find(id);
        }
        public List<Product> Search(string keyword)
        {
            
            var model = (from a in db.Products
                         join b in db.ProductCategories
                         on a.CategoryID equals b.ID
                         where a.Name.Contains(keyword)
                         select new
                         {
                             CateMetaTitle = b.MetaTitle,
                             CateName = b.Name,
                             CreatedDate = a.CreatedDate,
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             Price = a.Price
                         }).AsEnumerable().Select(x => new Product()
                         {
                             
                             CreatedDate = x.CreatedDate,
                             ID = x.ID,
                             Image = x.Images,
                             Name = x.Name,
                             MetaTitle = x.MetaTitle,
                             Price = x.Price
                         });
           
            return model.ToList();
        }
        public Product GetById(long id)
        {
            return db.Products.Find(id);
        }
        public long Create(Product Product)
        {

            if (string.IsNullOrEmpty(Product.MetaTitle))
            {

            }
            Product.CreatedDate = DateTime.Now;
            Product.ViewCount = 0;
            db.Products.Add(Product);
            db.SaveChanges();

            return Product.ID;
        }
        public bool Update(Product entity)
        {
            try
            {
                var Product = db.Products.Find(entity.ID);
                Product.Name = entity.Name;
                Product.MetaTitle = entity.MetaTitle;
                Product.Description = entity.Description;
                Product.CategoryID = entity.CategoryID;
                Product.Warranty = entity.Warranty;
                Product.MetaKeywods = entity.MetaKeywods;
                Product.MetaDescriptions = entity.MetaDescriptions;
                Product.Status = Product.Status;
                
                Product.TopHot = Product.TopHot;
                Product.ModifiedBy = entity.ModifiedBy;
                Product.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                return true;
            }
            catch
            {
                //logging
                return false;
            }

        }

        

        public IEnumerable<Product> ListAllPaging(int page, int pageSize)
        {
            IQueryable<Product> model = db.Products;
            return model.OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
        }
    }
}
