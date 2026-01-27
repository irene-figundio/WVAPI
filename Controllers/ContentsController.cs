using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AI_Integration.Helpers;

namespace AI_Integration.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public sealed class ContentImageDto
        {
            public int Id { get; set; }
            public string ImageUrl { get; set; } = null!;
            public string? Caption { get; set; }
            public int? Position { get; set; }
        }

        public sealed class ContentLinkDto
        {
            public int Id { get; set; }
            public string LinkUrl { get; set; } = null!;
            public string? Description { get; set; }
        }

        public sealed class ContentDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string Text { get; set; } = null!;
            public DateTime PublishDate { get; set; }
            public string? CoverImage { get; set; }
            public string ContentType { get; set; } = null!;
            public bool IsPublished { get; set; }
            public List<ContentImageDto> Images { get; set; } = new();
            public List<ContentLinkDto> Links { get; set; } = new();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/contents", "", userAgent, "List contents (DTO)");
            var sw = Stopwatch.StartNew();
            try
            {
                var contents = await _unitOfWork.Query<Content>()
                    .OrderByDescending(c => c.PublishDate)
                    .Select(c => new ContentDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Text = c.Text,
                        PublishDate = c.PublishDate,
                        CoverImage = c.CoverImage,
                        ContentType = c.ContentType,
                        IsPublished = c.IsPublished ?? true,
                        Images = c.ContentImages.Select(i => new ContentImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            Caption = i.Caption,
                            Position = i.Position
                        }).ToList(),
                        Links = c.ContentLinks.Select(l => new ContentLinkDto
                        {
                            Id = l.Id,
                            LinkUrl = l.LinkUrl,
                            Description = l.Description
                        }).ToList()
                    }).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {contents.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(contents);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/contents/{id}", "", userAgent, "Get content by id (DTO)");
            var sw = Stopwatch.StartNew();
            try
            {
                var dto = await _unitOfWork.Query<Content>()
                    .Where(c => c.Id == id)
                    .Select(c => new ContentDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Text = c.Text,
                        PublishDate = c.PublishDate,
                        CoverImage = c.CoverImage,
                        ContentType = c.ContentType,
                        IsPublished = c.IsPublished ?? true,
                        Images = c.ContentImages.Select(i => new ContentImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            Caption = i.Caption,
                            Position = i.Position
                        }).ToList(),
                        Links = c.ContentLinks.Select(l => new ContentLinkDto
                        {
                            Id = l.Id,
                            LinkUrl = l.LinkUrl,
                            Description = l.Description
                        }).ToList()
                    }).FirstOrDefaultAsync();

                if (dto == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Content not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Content not found." });
                }

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(dto);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Content content, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/contents", content?.ToString(), userAgent, "Create content");
            var sw = Stopwatch.StartNew();
            if (content == null) return BadRequest(new { success = false, message = "Content info is required." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                content.CreatedAt = DateTime.Now;
                content.UpdatedAt = DateTime.Now;
                await _unitOfWork.InsertAsync(content);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, id = content.Id });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Content changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/contents/{id}", changes?.ToString(), userAgent, "Update content");
            var sw = Stopwatch.StartNew();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var content = await _unitOfWork.GetByIdAsync<Content>(id);
                if (content == null) return NotFound(new { success = false, message = "Content not found." });

                content.Title = changes.Title ?? content.Title;
                content.Text = changes.Text ?? content.Text;
                content.PublishDate = changes.PublishDate != default ? changes.PublishDate : content.PublishDate;
                content.CoverImage = changes.CoverImage ?? content.CoverImage;
                content.ContentType = changes.ContentType ?? content.ContentType;
                content.IsPublished = changes.IsPublished ?? content.IsPublished;
                content.UpdatedAt = DateTime.Now;

                _unitOfWork.Update(content);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("DELETE", $"api/contents/{id}", "", userAgent, "Delete content");
            var sw = Stopwatch.StartNew();
            try
            {
                var content = await _unitOfWork.GetByIdAsync<Content>(id);
                if (content == null) return NotFound(new { success = false, message = "Content not found." });

                _unitOfWork.Remove(content);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogNoContentAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return NoContent();
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
