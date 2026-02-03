using GScore.Application.DTOs;
using MediatR;

namespace GScore.Application.Usecases.Student.Queries.GetScoreStatistics;

public record GetScoreStatisticsQuery : IRequest<List<SubjectStatisticsDto>>;
