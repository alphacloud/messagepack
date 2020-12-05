#pragma warning disable 8618
namespace NetCoreWebApi.Models
{
    public class TestModel
    {
        public int Id { get; set; }
        public string Value { get; set; }

        /// <inheritdoc />
        public TestModel()
        {
        }

        /// <inheritdoc />
        public TestModel(int id)
        {
            Id = id;
            Value = $"Value: {id}";
        }
    }
}
