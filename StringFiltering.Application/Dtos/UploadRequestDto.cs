namespace StringFiltering.Application.Dtos;

public class UploadRequestDto
{
    public required string UploadId { get; set; }
    public int ChunkIndex { get; set; }
    public required string Data { get; set; }
    public bool IsLastChunk { get; set; }
}