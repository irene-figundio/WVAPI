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
    public class EventsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public sealed class PhotoDto
        {
            public int Id { get; set; }
            public string ImageUrl { get; set; } = null!;
            public string? Caption { get; set; }
        }

        public sealed class GalleryDto
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public List<PhotoDto> Photos { get; set; } = new();
        }

        public sealed class EventLinkDto
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

        public sealed class EventDto
        {
            public Guid Guid { get; set; }
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string Description { get; set; } = null!;
            public DateTime EventDate { get; set; }
            public DateTime? StartDate { get; set; }
            public TimeSpan? StartTime { get; set; }
            public DateTime? EndDate { get; set; }
            public TimeSpan? EndTime { get; set; }
            public string? CoverImage { get; set; }
            public DateTime? BookingEndDate { get; set; }
            public string? Location { get; set; }
            public string? Organizer { get; set; }
            public string? ContactInfo { get; set; }
            public decimal Price { get; set; }
            public bool IsOnline { get; set; }
            public int LangID { get; set; }
            public List<EventLinkDto> Links { get; set; } = new();
            public GalleryDto? Gallery { get; set; }
            public List<ExpertDto> Authors { get; set; } = new();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int langId = 1, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/events", $"langId={langId}", userAgent, "List events (DTO)");
            var sw = Stopwatch.StartNew();
            try
            {
                var events = await _unitOfWork.Query<Event>()
                    .Where(e => e.LangID == langId)
                    .OrderByDescending(e => e.EventDate)
                    .Select(e => new EventDto
                    {
                        Guid = e.Guid,
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        EventDate = e.EventDate,
                        StartDate = e.StartDate,
                        StartTime = e.StartTime,
                        EndDate = e.EndDate,
                        EndTime = e.EndTime,
                        CoverImage = e.CoverImage,
                        BookingEndDate = e.BookingEndDate,
                        Location = e.Location,
                        Organizer = e.Organizer,
                        ContactInfo = e.ContactInfo,
                        Price = e.Price,
                        IsOnline = e.IsOnline,
                        LangID = e.LangID,
                        Links = e.EventLinks.Select(l => new EventLinkDto
                        {
                            Id = l.Id,
                            LinkUrl = l.LinkUrl,
                            Description = l.Description
                        }).ToList(),
                        Gallery = e.Gallery == null ? null : new GalleryDto
                        {
                            Id = e.Gallery.Id,
                            Title = e.Gallery.Title,
                            Photos = e.Gallery.Photos.Select(p => new PhotoDto
                            {
                                Id = p.Id,
                                ImageUrl = p.ImageUrl,
                                Caption = p.Caption
                            }).ToList()
                        },
                        Authors = e.EventExperts.Select(ee => new ExpertDto
                        {
                            Id = ee.Expert.Id,
                            Name = ee.Expert.Name,
                            Bio = ee.Expert.Bio,
                            PhotoUrl = ee.Expert.PhotoUrl,
                            Email = ee.Expert.Email
                        }).ToList()
                    }).ToListAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {events.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(events);
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
            var log = WebApiLogHelper.NewLog("GET", $"api/events/{id}", $"langId={langId}", userAgent, "Get event by id (DTO)");
            var sw = Stopwatch.StartNew();
            try
            {
                var dto = await _unitOfWork.Query<Event>()
                    .Where(e => e.Guid == id && e.LangID == langId)
                    .Select(e => new EventDto
                    {
                        Guid = e.Guid,
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        EventDate = e.EventDate,
                        StartDate = e.StartDate,
                        StartTime = e.StartTime,
                        EndDate = e.EndDate,
                        EndTime = e.EndTime,
                        CoverImage = e.CoverImage,
                        BookingEndDate = e.BookingEndDate,
                        Location = e.Location,
                        Organizer = e.Organizer,
                        ContactInfo = e.ContactInfo,
                        Price = e.Price,
                        IsOnline = e.IsOnline,
                        LangID = e.LangID,
                        Links = e.EventLinks.Select(l => new EventLinkDto
                        {
                            Id = l.Id,
                            LinkUrl = l.LinkUrl,
                            Description = l.Description
                        }).ToList(),
                        Gallery = e.Gallery == null ? null : new GalleryDto
                        {
                            Id = e.Gallery.Id,
                            Title = e.Gallery.Title,
                            Photos = e.Gallery.Photos.Select(p => new PhotoDto
                            {
                                Id = p.Id,
                                ImageUrl = p.ImageUrl,
                                Caption = p.Caption
                            }).ToList()
                        },
                        Authors = e.EventExperts.Select(ee => new ExpertDto
                        {
                            Id = ee.Expert.Id,
                            Name = ee.Expert.Name,
                            Bio = ee.Expert.Bio,
                            PhotoUrl = ee.Expert.PhotoUrl,
                            Email = ee.Expert.Email
                        }).ToList()
                    }).FirstOrDefaultAsync();

                if (dto == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Event not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound(new { success = false, message = "Event not found." });
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

        [HttpPost("CreateEvent")]
        public async Task<IActionResult> CreateEvent([FromBody] Event @event, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/events/CreateEvent", @event?.ToString(), userAgent, "Create event via SP");
            var sw = Stopwatch.StartNew();
            if (@event == null) return BadRequest(new { success = false, message = "Event info is required." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var sql = "EXEC [dbo].[sp_CreaEvent] @Title={0}, @Description={1}, @EventDate={2}, @StartDate={3}, @StartTime={4}, @EndDate={5}, @EndTime={6}, @CoverImage={7}, @BookingEndDate={8}, @Location={9}, @Organizer={10}, @ContactInfo={11}, @Price={12}, @IsOnline={13}, @LangID={14}";
                var results = await _unitOfWork.Context.EventCreationResults.FromSqlRaw(sql,
                    @event.Title,
                    @event.Description,
                    @event.EventDate,
                    @event.StartDate ?? (object)DBNull.Value,
                    @event.StartTime ?? (object)DBNull.Value,
                    @event.EndDate ?? (object)DBNull.Value,
                    @event.EndTime ?? (object)DBNull.Value,
                    @event.CoverImage ?? (object)DBNull.Value,
                    @event.BookingEndDate ?? (object)DBNull.Value,
                    @event.Location ?? (object)DBNull.Value,
                    @event.Organizer ?? (object)DBNull.Value,
                    @event.ContactInfo ?? (object)DBNull.Value,
                    @event.Price,
                    @event.IsOnline,
                    @event.LangID != 0 ? @event.LangID : 1
                ).ToListAsync();

                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");

                var result = results.FirstOrDefault();
                if (result != null)
                {
                    return Ok(new { success = true, guid = result.EventGuid, id = result.EventId });
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
        public async Task<IActionResult> Put(Guid id, [FromBody] Event changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/events/{id}", changes?.ToString(), userAgent, "Update event");
            var sw = Stopwatch.StartNew();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var @event = await _unitOfWork.GetByIdAsync<Event>(id);
                if (@event == null) return NotFound(new { success = false, message = "Event not found." });

                @event.Id = changes.Id != 0 ? changes.Id : @event.Id;
                @event.Title = changes.Title ?? @event.Title;
                @event.Description = changes.Description ?? @event.Description;
                @event.EventDate = changes.EventDate != default ? changes.EventDate : @event.EventDate;
                @event.StartDate = changes.StartDate ?? @event.StartDate;
                @event.StartTime = changes.StartTime ?? @event.StartTime;
                @event.EndDate = changes.EndDate ?? @event.EndDate;
                @event.EndTime = changes.EndTime ?? @event.EndTime;
                @event.CoverImage = changes.CoverImage ?? @event.CoverImage;
                @event.GalleryId = changes.GalleryId ?? @event.GalleryId;
                @event.LangID = changes.LangID != 0 ? changes.LangID : @event.LangID;

                @event.BookingEndDate = changes.BookingEndDate ?? @event.BookingEndDate;
                @event.Location = changes.Location ?? @event.Location;
                @event.Organizer = changes.Organizer ?? @event.Organizer;
                @event.ContactInfo = changes.ContactInfo ?? @event.ContactInfo;
                @event.Price = changes.Price;
                @event.IsOnline = changes.IsOnline;

                _unitOfWork.Update(@event);
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
            var log = WebApiLogHelper.NewLog("DELETE", $"api/events/{id}", "", userAgent, "Delete event");
            var sw = Stopwatch.StartNew();
            try
            {
                var @event = await _unitOfWork.GetByIdAsync<Event>(id);
                if (@event == null) return NotFound(new { success = false, message = "Event not found." });
                _unitOfWork.Remove(@event);
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
