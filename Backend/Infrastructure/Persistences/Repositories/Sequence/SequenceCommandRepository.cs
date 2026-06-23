using Domain.Entities;
using Infrastructure.Persistences.Contexts;
using Infrastructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistences.Repositories
{
    public class SequenceCommandRepository : ISequenceCommandRepository
    {
        private readonly DbContextSystem _context;
       
        public SequenceCommandRepository(DbContextSystem context)
        {
            _context = context;
        }

        public async Task<string> GenerateProductCodeAsync()
        {
            var sequence = await _context.Sequence
                .FromSqlRaw("SELECT Name, IdStore, CurrentValue, LastUpdated FROM SEQUENCES WITH (UPDLOCK, ROWLOCK) WHERE Name = 'PRODUCT'")
                .FirstOrDefaultAsync();

            if (sequence == null)
            {
                sequence = new SequenceEntity
                {
                    Name = "PRODUCT",
                    CurrentValue = 1,
                    LastUpdated = DateTime.Now
                };
                await _context.Sequence.AddAsync(sequence);
            }
            else
            {
                sequence.CurrentValue++;
                sequence.LastUpdated = DateTime.Now;
                _context.Sequence.Update(sequence);
            }

            var date = DateTime.Now.ToString("yyMMdd");
            return $"{date}{sequence.CurrentValue.ToString().PadLeft(3, '0')}";
        }

        public async Task<string> GenerateMovementsCodeAsync(string sequenceName, string prefix, int storeId)
        {
            int result;

            var results = await _context.Database
                .SqlQueryRaw<int>(@"
                UPDATE SEQUENCES 
                SET CurrentValue = CurrentValue + 1,
                    LastUpdated = GETUTCDATE()
                OUTPUT INSERTED.CurrentValue
                WHERE Name = {0} AND IdStore = {1}",
                sequenceName, storeId)
                .ToListAsync();

            if (results.Count == 0)
            {
                await _context.Database.ExecuteSqlRawAsync(@"
                INSERT INTO SEQUENCES (Name, IdStore, CurrentValue, LastUpdated) 
                VALUES ({0}, {1}, 1, GETUTCDATE())",
                sequenceName, storeId);

                result = 1;
            }
            else
            {
                result = results.First();
            }

            return $"{prefix}-{result.ToString().PadLeft(6, '0')}"; ;
        }

        public async Task<string> GenerateTransferCodeAsync(string sequenceName, string prefix)
        {
            var results = await _context.Database
                .SqlQueryRaw<int>(@"
                UPDATE SEQUENCES 
                SET CurrentValue = CurrentValue + 1,
                    LastUpdated = GETUTCDATE()
                OUTPUT INSERTED.CurrentValue
                WHERE Name = {0}", sequenceName)
                .ToListAsync();

            var result = results.FirstOrDefault();

            return $"{prefix}-{result.ToString().PadLeft(6, '0')}";
        }
    }
}
