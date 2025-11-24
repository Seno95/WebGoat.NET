using WebGoatCore.Models;
using WebGoatCore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace WebGoatCore.Controllers
{
    [Route("[controller]/[action]")]
    public class BlogController : Controller
    {
        private readonly BlogEntryRepository _blogEntryRepository;
        private readonly BlogResponseRepository _blogResponseRepository;

        public BlogController(BlogEntryRepository blogEntryRepository, BlogResponseRepository blogResponseRepository, NorthwindContext context)
        {
            _blogEntryRepository = blogEntryRepository;
            _blogResponseRepository = blogResponseRepository;
        }

        public IActionResult Index()
        {
            return View(_blogEntryRepository.GetTopBlogEntries());
        }

        [HttpGet("{entryId}")]
        public IActionResult Reply(int entryId)
        {
            return View(_blogEntryRepository.GetBlogEntry(entryId));
        }

        [HttpPost("{entryId}")]
        public IActionResult Reply(int entryId, string contents)
        {
            // INPUT VALIDERING
            if (string.IsNullOrWhiteSpace(contents))
            {
                ModelState.AddModelError("contents", "Comment cannot be empty");
                return View(_blogEntryRepository.GetBlogEntry(entryId));
            }
            
            if (contents.Length > 1000)
            {
                ModelState.AddModelError("contents", "Comment must be less than 1000 characters");
                return View(_blogEntryRepository.GetBlogEntry(entryId));
            }

            if (ContainsDangerousPatterns(contents))
            {
                ModelState.AddModelError("contents", "Comment contains invalid characters");
                return View(_blogEntryRepository.GetBlogEntry(entryId));
            }

            var userName = User?.Identity?.Name ?? "Anonymous";
            var response = new BlogResponse()
            {
                Author = userName,
                Contents = contents,
                BlogEntryId = entryId,
                ResponseDate = DateTime.Now
            };
            _blogResponseRepository.CreateBlogResponse(response);

            return RedirectToAction("Index");
        }

        private bool ContainsDangerousPatterns(string input)
        {
            var dangerousPatterns = new[] { "<script", "javascript:", "onload=", "onerror=", "onclick=" };
            return dangerousPatterns.Any(pattern => 
                input.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(string title, string contents)
        {
            // VALIDERING FOR BLOG INDLÆG
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(contents))
            {
                ModelState.AddModelError("", "Title and contents are required");
                return View();
            }
            
            if (contents.Length > 5000) 
            {
                ModelState.AddModelError("contents", "Content too long");
                return View();
            }

            var blogEntry = _blogEntryRepository.CreateBlogEntry(title, contents, User!.Identity!.Name!);
            return View(blogEntry);
        }

    }
}