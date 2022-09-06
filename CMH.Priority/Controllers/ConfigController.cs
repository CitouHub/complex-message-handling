using CMH.Priority.Util;
using Microsoft.AspNetCore.Mvc;

namespace CMH.Priority.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly Config _config;

        public ConfigController(Config config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("")]
        public Config Get()
        {
            return _config;
        }

        [HttpPut]
        [Route("")]
        public void Update([FromBody] Config configUpdate)
        {
            var configProperties = _config.GetType().GetProperties();
            var configUpdateProperties = configUpdate.GetType().GetProperties();

            foreach (var configProperty in configProperties)
            {
                foreach (var configUpdateProperty in configUpdateProperties)
                {
                    if (configProperty.Name.ToLower() == configUpdateProperty.Name.ToLower() && 
                        configProperty.PropertyType == configUpdateProperty.PropertyType)
                    {
                        configProperty.SetValue(_config, configUpdateProperty.GetValue(configUpdate));
                        break;
                    }
                }
            }
        }

        [HttpPut]
        [Route("reset")]
        public void Reset()
        {
            _config.Reset();
        }
    }
}