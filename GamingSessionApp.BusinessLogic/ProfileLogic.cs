using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Threading.Tasks;
using System.Web;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Awards;
using GamingSessionApp.ViewModels.Profile;
using GamingSessionApp.ViewModels.Session;
using GamingSessionApp.ViewModels.Shared;
using Microsoft.AspNet.Identity;
using Image = System.Drawing.Image;

namespace GamingSessionApp.BusinessLogic
{
    public class ProfileLogic : BaseLogic, IProfileLogic
    {
        private readonly GenericRepository<UserProfile> _profileRepo;

        public ProfileLogic(UnitOfWork uow)
        {
            UoW = uow;
            _profileRepo = UoW.Repository<UserProfile>();
        }

        public async Task<UserProfileViewModel> GetMyProfile(string userId)
        {
            try
            {
                UserId = userId;

                UserProfileViewModel profile = await _profileRepo.Get(x => x.UserId == userId)
                    .Select(x => new UserProfileViewModel
                    {
                        DisplayName = x.DisplayName,
                        About = x.About,
                        Kudos = x.Kudos.Points.ToString(),
                        Rating = x.Rating,
                        ProfileImageUrl = x.ThumbnailUrl,
                        Registered = x.User.DateRegistered,
                        LastSignIn = x.User.LastSignIn,

                        Social = new UserSocialLinks
                        {
                            Xbox = x.Social.Xbox,
                            PlayStation = x.Social.PlayStation,
                            Steam = x.Social.Steam,
                            Facebook = x.Social.Facebook,
                            Twitter = x.Social.Twitter,
                            Twitch = x.Social.Twitch
                        },

                        BeginnerCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Beginner),
                        NoviceCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Novice),
                        IntermediateCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Intermediate),
                        AdvancedCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Advanced),
                        ExpertCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Expert),

                        //Awards (select highest level for each group)
                        Awards = x.Awards
                        .GroupBy(a => a.Award.GroupId, (key, ag) => ag.OrderByDescending(a => a.Award.LevelId).FirstOrDefault())
                        .Select(a => new AwardViewModel
                        {
                            Title = a.Award.Title,
                            Level = a.Award.Level,
                            Description = a.Award.Description,
                            Slug = a.Award.Slug,
                            
                        }).ToList(),

                        Statistics = new UserSessionStatistics
                        {
                            CompletedSessions = x.Sessions.Count(s => s.StatusId == (int)SystemEnums.SessionStatusEnum.Completed),
                            Platforms = x.Sessions
                            .Where(s => s.StatusId == (int)SystemEnums.SessionStatusEnum.Completed)
                            .GroupBy(s => s.PlatformId)
                            .Select(s => new UserPlatformStatistic
                            {
                                PlatformId = s.Select(p => p.PlatformId).FirstOrDefault(),
                                Platform = s.Select(p => p.Platform.Name).FirstOrDefault(),
                                CompletedCount = s.Count()
                            }).ToList()
                        },

                        Friends = x.Friends.Select(f => new UserFriendViewModel
                        {
                            DisplayName = f.Friend.DisplayName,
                            KudosPoints = f.Friend.Kudos.Points
                        }).OrderByDescending(f => f.KudosPoints).ToList(), 
                        //My Sessions
                        Sessions = x.Sessions.Where(s => s.Active && s.ScheduledDate >= DateTime.UtcNow).Select(s => new SessionListItemViewModel
                        {
                            Id = s.Id,
                            Creator = s.Creator.DisplayName,
                            Duration = s.Duration.Duration,
                            Game = s.Game.GameTitle,
                            MembersCount = s.Members.Count,
                            PlatformId = s.PlatformId,
                            RequiredCount = s.MembersRequired,
                            ScheduledTime = s.ScheduledDate,
                            Status = s.Status.Name,
                            StatusDescription = s.Status.Description,
                            Type = s.Type.Name,
                            TypeId = s.TypeId,
                            Platform = s.Platform.Name,
                            Summary = s.Information
                        }).OrderBy(s => s.ScheduledTime).Take(5).ToList(),

                        //Friends Sessions
                        FriendsSessions = x.Friends.SelectMany(f => f.Friend.Sessions)
                        .Where(f => f.Active && f.ScheduledDate >= DateTime.UtcNow)
                        .Select(s => new SessionListItemViewModel
                        {
                            Id = s.Id,
                            Creator = s.Creator.DisplayName,
                            Duration = s.Duration.Duration,
                            Game = s.Game.GameTitle,
                            MembersCount = s.Members.Count,
                            PlatformId = s.PlatformId,
                            RequiredCount = s.MembersRequired,
                            ScheduledTime = s.ScheduledDate,
                            Status = s.Status.Name,
                            StatusDescription = s.Status.Description,
                            Type = s.Type.Name,
                            TypeId = s.TypeId,
                            Platform = s.Platform.Name,
                            Summary = s.Information
                        })
                        .OrderBy(fs => fs.ScheduledTime).Take(10).ToList(),
                        //Kudos History
                        KudosHistory = x.Kudos.History.OrderByDescending(kh => kh.DateAdded).Take(10).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (profile == null) return null;

                //Format last sign-in date
                profile.LastSignIn = profile.LastSignIn.ToTimeZoneTime(GetUserTimeZone());
                profile.Registered = profile.Registered.ToTimeZoneTime(GetUserTimeZone());

                var now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                if (profile.LastSignIn.Date == now.Date)
                    profile.DisplayLastSignIn = "Today";
                else if (profile.LastSignIn.Date == now.Date.AddDays(1))
                    profile.DisplayLastSignIn = "Tomorrow";
                else
                    profile.DisplayLastSignIn = profile.LastSignIn.ToShortDateString();


                //Format Kudos Number
                profile.Kudos = int.Parse(profile.Kudos).ToString("n0");

                //Get the full image rather than the thumbnail
                profile.ProfileImageUrl = GetImageUrl(profile.ProfileImageUrl, "180x180");

                //Convert Session Times to Local Time
                foreach (var s in profile.Sessions)
                {
                    s.ScheduledTime = s.ScheduledTime.ToTimeZoneTime(GetUserTimeZone());
                    s.ScheduledDisplayTime = s.ScheduledTime.ToFullDateTimeString();
                }

                foreach (var s in profile.FriendsSessions)
                {
                    s.ScheduledTime = s.ScheduledTime.ToTimeZoneTime(GetUserTimeZone());
                    s.ScheduledDisplayTime = s.ScheduledTime.ToFullDateTimeString();
                }

                foreach (var kh in profile.KudosHistory)
                {
                    kh.DateAdded = kh.DateAdded.ToTimeZoneTime(GetUserTimeZone());
                }

                return profile;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">Username of the profile you want to fetch</param>
        /// <param name="userId">UserId of the user that is making the request</param>
        /// <returns></returns>
        public async Task<UserProfileViewModel> GetUserProfile(string username, string userId)
        {
            try
            {
                //Assign the userId of the user making the request
                UserId = userId;

                UserProfileViewModel profile = await _profileRepo.Get(x => x.DisplayName == username)
                    .Select(x => new UserProfileViewModel
                    {
                        DisplayName = x.DisplayName,
                        About = x.About,
                        Kudos = x.Kudos.Points.ToString(),
                        Rating = x.Rating,
                        ProfileImageUrl = x.ThumbnailUrl,
                        Registered = x.User.DateRegistered,
                        LastSignIn = x.User.LastSignIn,

                        Social = new UserSocialLinks
                        {
                            Xbox = x.Social.Xbox,
                            PlayStation = x.Social.PlayStation,
                            Steam = x.Social.Steam,
                            Facebook = x.Social.Facebook,
                            Twitter = x.Social.Twitter,
                            Twitch = x.Social.Twitch
                        },

                        BeginnerCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Beginner),
                        NoviceCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Novice),
                        IntermediateCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Intermediate),
                        AdvancedCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Advanced),
                        ExpertCount = x.Awards.Count(a => a.Award.LevelId == (int)SystemEnums.AwardLevelsEnum.Expert),

                        //Awards (select highest level for each group)
                        Awards = x.Awards
                        .GroupBy(a => a.Award.GroupId, (key, ag) => ag.OrderByDescending(a => a.Award.LevelId).FirstOrDefault())
                        .Select(a => new AwardViewModel
                        {
                            Title = a.Award.Title,
                            Level = a.Award.Level,
                            Description = a.Award.Description,
                            Slug = a.Award.Slug,

                        }).ToList(),

                        Statistics = new UserSessionStatistics
                        {
                            CompletedSessions = x.Sessions.Count(s => s.StatusId == (int)SystemEnums.SessionStatusEnum.Completed),
                            Platforms = x.Sessions
                            .Where(s => s.StatusId == (int)SystemEnums.SessionStatusEnum.Completed)
                            .GroupBy(s => s.PlatformId)
                            .Select(s => new UserPlatformStatistic
                            {
                                PlatformId = s.Select(p => p.PlatformId).FirstOrDefault(),
                                Platform = s.Select(p => p.Platform.Name).FirstOrDefault(),
                                CompletedCount = s.Count()
                            }).ToList()
                        },

                        Friends = x.Friends.Select(f => new UserFriendViewModel
                        {
                            DisplayName = f.Friend.DisplayName,
                            KudosPoints = f.Friend.Kudos.Points
                        }).OrderByDescending(f => f.KudosPoints).ToList(),
                        //My Sessions
                        Sessions = x.Sessions.Where(s => s.Active && s.ScheduledDate >= DateTime.UtcNow).Select(s => new SessionListItemViewModel
                        {
                            Id = s.Id,
                            Creator = s.Creator.DisplayName,
                            Duration = s.Duration.Duration,
                            Game = s.Game.GameTitle,
                            MembersCount = s.Members.Count,
                            PlatformId = s.PlatformId,
                            RequiredCount = s.MembersRequired,
                            ScheduledTime = s.ScheduledDate,
                            Status = s.Status.Name,
                            StatusDescription = s.Status.Description,
                            Type = s.Type.Name,
                            TypeId = s.TypeId,
                            Platform = s.Platform.Name,
                            Summary = s.Information
                        }).OrderBy(s => s.ScheduledTime).Take(5).ToList(),

                        //Friends Sessions
                        FriendsSessions = x.Friends.SelectMany(f => f.Friend.Sessions)
                        .Where(f => f.Active && f.ScheduledDate >= DateTime.UtcNow)
                        .Select(s => new SessionListItemViewModel
                        {
                            Id = s.Id,
                            Creator = s.Creator.DisplayName,
                            Duration = s.Duration.Duration,
                            Game = s.Game.GameTitle,
                            MembersCount = s.Members.Count,
                            PlatformId = s.PlatformId,
                            RequiredCount = s.MembersRequired,
                            ScheduledTime = s.ScheduledDate,
                            Status = s.Status.Name,
                            StatusDescription = s.Status.Description,
                            Type = s.Type.Name,
                            TypeId = s.TypeId,
                            Platform = s.Platform.Name,
                            Summary = s.Information
                        })
                        .OrderBy(fs => fs.ScheduledTime).Take(10).ToList(),
                        //Kudos History
                        KudosHistory = x.Kudos.History.OrderByDescending(kh => kh.DateAdded).Take(10).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (profile == null) return null;

                //Check if the requesting user is a friend of this user
                profile.IsFriend = await _profileRepo.Get(x => x.UserId == userId)
                    .SelectMany(x => x.Friends)
                    .AnyAsync(x => x.Friend.DisplayName == username);

                //Format last sign-in date
                profile.LastSignIn = profile.LastSignIn.ToTimeZoneTime(GetUserTimeZone());
                profile.Registered = profile.Registered.ToTimeZoneTime(GetUserTimeZone());

                var now = DateTime.UtcNow.ToTimeZoneTime(GetUserTimeZone());

                if (profile.LastSignIn.Date == now.Date)
                    profile.DisplayLastSignIn = "Today";
                else if (profile.LastSignIn.Date == now.Date.AddDays(1))
                    profile.DisplayLastSignIn = "Tomorrow";
                else
                    profile.DisplayLastSignIn = profile.LastSignIn.ToString("dd-MMM-yy");


                //Format Kudos Number
                profile.Kudos = int.Parse(profile.Kudos).ToString("n0");

                //Get the full image rather than the thumbnail
                profile.ProfileImageUrl = GetImageUrl(profile.ProfileImageUrl, "180x180");

                //Convert Session Times to Local Time
                foreach (var s in profile.Sessions)
                {
                    s.ScheduledTime = s.ScheduledTime.ToTimeZoneTime(GetUserTimeZone());
                    s.ScheduledDisplayTime = s.ScheduledTime.ToFullDateTimeString();
                }

                foreach (var s in profile.FriendsSessions)
                {
                    s.ScheduledTime = s.ScheduledTime.ToTimeZoneTime(GetUserTimeZone());
                    s.ScheduledDisplayTime = s.ScheduledTime.ToFullDateTimeString();
                }

                foreach (var kh in profile.KudosHistory)
                {
                    kh.DateAdded = kh.DateAdded.ToTimeZoneTime(GetUserTimeZone());
                }

                return profile;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get users profile");
                throw;
            }
        }

        public async Task<ValidationResult> AddFriend(string userName, string userId)
        {
            try
            {
                UserId = userId;

                //Find the user in the db
                string targetUserId = await _profileRepo.Get(x => x.DisplayName == userName)
                    .Select(x => x.UserId)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(targetUserId)) return VResult.AddError("Unable to add friend");

                //Now find get the current users profile
                UserProfile currentUser = await _profileRepo.Get(x => x.UserId == UserId)
                    .Include(x => x.Friends).FirstOrDefaultAsync();

                //Check that this user isn't already friends
                if (currentUser.Friends.Any(x => x.FriendId == targetUserId))
                    return VResult.AddError("You are already friends with this user");

                //Add the friend
                currentUser.Friends.Add(new UserFriend { FriendId = targetUserId });

                _profileRepo.Update(currentUser);
                await SaveChangesAsync();
                
                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to add friend: " + userName);
                return VResult.AddError("Unable to add friend");
            }
        }

        public UserMenuViewModel GetUserMenuInformation(string userId)
        {
            try
            {
                var menu = _profileRepo.Get(x => x.UserId == userId)
                    .Select(x => new UserMenuViewModel
                    {
                        ThumbnailUrl = x.ThumbnailUrl,
                        KudosPoints = x.Kudos.Points.ToString(),
                        UnreadMessages = x.Messages.Count(m => m.Read == false),
                        UnseenNotifications = x.Notifications.Count(n => n.Read == false)
                    }).FirstOrDefault();

                menu.KudosPoints = TrimKudos(menu.KudosPoints);

                return menu;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get user menu details for user: " + userId);
                return null;
            }
        }

        public async Task<object> GetUsersJson(string q)
        {
            try
            {
                string username = HttpContext.Current.User.Identity.GetUserName();

                var users = await _profileRepo.Get(x => x.DisplayName.Contains(q))
                    .Where(x => x.DisplayName != username)
                    .Select(x => x.DisplayName)
                    .Take(5).ToListAsync();

                return users;
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        public async Task<ValidationResult> ProcessImageUpload(HttpPostedFileBase file, string userId )
        {
            try
            {
                UserId = userId;

                //Safety checks
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png"};
                var extension = Path.GetExtension(file.FileName);
                if (!allowedExtensions.Contains(extension))
                {
                    // Not allowed
                    return VResult.AddError($"The file type {extension} is not allowed. " +
                                            $"Please select an image with the following supported file types: {string.Join(" ", allowedExtensions.ToList())}");
                }

                //Thumbnail path
                string thumbnailPath = HostingEnvironment.MapPath("~/Images/thumbnails");
                string imagePath = HostingEnvironment.MapPath("~/Images/180x180");
                string fortyEightPath = HostingEnvironment.MapPath("~/Images/48x48");

                //Random file name
                string fileName = Path.GetRandomFileName();

                //Full thumbnail path
                string thumbnailFullPath = Path.Combine(thumbnailPath, (fileName + extension));

                //Full image path
                string imageFullPath = Path.Combine(imagePath, (fileName + extension));

                //48 x 48 image path
                string fortyEightFullPath = Path.Combine(fortyEightPath, (fileName + extension));

                //Create Thumbnail image
                Image thumbnailImg = ResizeImage(file, 36, 36);
                Directory.CreateDirectory(imagePath);
                
                //Create 180 x 180 image
                Image largeImage =  ResizeImage(file, 180, 180);
                Directory.CreateDirectory(imagePath);

                //Create 48 x 48 image
                Image fortyEightImage = ResizeImage(file, 48, 48);
                Directory.CreateDirectory(fortyEightPath);

                //Save images
                thumbnailImg.Save(thumbnailFullPath);
                largeImage.Save(imageFullPath);
                fortyEightImage.Save(fortyEightFullPath);


                //Now load the old file location
                UserProfile user = await _profileRepo.Get(x => x.UserId == userId)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return VResult.AddError("Unable to find your user profile, please try again.");


                //Now delete old image from disk
                List<string> oldImagePaths = new List<string>();

                string oldFileName = Path.GetFileName(user.ThumbnailUrl);

                //Old image paths
                oldImagePaths.Add(HostingEnvironment.MapPath(user.ThumbnailUrl));//36x36
                oldImagePaths.Add(HostingEnvironment.MapPath($"~/Images/180x180/{oldFileName}"));//180x180
                oldImagePaths.Add(HostingEnvironment.MapPath($"~/Images/48x48/{oldFileName}"));//48x48

                //Now update users thumbnail in DB
                user.ThumbnailUrl = $"/Images/thumbnails/{fileName}{extension}";

                _profileRepo.Update(user);
                await SaveChangesAsync();

                //Loop and delete each existing image
                foreach (var path in oldImagePaths)
                {
                    //skip default icons
                    if (path.Contains("default"))
                        continue;

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error uploading a profile image");
                return VResult.AddError("Error processing the image, please try again. Or use a different image.");
            }
        }

        private Image ResizeImage(HttpPostedFileBase file, int width, int height)
        {
            using (Image img = Image.FromStream(file.InputStream))
            {
                Image resizedImg = new Bitmap(width, height, img.PixelFormat);
                Graphics g = Graphics.FromImage(resizedImg);
                g.FillRectangle(Brushes.White, 0, 0, width, height);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                Rectangle rect = new Rectangle(0, 0, width, height);
                g.DrawImage(img, rect);

                return resizedImg;
            }
        }

        public async Task<EditProfileViewModel> GetEditProfileModel(string userId)
        {
            try
            {
                EditProfileViewModel model = await _profileRepo.Get(x => x.UserId == userId)
                    .Select(x => new EditProfileViewModel
                    {
                        DisplayName = x.DisplayName,
                        ImageUrl = x.ThumbnailUrl,
                        About = x.About,
                        Website = x.Website,
                        XboxUsername = x.Social.Xbox,
                        PsnUsername = x.Social.PlayStation,
                        SteamUsername = x.Social.Steam,
                        Twitch = x.Social.Twitch,
                        Twitter = x.Social.Twitter,
                        Facebook = x.Social.Facebook
                    })
                    .FirstOrDefaultAsync();

                //Use 180x180 image instead of thumbnail
                //Get the full image rather than the thumbnail
                model.ImageUrl = GetImageUrl(model.ImageUrl, "180x180");

                return model;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get the edit details for the view model. User : " + userId);
                return null;
            }
        }

        public async Task<ValidationResult> EditProfile(EditProfileViewModel model, string userId)
        {
            try
            {
                UserProfile p = await _profileRepo.Get(x => x.UserId == userId)
                    .Include(x => x.User)
                    .Include(x => x.Social)
                    .FirstOrDefaultAsync();

                if (p == null)
                    return VResult.AddError("Unable to find your profile. Please try again later.");

                //If the user/display name has changed update 
                //Identity username
                if (p.DisplayName != model.DisplayName)
                {
                    p.User.UserName = model.DisplayName;
                    p.DisplayName = model.DisplayName;

                }

                //Now update the rest of the values
                p.About = model.About;
                p.Website = model.Website;
                p.Social.Xbox = model.XboxUsername;
                p.Social.PlayStation = model.PsnUsername;
                p.Social.Steam = model.SteamUsername;
                p.Social.Twitch = model.Twitch;
                p.Social.Twitter = model.Twitter;
                p.Social.Facebook = model.Facebook;

                //Save changes
                _profileRepo.Update(p);
                await SaveChangesAsync();

                return VResult;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error updating the profile for user : " + userId);
                return VResult.AddError("An error occured whilst trying to update your profile. Please try again later.");
            }
        }

        public async Task<List<FriendListItem>> GetUsersFriends(string userId)
        {
            try
            {
                var friends = await _profileRepo.Get(x => x.UserId == userId)
                    .SelectMany(x => x.Friends.Select(f => new FriendListItem
                    {
                        Thumbnail = f.Friend.ThumbnailUrl,
                        Kudos = f.Friend.Kudos.Points.ToString(),
                        DisplayName = f.Friend.DisplayName
                    }))
                    .OrderBy(x => x.DisplayName)
                    .ToListAsync();

                foreach (var f in friends)
                {
                    f.Kudos = TrimKudos(f.Kudos);
                }

                return friends;
            }
            catch (Exception ex)
            {
                LogError(ex, "Error fetching users friends for session invite. User :" + userId);
                throw;
            }
        }
    }
}
