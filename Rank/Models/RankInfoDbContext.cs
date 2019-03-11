using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rank.Models
{
    class RankInfoDbContext: DbContext
    {
        public RankInfoDbContext():base ("DefaultConnection"){}

        public DbSet<RankInfo> RankInfoes { get; set; }
    }
}
