using eqcportal.Models;

namespace eqcportal.Services
{
    public class PerformanceReviewService : IPerformanceReviewService
    {
        public decimal CalculateOverallRating(PerformanceReview review)
        {
            var total = review.TeamworkScore + review.SkillScore + review.PunctualityScore + review.AttitudeScore;
            return Math.Round(total / 4m, 2);
        }
    }
}
