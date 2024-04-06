using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entities
{
    public class FileReference : BaseEntity
    {
        /// <summary>
        /// 文件表主键Id
        /// </summary>
        [Required]
        public Guid FileInfoId { get; set; }

        /// <summary>
        /// 引用Id
        /// </summary>
        [Required]
        [DefaultValue("")]
        [StringLength(128)]
        [Comment("File reference ID")]
        public string ReferenceId { get; set; } = string.Empty;
    }
}
