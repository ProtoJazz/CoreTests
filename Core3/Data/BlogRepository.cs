using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core3.Models;

namespace Core3.Data
{
    public class BlogRepository : IBlogRepository
    {
        private BloggingContext _context;

        public BlogRepository(BloggingContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveAllAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public IEnumerable<Blog> GetAllBlogs()
        {
            return _context.Blogs.ToList();
        }

        public Blog GetBlog(int id)
        {
            throw new NotImplementedException();
        }
    }
}
