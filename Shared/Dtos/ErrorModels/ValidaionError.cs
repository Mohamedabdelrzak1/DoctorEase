namespace Shared.ErrorModels
{
    public class ValidaionError
    {
        public string Field { get; set; }
        public IEnumerable<string> Errors { get; set; }

    }
}