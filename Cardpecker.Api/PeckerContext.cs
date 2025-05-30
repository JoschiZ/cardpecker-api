using Microsoft.EntityFrameworkCore;

namespace Cardpecker.Api;

public class PeckerContext : DbContext
{
    public PeckerContext(DbContextOptions<PeckerContext> options) : base(options){}
}