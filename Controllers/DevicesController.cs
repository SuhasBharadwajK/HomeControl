using Microsoft.AspNetCore.Mvc;

namespace HomeControl.Controllers
{
    [Route("api/device")]
    public class DevicesController : Controller
    {
        static bool IsDeviceOn;

        [HttpGet("state")]
        public string GetDeviceState()
        {
            return IsDeviceOn ? "On" : "Off";
        }

        [HttpPost("toggle")]
        public string ToggleDevice()
        {
            IsDeviceOn = !IsDeviceOn;
            return this.GetDeviceState();
        }
    }
}