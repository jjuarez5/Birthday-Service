using Microsoft.AspNetCore.Mvc;

namespace Birthday_Service
{
    public interface IBirthdayClient
    {
        Task<Birthday> PostBirthday( Birthday birthday );
    }
}
