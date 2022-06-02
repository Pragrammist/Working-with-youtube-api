using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace YouTubePlaylist
{
    /// <summary>
    /// YouTube Data API v3 sample: create a playlist.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://developers.google.com/api-client-library/dotnet/get_started
    /// </summary>
    internal class PlaylistUpdates
    {
        [STAThread]
        static void Main(string[] args)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            int number = 6;

            Task task1 = new Task(() =>
            {
                int result = 1;
                for (int i = 1; i <= number; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Операция прервана");
                        return;
                    }

                    result *= i;
                    Console.WriteLine($"Факториал числа {number} равен {result}");
                    Thread.Sleep(5000);
                }
            });
            task1.Start();

            Console.WriteLine("Введите Y для отмены операции или другой символ для ее продолжения:");
            string s = Console.ReadLine();
            if (s == "Y")
                cancelTokenSource.Cancel();

            Console.Read();



        }
        private void Run()
        {
            //string path = @"C:\Users\F\source\repos\YouTubePlaylist\YouTubePlaylist\client_secrets.json";
            //YoutubeServiceFactory factory = new YoutubeServiceFactory(path);
            //var serv = factory.GetYtService();
            //ResponsePlaylists playlists = new ResponsePlaylists(serv, "snippet, contentDetails");

            //VideoAccesser accesser = new VideoAccesser(serv);
            //accesser.PlaylistId = "PLsQnxJ8haxTaqF-vjO3Kt5L63T78EM_0F";
            //var c = "PLsQnxJ8haxTaqF-vjO3Kt5L63T78EM_0F".Length;

            //var video = accesser[299];
            //var videos = accesser.GetVideoIds();
            //var d = video.Length;
            //Session session = new Session();
            //SavedPlaylists saved = new SavedPlaylists();


            //VideoAccesser accesser = new VideoAccesser();



            //PlaylistsService playlist = new PlaylistsService(saved, session, playlists);




            //VideoService video = new VideoService(accesser);


            









        }



        
    }
    internal class Hap
    {
        internal string Her { get; set; }
        internal string Hep { get; set; }
        internal string HeD { get; set; }
        
    }
    
    public class YoutubeServiceFactory
    {
        UserCredential _credential;
        string path = ""; string id = ""; string secret = "";
        public YoutubeServiceFactory(string path = "")
        {
            this.path = path;
        }
        public YoutubeServiceFactory(string id, string secret)
        {
            this.secret = secret;
            this.id = id;
        }
        private async Task<UserCredential> GetCredentialFromPath(string path)
        {
            UserCredential credential = null;
            try
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromFile(path).Secrets,
                    // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                    // user's account, but not other types of account access.
                    new[] { YouTubeService.Scope.YoutubeReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return credential;
        }

        private async Task<UserCredential> GetCredential(string id, string secret)
        {
            UserCredential credential = null;
            try
            {
                var t = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets { ClientId = id, ClientSecret = secret },
                    // This OAuth 2.0 access scope allows for read-only access to the authenticated 
                    // user's account, but not other types of account access.
                    new[] { YouTubeService.Scope.YoutubeReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
                credential = await t;
                
            }
            catch 
            {
                
            }
            _credential = credential;
            return credential;
        }
        public YouTubeService GetYtService()
        {
            UserCredential credential;

            if (_credential == null) {
                if (id != "" && secret != "")
                {
                    credential = GetCredential(id, secret).Result;
                }
                else if (path != "")
                {
                    credential = GetCredentialFromPath(path).Result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                credential = _credential;
            }
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            return youtubeService;
        }

        public bool CheckToken(string id, string secret)
        {
            var creditial = GetCredential(id, secret);
            if (creditial.Result is null)
            {
                return false;
            }
            //_credential = creditial;
            return true;
        }
        
    }

    public abstract class PlaylistsServiceBase
    {
        public ISession Session { get; protected set; }
        public ISavedPlaylists SavedPlaylists { get; protected set; }
        public IResponsePlaylists ResponsePlaylists { get; protected set; }

        protected PlaylistsServiceBase(ISavedPlaylists getSaved, ISession session, IResponsePlaylists rPlaylists)
        {
            Session = session;
            ResponsePlaylists = rPlaylists;
            SavedPlaylists = getSaved;
            
        }

        
    }

    public abstract class ServiceFactoryBase
    {
        protected ISavedPlaylists _saved;
        protected ISession _session;
        protected IResponsePlaylists _playlists; 
        protected IVideoAccesser _accesser;
        protected ServiceFactoryBase(ISavedPlaylists saved, ISession session, IResponsePlaylists playlists, IVideoAccesser accesser)
        {
            _saved = saved;
            _session = session;
            _playlists = playlists;
            _accesser = accesser;
        }

        public abstract PlaylistsServiceBase GetPlaylists();
        public abstract VideoServiceBase GetVideo();
    }


    public class ServiceFactory : ServiceFactoryBase
    {
        public ServiceFactory(YouTubeService service, ISavedPlaylists saved) : base(saved, new Session(), new ResponsePlaylists(service, "snippet, contentDetails"), new VideoAccesser(service))
        {

        }

        public override PlaylistsServiceBase GetPlaylists()
        {
            PlaylistsService service = new PlaylistsService(_saved, _session, _playlists);
            return service;
        }

        public override VideoServiceBase GetVideo()
        {
            VideoService service = new VideoService(_accesser);
            return service;
        }
    }

    public abstract class VideoServiceBase
    {
        public abstract IVideoAccesser Accesser { get; }
        protected VideoServiceBase(IVideoAccesser accesser)
        {
            
        }

        
        
    }

    public class VideoService : VideoServiceBase
    {

        IVideoAccesser _accesser;

        

        public VideoService(IVideoAccesser playlistItems) : base(playlistItems)
        {
            
            _accesser = playlistItems;
        }

        public override IVideoAccesser Accesser { get => _accesser; }

       

        public void SetPartForVideos(string part)
        {
            _accesser.Part = part;
        }

        public void SetPlaylistId(string id)
        {
            _accesser.PlaylistId = id;
        }

        public IEnumerable<string> GetPlaylistItemsIds()
        {
            return _accesser.GetVideoIds();
        }

        public string GetVideoImg(string id)
        {
            var v = _accesser[id];
            return v.Snippet.Thumbnails.Default__.Url;
        }
    }

    public class PlaylistsService : PlaylistsServiceBase
    {
        
        public PlaylistsService(ISavedPlaylists getSaved, ISession session, IResponsePlaylists rPlaylists) : base(getSaved, session, rPlaylists)
        {
            
        }

        public bool AddToAddPlaylistSession(string id)
        {
            return Session.AddPlaylistToAdd(id);
        }

        public bool AddToRemovePlaylistSession(string id)
        {
            return Session.AddPlaylistToRemove(id);
        }
        public bool ExecuteChanges()
        {
            var res = SavedPlaylists.AddPlaylists(Session.PlaylistsAdd);
            var res1 = SavedPlaylists.DeletePlaylists(Session.PlaylistsDelete);

            return res || res1;
        }

        public string GetUrlImgPlaylist(string id)
        {
            var pl = ResponsePlaylists.GetPlaylistById(id);
            var url = pl.Snippet.Thumbnails.Default__.Url;
            return url;
        }

    }

    public interface IPartable
    {
        string Part { get; set; }
    }

    public interface IPlaylistable<T> : IEnumerable<T>
    {
        public IEnumerable<T> GetPlaylists();
    }

    public interface IPlaylistableIds : IPlaylistable<string>
    {

    }

    public interface IPlaylistableObjects : IPlaylistable<Playlist>
    {
        
    }

    public interface ISavedPlaylists : IPlaylistableIds
    {
        public IEnumerable<string> SavedPlaylists { get; }
        public bool AddPlaylists(IEnumerable<string> playlistsId);
        public bool AddPlaylist(string playlistId);
        public bool DeletePlaylist(string playlistId);
        public bool DeletePlaylists(IEnumerable<string> playlistsId);
    }

    public interface ISession : IPlaylistableIds
    {
        IEnumerable<string> PlaylistsDelete { get; }
        IEnumerable<string> PlaylistsAdd { get; }

        // add to second collection(PlaylistsAdd)
        public bool AddPlaylistToAdd(string playlistId);
        public bool AddPlaylistsToAdd(IEnumerable<string> playlistsIds);
        public bool RemovePlaylistToAdd(string playlistId);
        public bool RemovePlaylistsToAdd(IEnumerable<string> playlistsIds);
        // add to first collection(PlaylistsDelete)
        public bool AddPlaylistToRemove(string playlistId);
        public bool AddPlaylistsToRemove(IEnumerable<string> playlistsIds);
        public bool RemovePlaylistToRemove(string playlistId);
        public bool RemovePlaylistsToRemove(IEnumerable<string> playlistsIds);
    }

    public interface IResponsePlaylists : IPlaylistableObjects, IPartable
    {
        IEnumerable<Playlist> Playlists { get; }
        public Task<IEnumerable<Playlist>> GetPlaylistsAsync();

        public string GetPlaylistId(string name);
        public string GetPlaylistName(string id);
        public Playlist GetPlaylistById(string id);
        public Playlist GetPlaylistByName(string name);
        public IEnumerable<string> GetNamesAllPlaylists();
    }

    public interface IVideoAccesser: IPartable, IEnumerable<PlaylistItem>
    {
        string PlaylistId { get; set; }
        int Count { get; }
        IEnumerable<PlaylistItem> GetPlaylistItems();
        IEnumerable<string> GetVideoIds();
        string this[int num] { get; }
        PlaylistItem this[string id] { get; }        
    }

    public class ResponsePlaylists : IResponsePlaylists
    {

        public ResponsePlaylists(YouTubeService service, string part)
        {
            _service = service;
            Part = part;
        }

        YouTubeService _service;
        IEnumerable<Playlist> _playlists;

        public IEnumerable<Playlist> Playlists => _playlists;

        public string Part { get; set; } = "snippet";

        public IEnumerator<Playlist> GetEnumerator()
        {
            _playlists = _playlists ?? GetPlaylists();
            return _playlists.GetEnumerator();
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsAsync()
        {
            var part = Part;
            
            if (part == "")
            {
                return null;
            }


            string pageToken;
            List<Playlist> playlists = new List<Playlist>();
            do
            {
                pageToken = null;
                var playlistListRequest = _service.Playlists.List(part);
                playlistListRequest.PageToken = pageToken;
                playlistListRequest.MaxResults = 50;

                playlistListRequest.Mine = true;

                // Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
                var playlistListResponse = await playlistListRequest.ExecuteAsync();

                Playlist[] buffer = new Playlist[50]; // 50 - max num of playlists in response


                for (int i = 0; i < playlistListResponse.Items.Count; i++)
                {
                    var itms = playlistListResponse.Items[i];
                    buffer[i] = itms;
                }
                pageToken = playlistListResponse.NextPageToken;
                playlists.AddRange(buffer);
            } while (pageToken != null);
            var arr = new Playlist[playlists.Count];
            playlists.CopyTo(arr);
            _playlists = arr;
            return playlists;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {

            _playlists = _playlists ?? GetPlaylists();
            return _playlists.GetEnumerator();
        }

        public IEnumerable<Playlist> GetPlaylists()
        {
            var part = Part;
            

            if (part == "")
            {
                return null;
            }

            if (_playlists != null)
            {
                return _playlists;
            }

            string pageToken;
            List<Playlist> playlists = new List<Playlist>();
            do
            {
                pageToken = null;
                var playlistListRequest = _service.Playlists.List(part);
                playlistListRequest.PageToken = pageToken;
                playlistListRequest.MaxResults = 50;

                playlistListRequest.Mine = true;

                // Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
                var playlistListResponse = playlistListRequest.Execute();

                Playlist[] buffer = new Playlist[50]; // 50 - max num of playlists in response


                for (int i = 0; i < playlistListResponse.Items.Count; i++)
                {
                    var itms = playlistListResponse.Items[i];
                    buffer[i] = itms;
                }
                pageToken = playlistListResponse.NextPageToken;
                playlists.AddRange(buffer);
            } while (pageToken != null);
            var arr = new Playlist[playlists.Count];
            playlists.CopyTo(arr);
            _playlists = arr;
            return playlists;
        }

        public string GetPlaylistId(string name)
        {
            return GetPlaylistByName(name)?.Snippet?.Title;
        }

        public string GetPlaylistName(string id)
        {
            return GetPlaylistById(id)?.Snippet?.Title;
        }

        public Playlist GetPlaylistById(string id)
        {
            var p = _service.Playlists.List(Part);
            p.Id = id;
            var res = p.Execute();
            var pl = res.Items.FirstOrDefault();
            return pl;
        }

        public Playlist GetPlaylistByName(string name)
        {
            GetPlaylists();

            var t =_playlists.First(p => p.Snippet.Title == name);
            return t;
        }

        public IEnumerable<string> GetNamesAllPlaylists()
        {
            return _playlists.Select(y => y.Snippet.Title).ToList();
        }
    }

    public class Session : ISession
    {
        public enum IEnumeratorBreacker { add , delete };
        public IEnumeratorBreacker Breacker { get; set; } = IEnumeratorBreacker.add;
        public Session()
        {
            _playlistsAdd = new List<string>();
            _playlistsDelete = new List<string>();
        }

        List<string> _playlistsDelete;
        List<string> _playlistsAdd;
        public IEnumerable<string> PlaylistsDelete => _playlistsDelete.AsReadOnly();

        public IEnumerable<string> PlaylistsAdd => _playlistsAdd.AsReadOnly();

        public bool AddPlaylistsToAdd(IEnumerable<string> playlistsIds)
        {
            _playlistsAdd.AddRange(playlistsIds);

            return true;
        }

        public bool AddPlaylistsToRemove(IEnumerable<string> playlistsIds)
        {
            _playlistsDelete.AddRange(playlistsIds);

            return true;
        }

        public bool AddPlaylistToAdd(string playlistId)
        {
            _playlistsAdd.Add(playlistId);
            return true;
        }

        public bool AddPlaylistToRemove(string playlistId)
        {
            _playlistsDelete.Add(playlistId);
            return true; 
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (Breacker == 0)
            {
                return _playlistsAdd.GetEnumerator();
            }
            else
            {
                return _playlistsDelete.GetEnumerator();
            }
        }

        public IEnumerable<string> GetPlaylists()
        {

            if (Breacker == 0)
            {
                return _playlistsAdd.AsReadOnly();
            }
            else
            {
                return _playlistsDelete.AsReadOnly();
            }
        }

        public bool RemovePlaylistsToAdd(IEnumerable<string> playlistsIds)
        {
            return false;
        }

        public bool RemovePlaylistsToRemove(IEnumerable<string> playlistsIds)
        {
            return false;
        }

        public bool RemovePlaylistToAdd(string playlistId)
        {
            return _playlistsAdd.Remove(playlistId);
        }

        public bool RemovePlaylistToRemove(string playlistId)
        {
            return _playlistsDelete.Remove(playlistId);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Breacker == 0)
            {
                return _playlistsAdd.GetEnumerator();
            }
            else
            {
                return _playlistsDelete.GetEnumerator();
            }
        }
    }

    public class VideoAccesser : IVideoAccesser
    {
        List<PlaylistItem> videos = null;
        private bool idHaveChanged = false;
        private string _id = string.Empty;
        YouTubeService _service;
        public VideoAccesser(YouTubeService service)
        {
            _service = service;
        }
        public string this[int num] { get => GetVideoIds()?.ElementAtOrDefault(num); }

        public PlaylistItem this[string id] { get
            {
                if (id == null || PlaylistId == null || PlaylistId == string.Empty || id == string.Empty)
                    return null;
                var list = _service.PlaylistItems.List(Part);
                list.PlaylistId = PlaylistId;
                list.VideoId = id;
                var exc = list.Execute();
                return exc.Items.FirstOrDefault();

            }
        }

        public string PlaylistId { get => _id; set 
            {
                idHaveChanged = true;
                _id = value;
            } 
        }

        public int Count => videos?.Count ?? 0;

        public string Part { get; set; } = "snippet, contentDetails";

        public IEnumerator<PlaylistItem> GetEnumerator()
        {
            return GetVideos().GetEnumerator();
        }

        public IEnumerable<PlaylistItem> GetPlaylistItems()
        {
            return GetVideos();
        }

        public IEnumerable<string> GetVideoIds()
        {
            return GetVideos()?.Select(v => v?.ContentDetails?.VideoId);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetVideos().GetEnumerator();
        }



        
        private IEnumerable<PlaylistItem> GetVideos()
        {
            
            if ((videos == null || idHaveChanged) && _id != string.Empty) {
                string pageToken = null;
                videos = new List<PlaylistItem>();
                var list = _service.PlaylistItems.List(Part);
                list.PlaylistId = PlaylistId;
                list.MaxResults = 50;
                do {

                    
                    list.PageToken = pageToken;
                    
                    var exc = list.Execute();
                    var buffer = exc.Items.ToList();
                    
                    videos.AddRange(buffer);
                    pageToken = exc.NextPageToken;
                    
                } while (pageToken != null);
                idHaveChanged = false;
            }
            return videos;
        }
    }
}

