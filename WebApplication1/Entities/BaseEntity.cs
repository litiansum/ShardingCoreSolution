namespace WebApplication1.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public long CreateTime { get; set; }

        public long LastUpdateTime { get; set; }
    }
}
