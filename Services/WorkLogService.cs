namespace WorkTracker.Services
{
    using Dapper;
    using WorkTracker.Data;
    using WorkTracker.Models;

    public class DayLog
    {
        public DateOnly Date { get; init; }
        public List<WorkItem> Items { get; init; } = new();

        // Sum of every entry's duration for the day.
        public TimeSpan TotalTime =>
            TimeSpan.FromTicks(Items.Sum(i => (i.EndTime - i.StartTime).Ticks));
    }

    public class WorkLogService
    {
        private readonly IDbConnectionFactory _factory;

        public WorkLogService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<long> AddAsync(WorkItem item)
        {
            const string sql = """
        INSERT INTO work_items
            (start_time, end_time, title, description, date_created, user_id)
        VALUES
            (@StartTime, @EndTime, @Title, @Description, @DateCreated, @UserId)
        RETURNING id;
        """;

            using var conn = await _factory.CreateConnectionAsync();
            return await conn.ExecuteScalarAsync<long>(sql, item);
        }

        // All entries for a date range, grouped into one DayLog per day (newest first).
        public async Task<IReadOnlyList<DayLog>> GetDailyLogAsync(
            string userId, DateTime from, DateTime to)
        {
            const string sql = """
            SELECT id, start_time, end_time, title, description, date_created, user_id
            FROM work_items
            WHERE user_id = @UserId AND start_time >= @From AND start_time < @To
            ORDER BY start_time;
            """;

            using var conn = await _factory.CreateConnectionAsync();
            var items = await conn.QueryAsync<WorkItem>(
                sql, new { UserId = userId, From = from, To = to });

            return items
                .GroupBy(i => DateOnly.FromDateTime(i.StartTime))
                .OrderByDescending(g => g.Key)
                .Select(g => new DayLog { Date = g.Key, Items = g.ToList() })
                .ToList();
        }

        // Convenience: everything logged on a single day.
        public Task<IReadOnlyList<DayLog>> GetForDayAsync(string userId, DateOnly day)
            => GetDailyLogAsync(userId,
                day.ToDateTime(TimeOnly.MinValue),
                day.AddDays(1).ToDateTime(TimeOnly.MinValue));
    }
}
