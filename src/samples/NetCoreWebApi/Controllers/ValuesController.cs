namespace NetCoreWebApi.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;


    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        readonly IOptions<MvcOptions> _mvcOptions;

        /// <inheritdoc />
        public ValuesController(IOptions<MvcOptions> mvcOptions)
        {
            _mvcOptions = mvcOptions;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<TestModel>> Get()
        {
            return new[] {new TestModel(1), new TestModel(2)};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<TestModel> Get(int id)
        {
            return new TestModel(id);
        }

        [FormatFilter]
        [HttpGet("format/{id}.{format?}")]
        public ActionResult<TestModel> GetWithFormat(int id)
        {
            return new TestModel(id);
        }

        /// <summary>
        ///     Echo model back.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<TestModel> Echo([FromBody] TestModel value)
        {
            return value;
        }
    }
}
