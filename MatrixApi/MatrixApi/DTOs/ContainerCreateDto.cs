namespace MatrixApi.DTOs
{
    public class ContainerCreateDto
    {
        public string Status { get; set; } = "In behandeling";
        public List<int> OrderIds { get; set; } = new();
    }
}
