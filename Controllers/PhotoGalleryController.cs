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
    public class PhotoGalleryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PhotoGalleryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int langId = 1, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", "api/photogallery", $"langId={langId}", userAgent, "List photogallery");
            var sw = Stopwatch.StartNew();
            try
            {
                var items = await _unitOfWork.Query<PhotoGallery>().Where(e => e.IsDeleted != true)
                    .Where(p => p.LangID == langId)
                    .ToListAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, $"{{ success = true, count = {items.Count} }}", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(items);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] int langId = 1, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/photogallery/{id}", $"langId={langId}", userAgent, "Get photogallery by id");
            var sw = Stopwatch.StartNew();
            try
            {
                var item = await _unitOfWork.Query<PhotoGallery>().Where(e => e.IsDeleted != true)
                    .Where(p => p.Id == id && p.LangID == langId)
                    .FirstOrDefaultAsync();
                if (item == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Item not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound();
                }
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(item);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PhotoGallery item, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("POST", "api/photogallery", item?.ToString(), userAgent, "Create photogallery");
            var sw = Stopwatch.StartNew();
            if (item == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                item.CreatedAt = DateTime.Now;
                await _unitOfWork.InsertAsync(item);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true, id = item.Id });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("bygallery/{galleryId:int}")]
        public async Task<IActionResult> GetByGalleryId(int galleryId, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("GET", $"api/photogallery/bygallery/{galleryId}", "", userAgent, "Get photogallery by gallery id");
            var sw = Stopwatch.StartNew();
            try
            {
                var items = await _unitOfWork.Query<PhotoGallery>().Where(e => e.IsDeleted != true)
                    .Where(p => p.GalleryId == galleryId)
                    .ToListAsync();
                if (items == null || items.Count == 0)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Items not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound();
                }
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(items);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] PhotoGallery changes, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("PUT", $"api/photogallery/{id}", changes?.ToString(), userAgent, "Update photogallery");
            var sw = Stopwatch.StartNew();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var item = await _unitOfWork.GetByIdAsync<PhotoGallery>(id);
                if (item == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Item not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound();
                }

                // Basic mapping (better to use Automapper or similar, but following pattern)
                item.ImageUrl = changes.ImageUrl ?? item.ImageUrl;
                item.Caption = changes.Caption ?? item.Caption;
                item.GalleryId = changes.GalleryId != 0 ? changes.GalleryId : item.GalleryId;
                item.LangID = changes.LangID != 0 ? changes.LangID : item.LangID;

                _unitOfWork.Update(item);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogOkAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "User-Agent")] string userAgent = "")
        {
            var log = WebApiLogHelper.NewLog("DELETE", $"api/photogallery/{id}", "", userAgent, "Delete photogallery");
            var sw = Stopwatch.StartNew();
            try
            {
                var item = await _unitOfWork.GetByIdAsync<PhotoGallery>(id);
                if (item == null)
                {
                    sw.Stop();
                    await WebApiLogHelper.LogNotFoundAsync(_unitOfWork, log, "{ success = false, message = 'Item not found.' }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                    return NotFound();
                }
                item.IsDeleted = true;
                item.DeletionDate = DateTime.Now;
                _unitOfWork.Update(item);
                await _unitOfWork.SaveChangesAsync();
                sw.Stop();
                await WebApiLogHelper.LogNoContentAsync(_unitOfWork, log, "{ success = true }", $"ElapsedMs={sw.ElapsedMilliseconds}");
                return NoContent();
            }
            catch (Exception ex)
            {
                sw.Stop();
                await WebApiLogHelper.LogErrorAsync(_unitOfWork, log, ex, $"ElapsedMs={sw.ElapsedMilliseconds}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
