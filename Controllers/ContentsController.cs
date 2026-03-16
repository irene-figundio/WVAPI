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

        public sealed class ExpertDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string? Bio { get; set; }
            public string? PhotoUrl { get; set; }
            public string? Email { get; set; }
        }

        public sealed class ContentDto
        {
            public Guid Guid { get; set; }
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string Text { get; set; } = null!;
            public DateTime PublishDate { get; set; }
            public string? CoverImage { get; set; }
            public string ContentType { get; set; } = null!;
            public bool IsPublished { get; set; }
            public List<ContentImageDto> Images { get; set; } = new();
            public List<ContentLinkDto> Links { get; set; } = new();
            public List<ExpertDto> Authors { get; set; } = new();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int langId = 1, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/contents", $"langId={langId}", userAgent, "List contents (DTO)");
            var sw = Stopwatch.StartNew();
            try
            {
                var contents = await _unitOfWork.Query<Content>()
                    .Where(c => c.LangID == langId)
                    .OrderByDescending(c => c.PublishDate)
                    .Select(c => new ContentDto
                    {
                        Guid = c.Guid,
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
                        }).ToList(),
                        Authors = c.ContentExperts.Select(ce => new ExpertDto
                        {
                            Id = ce.Expert.Id,
                            Name = ce.Expert.Name,
                            Bio = ce.Expert.Bio,
                            PhotoUrl = ce.Expert.PhotoUrl,
                            Email = ce.Expert.Email
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

        [HttpGet("type/{contentType}")]
        public async Task<IActionResult> GetByType(string contentType, [FromQuery] int langId = 1, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/contents/type/{contentType}", $"langId={langId}", userAgent, $"List contents by type ({contentType})");
            var sw = Stopwatch.StartNew();
            try
            {
                var contents = await _unitOfWork.Query<Content>()
                    .Where(c => c.ContentType.ToLower() == contentType.ToLower() && c.LangID == langId)
                    .OrderByDescending(c => c.PublishDate)
                    .Select(c => new ContentDto
                    {
                        Guid = c.Guid,
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
                        }).ToList(),
                        Authors = c.ContentExperts.Select(ce => new ExpertDto
                        {
                            Id = ce.Expert.Id,
                            Name = ce.Expert.Name,
                            Bio = ce.Expert.Bio,
                            PhotoUrl = ce.Expert.PhotoUrl,
                            Email = ce.Expert.Email
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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] int langId = 1, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/contents/{id}", $"langId={langId}", userAgent, "Get content by id (DTO)");
            var sw = Stopwatch.StartNew();
            try
            {
                var dto = await _unitOfWork.Query<Content>()
                    .Where(c => c.Guid == id && c.LangID == langId)
                    .Select(c => new ContentDto
                    {
                        Guid = c.Guid,
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
                        }).ToList(),
                        Authors = c.ContentExperts.Select(ce => new ExpertDto
                        {
                            Id = ce.Expert.Id,
                            Name = ce.Expert.Name,
                            Bio = ce.Expert.Bio,
                            PhotoUrl = ce.Expert.PhotoUrl,
                            Email = ce.Expert.Email
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

        [HttpPost("CreateContent")]
        public async Task<IActionResult> CreateContent([FromBody] Content content, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/contents/CreateContent", content?.ToString(), userAgent, "Create content via SP");
            var sw = Stopwatch.StartNew();
            if (content == null) return BadRequest(new { success = false, message = "Content info is required." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var sql = "EXEC [dbo].[sp_CreaContent] @Title={0}, @Text={1}, @PublishDate={2}, @CoverImage={3}, @ContentType={4}, @IsPublished={5}, @LangID={6}";
                var results = await _unitOfWork.Context.ContentCreationResults.FromSqlRaw(sql,
                    content.Title,
                    content.Text,
                    content.PublishDate,
                    content.CoverImage ?? (object)DBNull.Value,
                    content.ContentType,
                    content.IsPublished ?? true,
                    content.LangID != 0 ? content.LangID : 1
                ).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                var result = results.FirstOrDefault();
                if (result != null)
                {
                    return Ok(new { success = true, guid = result.ContentGuid, id = result.ContentId });
                }
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Content changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/contents/{id}", changes?.ToString(), userAgent, "Update content");
            var sw = Stopwatch.StartNew();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var content = await _unitOfWork.GetByIdAsync<Content>(id);
                if (content == null) return NotFound(new { success = false, message = "Content not found." });

                content.Id = changes.Id != 0 ? changes.Id : content.Id;
                content.Title = changes.Title ?? content.Title;
                content.Text = changes.Text ?? content.Text;
                content.PublishDate = changes.PublishDate != default ? changes.PublishDate : content.PublishDate;
                content.CoverImage = changes.CoverImage ?? content.CoverImage;
                content.ContentType = changes.ContentType ?? content.ContentType;
                content.IsPublished = changes.IsPublished ?? content.IsPublished;
                content.LangID = changes.LangID != 0 ? changes.LangID : content.LangID;
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

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, [FromHeader(Name = "User-Agent")] string userAgent = "")
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
