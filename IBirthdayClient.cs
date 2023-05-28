﻿using Microsoft.AspNetCore.Mvc;

namespace Birthday_Service
{
    public interface IBirthdayClient
    {
        Task<Birthday> PostBirthday( Birthday birthday );
        Task<List<BirthdayResponse>> GetBirthdays(string userId);
        Task<bool> ValidateUserId( string userId );
    }
}
