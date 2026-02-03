using GScore.Application.DTOs;
using GScore.Application.Exceptions;
using GScore.Application.Exceptions.Errors;
using GScore.Application.Interfaces;
using GScore.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GScore.Application.Usecases.Student.Queries.GetStudentByRegistration;

public class GetStudentByRegistrationHandler(IApplicationDbContext context)
    : IRequestHandler<GetStudentByRegistrationQuery, StudentScoreDto>
{
    public async Task<StudentScoreDto> Handle(GetStudentByRegistrationQuery request, CancellationToken cancellationToken)
    {
        var student = await context.Students
            .AsNoTracking()
            .Include(s => s.Scores)
            .FirstOrDefaultAsync(s => s.RegistrationNumber == request.RegistrationNumber, cancellationToken)
            ?? throw new NotFoundException(StudentErrors.NotFound);

        var scores = student.Scores
            .Where(s => s.Score.HasValue)
            .ToDictionary(
                s => GetSubjectName(s.Subject),
                s => s.Score);

        return new StudentScoreDto(
            student.RegistrationNumber,
            student.ForeignLanguageCode,
            scores);
    }

    private static string GetSubjectName(SubjectType subject) => subject switch
    {
        SubjectType.Toan => "toan",
        SubjectType.NguVan => "ngu_van",
        SubjectType.NgoaiNgu => "ngoai_ngu",
        SubjectType.VatLi => "vat_li",
        SubjectType.HoaHoc => "hoa_hoc",
        SubjectType.SinhHoc => "sinh_hoc",
        SubjectType.LichSu => "lich_su",
        SubjectType.DiaLi => "dia_li",
        SubjectType.GDCD => "gdcd",
        _ => subject.ToString().ToLower()
    };
}
