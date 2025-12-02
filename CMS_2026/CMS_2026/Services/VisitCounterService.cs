using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CMS_2026.Data.Entities;
using CMS_2026.Services;

namespace CMS_2026.Services
{
    public class VisitCounterService
    {
        private readonly IDataService _dataService;
        public static ConcurrentDictionary<string, PP_Visit> OnlineSessions = new();
        public static ConcurrentDictionary<string, int> UrlStats = new();

        private static readonly List<string> MacosPlatforms = new() { "Macintosh", "MacIntel", "MacPPC", "Mac68K" };
        private static readonly List<string> WindowsPlatforms = new() { "Win32", "Win64", "Windows", "WinCE" };
        private static readonly List<string> IosPlatforms = new() { "iPhone", "iPad", "iPod" };

        public VisitCounterService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public static string GetDeviceName(string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return "NA";

            if (MacosPlatforms.Any(t => userAgent.Contains(t)))
                return "Mac OS";
            if (IosPlatforms.Any(t => userAgent.Contains(t)))
                return "Ios";
            if (WindowsPlatforms.Any(t => userAgent.Contains(t)))
                return "Windows";
            if (userAgent.Contains("Android"))
                return "Android";
            if (userAgent.Contains("Linux"))
                return "Linux";

            return "NA";
        }

        public void OnSessionStart(string sessionId, string? referer, string? ipAddress, string? userAgent, string? browser)
        {
            var visitData = new PP_Visit
            {
                FirstTick = DateTime.Now.Ticks,
                Referer = referer ?? "NA",
                Ip = ipAddress ?? "NA",
                Device = GetDeviceName(userAgent),
                Browser = browser ?? "NA",
                Date = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
                Urls = new List<KeyValuePair<string, long>>()
            };

            OnlineSessions.TryAdd(sessionId, visitData);
        }

        public void OnRequestBegin(string sessionId, string currentUrl, string? ipAddress, string? userAgent, string? browser)
        {
            if (!OnlineSessions.TryGetValue(sessionId, out var visitData))
            {
                OnSessionStart(sessionId, null, ipAddress, userAgent, browser);
                if (!OnlineSessions.TryGetValue(sessionId, out visitData))
                    return;
            }

            if (currentUrl == visitData.LastUrl || 
                currentUrl.EndsWith(".ashx") || 
                currentUrl.StartsWith("/admin"))
                return;

            long currentTicks = DateTime.Now.Ticks;
            if (visitData.LastTick > 0)
            {
                var stayTime = (long)new TimeSpan(currentTicks - visitData.LastTick).TotalSeconds;
                visitData.Urls?.Add(new KeyValuePair<string, long>(visitData.LastUrl ?? string.Empty, stayTime));
            }
            else
            {
                visitData.SessionId = $"{DateTime.Now:yyMMddHHmmss}_{sessionId}";
                visitData.Ip = ipAddress ?? "NA";
                visitData.Browser = browser ?? "NA";
                visitData.Device = GetDeviceName(userAgent);
                visitData.Urls = new List<KeyValuePair<string, long>>();
                visitData.Date = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            }

            visitData.LastUrl = currentUrl;
            visitData.LastTick = currentTicks;
        }

        public void OnRequestPing(string sessionId)
        {
            if (OnlineSessions.TryGetValue(sessionId, out var visitData))
            {
                visitData.HeartBeat = DateTime.Now.Ticks;
            }
        }

        public void OnSessionEnd(string sessionId)
        {
            if (!OnlineSessions.TryRemove(sessionId, out var visitData))
                return;

            if (visitData.HeartBeat == 0 || visitData.LastTick == 0)
                return;

            try
            {
                var stayTime = (long)new TimeSpan(visitData.HeartBeat - visitData.LastTick).TotalSeconds;
                visitData.Urls?.Add(new KeyValuePair<string, long>(visitData.LastUrl ?? string.Empty, stayTime));
                
                visitData.StayTime = (int)new TimeSpan(visitData.HeartBeat - visitData.FirstTick).TotalSeconds;
                visitData.ClickCount = visitData.Urls?.Count ?? 0;
                visitData.MakeOrder = false;
                visitData.Country = "NA";
                visitData.Referer = visitData.Referer ?? "NA";

                if (visitData.Urls != null)
                {
                    visitData.JsonDetails = JsonSerializer.Serialize(visitData.Urls);
                    
                    visitData.Urls
                        .GroupBy(v => v.Key)
                        .Select(g => g.Key)
                        .ToList()
                        .ForEach(url => UrlStats.AddOrUpdate(url, 1, (u, counter) => counter + 1));
                }

                _dataService.Insert(visitData);

                // Update daily stats
                var daily = _dataService.GetOne<PP_Stats_Daily>(visitData.Date);
                if (daily != null)
                {
                    daily.VisitCount++;
                    _dataService.Update(daily);
                }
                else
                {
                    var newDaily = new PP_Stats_Daily
                    {
                        Date = visitData.Date,
                        VisitCount = 1,
                        OrderCount = 0
                    };
                    _dataService.Insert(newDaily);
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error during session end: {ex.Message}");
            }
        }

        public void RefreshVisitStats(DateTime now)
        {
            int sevenDaysAgo = int.Parse(now.AddDays(-7).ToString("yyyyMMdd"));
            var oldStats = _dataService.GetList<PP_Stats_Daily>(x => x.Date < sevenDaysAgo);
            foreach (var stat in oldStats)
            {
                _dataService.Delete<PP_Stats_Daily>(stat.Date);
            }

            int today = int.Parse(now.ToString("yyyyMMdd"));
            var todayStats = _dataService.GetOne<PP_Stats_Daily>(today);
            
            if (todayStats == null)
            {
                todayStats = new PP_Stats_Daily
                {
                    Date = today,
                    VisitCount = _dataService.GetList<PP_Visit>(x => x.Date == today).Count,
                    OrderCount = _dataService.GetList<PP_Order>(x => x.CreatedTime.Date == now.Date).Count
                };
                _dataService.Insert(todayStats);
            }
            else
            {
                todayStats.VisitCount = _dataService.GetList<PP_Visit>(x => x.Date == today).Count;
                todayStats.OrderCount = _dataService.GetList<PP_Order>(x => x.CreatedTime.Date == now.Date).Count;
                _dataService.Update(todayStats);
            }
        }
    }
}

