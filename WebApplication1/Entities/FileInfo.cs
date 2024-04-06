using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class FileInfo : BaseEntity
    {
        [Required]
        [MaxLength(256)]
        public string FileId { get; set; } = null!;

        [Required]
        [MaxLength(128)]
        public string KeyId { get; private set; } = null!;

        [Required]
        [MaxLength(512)]
        public string EncryptKey { get; private set; } = null!;

        [Required]
        [MaxLength(128)]
        public string BucketName { get; private set; } = null!;

        /// <summary>
        /// 原文件名
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string FileName { get; private set; } = null!;

        /// <summary>
        /// 过期时间
        /// </summary>
        public long ExpirationTime { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; private set; }
    }
}
