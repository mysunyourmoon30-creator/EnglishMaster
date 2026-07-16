using EnglishMaster.Application.Features.LessonSections;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Lessons;

internal sealed class EfLessonSectionRepository : ILessonSectionRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfLessonSectionRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(LessonSection lessonSection, CancellationToken cancellationToken)
    {
        await dbContext.LessonSections.AddAsync(lessonSection, cancellationToken);
    }

    public async Task<LessonSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.LessonSections
            .FirstOrDefaultAsync(lessonSection => lessonSection.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<LessonSection>> GetByLessonIdAsync(
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        return await dbContext.LessonSections
            .Where(lessonSection => lessonSection.LessonId == lessonId)
            .OrderBy(lessonSection => lessonSection.SortOrder)
            .ThenBy(lessonSection => lessonSection.Title)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
