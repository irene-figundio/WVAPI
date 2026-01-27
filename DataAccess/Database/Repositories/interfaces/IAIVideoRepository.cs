using AI_Integration.DataAccess.Database.Models;

namespace AI_Integration.DataAccess.Database.Repositories.interfaces
{
    public interface IAIVideoRepository
    {
        List<AIVideo> GetPlaylist(bool includeLandscape);
        List<AIVideo> GetVideosBySession(int sessionId);
        List<AIVideo> GetAllActiveVideos();

        //public List<AIVideo> GetPlaylist();
        //public List<AIVideo> GetPlaylistLandScape();
        //public List<AIVideo> GetPlaylist(bool includeLandscape);
        //void AddVideo(AIVideo video);
        //IEnumerable<AIVideo> GetAll();
        //public Boolean Exists(int videoId);

        //void RemoveVideo(AIVideo video);
        //void UpdateVideo(AIVideo video);
        //void DeleteVideoById(int id);
        //void DeleteAll();

        //void AddVideoList(List<AIVideo> videoList);
        //void DeleteVideoBySessionId(int sessionId);

        //List<AIVideo> GetVideoBySessionId(int sessionId);

        //List<AIVideo> GetAllActiveVideos();



    }
}
