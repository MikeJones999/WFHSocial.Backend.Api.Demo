using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WFHSocial.Api.Application.Interfaces.DomainEvents;
using WFHSocial.Api.Application.Interfaces.Services;
using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.DTOs.UserProfileModels;
using WFHSocial.Api.Domain.Users.Interfaces.Repository;
using WFHSocial.Api.Domain.Users.Models;
using WFHSocial.Shared.FileUploads;

namespace WFHSocial.Api.Application.Services.UserSettingsServices
{
    public class UserProfileAndSettingsService: IUserProfileAndSettingsService
    {
        private readonly ILogger<UserProfileAndSettingsService> _logger;
        private readonly IUserRepository _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBlobServices _blobServices;
        private readonly IProfileImageFileUploadedEvents _uploadEvents;

        public UserProfileAndSettingsService(ILogger<UserProfileAndSettingsService> logger, 
            IUserRepository context, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager,
            IBlobServices blobServices,
            //IBackgroundJobClient backgroundJobs,
            IProfileImageFileUploadedEvents uploadEvents)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _blobServices = blobServices;
            _uploadEvents = uploadEvents;
        }

        public async Task<UserProfileResponse?> GetUserProfileAndSettingsAsync(Guid userId)
        {
            ApplicationUser? user = await _context.GetUserByUserIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("WFH - Unable to find User with the given userId: {UserId} in the database. Request method: {Method}", userId, nameof(this.GetUserProfileAndSettingsAsync));
                return null;
            }

            UserProfileResponse response = _mapper.Map<UserProfileResponse>(user);
            response.HasProfileImage = user.HasProfilePicture;
            return response;
        }

        public async Task<UserProfileUpdateResponse> UpdateUserProfileAndSettingsAsync(UserProfileUpdateRequest userProfile)
        {
            ApplicationUser? user = await _context.GetUserByUserIdAsync(userProfile.UserId);
            UserProfileUpdateResponse response = new UserProfileUpdateResponse();
            if (user == null)
            {
                _logger.LogWarning("WFH - Unable to find User with the given userId: {UserId} in the database. Request method: {Method}", userProfile.UserId, nameof(this.UpdateUserProfileAndSettingsAsync));
                response.Message = "Failed to verify user - please log out and try again.";
                return response;
            }

            bool dataChanged = HasChangeOccurredIntheProvidedData(userProfile.SettingViews, user);
            _context.UpdateSettings(user.UserSettings);

            bool displayNameChanged = CheckAndUpdateDisplayNameIfChanged(userProfile, user);

            bool profilePicSettingChanged = HasProfilePictureChanged(userProfile.SettingViews, user);

            if (profilePicSettingChanged)
            {
                await HandleProfilePictureChangedAsync(user, response);
            }

            if (!string.IsNullOrEmpty(response.Message))
            {
                return response;
            }

            dataChanged = profilePicSettingChanged || dataChanged || displayNameChanged;
            await ProcessTheChangesToTheProfileIfRequiredAsync(response, dataChanged, displayNameChanged);
            return response;
        }

        public async Task<UploadResult> UpdateProfileImageAsync(IFormFile file, Guid userId)
        {
            ApplicationUser? user = await _context.GetUserByUserIdAsync(userId);
            UploadResult uploadResult = new UploadResult();
            string untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;

            if (user == null)
            {
                _logger.LogWarning("Unable to find User with the given userId: {UserId} in the database. Request method: {Method}", userId, nameof(this.UpdateProfileImageAsync));
                uploadResult.ErrorMessage = "Failed to verify user - please log out and try again";
                return uploadResult;
            }

            uploadResult.StoredFileNamed =  GetBlobNameForProfileImage(user);
            await DeleteBlobAndUploadFileAsync(file, user, uploadResult);
            //BackgroundJob.Enqueue(() => _uploadEvents.DeleteFileAsync(untrustedFileName)); //delete the untrusted file
            return uploadResult;
        }

        private static bool CheckAndUpdateDisplayNameIfChanged(UserProfileUpdateRequest userProfile, ApplicationUser user)
        {
            bool displayNameChanged = userProfile.DisplayName != user.DisplayName;

            if (displayNameChanged)
            {
                user.DisplayName = userProfile.DisplayName;
            }

            return displayNameChanged;
        }      

        private static bool HasProfilePictureChanged(List<UserSettingView> settingViews, ApplicationUser user)
        {          
            UserSettingView? proflePicSetting = settingViews.FirstOrDefault(x => x.SettingType == UserSettingTypeView.AllowUseOfProfilePicture);
            if (proflePicSetting is not null && !proflePicSetting.IsOn && user.HasProfilePicture)
            {                   
                return true;
            }
            return false;          
        }

        private async Task HandleProfilePictureChangedAsync(ApplicationUser user, UserProfileUpdateResponse response)
        {
            try
            {                            
                string blobName = GetBlobNameForProfileImage(user); 
                await _blobServices.DeleteFileAsync(blobName);
                user.HasProfilePicture = false;       
            }
            catch (Exception ex)
            {
                string errorMessage = $"Failed to update profile picture setting - please try again.";
                _logger.LogWarning("{error} Error Occured at: {methodName} {exErrorMessage}.", errorMessage, nameof(this.UpdateProfileImageAsync), ex.Message);
                response.Message = errorMessage;
            }
        }    


        private async Task DeleteBlobAndUploadFileAsync(IFormFile file, ApplicationUser user, UploadResult uploadResult)
        {
            if (user.HasProfilePicture)
            {
                string blobName = GetBlobNameForProfileImage(user);  
                await _blobServices.DeleteFileAsync(blobName);  //TODO move first then delet after successful upload
            }

            await _blobServices.UploadAsync(file, uploadResult.StoredFileNamed!);
            user.HasProfilePicture = true;
            if(await _context.SaveAsync())
            {
                uploadResult.IsSuccessful = true;
            }          
        }

        private async Task ProcessTheChangesToTheProfileIfRequiredAsync(UserProfileUpdateResponse response, bool changeDetected, bool displayNameChanged)
        {
            try
            {
                if (changeDetected &&  await _context.SaveAsync())
                {                  
                    string displayNameEdit = displayNameChanged ? "Please log out and log back in for display name changes to take effect." : string.Empty;
                    response.Success = true;
                    response.Message = $"Profile has been updated and saved successfully. {displayNameEdit}";                    
                }
                else
                {
                    response.Success = true;
                    response.Message = "No changes detected - save unnecessary";
                }
            }
            catch (Exception)
            {
                _logger.LogWarning("Failed to save profile change correctly. Request method: {Method}", nameof(this.ProcessTheChangesToTheProfileIfRequiredAsync));
                response.Message = "Failed to save correctly - please try again";
            }
        }
       

        private static bool HasChangeOccurredIntheProvidedData(List<UserSettingView> settingViews, ApplicationUser user)
        {
            foreach (UserSettingView setting in settingViews)
            {
                UserSetting? existingSetting = user.UserSettings.FirstOrDefault(x => x.Id == setting.Id);
                if (existingSetting is not null && existingSetting.IsOn != setting.IsOn)
                {
                    existingSetting.IsOn = setting.IsOn;
                    return true;  
                }
            }
            return false;
        }

        private string GetBlobNameForProfileImage(ApplicationUser user)
        {
            return $"{user.Id}.db4.png";
            //TODO requires config setting or based upon the user uniqueness (createdAtDate?
            //return $"{user.Id}.{user.CreatedDateUtc}.png";
            //CreatedAtDateUtc needs to be fedd up to UI
        }
    }
}
