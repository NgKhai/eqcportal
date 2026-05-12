using eqcportal.Models;

namespace eqcportal.Services
{
    public interface IPerformanceReviewService
    {
        decimal CalculateOverallRating(PerformanceReview review);
    }
}
