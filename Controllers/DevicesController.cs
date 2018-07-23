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
        GpioPinValue high;
        GpioPinValue low;
        bool isRelay = true; // Set this value to false if the circuit being used with the Pi is not a relay.

        private IGpioController GpioController { get; set; }

        private IGpioPin GpioPin { get; set; }


        public DevicesController()
        {
            // This condition is used to check if a relay is used with the Raspberry Pi.
            // A relay behaves differently from a regular circuit in that relay is 
            // triggered on when the GPIO output is LOW and vice versa.
            if (isRelay)
            {
                high = GpioPinValue.Low;
                low = GpioPinValue.High;
            }
            else
            {
                high = GpioPinValue.High;
                low = GpioPinValue.Low;
            }

            this.GpioController = Bifrost.Devices.Gpio.GpioController.Instance;
            this.GpioPin = this.GpioController.OpenPin(this.pinId);
            this.GpioPin.SetDriveMode(GpioPinDriveMode.Output);
            this.GpioPin.Write(high);
        }

        [HttpGet("state")]
        public dynamic GetDeviceState(bool isStateUpdated = false)
        {
            return new { isDeviceOn, isStateUpdated };
        }

        [HttpGet("toggle")]
        public dynamic ToggleDevice()
        {
            if (isDeviceOn)
            {
                this.GpioPin.Write(low);
            }
            else
            {
                this.GpioPin.Write(high);
            }

            isDeviceOn = !isDeviceOn;
            return Ok(this.GetDeviceState(true));
        }

        [HttpPost("switch")]
        public dynamic SwitchOnDevice([FromBody]DeviceRequest deviceRequest)
        {
            if (deviceRequest == null || string.IsNullOrEmpty(deviceRequest.requestedState))
            {
                return NoContent();
            }

            var isStateUpdated = false;
            if (deviceRequest.requestedState.ToLower() == "on" && !isDeviceOn)
            {
                this.GpioPin.Write(high);
                isDeviceOn = true;
                isStateUpdated = true;
            }
            else if (deviceRequest.requestedState.ToLower() == "off" && isDeviceOn)
            {
                this.GpioPin.Write(low);
                isDeviceOn = false;
                isStateUpdated = true;
            }

            return Ok(this.GetDeviceState(isStateUpdated));
        }
    }
}