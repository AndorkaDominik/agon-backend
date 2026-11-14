namespace Transfer.Dtos;

public class TrainerPaginationResponseDto
{
    public ICollection<TrainerSearchDto> Trainers { get; set; } = new List<TrainerSearchDto>();
}
