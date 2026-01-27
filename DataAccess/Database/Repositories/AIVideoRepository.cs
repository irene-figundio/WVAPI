using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using AI_Integration.Model;
using Microsoft.EntityFrameworkCore;

namespace AI_Integration.DataAccess.Database.Repositories
{
    public class AIVideoRepository : IAIVideoRepository
    {
        private readonly ApplicationDbContext _db;

        public AIVideoRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<AIVideo> GetPlaylist(bool includeLandscape)
        {
            return _db.Set<AIVideo>()
                       .Where(v => v.IsDeleted != true && v.IsLandscape == includeLandscape)
                       .OrderBy(v => v.Play_Priority ?? int.MaxValue)
                       .ToList();
        }

        public List<AIVideo> GetVideosBySession(int sessionId)
        {
            return _db.Set<AIVideo>()
                       .Where(v => v.ID_Session == sessionId && v.IsDeleted != true)
                       .OrderBy(v => v.Play_Priority ?? int.MaxValue)
                       .ToList();
        }

        public List<AIVideo> GetAllActiveVideos()
        {
            var now = DateTime.Now;
            return (from v in _db.Set<AIVideo>()
                    join s in _db.Set<AdSession>() on v.ID_Session equals s.Id
                    join c in _db.Set<AdCampaign>() on s.ID_Campaing equals c.Id
                    where v.IsDeleted != true
                       && s.StartDate <= now && s.EndDate >= now
                       && c.StartDate <= now && c.EndDate >= now
                       && (s.IsDeleted == null || s.IsDeleted == false)
                       && (c.IsDeleted == null || c.IsDeleted == false)
                    select v).ToList();
        }

        //    public void AddVideo(AIVideo video)
        //    {
        //        _db.AIVideo.Add(video);
        //        _db.SaveChanges();
        //    }

        //    public IEnumerable<AIVideo> GetAll()
        //    {
        //        return _db.AIVideo.ToList();
        //    }

        //    public List<AIVideo> GetPlaylistLandScape()
        //    {
        //        var playlist = _db.AIVideo
        //         .Where(video => video.IsDeleted != true && video.IsLandscape == true)
        //         .OrderBy(video => video.Play_Priority)
        //         .Select(video => new AIVideo
        //         {
        //             Id = video.Id,
        //             Dir_Path = video.Dir_Path,
        //             Title = video.Title,
        //             Url_Video = video.Url_Video,
        //             Play_Priority = video.Play_Priority
        //         })
        //         .ToList();
        //        return playlist;
        //    }

        //    public Boolean Exists(int videoId)
        //    {
        //        if (GetByVideoId(videoId) != null)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }

        //    private AIVideo GetByVideoId(int videoId)
        //    {
        //        return _db.AIVideo.FirstOrDefault(x => x.Id == videoId);
        //    }

        //    public List<AIVideo> GetPlaylist()
        //    {
        //        //var playlist = _db.AIVideo
        //        // .Where(video => video.IsDeleted != true && video.IsLandscape != true)
        //        // .OrderBy(video => video.Play_Priority)
        //        // .Select(video => new AIVideo
        //        // {
        //        //     Id = video.Id,
        //        //     Dir_Path = video.Dir_Path,
        //        //     Title = video.Title,
        //        //     Url_Video = video.Url_Video,
        //        //     Play_Priority = video.Play_Priority
        //        // })
        //        // .ToList();
        //        // Imposta startDate all'istante corrente
        //        DateTime startDate = DateTime.Now;  // Ottiene la data e l'ora corrente
        //        DateTime endDate = startDate.AddHours(6);  // aggiunge 6 ore            

        //        var playlist = _db.AIVideo
        //            .Join(_db.AdSession,  // Unisci con AdSession
        //                  video => video.ID_Session,  // Chiave esterna in AIVideo
        //                  session => session.Id,  // Chiave primaria in AdSession
        //                  (video, session) => new { Video = video, Session = session }) // Risultato della join
        //            .Join(_db.AdCampaign,  // Unisci con AdCampaign
        //                  vs => vs.Session.ID_Campaing,  // Chiave esterna in AdSession
        //                  campaign => campaign.Id,  // Chiave primaria in AdCampaign
        //                  (vs, campaign) => new { vs.Video, vs.Session, Campaign = campaign }) // Risultato della join
        //            .Where(vsc =>
        //                (!vsc.Video.IsDeleted.HasValue || vsc.Video.IsDeleted == false)  // Verifica che il video non sia eliminato
        //                && (!vsc.Video.IsLandscape.HasValue || vsc.Video.IsLandscape == false)  // Verifica che il video non sia Landscape
        //                && vsc.Session.StartDate <= endDate  // La sessione inizia entro la fine del periodo specificato
        //                && vsc.Session.EndDate >= startDate)  // La sessione finisce dopo l'inizio del periodo specificato
        //            .OrderBy(vsc => vsc.Video.Play_Priority)  // Ordina per priorità di riproduzione
        //            .ThenByDescending(vsc => vsc.Campaign.Budget)  // Poi ordina per budget decrescente della campagna
        //            .Select(vsc => vsc.Video)  // Seleziona solo i dettagli del video
        //            .ToList();

        //        return playlist;
        //    }

        //    public List<AIVideo> GetPlaylist(bool includeLandscape)
        //    {
        //        // Imposta startDate all'istante corrente
        //        DateTime startDate = DateTime.Now;  // Ottiene la data e l'ora corrente
        //        DateTime endDate = startDate.AddHours(6);  // aggiunge 6 ore            

        //        var playlist = _db.AIVideo
        //            .Join(_db.AdSession,  // Unisci con AdSession
        //                  video => video.ID_Session,  // Chiave esterna in AIVideo
        //                  session => session.Id,  // Chiave primaria in AdSession
        //                  (video, session) => new { Video = video, Session = session }) // Risultato della join
        //            .Join(_db.AdCampaign,  // Unisci con AdCampaign
        //                  vs => vs.Session.ID_Campaing,  // Chiave esterna in AdSession
        //                  campaign => campaign.Id,  // Chiave primaria in AdCampaign
        //                  (vs, campaign) => new { vs.Video, vs.Session, Campaign = campaign }) // Risultato della join
        //            .Where(vsc =>
        //                (!vsc.Video.IsDeleted.HasValue || vsc.Video.IsDeleted == false)  // Verifica che il video non sia eliminato
        //                && (!vsc.Video.IsLandscape.HasValue || vsc.Video.IsLandscape == includeLandscape)  // Usa il parametro landscape
        //                && vsc.Session.StartDate <= endDate  // La sessione inizia entro la fine del periodo specificato
        //                && vsc.Session.EndDate >= startDate)  // La sessione finisce dopo l'inizio del periodo specificato
        //            .OrderBy(vsc => vsc.Video.Play_Priority)  // Ordina per priorità di riproduzione
        //            .ThenByDescending(vsc => vsc.Campaign.Budget)  // Poi ordina per budget decrescente della campagna
        //            .Select(vsc => vsc.Video)  // Seleziona solo i dettagli del video
        //            .ToList();

        //        return playlist;
        //    }


        //    public void RemoveVideo(AIVideo video)
        //    {
        //        _db.AIVideo.Remove(video);
        //        _db.SaveChanges();
        //    }

        //    public void UpdateVideo(AIVideo video)
        //    {
        //        _db.AIVideo.Update(video);
        //        _db.SaveChanges();
        //    }

        //    public void DeleteVideoById(int id)
        //    {
        //        _db.AIVideo.RemoveRange(_db.AIVideo.Where(x => x.Id == id));
        //        _db.SaveChanges();
        //    }

        //    public void DeleteAll()
        //    {
        //        _db.AIVideo.RemoveRange(_db.AIVideo);
        //        _db.SaveChanges();
        //    }

        //    public void AddVideoList(List<AIVideo> videoList)
        //    {
        //        _db.AIVideo.AddRange(videoList);
        //        _db.SaveChanges();
        //    }

        //    public void DeleteVideoBySessionId(int sessionId)
        //    {
        //        _db.AIVideo.RemoveRange(_db.AIVideo.Where(x => x.Id == sessionId));
        //        _db.SaveChanges();
        //    }

        //    public List<AIVideo> GetVideoBySessionId(int sessionId)
        //    {
        //        return _db.AIVideo.Where(x => x.Id == sessionId).ToList();
        //    }



        //    public List<AIVideo> GetAllActiveVideos()
        //    {
        //        var activeVideos = (from video in _db.AIVideo
        //                              join session in _db.AdSession on video.ID_Session equals session.Id
        //                              join campaign in _db.AdCampaign on session.ID_Campaing equals campaign.Id
        //                              where video.IsDeleted != true
        //                                    && session.EndDate < DateTime.Now && session.StartDate > DateTime.Now
        //                                    && campaign.EndDate < DateTime.Now && campaign.StartDate > DateTime.Now
        //                              select video).ToList();
        //        return activeVideos;
        //    }
        //}
    }
}