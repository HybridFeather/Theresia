using System.ComponentModel.DataAnnotations.Schema;

namespace Theresia.Entity
{
    [Table("MediaMetadata")]
    public class MediaMetadataEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Sha256
        /// </summary>
        public string Sha256 { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public string Height { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public string Width { get; set; }
        /// <summary>
        /// 帧率
        /// </summary>
        public int FrameRate { get; set; }
        /// <summary>
        /// 码率 k
        /// </summary>
        public int BitRate { get; set; }
        /// <summary>
        /// 时长 单位秒
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 编码格式
        /// </summary>
        public string Codec { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string FilePath { get; set; }


        public override bool Equals(object? obj)
        {
            MediaMetadataEntity? other = obj as MediaMetadataEntity;
            if (obj == null || other == null)
                return false;

            // 根据属性比较两个对象是否相等
            return this.Id == other.Id
                   && this.Sha256 == other.Sha256;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Sha256);
        }
    }
}
