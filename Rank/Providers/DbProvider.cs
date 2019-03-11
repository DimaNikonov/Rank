using Rank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rank.Providers
{
    
    public class DbProvider
    {
        private RankInfoDbContext context;

        public async Task SaveToDb(RankInfo rankInfo)
        {
            using (this.context = new RankInfoDbContext())
            {
                context.RankInfoes.Add(rankInfo);
                await context.SaveChangesAsync();
            }
        }
    }
}
