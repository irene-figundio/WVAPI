using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VITBO.Services.Interfaces;
using VITBO.Models;

namespace VITBO.Controllers
{
    [Authorize]
    public class AiVideoController : Controller
    {
        private readonly IAiVideoService _aiVideoService;


        public AiVideoController(IAiVideoService aiVideoService)
        {
            _aiVideoService = aiVideoService;
        }

        public async Task<IActionResult> Index([FromQuery] string? q = null, [FromQuery] int page = 1)
        {
            ViewData["CurrentFilter"] = q;
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var result = await _aiVideoService.GetVideosAsync(q, page, sessionToken, GetUserAgent(), HttpContext.RequestAborted);
            return View(result);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateVideoRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            var success = await _aiVideoService.DeleteVideoAsync(id, sessionToken, GetUserAgent(), HttpContext.RequestAborted);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVideoRequest model)
        {
            if (ModelState.IsValid)
            {
                string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var success = await _aiVideoService.CreateVideoAsync(model, sessionToken, GetUserAgent(), HttpContext.RequestAborted);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create video. Please try again.");
            }
            return View(model);

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
            // To get the video details, we fetch all and filter since we don't have a GetById endpoint locally mapped.
            var videos = await _aiVideoService.GetVideosAsync(null, 1, sessionToken, GetUserAgent(), HttpContext.RequestAborted);
            // If there are many videos, this might miss it if it's not on page 1.
            // Ideally, we'd add GetById. Assuming we have a limited number or we can fetch a larger page size.
            // But let's fetch a larger page just in case, or we use GetVideosAsync with a loop/larger page.
            // Wait, we can add a GetById or just iterate.
            // Actually, the easiest way is to add GetByIdAsync, or just use the existing one if we assume it's there.
            // Let's implement GetByIdAsync if needed, or just fetch the first 1000 items here.

            // Note: Since GetVideosAsync applies local pagination (taking only 20 items for page 1),
            // fetching the video this way might fail if the video is on a subsequent page.
            // We should use an approach that fetches the full list or a dedicated GetById endpoint.
            // Given we don't have GetById endpoint defined, let's just loop over pages to find it
            // or pass an arbitrarily large page size.
            // Since we implemented GetVideosAsync to fetch all videos first from /all and then paginate locally,
            // we will temporarily modify GetVideosAsync to support a large page size, or simply iterate.

            // To be safe and simple:
            var allVideos = await _aiVideoService.GetVideosAsync(null, 1, sessionToken, GetUserAgent(), HttpContext.RequestAborted);
            var video = allVideos.Items.FirstOrDefault(v => v.Id == id);

            // If we didn't find it on page 1, check other pages (assuming a reasonable number of items)
            if (video == null && allVideos.Total > 20)
            {
                for (int p = 2; p <= Math.Ceiling(allVideos.Total / 20.0); p++)
                {
                    var moreVideos = await _aiVideoService.GetVideosAsync(null, p, sessionToken, GetUserAgent(), HttpContext.RequestAborted);
                    video = moreVideos.Items.FirstOrDefault(v => v.Id == id);
                    if (video != null) break;
                }
            }

            if (video == null)
            {
                return NotFound();
            }

            var model = new UpdateVideoRequest
            {
                Id = video.Id,
                Title = video.Title,
                Url_Video = video.Url_Video,
                IsLandscape = video.IsLandscape,
                Play_Priority = video.Play_Priority,
                ID_Session = video.ID_Session
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateVideoRequest model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                string sessionToken = HttpContext.User.FindFirst("JWToken")?.Value ?? HttpContext.Session.GetString("JWToken") ?? string.Empty;
                var success = await _aiVideoService.UpdateVideoAsync(id, model, sessionToken, GetUserAgent(), HttpContext.RequestAborted);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to update video. Please try again.");
            }
            return View(model);
        }

        protected string? GetUserAgent()
        {
            return Request.Headers["User-Agent"].ToString();
        }
    }
}
