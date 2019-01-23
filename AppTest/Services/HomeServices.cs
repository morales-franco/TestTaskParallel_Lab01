using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AppTest.Services
{
    public class HomeServices
    {
        internal List<string> GetCategories()
        {
            Thread.Sleep(5000);
            return GetListHarcoding("Categories");
        }

        internal List<string> GetPosts()
        {
            Thread.Sleep(5000);
            return GetListHarcoding("Posts");
        }

        internal List<string> GetTags()
        {
            Thread.Sleep(5000);
            return GetListHarcoding("Tags");
        }


        internal async Task<List<string>> GetCategoriesAsync()
        {
            Debug.WriteLine("GetCategoriesAsync");
            await Task.Delay(5000);
            return GetListHarcoding("Categories");
        }

        internal async Task<List<string>> GetPostsAsync()
        {
            Debug.WriteLine("GetPostsAsync");
            await Task.Delay(5000);
            return GetListHarcoding("Posts");
        }

        internal async Task<List<string>> GetTagsAsync()
        {
            Debug.WriteLine("GetPostsAsync");
            await Task.Delay(5000);
            return GetListHarcoding("Tags");
        }


        private List<string> GetListHarcoding(string text)
        {
            var list = new List<string>();
            for (int i = 0; i < 25; i++)
            {
                list.Add($"{ text }_{ i }");
            }

            return list;
        }
    }
}