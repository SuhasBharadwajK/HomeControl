using Bifrost.Devices.Gpio;
using Bifrost.Devices.Gpio.Abstractions;
using Bifrost.Devices.Gpio.Core;
using Microsoft.AspNetCore.Mvc;

namespace HomeControl.Controllers
{
    [Route("api/device")]
    public class DevicesController : Controller
    {
        static bool IsDeviceOn;
        int pinId = 2;

        private IGpioController GpioController { get; set; }

        private IGpioPin GpioPin { get; set; }


        public DevicesController()
        {
            this.GpioController = Bifrost.Devices.Gpio.GpioController.Instance;
            var pin = this.GpioController.OpenPin(this.pinId);
        }

        [HttpGet("state")]
        public dynamic GetDeviceState()
        {
            return new { IsDeviceOn };
        }

        [HttpGet("toggle")]
        public dynamic ToggleDevice()
        {
            this.GpioPin.SetDriveMode(GpioPinDriveMode.Output);

            if (IsDeviceOn)
            {
                this.GpioPin.Write(GpioPinValue.Low);
            }
            else
            {
                this.GpioPin.Write(GpioPinValue.High);
            }

            IsDeviceOn = !IsDeviceOn;
            return this.GetDeviceState();
        }

        [HttpPost("on")]
        public dynamic SwitchOnDevice()
        {
            if (!IsDeviceOn)
            {
                this.GpioPin.Write(GpioPinValue.High);
            }

            return this.GetDeviceState();
        }

        [HttpPost("post")]
        public dynamic SwitchOffDevice()
        {
            if (IsDeviceOn)
            {
                this.GpioPin.Write(GpioPinValue.Low);
            }

            return this.GetDeviceState();
        }
    }
}