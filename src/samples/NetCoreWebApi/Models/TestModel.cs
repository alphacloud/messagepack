#pragma warning disable 8618
namespace NetCoreWebApi.Models;

/// <summary>
///     Test model.
/// </summary>
public class TestModel
{
    /// <summary>
    ///     Gets or sets model Id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets model value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Constructor.
    /// </summary>
    public TestModel()
    {
    }

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="id">Model Id.</param>
    public TestModel(int id)
    {
        Id = id;
        Value = $"Value: {id}";
    }
}
