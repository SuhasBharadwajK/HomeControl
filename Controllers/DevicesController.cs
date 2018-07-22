using Bifrost.Devices.Gpio;
using Bifrost.Devices.Gpio.Abstractions;
using Bifrost.Devices.Gpio.Core;
using HomeControl.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeControl.Controllers
{
    [Route("api/device")]
    public class DevicesController : ControllerBase
    {
        static bool isDeviceOn;
        int pinId = 2;

        private IGpioController GpioController { get; set; }

        private IGpioPin GpioPin { get; set; }


        public DevicesController()
        {
            this.GpioController = Bifrost.Devices.Gpio.GpioController.Instance;
            this.GpioPin = this.GpioController.OpenPin(this.pinId);
        }

        [HttpGet("state")]
        public dynamic GetDeviceState(bool isStateUpdated = false)
        {
            return new { isDeviceOn, isStateUpdated };
        }

        [HttpGet("toggle")]
        public dynamic ToggleDevice()
        {
            this.GpioPin.SetDriveMode(GpioPinDriveMode.Output);

            if (isDeviceOn)
            {
                this.GpioPin.Write(GpioPinValue.Low);
            }
            else
            {
                this.GpioPin.Write(GpioPinValue.High);
            }

            isDeviceOn = !isDeviceOn;
            return Ok(this.GetDeviceState(true));
        }

        [HttpPost("switch")]
        public dynamic SwitchOnDevice([FromBody]DeviceRequest deviceRequest)
        {
            var isStateUpdated = false;
            if (deviceRequest.requestedState.ToLower() == "on" && !isDeviceOn)
            {
                this.GpioPin.Write(GpioPinValue.High);
                isDeviceOn = true;
                isStateUpdated = true;
            }
            else if (deviceRequest.requestedState.ToLower() == "off" && isDeviceOn)
            {
                this.GpioPin.Write(GpioPinValue.Low);
                isDeviceOn = false;
                isStateUpdated = true;
            }

            return Ok(this.GetDeviceState(isStateUpdated));
        }
    }
}