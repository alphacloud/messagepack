namespace NetCoreWebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;


/// <summary>
///     Sample controller.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    // ReSharper disable once NotAccessedField.Local
    readonly IOptions<MvcOptions> _mvcOptions;

    /// <inheritdoc />
    public ValuesController(IOptions<MvcOptions> mvcOptions)
    {
        _mvcOptions = mvcOptions;
    }

    // GET api/values
    /// <summary>
    ///     Retrieve sample models.
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<TestModel>> Get()
    {
        return new[] {new TestModel(1), new TestModel(2)};
    }

    // GET api/values/5
    /// <summary>
    ///     Retrieve sample model by id.
    /// </summary>
    /// <param name="id">Id.</param>
    [HttpGet("{id}")]
    public ActionResult<TestModel> Get(int id)
    {
        return new TestModel(id);
    }

    /// <summary>
    ///     Retrieve model, specify response format using file extension-like style.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [FormatFilter]
    [HttpGet("format/{id}")]
    [HttpGet("format/{id}.{format}")]
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
    [HttpPut]
    public ActionResult<TestModel> Echo([FromBody] TestModel value)
    {
        return value;
    }

}
