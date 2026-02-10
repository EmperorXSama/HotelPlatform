using HotelManagement.BlazorServer.Models;
using HotelManagement.BlazorServer.models.Request.Hotels;
using HotelManagement.BlazorServer.models.Response.RefereneceData;
using HotelManagement.BlazorServer.Services.Hotel;
using Microsoft.AspNetCore.Components;

namespace HotelManagement.BlazorServer.Components.Pages.Hotels;

public partial class Create : ComponentBase
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private IHotelApiService HotelService { get; set; } = default!;
    
    
    
    private void GoBack()
    {
        Navigation.NavigateTo("/hotels");
    }
    
    
    // Form Models
    private HotelBasicInfoModel _hotelModel = new();
    private AddressModel _addressModel = new();
    private List<UploadedPictureModel> _uploadedPictures = [];
    private List<SelectedAmenityModel> _selectedAmenities = [];

    // State
    private bool _isSubmitting;
    private List<string> _validationErrors = [];
    private List<string> _apiErrors = [];

    // Configs
    private readonly PictureUploaderConfig _pictureUploaderConfig = new()
    {
        Title = "Hotel Pictures",
        Subtitle = "Upload images of your property. The first image will be the main photo.",
        MaxFiles = 10,
        MaxFileSizeBytes = 5 * 1024 * 1024,
        RequireAtLeastOne = true
    };

    private readonly AmenitySelectorConfig _amenitySelectorConfig = new()
    {
        ContextType = AmenityContextType.Hotel,
        Title = "Hotel Amenities",
        AllowUpchargeEditing = true,
        DefaultCurrency = "USD",
        InitialVisibleCount = 8
    };

    private bool IsValid
    {
        get
        {
            _validationErrors = Validate();
            return _validationErrors.Count == 0;
        }
    }

    private List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(_hotelModel.Name))
            errors.Add("Hotel name is required");

        if (string.IsNullOrWhiteSpace(_addressModel.Street))
            errors.Add("Street address is required");

        if (string.IsNullOrWhiteSpace(_addressModel.City))
            errors.Add("City is required");

        if (string.IsNullOrWhiteSpace(_addressModel.Country))
            errors.Add("Country is required");

        var validPictures = _uploadedPictures.Count(p => !p.HasError && !p.IsUploading);
        if (validPictures == 0)
            errors.Add("At least one picture is required");

        var uploadingPictures = _uploadedPictures.Count(p => p.IsUploading);
        if (uploadingPictures > 0)
            errors.Add($"Please wait for {uploadingPictures} picture(s) to finish uploading");

        return errors;
    }

    private async Task CreateHotel()
    {
        _apiErrors = [];
        _validationErrors = Validate();
        
        if (_validationErrors.Any())
            return;

        _isSubmitting = true;
        StateHasChanged();

        try
        {
            // Build the request
            var request = BuildCreateHotelRequest();

            // Call API
            var result = await HotelService.CreateAsync(request);

            result.Switch(
                response => Navigation.NavigateTo($"/hotels/{response.Id}"),  // Use response.Id
                errors => _apiErrors = errors.Select(e => e.Description).ToList()
            );
        }
        catch (Exception ex)
        {
            _apiErrors = [$"An unexpected error occurred: {ex.Message}"];
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private CreateHotelRequest BuildCreateHotelRequest()
    {
        // Build pictures list (only successfully uploaded ones)
        var pictures = _uploadedPictures
            .Where(p => !p.HasError && !p.IsUploading && p.StoredFileId != Guid.Empty)
            .OrderBy(p => p.DisplayOrder)
            .Select(p => new CreateHotelPictureRequest(
                p.StoredFileId,
                p.AltText,
                p.IsMain
            ))
            .ToList();

        // Ensure exactly one main picture
        if (pictures.Any() && !pictures.Any(p => p.IsMain))
        {
            pictures[0] = pictures[0] with { IsMain = true };
        }

        // Build amenities list
        var amenities = _selectedAmenities
            .Select(a => new CreateHotelAmenityRequest(
                a.AmenityId,
                (int)a.UpchargeType,
                a.UpchargeAmount,
                a.Currency
            ))
            .ToList();

        // Build address
        var address = new CreateHotelAddressRequest(
            _addressModel.Street,
            _addressModel.City,
            _addressModel.Country,
            _addressModel.PostalCode,
            _addressModel.Latitude,
            _addressModel.Longitude
        );

        return new CreateHotelRequest(
            _hotelModel.Name,
            _hotelModel.Description,
            address,
            pictures,
            amenities.Count > 0 ? amenities : null
        );
    }



    // View Models
    private class HotelBasicInfoModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    private class AddressModel
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}