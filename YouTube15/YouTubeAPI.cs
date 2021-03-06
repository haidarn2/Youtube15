﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace YouTube15
{
    class YouTubeAPI
    {
        private HttpListener _listener;

        private Dictionary<string, YoutubeVideo> videos = new Dictionary<string, YoutubeVideo>();

        private List<string> priorityList = new List<string>();

        public YouTubeAPI()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://127.0.0.1:60024/");
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(ProcessRequest), null);

            // add dummy video
            videos.Add("12345", new YoutubeVideo("12345", "Dummy video title", "Dummy video uploader"));
            priorityList.Add("12345");
        }

        private void ProcessRequest(IAsyncResult result)
        {
            HttpListenerContext context = _listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            //Answer getCommand/get post data/do whatever
            string data;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                data = reader.ReadToEnd();
            }

            dynamic json = JObject.Parse(data);

            string id = (string) json.id;
            string title = (string) json.title;
            string uploader = (string) json.uploader;
            string currentTime = (string) json.currentTime;
            string duration = (string) json.duration;
            bool paused = (bool) json.paused;
            bool terminate = (bool) json.terminate;
            
            if (videos.ContainsKey(id))
            {
                YoutubeVideo video = videos[id];
                if (currentTime != null)
                    video.setCurrentTime(currentTime);
                if (duration != null)
                    video.setDuration(duration);

                // video is now unpaused, takes priority.
                if (!paused && video.paused)
                {
                    video.paused = false;
                    priorityList.Remove(id);
                    priorityList.Add(id);
                }
                else { video.paused = paused; }

                // terminate flag is on, so tab or window was closed.
                if (terminate)
                {
                    videos.Remove(id);
                    priorityList.Remove(id);
                }
            }
            else
            {
                videos.Add((string)json.id, new YoutubeVideo((string)json.id, (string)json.title, (string)json.uploader));
                priorityList.Add((string)json.id);
            }

            response.OutputStream.Close();

            _listener.BeginGetContext(new AsyncCallback(ProcessRequest), null);
        }

        public String getVideoTitle()
        {
            return videos[priorityList.Last()].getVideoTitle();
        }

        public String getUploader()
        {
            return videos[priorityList.Last()].getUploader();
        }

        public String getCurrentTime()
        {
            return videos[priorityList.Last()].getCurrentTime();
        }

        public String getDuration()
        {
            return videos[priorityList.Last()].getDuration();
        }

        public Boolean playing()
        {
            return videos[priorityList.Last()].playing();
        }

        public Boolean noVideoDetected()
        {
            return videos[priorityList.Last()].getID() == "12345";
        }

        private class YoutubeVideo
        {
            private String  id, videoTitle, uploader, currentTime, duration;
            public Boolean paused;
            public YoutubeVideo(string id, string videoTitle, string uploader)
            {
                this.id = id;
                this.videoTitle = videoTitle;
                this.uploader = uploader;
                this.currentTime = "0";
                this.duration = "0";
                this.paused = true;
            }

            public Boolean playing()
            {
                return !paused;
            }

            public String getID()
            {
                return id;
            }

            public String getVideoTitle()
            {
                return videoTitle;
            }

            public String getUploader()
            {
                return uploader;
            }

            public String getCurrentTime()
            {
                return currentTime;
            }

            public String getDuration()
            {
                return duration;
            }

            public void setCurrentTime(string currentTime)
            {
                this.currentTime = currentTime;
            }

            public void setDuration(string duration)
            {
                this.duration = duration;
            }

        }
    }
}
