using System.Text.RegularExpressions;

namespace BlogProject.Helpers
{
    public static class ReadTimeHelper
    {
        public static int GetReadTime(string blogPost)
        {
            var post = Regex.Replace(blogPost, @"[^\w\s]*", "");
            var words = post.Split(' ');
            var wordCount = words.Length;

            return wordCount / 228 + 1;
        }
    }
}
