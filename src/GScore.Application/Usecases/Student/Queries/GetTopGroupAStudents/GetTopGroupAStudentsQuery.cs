using GScore.Application.DTOs;
using MediatR;

namespace GScore.Application.Usecases.Student.Queries.GetTopGroupAStudents;

public record GetTopGroupAStudentsQuery : IRequest<List<TopStudentDto>>;
