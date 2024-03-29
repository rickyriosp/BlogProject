﻿using BlogProject.Data;
using BlogProject.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProject.ApiControllers
{
    [ApiController]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the requested amount of posts, ordered by creation date.
        /// </summary>
        /// <remarks>
        /// More elaborate description
        /// </remarks>
        /// <param name="postQty">The amount of posts to get</param>
        /// <returns>A list of the most recent Posts ordered by creation date</returns>
        /// <response code="200">Returns a list with the posts ordered by creation date</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PostDTO>>> GetRecent(int? postQty)
        {
            var size = postQty ?? 3;
            var posts = new List<PostDTO>();
            var postModels = await _context.Posts
                .Where(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady)
                .OrderByDescending(p => p.Created)
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .Take(size)
                .ToListAsync();

            foreach (var post in postModels)
            {
                posts.Add(new PostDTO
                {
                    Id = post.Id,
                    BlogName = post.Blog.Name,
                    Author = post.BlogUser.DisplayName,
                    Title = post.Title,
                    Abstract = post.Abstract,
                    Created = post.Created,
                    Slug = post.Slug,
                    ImageData = Convert.ToBase64String(post.ImageData),
                    ContentType = post.ContentType,
                });
            }

            return Ok(posts);
        }

        /// <summary>
        /// Retrieves a post by unique id.
        /// </summary>
        /// <remarks>
        /// Sample value of post:
        /// 
        ///     {
        ///         "id": 0
        ///         "blogName": "string"
        ///         "author": "string"
        ///         "title": "string"
        ///         "abstract": "string"
        ///         "created": "2022-02-16"
        ///         "slug": "string"
        ///         "imageData": "string"
        ///         "contentType": "string"
        ///     }
        /// </remarks>
        /// <param name="id">The post id to query</param>
        /// <returns>The post that matches the id</returns>
        /// <response code="200">Returns the requested post</response>
        /// <response code="404">If the post is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostDTO>> GetPost(int id)
        {
            var post = await _context.Posts
                .Where(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady)
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var postDTO = new PostDTO
            {
                Id = post.Id,
                BlogName = post.Blog.Name,
                Author = post.BlogUser.DisplayName,
                Title = post.Title,
                Abstract = post.Abstract,
                Created = post.Created,
                Slug = post.Slug,
                ImageData = Convert.ToBase64String(post.ImageData),
                ContentType = post.ContentType,
            };

            return Ok(postDTO);
        }
    }
}
