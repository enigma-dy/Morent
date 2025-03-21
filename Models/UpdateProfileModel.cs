using System;

namespace MoRent_V2.Models;

public class UpdateProfileModel
{

    public string? FullName { get; set; }

    public IFormFile? ProfileImage { get; set; }


}