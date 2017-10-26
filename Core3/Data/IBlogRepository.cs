using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core3.Models;

namespace Core3.Data
{
    public interface IBlogRepository
    {

        // Basic DB Operations
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAllAsync();

        // Blogs
        IEnumerable<Blog> GetAllBlogs();
        Blog GetBlog(int id);

        /*
        // Posts
        IEnumerable<Post> GetPosts(int id);
        IEnumerable<Post> GetPostsWithTalks(int id);
        IEnumerable<Post> GetPostsByMoniker(string moniker);
        IEnumerable<Post> GetPostsByMonikerWithTalks(string moniker);
        Post GetPost(int PostId);
        Post GetPostWithTalks(int PostId);
        */
       
    }
}
