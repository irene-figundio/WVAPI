using Abp.Collections.Extensions;
using AI_Integration.DataAccess;
using AI_Integration.DataAccess.Database;
using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Helpers;
using AI_Integration.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AI_Integration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly VideoHelper _videoHelper;
        private readonly IUnitOfWork _unitOfWork;
        public VideoController(VideoHelper videoHelper, IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _videoHelper = videoHelper;
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        // GET: /api/video/random
        [HttpGet("random")]
        public IActionResult GetRandomVideo([FromHeader(Name = "User-Agent")] string userAgent)
        {
            // Correzione valori di log (metodo/URL)
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "GET",
                RequestUrl = "api/video/random",
                RequestBody = "",
                UserAgent = userAgent,
                AdditionalInfo = "Random video"
            };

            var randomVideo = _videoHelper.GetRandomVideo(log);
            // Se desideri persistere questo log:
            // _ = _unitOfWork.InsertAsync(log); _ = _unitOfWork.SaveChangesAsync();
            return Ok(randomVideo);
        }

        // GET: /api/video/playlist?landscape=1|0
        [HttpGet("playlist")]
        public IActionResult GetPlaylist([FromQuery] int landscape)
        {
            try
            {
                var includeLandscape = (landscape == 1);
                var initialPath = includeLandscape ? "/Video/Landscape" : "/Video/Portrait";

                // Query di dominio: teniamo il repo specifico (snello) per la playlist
                var playlist = _unitOfWork.AIVideo.GetPlaylist(includeLandscape);

                // Normalizza URL
                foreach (var video in playlist)
                {
                    video.Url_Video = Path.Combine(initialPath, video.Url_Video ?? string.Empty).Replace("\\", "/");
                }

                return Ok(playlist);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[playlist] error: {ex}");
                return StatusCode(500, new { answer = "Si è verificato un errore durante la richiesta. Riprova più tardi." });
            }
        }

        #region SavePhisicalVideo
        // POST: /api/video/upload
        // Content-Type: multipart/form-data
        [Authorize]
        [HttpPost("upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
        [RequestSizeLimit(500_000_000)]
        public async Task<IActionResult> Upload(
            [FromForm(Name = "file")] IFormFile file,
            [FromForm] string Title,
            [FromForm] bool? isLandscape,
            [FromForm] int idUser,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/video/upload",
                RequestBody = "", // evita di loggare binari
                UserAgent = userAgent,
                AdditionalInfo = "Upload video file"
            };

            if (file == null || file.Length == 0)
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = "{ success = false, message = 'File mancante o vuoto.' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = "File mancante o vuoto." });
            }

            var allowedExt = _allowedExtensions;      // vedi helpers esistenti nella classe
            var allowedMime = _allowedContentTypes;

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
            if (!allowedExt.Contains(ext))
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = "{ success = false, message = 'Estensione non supportata.' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = "Estensione non supportata. Formati consentiti: mp4, webm, mov, avi, mkv." });
            }

            if (!string.IsNullOrWhiteSpace(file.ContentType) && !allowedMime.Contains(file.ContentType))
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = "{ success = false, message = 'MIME type non supportato.' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = $"MIME type non supportato: {file.ContentType}" });
            }

            string? finalPath = null;
            string? safeName = null;

            try
            {
                var landscape = isLandscape ?? false;
                var relBase = landscape ? "/Video/Landscape" : "/Video/Portrait";

                var webRoot = string.IsNullOrWhiteSpace(_environment.WebRootPath)
                    ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
                    : _environment.WebRootPath;

                var physicalDir = Path.Combine(webRoot, "MediaStore", landscape ? "Landscape" : "Portrait");
                Directory.CreateDirectory(physicalDir);

                safeName = MakeSafeFileName(file.FileName); // slug + timestamp + estensione
                finalPath = Path.Combine(physicalDir, safeName);
                while (System.IO.File.Exists(finalPath))
                {
                    safeName = MakeSafeFileName(file.FileName);
                    finalPath = Path.Combine(physicalDir, safeName);
                }

                // Salva il file su disco
                await using (var stream = new FileStream(finalPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }

                var relativeUrl = Path.Combine(relBase, safeName).Replace("\\", "/");
                var absoluteUrl = $"{Request.Scheme}://{Request.Host}{relativeUrl}";

                // 1) Crea e salva l'entità su DB
                var entity = new AIVideo
                {
                    ID_Session = 1, // <--- usa un ID reale se necessario
                    Dir_Path = Path.Combine("Video", landscape ? "Landscape" : "Portrait").Replace("\\", "/"),
                    Url_Video = safeName, // solo il nome file, in linea con gli altri endpoint
                    IsLandscape = landscape,
                    Title = string.IsNullOrWhiteSpace(Title) ? file.FileName : Title,
                    DataCreation = DateTime.Now,
                    DataUpdate = DateTime.Now,
                    Creation_User = idUser,
                    IsDeleted = false,
                    Play_Priority = 99
                };

                await _unitOfWork.InsertAsync(entity);
                await _unitOfWork.SaveChangesAsync(); // da qui entity.Id è valorizzato

                // 2) Logga l'esito includendo l'ID
                log.ResponseCode = 201;
                log.ResponseMessage = "Created";
                log.ResponseBody = JsonSerializer.Serialize(new { success = true, relativeUrl, id = entity.Id });
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();

                // 3) Costruisci il payload *dopo* il salvataggio così da includere l'ID
                var payload = new
                {
                    id = entity.Id,
                    success = true,
                    relativeUrl,
                    absoluteUrl,
                    fileName = safeName,
                    size = file.Length,
                    orientation = landscape ? "Landscape" : "Portrait"
                };

                return Created(relativeUrl, payload);
            }
            catch (Exception ex)
            {
                // Se il DB fallisce dopo aver scritto il file, prova a rimuovere il file per non lasciarlo orfano
                if (!string.IsNullOrEmpty(finalPath) && System.IO.File.Exists(finalPath))
                {
                    try { System.IO.File.Delete(finalPath); } catch { /* best effort */ }
                }

                log.ResponseCode = 500;
                log.ResponseMessage = "Internal Server Error";
                log.ResponseBody = "{ success = false, message = '" + ex.Message + "' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();

                return StatusCode(500, new { success = false, message = $"Errore durante l'upload: {ex.Message}" });
            }
        }
        // --- Helpers (puoi metterli in fondo alla classe) ---

        private static readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".mp4", ".webm", ".mov", ".avi", ".mkv" };

        private static readonly HashSet<string> _allowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        { "video/mp4", "video/webm", "video/quicktime", "video/x-msvideo", "video/x-matroska" };

        private static string MakeSafeFileName(string original)
        {
            var name = Path.GetFileNameWithoutExtension(original) ?? "video";
            var ext = Path.GetExtension(original) ?? "";
            // keep [a-z0-9_-], sostituisci il resto con '-'
            var slug = Regex.Replace(name.ToLowerInvariant(), @"[^a-z0-9_-]+", "-").Trim('-');
            if (string.IsNullOrWhiteSpace(slug)) slug = "video";
            // timestamp per unicità leggibile
            return $"{slug}-{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext.ToLowerInvariant()}";
        }
        #endregion


        [Authorize]
        // POST: /api/video/add
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AIVideo video,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "POST",
                RequestUrl = "api/video/add",
                RequestBody = video?.ToString() ?? "",
                UserAgent = userAgent,
                AdditionalInfo = ""
            };

            // Validazioni base
            if (video == null || string.IsNullOrWhiteSpace(video.Url_Video))
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = "{ success = false, message = 'Video informations are required.' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = "Video informations are required." });
            }

            // In DB ID_Session è NOT NULL (int). Consideriamo non valido se <= 0 o se non esiste la sessione.
            if (video.ID_Session <= 0)
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = "{ success = false, message = 'Invalid or missing Session ID.' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = "Invalid or missing Session ID." });
            }

            var sessionExists = await _unitOfWork.Query<AdSession>()
                                                 .AnyAsync(s => s.Id == video.ID_Session && s.IsDeleted != true);
            if (!sessionExists)
            {
                log.ResponseCode = 400;
                log.ResponseMessage = "Bad Request";
                log.ResponseBody = "{ success = false, message = 'Invalid or missing Session ID.' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return BadRequest(new { success = false, message = "Invalid or missing Session ID." });
            }

            try
            {
                var landscape = video.IsLandscape ?? false;
                var initialPath = landscape ? "MediaStore/Landscape" : "MediaStore/Portrait";
                var videosDirectory = Path.Combine(_environment.WebRootPath, initialPath).Replace("\\", "/");
                var videosFile = Path.Combine(videosDirectory, video.Url_Video);

                Console.WriteLine($"Checking file at: {videosFile}");

                // Verifica esistenza file fisico
                if (!System.IO.File.Exists(videosFile))
                {
                    log.ResponseCode = 400;
                    log.ResponseMessage = "Bad Request";
                    log.ResponseBody = "{ success = false, message = 'The provided video path does not exist.' }";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return BadRequest(new { success = false, message = "The provided video path does not exist." });
                }

                // Persistenza: generici
                await _unitOfWork.InsertAsync(video);
                log.ResponseBody = "{ success = true, message = 'Video Added successfully.' }";
                log.ResponseCode = 200;
                log.ResponseMessage = "OK";

                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { success = true, message = "Video Added successfully." });
            }
            catch (Exception ex)
            {
                log.ResponseCode = 500;
                log.ResponseMessage = "Internal Server Error";
                log.ResponseBody = "{ success = false, message = '" + ex.Message + "' }";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();

                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
    int id,
    [FromBody] UpdateVideoRequest req,
    [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "PUT",
                RequestUrl = $"api/video/{id}",
                RequestBody = "", // evita di loggare payload completo se contiene path sensibili
                UserAgent = userAgent,
                AdditionalInfo = "Update video metadata"
            };

            var video = await _unitOfWork.GetByIdAsync<AIVideo>(id);
            if (video == null || video.IsDeleted == true)
            {
                log.ResponseCode = 404; log.ResponseMessage = "Not Found";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return NotFound();
            }

            // Se si cambia sessione, verifica che esista
            if (req.ID_Session.HasValue && req.ID_Session.Value > 0 && req.ID_Session.Value != video.ID_Session)
            {
                var sessionExists = await _unitOfWork.Query<AdSession>()
                                                     .AnyAsync(s => s.Id == req.ID_Session.Value && s.IsDeleted != true);
                if (!sessionExists)
                {
                    log.ResponseCode = 400; log.ResponseMessage = "Bad Request";
                    log.ResponseBody = "{ success = false, message = 'Invalid Session ID.' }";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return BadRequest(new { success = false, message = "Invalid Session ID." });
                }
                video.ID_Session = req.ID_Session.Value;
            }

            // Se si cambia orientamento o path, opzionalmente verifica l'esistenza del file fisico
            if (req.IsLandscape.HasValue) video.IsLandscape = req.IsLandscape.Value;
            if (!string.IsNullOrWhiteSpace(req.Url_Video))
            {
                // Verifica file (opzionale ma utile per coerenza con POST add)
                var landscape = video.IsLandscape ?? false;
                var initialPath = landscape ? "MediaStore/Landscape" : "MediaStore/Portrait";
                var videosDirectory = Path.Combine(_environment.WebRootPath, initialPath).Replace("\\", "/");
                var videosFile = Path.Combine(videosDirectory, req.Url_Video);
                if (!System.IO.File.Exists(videosFile))
                {
                    log.ResponseCode = 400; log.ResponseMessage = "Bad Request";
                    log.ResponseBody = "{ success = false, message = 'The provided video path does not exist.' }";
                    await _unitOfWork.InsertAsync(log);
                    await _unitOfWork.SaveChangesAsync();
                    return BadRequest(new { success = false, message = "The provided video path does not exist." });
                }

                video.Url_Video = req.Url_Video;
            }

            if (req.Title != null) video.Title = req.Title;
            if (req.Play_Priority.HasValue) video.Play_Priority = req.Play_Priority.Value;
            if (req.IsDeleted.HasValue)
            {
                video.IsDeleted = req.IsDeleted.Value;
                if (req.IsDeleted == true)
                {
                    video.DeletionTime = DateTime.Now;
                }
            }

            video.DataUpdate = DateTime.Now;
            // video.LastModification_User = <callerId se disponibile>

            _unitOfWork.Update(video);

            var session = await _unitOfWork.GetByIdAsync<AdSession>(video.ID_Session);
            video.Session = session;


            log.ResponseCode = 200; log.ResponseMessage = "OK";
            await _unitOfWork.InsertAsync(log);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { success = true });
        }


        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            [FromHeader(Name = "User-Agent")] string userAgent)
        {
            var log = new WebAPILog
            {
                DateTimeStamp = DateTime.Now,
                RequestMethod = "DELETE",
                RequestUrl = $"api/video/{id}",
                RequestBody = "",
                UserAgent = userAgent,
                AdditionalInfo = "Soft delete video"
            };

            var video = await _unitOfWork.GetByIdAsync<AIVideo>(id);
            if (video == null || video.IsDeleted == true)
            {
                log.ResponseCode = 404; log.ResponseMessage = "Not Found";
                await _unitOfWork.InsertAsync(log);
                await _unitOfWork.SaveChangesAsync();
                return NotFound();
            }

            video.IsDeleted = true;
            video.DeletionTime = DateTime.Now;
            // video.Deletion_User = <callerId se disponibile>

            _unitOfWork.Update(video);

            log.ResponseCode = 204; log.ResponseMessage = "No Content";
            await _unitOfWork.InsertAsync(log);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var v = await _unitOfWork.GetByIdAsync<AIVideo>(id);
            if (v == null || v.IsDeleted == true)
                return NotFound();

            // arricchisco con public url (senza toccare l’entità)
            var publicUrl = ((v.IsLandscape ?? false) ? "/Media/landscape" : "/Media/portrait")  + "/" + (v.Url_Video ?? string.Empty);
            var urlVideo = _videoHelper.BuildPublicUrl(v.Url_Video, v.IsLandscape ?? false);
            var dto = new VideoListItemDto
            {
                Id = v.Id,
                ID_Session = v.ID_Session,
                Title = v.Title,
                Url_Video = urlVideo,
                IsLandscape = v.IsLandscape,
                Play_Priority = v.Play_Priority,
                DataCreation = v.DataCreation,
                PublicUrl = publicUrl.Replace("\\", "/")
            };

            List<AdSession> sessions = await _unitOfWork
                .Query<AdSession>()
                .Where(s => s.Id == v.ID_Session)                
                .ToListAsync();
            if (sessions.Count > 0)
            {
                dto.Sessions = sessions;
            }

            return Ok(dto);
        }


        // GET: /api/video?sessionId=&landscape=&q=&page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] int? sessionId = null,
            [FromQuery] bool? landscape = null,
            [FromQuery] string? q = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            page = page <= 0 ? 1 : page;
            pageSize = Math.Clamp(pageSize, 1, 100);

            var qry = _unitOfWork.Query<AIVideo>()  // IQueryable<AIVideo> (AsNoTracking)
                                 .Where(v => v.IsDeleted != true);

            if (sessionId.HasValue)
                qry = qry.Where(v => v.ID_Session == sessionId.Value);

            if (landscape.HasValue)
                qry = qry.Where(v => v.IsLandscape == landscape.Value);

            if (!string.IsNullOrWhiteSpace(q))
                qry = qry.Where(v => v.Title != null && v.Title.Contains(q));

            var total = await qry.CountAsync();

            // ordinamento predefinito per priorità, poi data creazione
            var items = await qry
                .OrderBy(v => v.Play_Priority ?? int.MaxValue)
                .ThenByDescending(v => v.DataCreation)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new VideoListItemDto
                {
                    Id = v.Id,
                    ID_Session = v.ID_Session,
                    Title = v.Title,
                    Url_Video = v.Url_Video,
                    IsLandscape = v.IsLandscape,
                    Play_Priority = v.Play_Priority,
                    DataCreation = v.DataCreation,
                    // costruisco il public url
                    PublicUrl = ((v.IsLandscape ?? false) ? "/Media/Landscape" : "/Media/Portrait")
                                + "/"
                                + (v.Url_Video ?? string.Empty)
                })
                .ToListAsync();

            // normalizza eventuali backslash nel public url
            foreach (var it in items)
            {
                it.PublicUrl = it.PublicUrl.Replace("\\", "/");
                it.Sessions = new List<AdSession>();
                var sessions = await _unitOfWork
                    .Query<AdSession>()
                    .Where(s => s.Id == it.ID_Session)                    
                    .ToListAsync();
                if (sessions.Count > 0)
                {
                    it.Sessions = sessions;
                }

            }

            return Ok(new PagedResult<VideoListItemDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                Total = total
            });
        }
        // GET: /api/video/all?includeDeleted=false
        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var qry = _unitOfWork.Query<AIVideo>();

            if (!includeDeleted)
                qry = qry.Where(v => v.IsDeleted != true);

            var items = await qry
                .AsNoTracking()
                .OrderBy(v => v.Play_Priority ?? int.MaxValue)
                .ThenByDescending(v => v.DataCreation)
                .Select(v => new VideoListItemDto
                {
                    Id = v.Id,
                    ID_Session = v.ID_Session,
                    Title = v.Title,
                    Url_Video = v.Url_Video,
                    IsLandscape = v.IsLandscape,
                    Play_Priority = v.Play_Priority,
                    DataCreation = v.DataCreation,
                    PublicUrl = ((v.IsLandscape ?? false) ? "/Video/Landscape" : "/Video/Portrait")
                                + "/"
                                + (v.Url_Video ?? string.Empty)
                })
                .ToListAsync();

            foreach (var it in items)
            {
                it.PublicUrl = it.PublicUrl.Replace("\\", "/");
                it.Sessions = new List<AdSession>();
                var sessions = await _unitOfWork
                    .Query<AdSession>()
                    .Where(s => s.Id == it.ID_Session)
                    .ToListAsync();
                if (sessions.Count > 0)
                {
                    it.Sessions = sessions;
                }
            }
                return Ok(items);
        }


        //// GET: /api/video/random
        //[HttpGet("random")]
        //public IActionResult GetRandomVideo([FromHeader(Name = "User-Agent")] string userAgent)
        //{
        //    var log = new WebAPILog
        //    {
        //        DateTimeStamp = DateTime.Now,
        //        RequestMethod = "POST",
        //        RequestUrl = "api/email/sendemail",
        //        RequestBody = "",
        //        UserAgent = userAgent,
        //        AdditionalInfo = ""
        //    };
        //    var randomVideo = _videoHelper.GetRandomVideo(log);
        //    return Ok(randomVideo);
        //}

        //[HttpGet("playlist")]
        //public IActionResult GetPlaylist([FromQuery] int landscape)
        //{
        //    try
        //    {
        //        var includeLandscape = false;
        //        // Implement your logic to fetch playlist based on the provided landscape parameter
        //        List<AIVideo> playlist = new List<AIVideo>();
        //        var initialPath = "/Video";
        //        // E.g., Fetch data from database or another service
        //        if (landscape == 1)
        //        {
        //            initialPath = initialPath + "/Landscape";
        //            //playlist = _unitOfWork.AIVideo.GetPlaylistLandScape();
        //            includeLandscape = true;
        //        }                
        //        else {
        //            initialPath = initialPath + "/Portrait";
        //            includeLandscape = false;
        //            //playlist = _unitOfWork.AIVideo.GetPlaylist();               
        //        }
        //        playlist = _unitOfWork.AIVideo.GetPlaylist(includeLandscape);
        //        // Return the playlist data as JSON

        //        foreach (var video in playlist)
        //        {
        //            // Combina il percorso relativo
        //            video.Url_Video = Path.Combine(initialPath, video.Url_Video).Replace("\\", "/");
        //        }
        //        return Ok(playlist);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error for debugging purposes
        //        Console.Error.WriteLine($"An error occurred while processing the request: {ex}");

        //        // Return a generic error response
        //        return StatusCode(500, new { answer = "Si è verificato un errore durante la richiesta. Riprova più tardi." });
        //    }
        //}


        //[HttpPost("add")]
        //public IActionResult Add([FromBody] AIVideo video)
        //{
        //    //crea un oggetto WebAPILog e lo inizializza
        //    var log = new WebAPILog
        //    {
        //        DateTimeStamp = DateTime.Now,
        //        RequestMethod = "POST",
        //        RequestUrl = "api/video/add",
        //        RequestBody = video.ToString(),
        //        AdditionalInfo = ""
        //    };

        //    var landscape = video.IsLandscape ?? false;

        //    if (video == null || string.IsNullOrEmpty(video.Url_Video))
        //    {
        //        log.ResponseCode = 400;
        //        log.ResponseMessage = "Bad Request";
        //        log.ResponseBody = "{ success = false, message = 'Video informations are required.' }";
        //        return BadRequest(new { success = false, message = "Video informations are required." });
        //    }
        //    if (video.ID_Session == null || !_unitOfWork.ADSession.Exists(video.ID_Session))
        //    {
        //        log.ResponseCode = 400;
        //        log.ResponseMessage = "Bad Request";
        //        log.ResponseBody = "{ success = false, message = 'Invalid or missing Session ID.' }";
        //        _unitOfWork.WebAPILog.AddLog(log);
        //        return BadRequest(new { success = false, message = "Invalid or missing Session ID." });
        //    }
        //    try
        //    {
        //        var initialPath = "Video";
        //        if (landscape)
        //        {
        //            initialPath = initialPath + "/Landscape";
        //        }
        //        else
        //        {
        //            initialPath = initialPath + "/Portrait";          
        //        }
        //        var videosDirectory = Path.Combine(_environment.WebRootPath, initialPath).Replace("\\", "/");
        //        var videosFile = Path.Combine(videosDirectory, video.Url_Video);

        //        // Log the full file path for debugging
        //        Console.WriteLine($"Checking file at: {videosFile}");

        //        // Controlla se il percorso del file è valido
        //        if (!System.IO.File.Exists(videosFile))
        //        {
        //            log.ResponseCode = 400;
        //            log.ResponseMessage = "Bad Request";
        //            log.ResponseBody = "{ success = false, message = 'The provided video path does not exist.' }";
        //            _unitOfWork.WebAPILog.AddLog(log);
        //            return BadRequest(new { success = false, message = "The provided video path does not exist." });
        //        }
        //        _unitOfWork.AIVideo.AddVideo(video);
        //        log.ResponseBody = "{success = true, message = 'Video Added successfully.' }";
        //        log.ResponseCode = 200;
        //        log.ResponseMessage = "OK";
        //        _unitOfWork.WebAPILog.AddLog(log);
        //        return Ok(new { success = true, message = "Video Added successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        log.ResponseCode = 500;
        //        log.ResponseMessage = $"Si è verificato un errore: {ex.Message}";
        //        _unitOfWork.WebAPILog.AddLog(log);
        //        return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
        //    }
        //}


    }
}
