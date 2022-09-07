using CMH.Data.Model;
using CMH.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CMH.Priority.Controller
{
    [ApiController]
    [Route("v1/[controller]")]
    public class DataSourceController : ControllerBase
    {
        private readonly IDataSourceRepository _dataSourceRepository;

        public DataSourceController(IDataSourceRepository dataSourceRepository)
        {
            _dataSourceRepository = dataSourceRepository;
        }

        [HttpPost]
        [Route("")]
        public DataSource Add([FromBody] DataSource dataSource)
        {
            return _dataSourceRepository.Add(dataSource);
        }

        [HttpGet]
        [Route("")]
        public List<DataSource> GetAll()
        {
            return _dataSourceRepository.GetAll();
        }

        [HttpGet]
        [Route("{id}")]
        public DataSource? Get(short id)
        {
            return _dataSourceRepository.Get(id);
        }

        [HttpPut]
        [Route("{id}")]
        public void Update(short id, [FromBody] DataSource dataSourceUpdate)
        {
            _dataSourceRepository.Update(id, dataSourceUpdate);
        }

        [HttpDelete]
        [Route("{id}")]
        public void Delete(short id)
        {
            _dataSourceRepository.Delete(id);
        }
    }
}