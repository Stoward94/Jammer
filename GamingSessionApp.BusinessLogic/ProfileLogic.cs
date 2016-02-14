using System;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.UI.WebControls;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Profile;
using Microsoft.AspNet.Identity;
using Image = System.Drawing.Image;

namespace GamingSessionApp.BusinessLogic
{
    public class ProfileLogic : BaseLogic
    {
        private readonly GenericRepository<UserProfile> _profileRepo;

        public ProfileLogic()
        {
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
                        KudosPoints = x.Kudos.Points,
                        XboxUsername = x.XboxGamertag,
                        XboxUrl = x.XboxUrl,
                        PsnUsername = x.PlayStationNetwork,
                        PsnUrl = x.PlayStationUrl,
                        SteamUsername = x.SteamName,
                        SteamUrl = x.SteamUrl,
                        ProfileImageUrl = x.ThumbnailUrl,
                        Friends = x.Friends.Select(f => new UserFriendViewModel
                        {
                            DisplayName = f.Friend.DisplayName,
                            KudosPoints = f.Friend.Kudos.Points
                        }).OrderByDescending(f => f.KudosPoints).ToList(), 
                        //My Sessions
                        Sessions = x.User.Sessions.Where(s => s.Active).OrderBy(s => s.ScheduledDate).ToList(),
                        //Friends Sessions
                        FriendsSessions = x.Friends.SelectMany(f => f.Friend.User.Sessions)
                        .Where(s => s.Active).OrderBy(fs => fs.ScheduledDate).Take(10).ToList(),
                        //Kudos History
                        KudosHistory = x.Kudos.History.OrderByDescending(kh => kh.DateAdded).Take(10).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (profile == null) return null;

                //Get the full image rather than the thumbnail
                string fileName = Path.GetFileName(profile.ProfileImageUrl);
                profile.ProfileImageUrl = $"/Images/180x180/{fileName}";

                //Convert Session Times to Local Time
                foreach (var s in profile.Sessions)
                {
                    s.CreatedDate = s.CreatedDate.ToTimeZoneTime(GetUserTimeZone());
                    s.ScheduledDate = s.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());
                }

                foreach (var s in profile.FriendsSessions)
                {
                    s.CreatedDate = s.CreatedDate.ToTimeZoneTime(GetUserTimeZone());
                    s.ScheduledDate = s.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());
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

        public async Task<UserProfileViewModel> GetUserProfile(string userName)
        {
            try
            {
                UserProfileViewModel profile = await _profileRepo.Get(x => x.DisplayName == userName)
                    .Select(x => new UserProfileViewModel
                    {
                        DisplayName = x.DisplayName,
                        KudosPoints = x.Kudos.Points,
                        ProfileImageUrl = x.ThumbnailUrl,
                        Friends = x.Friends.Select(f => new UserFriendViewModel
                        {
                            DisplayName = f.Friend.DisplayName,
                            KudosPoints = f.Friend.Kudos.Points
                        }).OrderByDescending(f => f.KudosPoints).ToList(),
                        //My Sessions
                        Sessions = x.User.Sessions.Where(s => s.Active).OrderBy(s => s.ScheduledDate).ToList(),
                        //Friends Sessions
                        FriendsSessions = x.Friends.SelectMany(fs => fs.Profile.User.Sessions)
                        .Where(s => s.Active).OrderBy(fs => fs.ScheduledDate).Take(10).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (profile == null) return null;
                
                //Convert Session Times to Local Time
                foreach (var s in profile.Sessions)
                {
                    s.CreatedDate = s.CreatedDate.ToTimeZoneTime(GetUserTimeZone());
                    s.ScheduledDate = s.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());
                }

                foreach (var s in profile.FriendsSessions)
                {
                    s.CreatedDate = s.CreatedDate.ToTimeZoneTime(GetUserTimeZone());
                    s.ScheduledDate = s.ScheduledDate.ToTimeZoneTime(GetUserTimeZone());
                }

                return profile;
            }
            catch (Exception ex)
            {
                LogError(ex, "Unable to get users profile");
                throw;
            }
        }

        public async Task<ValidationResult> AddFriend(string userName)
        {
            try
            {
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
                return _profileRepo.Get(x => x.UserId == userId)
                    .Select(x => new UserMenuViewModel
                    {
                        ThumbnailUrl = x.ThumbnailUrl,
                        KudosPoints = x.Kudos.Points,
                        UnreadMessages = x.Messages.Count(m => m.Read == false),
                        UnseenNotifications = x.Notifications.Count(n => n.Read == false)
                    }).FirstOrDefault();
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
                                            $"Please select an image with the following supported file types: {allowedExtensions.ToList()}");
                }

                //Thumbnail path
                string thumbnailPath = HostingEnvironment.MapPath("~/Images/thumbnails");
                string imagePath = HostingEnvironment.MapPath("~/Images/180x180");

                //Random file name
                string fileName = Path.GetRandomFileName();

                //Full thumbnail path
                string thumbnailFullPath = Path.Combine(thumbnailPath, (fileName + extension));

                //Full image path
                string imageFullPath = Path.Combine(imagePath, (fileName + extension));

                //Create Thumbnail image
                Image thumbnailImg = ResizeImage(file, 36, 36);

                thumbnailImg.Save(thumbnailFullPath);
                
                //Create 180 x 180 image
                Image largeImage =  ResizeImage(file, 180, 180);

                largeImage.Save(imageFullPath);


                //Now load the old file location
                UserProfile user = await _profileRepo.Get(x => x.UserId == userId)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return VResult.AddError("Unable to find your user profile, please try again.");


                //Delete now old image from disk
                string oldThumbnailPath = HostingEnvironment.MapPath(user.ThumbnailUrl);
                string oldFileName = Path.GetFileName(oldThumbnailPath);
                string oldImagePath = HostingEnvironment.MapPath($"~/Images/180x180/{oldFileName}");

                //Now update users thumbnail in DB
                user.ThumbnailUrl = $"/Images/thumbnails/{fileName}{extension}";

                _profileRepo.Update(user);

                await SaveChangesAsync();

                if (File.Exists(oldThumbnailPath))
                {
                    File.Delete(oldThumbnailPath);
                }

                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
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
                        XboxUsername = x.XboxGamertag,
                        XboxUrl = x.XboxUrl,
                        PsnUsername = x.PlayStationNetwork,
                        PsnUrl = x.PlayStationUrl,
                        SteamUsername = x.SteamName,
                        SteamUrl = x.SteamUrl
                    })
                    .FirstOrDefaultAsync();

                //Use 180x180 image instead of thumbnail
                //Get the full image rather than the thumbnail
                string fileName = Path.GetFileName(model.ImageUrl);
                model.ImageUrl = $"/Images/180x180/{fileName}";

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
                p.XboxGamertag = model.XboxUsername;
                p.XboxUrl = model.XboxUrl;
                p.PlayStationNetwork = model.PsnUsername;
                p.PlayStationUrl = model.PsnUrl;
                p.SteamName = model.SteamUsername;
                p.SteamUrl = model.SteamUrl;

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
    }
}
