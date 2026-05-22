using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces.Sequence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories.Sequence
{
    public class SequenceQueryRepository : ISequenceQueryRepository
    {
        private readonly DbContextSystem _context;

        public SequenceQueryRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<string> ViewProductCodeAsync()
        {
            var sequence = await _context.Sequence
                .FirstOrDefaultAsync(s => s.Name == "PRODUCT");

            var nextValue = sequence == null ? 1 : sequence.CurrentValue + 1;

            var date = DateTime.Now.ToString("yyMMdd");
            var sequential = nextValue.ToString().PadLeft(3, '0');

            return $"{date}{sequential}";
        }
    }
}
