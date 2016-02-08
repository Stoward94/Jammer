using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamingSessionApp.DataAccess;
using GamingSessionApp.Helpers;
using GamingSessionApp.Models;
using GamingSessionApp.ViewModels.Profile;
using GamingSessionApp.ViewModels.Session;

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
    }
}
